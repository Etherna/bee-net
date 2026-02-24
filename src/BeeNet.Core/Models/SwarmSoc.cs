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
using Etherna.BeeNet.Hashing.Signer;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using System;

namespace Etherna.BeeNet.Models
{
    public class SwarmSoc(
        SwarmSocIdentifier identifier,
        EthAddress owner,
        SwarmCac innerChunk,
        SwarmHash? hash = null,
        SwarmSocSignature? signature = null)
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
            ArgumentNullException.ThrowIfNull(swarmChunkBmt);
            
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
            var innerChunkHash = swarmChunkBmt.Hash(innerChunkSpanData);
            var innerChunk = new SwarmCac(innerChunkHash, innerChunkSpanData);
            
            // Recover owner information.
            var toSignDigest = BuildToSignDigest(identifier, innerChunkHash, swarmChunkBmt.Hasher);
            var owner = signature.RecoverOwner(toSignDigest);

            return new SwarmSoc(identifier, owner, innerChunk, hash, signature);
        }
        
        // Properties.
        public override SwarmHash Hash => hash ??= BuildHash(Identifier, Owner, new Hasher());
        public SwarmSocIdentifier Identifier { get; } = identifier;
        public SwarmSocSignature? Signature { get; set; } = signature;
        public EthAddress Owner { get; } = owner;
        public SwarmCac InnerChunk { get; } = innerChunk;
        
        // Static properties.
        /// <summary>
        /// Replicas SOC owner
        /// </summary>
        public static EthAddress ReplicasOwner => ReplicasOwnerPrivateKey.GetPublicAddress(); //"0xdc5b20847f43d67928f49cd4f85d696b5a7617b5"
        public static EthECKey ReplicasOwnerPrivateKey { get; } = new("0x0100000000000000000000000000000000000000000000000000000000000000");
        
        // Methods.
        public SwarmHash BuildHash(Hasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher);
            
            hash = BuildHash(Identifier, Owner, hasher);
            return hash.Value;
        }

        public override ReadOnlyMemory<byte> GetFullPayload() => GetFullPayloadToByteArray();
        
        public override byte[] GetFullPayloadToByteArray()
        {
            var buffer = new byte[SwarmSocIdentifier.IdentifierSize +
                                  SwarmSocSignature.SignatureSize +
                                  InnerChunk.SpanData.Length];
            var cursor = 0;
            
            Identifier.ToReadOnlyMemory().CopyTo(buffer);
            cursor += SwarmSocIdentifier.IdentifierSize;
            
            (Signature ?? new byte[SwarmSocSignature.SignatureSize]).ToReadOnlyMemory().CopyTo(buffer.AsMemory(cursor));
            cursor += SwarmSocSignature.SignatureSize;
            
            InnerChunk.SpanData.CopyTo(buffer.AsMemory(cursor));
            
            return buffer;
        }

        /// <summary>
        /// Sign signs a SOC using the given signer.
        /// It returns a signed SOC chunk ready for submission to the network.
        /// </summary>
        /// <param name="signer"></param>
        /// <returns></returns>
        public void Sign(ISigner signer, Hasher hasher)
        {
            ArgumentNullException.ThrowIfNull(signer);
            
            if (Owner != signer.PublicAddress)
                throw new ArgumentException("Invalid signer owner", nameof(signer));

            var toSignBytes = ToSignDigest(hasher);
            Signature = new SwarmSocSignature(signer.Sign(toSignBytes));
        }

        public void SignWithPrivateKey(EthECKey privateKey, Hasher hasher)
        {
            ArgumentNullException.ThrowIfNull(privateKey);
            ArgumentNullException.ThrowIfNull(hasher);
            
            if (Owner != privateKey.GetPublicAddress())
                throw new ArgumentException("Invalid owner from private key", nameof(privateKey));

            var signer = new EthereumMessageSigner();
            Signature = signer.Sign(ToSignDigest(hasher), privateKey).HexToByteArray();
        }

        public byte[] ToSignDigest(Hasher hasher) => BuildToSignDigest(Identifier, InnerChunk.Hash, hasher);

        public bool ValidateSoc(Hasher hasher)
        {
            // Rebuild hash, has same cost than verify.
            hash = BuildHash(Identifier, Owner, hasher);
            
            // Verify signature.
            if (!Signature.HasValue ||
                Signature.Value.RecoverOwner(ToSignDigest(hasher)) != Owner)
                return false;
            
            // Disperse replica validation.
            if (Owner == ReplicasOwner)
                return InnerChunk.Hash.ToReadOnlyMemory()[1..32].Span.SequenceEqual(
                    Identifier.ToReadOnlyMemory()[1..32].Span);
            
            return true;
        }
        
        // Static methods.
        public static SwarmHash BuildHash(
            SwarmSocIdentifier identifier,
            EthAddress owner,
            Hasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher);
            
            return hasher.ComputeHash([identifier.ToReadOnlyMemory(), owner.ToReadOnlyMemory()]);
        }

        public static byte[] BuildToSignDigest(
            SwarmSocIdentifier identifier,
            SwarmHash innerChunkHash,
            Hasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher);
            
            return hasher.ComputeHash(
            [
                identifier.ToReadOnlyMemory(),
                innerChunkHash.ToReadOnlyMemory()
            ]);
        }
    }
}