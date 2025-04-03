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

using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Stores;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    public class SingleOwnerChunk(
        ReadOnlyMemory<byte> id,
        ReadOnlyMemory<byte>? signature,
        EthAddress owner,
        ReadOnlyMemory<byte> chunkSpanData)
    {
        // Consts.
        public const int MaxSocDataSize = MinSocDataSize + SwarmChunk.DataSize;
        public const int MinSocDataSize = SwarmHash.HashSize + SocSignatureSize + SwarmChunk.SpanSize;
        public const int SocSignatureSize = 65;
        
        // Static builders.
        public static (SingleOwnerChunk soc, SwarmHash innerChunkHash) BuildFromBytes(
            ReadOnlyMemory<byte> data,
            IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            
            if (data.Length < MinSocDataSize)
                throw new ArgumentOutOfRangeException(nameof(data), "Data length is too small");

            // Extract all fields.
            var cursor = 0;
            var id = data[..SwarmHash.HashSize];
            cursor += SwarmHash.HashSize;

            var signature = data[cursor..(cursor + SocSignatureSize)];
            cursor += SocSignatureSize;
            
            var chunkSpanAndData = data[cursor..];
            var chunkBmt = new SwarmChunkBmt(hasher);
            var chunkHash = chunkBmt.Hash(
                chunkSpanAndData[..SwarmChunk.SpanSize],
                chunkSpanAndData[SwarmChunk.SpanSize..]);

            // Recover owner information.
            var signer = new EthereumMessageSigner();
            var toSignDigest = hasher.ComputeHash([id, chunkHash.ToReadOnlyMemory()]);
            var owner = signer.EcRecover(toSignDigest, signature.ToArray().ToHex());

            return (new SingleOwnerChunk(
                    id,
                    signature,
                    owner,
                    chunkSpanAndData),
                chunkHash);
        }

        public static async Task<SingleOwnerChunk> BuildNextSocFromFeedAsync(
            IReadOnlyChunkStore chunkStore,
            ReadOnlyMemory<byte> contentData,
            SwarmFeedBase feed,
            SwarmFeedIndexBase? knownNearIndex,
            Func<IHasher> hasherBuilder,
            DateTimeOffset? timestamp = null)
        {
            ArgumentNullException.ThrowIfNull(feed, nameof(feed));
            ArgumentNullException.ThrowIfNull(hasherBuilder, nameof(hasherBuilder));
            
            var feedChunk = await feed.BuildNextFeedChunkAsync(
                chunkStore,
                contentData,
                knownNearIndex,
                hasherBuilder,
                timestamp).ConfigureAwait(false);
            var feedId = feed.BuildIdentifier(feedChunk.Index, hasherBuilder());
            
            return new SingleOwnerChunk(feedId, null, feed.Owner, feedChunk.SpanData);
        }
        
        // Properties.
        public ReadOnlyMemory<byte> Id => id;
        public ReadOnlyMemory<byte>? Signature { get; set; } = signature;
        public EthAddress Owner => owner;
        public ReadOnlyMemory<byte> ChunkSpanData => chunkSpanData;
        
        // Static properties.
        /// <summary>
        /// Ethereum Address for SOC owner of Dispersed Replicas
        /// Generated from private key 0x0100000000000000000000000000000000000000000000000000000000000000
        /// </summary>
        public static EthAddress ReplicasOwner { get; } = new("dc5b20847f43d67928f49cd4f85d696b5a7617b5");
        
        // Methods.
        public SwarmHash BuildHash(IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            return hasher.ComputeHash([Id, Owner.ToReadOnlyMemory()]);
        }

        public byte[] ToByteArray()
        {
            if (!Signature.HasValue)
                throw new InvalidOperationException("SOC has not been signed");
            
            List<byte> buffer = [];
            buffer.AddRange(Id.Span);
            buffer.AddRange(Signature.Value.Span);
            buffer.AddRange(ChunkSpanData.Span);
            return buffer.ToArray();
        }

        public void SignWithPrivateKey(EthECKey privateKey, IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(privateKey, nameof(privateKey));
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            
            if (Owner != privateKey.GetPublicAddress())
                throw new ArgumentException("Invalid private key owner", nameof(privateKey));
            
            var chunkBmt = new SwarmChunkBmt(hasher);
            var innerChunkHash = chunkBmt.Hash(
                ChunkSpanData[..SwarmChunk.SpanSize],
                ChunkSpanData[SwarmChunk.SpanSize..]);
            var toSignDigest = hasher.ComputeHash([Id, innerChunkHash.ToReadOnlyMemory()]);

            var signer = new EthereumMessageSigner();
            var signature = signer.Sign(toSignDigest, privateKey);

            Signature = signature.HexToByteArray();
        }

        // Static methods.
        [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        public static bool IsValidChunk(SwarmChunk chunk, IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(chunk, nameof(chunk));
            
            try
            {
                var (soc, innerChunkHash) = BuildFromBytes(chunk.SpanData, hasher);

                //disperse replica validation
                if (soc.Owner == ReplicasOwner &&
                    !innerChunkHash.ToReadOnlyMemory()[1..32].Span.SequenceEqual(soc.Id[1..32].Span))
                    return false;

                return chunk.Hash == soc.BuildHash(hasher);
            }
            catch
            {
                return false;
            }
        }
    }
}