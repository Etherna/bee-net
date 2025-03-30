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
using Nethereum.Signer;
using Nethereum.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
        
        // Static properties.
        /// <summary>
        /// Ethereum Address for SOC owner of Dispersed Replicas
        /// Generated from private key 0x0100000000000000000000000000000000000000000000000000000000000000
        /// </summary>
        public static EthAddress ReplicasOwner { get; } = new("dc5b20847f43d67928f49cd4f85d696b5a7617b5");
        
        // Properties.
        public ReadOnlyMemory<byte> Id { get; } = id;
        public ReadOnlyMemory<byte> Signature { get; } = signature;
        public EthAddress Owner { get; } = owner;
        public ReadOnlyMemory<byte> ChunkData => chunkData;
        
        // Static methods.
        public static (SingleOwnerChunk soc, SwarmHash innerChunkHash) BuildFromBytes(
            ReadOnlyMemory<byte> data,
            IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            
            if (data.Length < MinSocDataSize)
                throw new ArgumentOutOfRangeException(nameof(data), "Data length is too small");

            // Extract all fields.
            var cursor = 0;
            var id = data[cursor..SwarmHash.HashSize].ToArray();
            cursor += SwarmHash.HashSize;

            var signature = data[cursor..SocSignatureSize].ToArray();
            cursor += SocSignatureSize;
            
            var chunkSpanAndData = data[cursor..];
            var chunkBmt = new SwarmChunkBmt(hasher);
            var chunkHash = chunkBmt.Hash(
                chunkSpanAndData[..SwarmChunk.SpanSize].ToArray(),
                chunkSpanAndData[SwarmChunk.SpanSize..].ToArray());

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

        [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        public static bool IsValidChunk(SwarmChunk chunk, IHasher hasher)
        {
            ArgumentNullException.ThrowIfNull(chunk, nameof(chunk));
            
            try
            {
                var (soc, innerChunkHash) = BuildFromBytes(chunk.Data, hasher);

                //disperse replica validation
                if (soc.Owner == ReplicasOwner &&
                    !ByteArrayComparer.Current.Equals(innerChunkHash.ToByteArray()[1..32].ToArray(), soc.Id[1..32].ToArray()))
                    return false;

                return chunk.Hash == soc.BuildHash(hasher);
            }
            catch
            {
                return false;
            }
        }
        
        // Methods.
        public SwarmHash BuildHash(IHasher hasher)
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