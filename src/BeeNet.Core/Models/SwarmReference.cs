using Etherna.BeeNet.Manifest;
using Etherna.BeeNet.Stores;
using Etherna.BeeNet.TypeConverters;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    [TypeConverter(typeof(SwarmReferenceTypeConverter))]
    public readonly struct SwarmReference : IEquatable<SwarmReference>, IParsable<SwarmReference>
    {
        // Consts.
        public const int EncryptedSize = SwarmHash.HashSize + EncryptionKey256.KeySize;
        public const int PlainSize = SwarmHash.HashSize;
        
        // Fields.
        private readonly ReadOnlyMemory<byte> byteReference;
        
        // Constructors.
        public SwarmReference(ReadOnlyMemory<byte> reference)
        {
            if (!IsValidReference(reference))
                throw new ArgumentOutOfRangeException(nameof(reference));
            
            byteReference = reference;
        }

        public SwarmReference(string reference)
        {
            ArgumentNullException.ThrowIfNull(reference, nameof(reference));
            
            try
            {
                byteReference = reference.HexToByteArray();
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid reference", nameof(reference));
            }
            
            if (!IsValidReference(byteReference))
                throw new ArgumentOutOfRangeException(nameof(reference));
        }
        
        public SwarmReference(
            SwarmHash hash,
            EncryptionKey256? encryptionKey)
        {
            if (encryptionKey.HasValue)
            {
                var bytes = new byte[EncryptedSize];
                hash.ToReadOnlyMemory().CopyTo(bytes.AsMemory()[..PlainSize]);
                encryptionKey.Value.ToReadOnlyMemory().CopyTo(bytes.AsMemory()[PlainSize..]);
                byteReference = bytes;
            }
            else
            {
                byteReference = hash.ToReadOnlyMemory();
            }
        }
        
        // Static builders.
        public static async Task<SwarmReference> ResolveFromAddressAsync(
            SwarmAddress address,
            IReadOnlyChunkStore chunkStore) =>
            (await address.ResolveToResourceInfoAsync(
                chunkStore, ManifestPathResolver.IdentityResolver).ConfigureAwait(false)).Result.Reference;
        
        public static async Task<SwarmReference> ResolveFromStringAsync(
            string referenceOrAddress,
            IReadOnlyChunkStore chunkStore)
        {
            if (SwarmHash.IsValidHash(referenceOrAddress))
                return new SwarmReference(SwarmHash.FromString(referenceOrAddress), null);
            return (await SwarmAddress.FromString(referenceOrAddress).ResolveToResourceInfoAsync(
                    chunkStore, ManifestPathResolver.IdentityResolver).ConfigureAwait(false))
                .Result.Reference;
        }

        // Properties.
        public EncryptionKey256? EncryptionKey =>
            IsEncrypted ? new EncryptionKey256(byteReference[PlainSize..]) : (EncryptionKey256?)null;
        public SwarmHash Hash => new(byteReference[..PlainSize]);
        public bool IsEncrypted => byteReference.Length == EncryptedSize;
        public int Size => byteReference.Length;
        
        // Static properties.
        public static SwarmReference Zero { get; } = SwarmHash.Zero;
        
        // Methods.
        public bool Equals(SwarmReference other) => byteReference.Span.SequenceEqual(other.byteReference.Span);
        public override bool Equals(object? obj) => obj is SwarmReference other && Equals(other);
        public override int GetHashCode() => ByteArrayComparer.Current.GetHashCode(byteReference.ToArray());
        public byte[] ToByteArray() => byteReference.ToArray();
        public ReadOnlyMemory<byte> ToReadOnlyMemory() => byteReference;
        public override string ToString() => byteReference.ToArray().ToHex();
        
        // Static methods.
        public static SwarmReference FromByteArray(byte[] value) => new(value);
        public static SwarmReference FromString(string value) => new(value);
        public static SwarmReference FromSwarmHash(SwarmHash hash) => new(hash, null);
        public static bool IsValidReference(ReadOnlyMemory<byte> value) => value.Length is PlainSize or EncryptedSize;
        public static bool IsValidReference(string value)
        {
            try
            {
                return IsValidReference(value.HexToByteArray());
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public static SwarmReference Parse(string s, IFormatProvider? provider) => FromString(s);
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out SwarmReference result)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                result = default;
                return false;
            }

#pragma warning disable CA1031
            try
            {
                result = FromString(s);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
#pragma warning restore CA1031
        }
        
        // Operator methods.
        public static bool operator ==(SwarmReference left, SwarmReference right) => left.Equals(right);
        public static bool operator !=(SwarmReference left, SwarmReference right) => !(left == right);
        
        // Implicit conversion operator methods.
        public static implicit operator SwarmReference(string value) => new(value);
        public static implicit operator SwarmReference(byte[] value) => new(value);
        public static implicit operator SwarmReference(SwarmHash hash) => new(hash, null);
        
        // Explicit conversion operator methods.
        public static explicit operator string(SwarmReference value) => value.ToString();
        public static explicit operator ReadOnlyMemory<byte>(SwarmReference value) => value.ToReadOnlyMemory();
        public static explicit operator byte[](SwarmReference value) => value.ToByteArray();
    }
}