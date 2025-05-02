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
        // Private classes.
        private class ChunkCacheLevel
        {
            public required SwarmCac Chunk { get; init; }
            public required XorEncryptKey? EncryptionKey { get; init; }
            public int Position { get; set; }
        }
        
        // Fields.
        private readonly List<ChunkCacheLevel> chunkCacheLevels = new();
        private readonly IReadOnlyChunkStore chunkStore;
        private readonly uint maxSegmentsInChunk;
        private readonly int segmentSize;
        private readonly bool useRecursiveEncryption;

        // Constructor.
        private ChunkDataStream(
            SwarmCac rootChunk,
            XorEncryptKey? encryptionKey,
            bool useRecursiveEncryption,
            IReadOnlyChunkStore chunkStore,
            long length)
        {
            chunkCacheLevels.Add(new ChunkCacheLevel
            {
                Chunk = rootChunk,
                EncryptionKey = encryptionKey
            });
            this.chunkStore = chunkStore;
            segmentSize = SwarmHash.HashSize * (useRecursiveEncryption ? 2 : 1);
            maxSegmentsInChunk = (uint)(SwarmCac.DataSize / segmentSize);
            this.useRecursiveEncryption = useRecursiveEncryption;
            Length = length;
            Position = 0;
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
            // Seek position on chunks' cache to start reading.
            await SeekChunksCacheAsync(0, (ulong)Position, cancellationToken).ConfigureAwait(false);
            
            // Read from chunks.
            var dataToRead = (int)Math.Min(buffer.Length, Length - Position);
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
            // Read from current cached data chunk.
            var cacheDataLevel = chunkCacheLevels.Last();
            var dataToReadFromChunk = Math.Min(cacheDataLevel.Chunk.Data.Length - cacheDataLevel.Position, buffer.Length);
            
            var dataArray = cacheDataLevel.Chunk.Data.ToArray();
            cacheDataLevel.EncryptionKey?.EncryptDecrypt(dataArray);
            
            dataArray.AsMemory()[cacheDataLevel.Position..(cacheDataLevel.Position + dataToReadFromChunk)].CopyTo(buffer);
            
            // If all data has been read, return.
            if (buffer.Length == dataToReadFromChunk)
                return;
            
            // Else, cached next data chunk and read it.
            await CacheNextDataChunkAsync(cancellationToken).ConfigureAwait(false);
            await CopyDataToBufferAsync(buffer[dataToReadFromChunk..], cancellationToken).ConfigureAwait(false);
        }

        private async Task CacheNextDataChunkAsync(CancellationToken cancellationToken)
        {
            // Remove current data chunk.
            chunkCacheLevels.RemoveAt(chunkCacheLevels.Count - 1);
            
            // Find first parent where selected segment is not the last, remove others.
            var currentLevel = chunkCacheLevels.Last();
            while (currentLevel.Position == currentLevel.Chunk.Data.Length - segmentSize)
            {
                chunkCacheLevels.RemoveAt(chunkCacheLevels.Count - 1);
                currentLevel = chunkCacheLevels.Last();
            }
            
            // Increment selected segment.
            currentLevel.Position += segmentSize;
            
            // Find first child data chunk.
            do
            {
                var dataArray = currentLevel.Chunk.Data.ToArray();
                currentLevel.EncryptionKey?.EncryptDecrypt(dataArray);
                
                // Read child chunk reference.
                var cursor = currentLevel.Position;
                var childHash = new SwarmHash(dataArray.AsMemory()[cursor..(cursor + SwarmHash.HashSize)]);
                cursor += SwarmHash.HashSize;

                var childEncryptionKey = useRecursiveEncryption
                    ? new XorEncryptKey(dataArray.AsMemory()[cursor..(cursor + XorEncryptKey.KeySize)])
                    : (XorEncryptKey?)null;

                var childChunk = await chunkStore.GetAsync(childHash, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (childChunk is not SwarmCac childCac)
                    throw new InvalidOperationException($"Chunk {childHash} is not a Content Addressed Chunk.");

                currentLevel = new ChunkCacheLevel { Chunk = childCac, EncryptionKey = childEncryptionKey };
                chunkCacheLevels.Add(currentLevel);
                
            } while (SwarmCac.SpanToLength(currentLevel.Chunk.Span.Span) > SwarmCac.DataSize);
        }

        /// <summary>
        /// Recursive method to find in chunks' cache the current branch with rigth position to start reading.
        /// Recursion:
        /// - input: argument current level has always the right chunk selected, with position to determinate.
        ///          The offset indicates how many bytes still needs to skip
        /// - check: verify if current chunk is a leaf, in that case set position on the leaf
        /// - recursive call: find the chunk at next cache level containing the searched offset,
        ///                   and update position at current level according to it
        /// </summary>
        /// <param name="currentLevelIndex">Current max level cache with known right chunk</param>
        /// <param name="offset">Data offset to apply</param>
        private async Task SeekChunksCacheAsync(int currentLevelIndex, ulong offset, CancellationToken cancellationToken)
        {
            var currentLevel = chunkCacheLevels[currentLevelIndex];
            
            var dataArray = currentLevel.Chunk.Data.ToArray();
            currentLevel.EncryptionKey?.EncryptDecrypt(dataArray);
            
            // If is a data chunk, set position and return.
            var chunkDataLength = SwarmCac.SpanToLength(currentLevel.Chunk.Span.Span);
            if (chunkDataLength <= SwarmCac.DataSize)
            {
                //check offset consistency
                if (chunkDataLength < offset)
                    throw new InvalidOperationException("Current chunk's can't resolve required offset");
                
                //clear cache after this level
                while (chunkCacheLevels.Count - 1 > currentLevelIndex)
                    chunkCacheLevels.RemoveAt(chunkCacheLevels.Count - 1);
                
                currentLevel.Position = (int)offset;
                return;
            }
            
            // Else, identify the right segment to visit.
            var segmentsAmount = (uint)(dataArray.Length / segmentSize);
            ulong dataBySegment = SwarmCac.DataSize;
            while (dataBySegment * segmentsAmount < chunkDataLength)
                dataBySegment *= maxSegmentsInChunk;

            var selectedSegment = (int)(offset / dataBySegment);
            var selectedSegmentPosition = selectedSegment * segmentSize;
            var offsetRest = offset - dataBySegment * (ulong)selectedSegment;

            // If current cache needs to be updated.
            if (selectedSegmentPosition != currentLevel.Position || currentLevelIndex == chunkCacheLevels.Count - 1)
            {
                while (chunkCacheLevels.Count - 1 > currentLevelIndex)
                    chunkCacheLevels.RemoveAt(chunkCacheLevels.Count - 1);
                
                currentLevel.Position = selectedSegmentPosition;
            
                // Read child chunk reference.
                var cursor = currentLevel.Position;
                var childHash = new SwarmHash(dataArray.AsMemory()[cursor..(cursor + SwarmHash.HashSize)]);
                cursor += SwarmHash.HashSize;
            
                var childEncryptionKey = useRecursiveEncryption ?
                    new XorEncryptKey(dataArray.AsMemory()[cursor..(cursor + XorEncryptKey.KeySize)]) :
                    (XorEncryptKey?)null;
            
                // Get child chunk and add to cache.
                var childChunk = await chunkStore.GetAsync(childHash, cancellationToken: cancellationToken).ConfigureAwait(false);
                if (childChunk is not SwarmCac childCac)
                    throw new InvalidOperationException($"Chunk {childHash} is not a Content Addressed Chunk.");
                chunkCacheLevels.Add(new ChunkCacheLevel{Chunk = childCac, EncryptionKey = childEncryptionKey});
            }
            
            // Do recursion on next cache level.
            await SeekChunksCacheAsync(currentLevelIndex + 1, offsetRest, cancellationToken).ConfigureAwait(false);
        }
    }
}