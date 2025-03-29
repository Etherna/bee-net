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

using Etherna.BeeNet.Extensions;
using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Stores;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    public class SwarmFeedChunk : SwarmChunk
    {
        // Consts.
        public const int MaxChunkSize = MinChunkSize + DataSize;
        public const int MaxPayloadSize = DataSize - TimeStampSize; //creation timestamp
        public const int MinChunkSize = SwarmHash.HashSize + SwarmSignature.SignatureSize + SpanSize;
        public const int MinDataSize = TimeStampSize;
        public const int TimeStampSize = sizeof(ulong);

        // Constructor.
        public SwarmFeedChunk(
            SwarmFeedIndexBase index,
            byte[] data,
            SwarmHash hash) :
            base(hash, data)
        {
            if (data.Length < MinDataSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Data can't be shorter than {TimeStampSize} bytes");
            
            Index = index ?? throw new ArgumentNullException(nameof(index));
        }

        // Properties.
        public SwarmFeedIndexBase Index { get; }
        public ReadOnlyMemory<byte> Payload => Data[TimeStampSize..];
        public DateTimeOffset TimeStamp => Data[..TimeStampSize].Span.UnixTimeSecondsToDateTimeOffset();

        // Methods.
        [SuppressMessage("Design", "CA1062:Validate arguments of public methods")]
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not SwarmFeedChunk objFeedChunk) return false;
            return GetType() == obj.GetType() &&
                Hash.Equals(objFeedChunk.Hash) &&
                Data.Span.SequenceEqual(objFeedChunk.Data.Span) &&
                Index.Equals(objFeedChunk.Index) &&
                Span.Span.SequenceEqual(objFeedChunk.Span.Span);
        }
        
        public override int GetHashCode() =>
            Hash.GetHashCode() ^
            _data.GetHashCode() ^
            Index.GetHashCode() ^
            _span.GetHashCode();
        
        public async Task<(SwarmChunk, SingleOwnerChunk)> UnwrapChunkAndSocAsync(
            IChunkStore chunkStore,
            IHasher? hasher = null)
        {
            ArgumentNullException.ThrowIfNull(chunkStore, nameof(chunkStore));

            hasher ??= new Hasher();
            
            var (soc, chunkHash) = SingleOwnerChunk.BuildFromBytes(_data, hasher);
            
            // Check if is legacy payload. Possible lengths:
            if (soc.ChunkData.Length is
                16 + SwarmHash.HashSize or   // unencrypted ref: span+timestamp+ref => 8+8+32=48
                16 + SwarmHash.HashSize * 2) // encrypted ref: span+timestamp+ref+decryptKey => 8+8+64=80
            {
                var hash = new SwarmHash(soc.ChunkData[16..].ToArray());
                return (await chunkStore.GetAsync(hash).ConfigureAwait(false), soc);
            }

            return (new SwarmChunk(
                chunkHash,
                soc.ChunkData.ToArray()), soc);
        }
    }
}
