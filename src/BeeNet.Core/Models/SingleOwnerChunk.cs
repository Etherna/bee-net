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
using Nethereum.Signer;
using System;
using System.Collections.Generic;

namespace Etherna.BeeNet.Models
{
    public class SingleOwnerChunk(
        byte[] id,
        byte[] signature,
        EthAddress owner,
        byte[] chunkData)
    {
        // Consts.
        public const int MaxSocDataSize = MinSocDataSize + SwarmChunk.DataSize;
        public const int MinSocDataSize = SwarmHash.HashSize + SocSignatureSize + SwarmChunk.SpanSize;
        public const int SocSignatureSize = 65;
        
        // Properties.
        public ReadOnlyMemory<byte> Id { get; } = id;
        public ReadOnlyMemory<byte> Signature { get; } = signature;
        public EthAddress Owner { get; } = owner;
        public ReadOnlyMemory<byte> ChunkData => chunkData;
        
        // Static methods.
        public static (SingleOwnerChunk soc, SwarmHash chunkHash) BuildFromBytes(
            ReadOnlyMemory<byte> data,
            IHasher hasher)
        {
            if (data.Length < MinSocDataSize)
                throw new ArgumentOutOfRangeException(nameof(data), "Data length is too small");

            // Extract all fields.
            var cursor = 0;
            var id = data[cursor..SwarmHash.HashSize].ToArray();
            cursor += SwarmHash.HashSize;

            var signature = data[cursor..SocSignatureSize].ToArray();
            cursor += SocSignatureSize;
            
            var chunkSpanAndData = data[cursor..];
            var chunkHash = SwarmChunkBmtHasher.Hash(
                chunkSpanAndData[..SwarmChunk.SpanSize].ToArray(),
                chunkSpanAndData[SwarmChunk.SpanSize..].ToArray(),
                hasher);

            // Recover owner information.
            var signer = new EthereumMessageSigner();
            var toSignDigest = hasher.ComputeHash(id, chunkHash.ToByteArray());
            var owner = signer.EcRecover(toSignDigest, new EthECDSASignature(signature));

            return (new SingleOwnerChunk(
                    id,
                    signature,
                    owner,
                    chunkSpanAndData.ToArray()),
                chunkHash);
        }
        
        // Methods.
        public SwarmHash CalculateHash(IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            return hasher.ComputeHash(Id.ToArray(), Owner.ToByteArray());
        }

        public byte[] ToByteArray()
        {
            List<byte> buffer = [];
            buffer.AddRange(Id.ToArray());
            buffer.AddRange(Signature.ToArray());
            buffer.AddRange(ChunkData.ToArray());
            return buffer.ToArray();
        }
    }
}