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
using Etherna.BeeNet.Hashing.Bmt;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using System;
using System.Linq;

namespace Etherna.BeeNet.Models
{
    public class SingleOwnerChunk(
        byte[] id,
        byte[] signature,
        byte[] owner,
        SwarmChunk chunk)
    {
        // Consts.
        public const int MinChunkSize = SwarmHash.HashSize + SocSignatureSize + SwarmChunk.SpanSize;
        public const int SocSignatureSize = 65;
        
        // Properties.
        public ReadOnlyMemory<byte> Id { get; } = id;
        public ReadOnlyMemory<byte> Signature { get; } = signature;
        public ReadOnlyMemory<byte> Owner { get; } = owner;
        public SwarmChunk Chunk { get; } = chunk;
        
        // Static methods.
        public static SingleOwnerChunk DeserializeFromChunk(SwarmChunk chunk)
        {
            ArgumentNullException.ThrowIfNull(chunk, nameof(chunk));

            var data = chunk.Data;
            if (data.Length < MinChunkSize)
                throw new ArgumentOutOfRangeException(nameof(chunk), "Chunk data length is too small");

            // Extract all fields.
            var cursor = 0;
            var id = data.Slice(cursor, SwarmHash.HashSize).ToArray();
            cursor += SwarmHash.HashSize;

            var signature = data.Slice(cursor, SocSignatureSize);
            cursor += SocSignatureSize;

            var hasher = new Hasher();
            var containedChunkSpanAndData = data.Slice(cursor);
            var containedChunk = SwarmChunk.BuildFromSpanAndData(
                SwarmChunkBmtHasher.Hash(
                    containedChunkSpanAndData[..SwarmChunk.SpanSize].ToArray(),
                    containedChunkSpanAndData[SwarmChunk.SpanSize..].ToArray(),
                    hasher),
                containedChunkSpanAndData.Span);

            var toSignDigest = hasher.ComputeHash(id.Concat(containedChunk.Hash.ToByteArray()).ToArray());

            // recover owner information
            var signer = new EthereumMessageSigner();
            var owner = signer.EcRecover(toSignDigest, new EthECDSASignature(signature.ToArray()));

            return new SingleOwnerChunk(
                id,
                signature.ToArray(),
                owner.HexToByteArray(),
                containedChunk);
        }
    }
}