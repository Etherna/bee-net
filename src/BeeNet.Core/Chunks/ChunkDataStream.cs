// Copyright 2021-present Etherna SA
// This file is part of Bee.Net.
// 
// Bee.Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Bee.Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Bee.Net.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using Nethereum.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Chunks
{
    public sealed class ChunkDataStream : Stream
    {
        // Fields.
        private readonly uint chunkSegmentSize;
        private readonly IReadOnlyChunkStore chunkStore;
        private List<SwarmCac> levelsCache = new();
        private readonly uint maxSegmentsInChunk;
        private readonly SwarmCac rootChunk;
        private readonly XorEncryptKey? rootEncryptionKey;
        private readonly bool useRecursiveEncryption;

        // Constructor.
        private ChunkDataStream(
            SwarmCac rootChunk,
            XorEncryptKey? rootEncryptionKey,
            bool useRecursiveEncryption,
            IReadOnlyChunkStore chunkStore,
            long length)
        {
            this.chunkStore = chunkStore;
            this.rootChunk = rootChunk;
            this.rootEncryptionKey = rootEncryptionKey;
            this.useRecursiveEncryption = useRecursiveEncryption;
            Length = length;
            Position = 0;
            
            // Define segments info.
            chunkSegmentSize = (uint)(SwarmHash.HashSize * (useRecursiveEncryption ? 2 : 1));
            maxSegmentsInChunk = SwarmCac.DataSize / chunkSegmentSize;
        }

        // Static builder.
        public static async Task<Stream> BuildNewAsync(
            SwarmChunkReference chunkReference,
            IReadOnlyChunkStore chunkStore)
        {
            ArgumentNullException.ThrowIfNull(chunkReference, nameof(chunkReference));
            ArgumentNullException.ThrowIfNull(chunkStore, nameof(chunkStore));
            
            var rootChunk = await chunkStore.GetAsync(chunkReference.Hash).ConfigureAwait(false);
            if (rootChunk is not SwarmCac rootCac) //soc are not supported
                throw new InvalidOperationException($"Chunk {chunkReference.Hash} is not a Content Addressed Chunk.");
            
            return BuildNew(
                rootCac,
                chunkReference.EncryptionKey,
                chunkReference.UseRecursiveEncryption,
                chunkStore);
        }
        
        public static Stream BuildNew(
            SwarmCac rootChunk,
            XorEncryptKey? encryptionKey,
            bool useRecursiveEncryption,
            IReadOnlyChunkStore chunkStore)
        {
            ArgumentNullException.ThrowIfNull(rootChunk, nameof(rootChunk));
            
            var length =  SwarmCac.SpanToLength(rootChunk.Span.Span);

            return new ChunkDataStream(
                rootChunk,
                encryptionKey,
                useRecursiveEncryption,
                chunkStore,
                (long)length);
        }
        
        // Properties.
        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length { get; }
        public override long Position { get; set; }

        // Methods.
        public override void Flush() { }
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            var task = ReadAsync(buffer, offset, count);
            task.Wait();
            return task.Result;
        }
        
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            await ReadAsync(buffer.AsMemory(offset, count), cancellationToken).ConfigureAwait(false);
        
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            // Return 0 when the stream is at the end.
            var dataToRead = (int)Math.Min(buffer.Length, Length - Position);
            if (dataToRead == 0)
                return 0;
            
            // Read data from chunks.
            await CopyDataToBufferAsync(buffer[..dataToRead], cancellationToken).ConfigureAwait(false);
            
            Position += dataToRead;
            return dataToRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var newPosition = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => Position + offset,
                SeekOrigin.End => Length + offset,
                _ => throw new ArgumentException("Invalid seek origin", nameof(origin))
            };

            if (newPosition < 0)
                throw new IOException("Cannot seek before the beginning of the stream");
            if (newPosition > Length)
                throw new IOException("Cannot seek past the end of the stream");

            Position = newPosition;
            return Position;
        }

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        
        // Private helpers.
        private async Task CopyDataToBufferAsync(Memory<byte> buffer, CancellationToken cancellationToken)
        {
            List<SwarmCac> newLevelsCache = [];
            
            // Init with root level info.
            var levelStartDataOffset = (ulong)Position;
            var levelEndDataOffset = (ulong)(Length - (Position + buffer.Length));
            (SwarmCac Chunk, XorEncryptKey? EncKey)[] levelChunkKeyPairs = [new(rootChunk, rootEncryptionKey)];
            
            // Reuse these for memory optimization.
            var chunkDataBuffer = new byte[SwarmCac.DataSize];
            var levelChildHashKeyPairs = new List<(SwarmHash Hash, XorEncryptKey? EncKey)>();

            // Iterate on all levels from root to data chunks.
            for (var levelIndex = 0; levelChunkKeyPairs.Length != 0; levelIndex++)
            {
                // Init.
                levelChildHashKeyPairs.Clear();
                //optimize value search for chunks in level. Analyzing from right, it can only be monotonically non-decreasing
                ulong dataSizeBySegment = SwarmCac.DataSize;

                // Parse all chunks on current level. Start from end to copy data first, if present in level.
                for (int chunkIndex = levelChunkKeyPairs.Length - 1; chunkIndex >= 0; chunkIndex--)
                {
                    var chunkKeyPair = levelChunkKeyPairs[chunkIndex];
                    var isFirstChunkInLevel = chunkIndex == 0;
                    var isLastChunkInLevel = chunkIndex == levelChunkKeyPairs.Length - 1;

                    // If it's the last chunk on the level, and if some data remains to read in level, cache the chunk.
                    if (isLastChunkInLevel && levelEndDataOffset > 0)
                        newLevelsCache.Add(chunkKeyPair.Chunk);
                    
                    // Decode chunk's data.
                    chunkKeyPair.Chunk.Data.CopyTo(chunkDataBuffer);
                    chunkKeyPair.EncKey?.EncryptDecrypt(chunkDataBuffer.AsSpan(0, chunkKeyPair.Chunk.Data.Length));
                    
                    // If is a data chunk, report data on buffer and update bounds. Then continue.
                    if (chunkKeyPair.Chunk.IsDataChunk)
                    {
                        //check end offset consistency
                        var dataToCopyStart = isFirstChunkInLevel ? (int)levelStartDataOffset : 0;
                        var dataToCopySize = chunkKeyPair.Chunk.Data.Length - dataToCopyStart - (int)levelEndDataOffset;
                        if (dataToCopySize <= 0)
                            throw new InvalidOperationException("Invalid data to copy size");
                        
                        //copy data to end of buffer, and shrink buffer
                        chunkDataBuffer[dataToCopyStart..(dataToCopyStart + dataToCopySize)].CopyTo(buffer[^dataToCopySize..]);
                        buffer = buffer[..^dataToCopySize];
                        
                        //update level bounds
                        levelEndDataOffset = 0;

                        continue;
                    }

                    // Else if it's an intermediate chunk
                    if (chunkKeyPair.Chunk.Data.Length % chunkSegmentSize != 0)
                        throw new InvalidOperationException("Intermediate chunk's data length is not multiple of segment size.");
            
                    // Find referred data size by segment.
                    var referredDataSize = SwarmCac.SpanToLength(chunkKeyPair.Chunk.Span.Span);
                    var segmentsAmount = (uint)(chunkKeyPair.Chunk.Data.Length / chunkSegmentSize);
                    while (dataSizeBySegment * segmentsAmount < referredDataSize)
                        dataSizeBySegment *= maxSegmentsInChunk;
                    
                    // Define chunk's segments to read and set bounds for the next level. 
                    var chunkStartPosition = 0;
                    if (isFirstChunkInLevel)
                    {
                        var startSegmentsToSkip = levelStartDataOffset / dataSizeBySegment;
                        chunkStartPosition = (int)(startSegmentsToSkip * chunkSegmentSize);
                        levelStartDataOffset -= startSegmentsToSkip * dataSizeBySegment;
                    }
                    var chunkEndPosition = chunkKeyPair.Chunk.Data.Length;
                    if (isLastChunkInLevel)
                    {
                        var endSegmentsToSkip = levelEndDataOffset / dataSizeBySegment;
                        chunkEndPosition = chunkKeyPair.Chunk.Data.Length - (int)(endSegmentsToSkip * chunkSegmentSize);
                        if (endSegmentsToSkip > 0)
                            levelEndDataOffset -= referredDataSize % dataSizeBySegment == 0
                                ? endSegmentsToSkip * dataSizeBySegment
                                : (endSegmentsToSkip - 1) * dataSizeBySegment + referredDataSize % dataSizeBySegment;
                    }

                    // Reverse read child chunks references and prepend them on hash list.
                    for (var cursor = chunkEndPosition; cursor > chunkStartPosition;)
                    {
                        XorEncryptKey? childEncryptionKey = null;
                        if (useRecursiveEncryption)
                        {
                            cursor -= XorEncryptKey.KeySize;
                            childEncryptionKey =
                                new XorEncryptKey(chunkDataBuffer.Slice(cursor, cursor + XorEncryptKey.KeySize));
                        }
                        
                        cursor -= SwarmHash.HashSize;
                        var childHash = new SwarmHash(chunkDataBuffer.Slice(cursor, cursor + SwarmHash.HashSize));

                        levelChildHashKeyPairs.Insert(0, (childHash, childEncryptionKey));
                    }
                }

                // Search next level chunks.
                if (levelChildHashKeyPairs.Count != 0)
                {
                    IReadOnlyDictionary<SwarmHash, SwarmChunk> childChunksPool;
                
                    //with cache
                    if (levelsCache.Count > levelIndex + 1 &&
                        levelChildHashKeyPairs.Any(p => p.Hash == levelsCache[levelIndex + 1].Hash))
                    {
                        var hashesToGetFromStore = levelChildHashKeyPairs
                            .Select(p => p.Hash)
                            .Where(h => h != levelsCache[levelIndex + 1].Hash).ToArray();
                        IEnumerable<KeyValuePair<SwarmHash, SwarmChunk>> chunksFromStore =
                            hashesToGetFromStore.Length != 0
                                ? await chunkStore.GetAsync(hashesToGetFromStore, cancellationToken: cancellationToken).ConfigureAwait(false)
                                : Array.Empty<KeyValuePair<SwarmHash, SwarmChunk>>();
                    
                        childChunksPool = new Dictionary<SwarmHash, SwarmChunk>(chunksFromStore)
                        {
                            [levelsCache[levelIndex + 1].Hash] = levelsCache[levelIndex + 1]
                        };
                    }
                    else //or without cache
                    {
                        childChunksPool = await chunkStore.GetAsync(
                            levelChildHashKeyPairs.Select(p => p.Hash),
                            cancellationToken: cancellationToken).ConfigureAwait(false);
                    }
                
                    // Resolve child chunks from hash list.
                    levelChunkKeyPairs = levelChildHashKeyPairs.Select(pair =>
                        ((SwarmCac)childChunksPool[pair.Hash], pair.EncKey)).ToArray();
                }
                else
                {
                    // No more chunks on next level.
                    levelChunkKeyPairs = [];
                }
            }
            
            // Verify the full buffer has been written.
            if (buffer.Length != 0)
                throw new InvalidOperationException("Not full buffer has been written");

            // Replace cache.
            levelsCache = newLevelsCache;
        }
    }
}