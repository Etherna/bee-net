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
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using System;
using System.Collections.Generic;

namespace Etherna.BeeNet.Models
{
    public class SwarmSoc(
        SwarmSocIdentifier identifier,
        EthAddress owner,
        SwarmCac innerChunk,
        SwarmHash? hash,
        SwarmSocSignature? signature)
        : SwarmChunk
    {
        // Consts.
        public const int MaxSocSize = MinSocSize + SwarmCac.DataSize;
        public const int MinSocSize = SwarmSocIdentifier.IdentifierSize + SwarmSocSignature.SignatureSize + SwarmCac.SpanSize;
        
        // Fields.
        private SwarmHash? hash = hash;

        // Static builders.
        public static SwarmSoc BuildFromBytes(
            SwarmHash? hash,
            ReadOnlyMemory<byte> data,
            SwarmChunkBmt swarmChunkBmt)
        {
            ArgumentNullException.ThrowIfNull(swarmChunkBmt, nameof(swarmChunkBmt));
            
            if (data.Length < MinSocSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Data can't be less than {MinSocSize} bytes.");
            if (data.Length > MaxSocSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Data can't be greater than {MaxSocSize} bytes.");

            // Extract all fields.
            var cursor = 0;
            var identifier = new SwarmSocIdentifier(data[..SwarmSocIdentifier.IdentifierSize]);
            cursor += SwarmSocIdentifier.IdentifierSize;

            var signature = new SwarmSocSignature(data[cursor..(cursor + SwarmSocSignature.SignatureSize)]);
            cursor += SwarmSocSignature.SignatureSize;

            var innerChunkSpanData = data[cursor..];
            var innerChunkHash = swarmChunkBmt.Hash(
                innerChunkSpanData[..SwarmCac.SpanSize],
                innerChunkSpanData[SwarmCac.SpanSize..]);
            var innerChunk = new SwarmCac(innerChunkHash, innerChunkSpanData);
            
            // Recover owner information.
            var toSignDigest = BuildToSignDigest(identifier, innerChunkHash, swarmChunkBmt.Hasher);
            var owner = signature.RecoverOwner(toSignDigest);

            return new SwarmSoc(identifier, owner, innerChunk, hash, signature);
        }
        
        // Properties.
        public override SwarmHash Hash => hash ?? throw new InvalidOperationException("Hash has not been calculated.");
        public SwarmSocIdentifier Identifier { get; } = identifier;
        public SwarmSocSignature? Signature { get; set; } = signature;
        public EthAddress Owner { get; } = owner;
        public SwarmCac InnerChunk { get; } = innerChunk;
        
        // Static properties.
        /// <summary>
        /// Ethereum Address for SOC owner of Dispersed Replicas
        /// Generated from private key 0x0100000000000000000000000000000000000000000000000000000000000000
        /// </summary>
        public static EthAddress ReplicasOwner { get; } = new("dc5b20847f43d67928f49cd4f85d696b5a7617b5");
        
        // Methods.
        public SwarmHash BuildHash(Hasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            
            hash = BuildHash(Identifier, Owner, hasher);
            return hash.Value;
        }

        public override ReadOnlyMemory<byte> GetFullPayload() => GetFullPayloadToByteArray();
        
        public override byte[] GetFullPayloadToByteArray()
        {
            if (!Signature.HasValue)
                throw new InvalidOperationException("SOC has not been signed");

            var buffer = new byte[SwarmSocIdentifier.IdentifierSize +
                                  SwarmSocSignature.SignatureSize +
                                  InnerChunk.SpanData.Length];
            var cursor = 0;
            
            Identifier.ToReadOnlyMemory().CopyTo(buffer);
            cursor += SwarmSocIdentifier.IdentifierSize;
            
            Signature.Value.ToReadOnlyMemory().CopyTo(buffer.AsMemory(cursor));
            cursor += SwarmSocSignature.SignatureSize;
            
            InnerChunk.SpanData.CopyTo(buffer.AsMemory(cursor));
            
            return buffer;
        }

        public bool IsValidSoc(Hasher hasher)
        {
            // Verify hash.
            if (hash != null &&
                hash != BuildHash(Identifier, Owner, hasher))
                return false;
            
            // Verify signature.
            if (!Signature.HasValue ||
                Signature.Value.RecoverOwner(ToSignDigest(hasher)) != Owner)
                return false;
            
            // Disperse replica validation.
            if (Owner == ReplicasOwner &&
                !InnerChunk.Hash.ToReadOnlyMemory()[1..32].Span.SequenceEqual(Identifier.ToReadOnlyMemory()[1..32].Span))
                return false;
            
            return true;
        }

        public void SignWithPrivateKey(EthECKey privateKey, Hasher hasher)
        {
            ArgumentNullException.ThrowIfNull(privateKey, nameof(privateKey));
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            
            if (Owner != privateKey.GetPublicAddress())
                throw new ArgumentException("Invalid owner from private key", nameof(privateKey));

            var signer = new EthereumMessageSigner();
            Signature = signer.Sign(ToSignDigest(hasher), privateKey).HexToByteArray();
        }

        public byte[] ToSignDigest(Hasher hasher) => BuildToSignDigest(Identifier, InnerChunk.Hash, hasher);
        
        // Static methods.
        public static SwarmHash BuildHash(
            SwarmSocIdentifier identifier,
            EthAddress owner,
            Hasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            
            return hasher.ComputeHash([identifier.ToReadOnlyMemory(), owner.ToReadOnlyMemory()]);
        }

        public static byte[] BuildToSignDigest(
            SwarmSocIdentifier identifier,
            SwarmHash innerChunkHash,
            Hasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            
            return hasher.ComputeHash(
            [
                identifier.ToReadOnlyMemory(),
                innerChunkHash.ToReadOnlyMemory()
            ]);
        }
    }
}