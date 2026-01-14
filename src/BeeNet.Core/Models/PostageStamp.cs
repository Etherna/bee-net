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
using Etherna.BeeNet.TypeConverters;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Models
{
    [TypeConverter(typeof(PostageStampTypeConverter))]
    public readonly struct PostageStamp(
        PostageBatchId batchId,
        PostageBucketIndex bucketIndex,
        DateTimeOffset timeStamp,
        ReadOnlyMemory<byte> signature)
        : IEquatable<PostageStamp>, IParsable<PostageStamp>
    {
        // Consts.
        public const int StampSize = 113;
        
        // Properties.
        public PostageBatchId BatchId { get; } = batchId;
        public PostageBucketIndex BucketIndex { get; } = bucketIndex;
        public DateTimeOffset TimeStamp { get; } = timeStamp;
        public ReadOnlyMemory<byte> Signature => signature;

        // Methods.
        public bool Equals(PostageStamp other) =>
            BatchId.Equals(other.BatchId) &&
            BucketIndex.Equals(other.BucketIndex) &&
            TimeStamp.Equals(other.TimeStamp) &&
            Signature.Span.SequenceEqual(other.Signature.Span);
        
        public override bool Equals(object? obj) => obj is PostageStamp other && Equals(other);
        
        public override int GetHashCode() => HashCode.Combine(BatchId, BucketIndex, TimeStamp, Signature);
        
        /// <summary>
        /// Returns ethereum address that signed postage batch
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="hasher"></param>
        /// <returns></returns>
        public EthAddress RecoverBatchOwner(SwarmHash hash, Hasher hasher)
        {
            var signer = new EthereumMessageSigner();
            var toSign = ToSignDigest(hash, hasher);
            return signer.EcRecover(toSign, new EthECDSASignature(signature.ToArray()));
        }
        
        public byte[] ToByteArray()
        {
            List<byte> buffer = [];
            buffer.AddRange(BatchId.ToReadOnlyMemory().Span);
            buffer.AddRange(BucketIndex.ToByteArray());
            buffer.AddRange(TimeStamp.ToUnixTimeNanosecondsByteArray());
            buffer.AddRange(Signature.Span);
            return buffer.ToArray();
        }

        public byte[] ToSignDigest(SwarmHash hash, Hasher hasher) =>
            BuildSignDigest(hash, BatchId, BucketIndex, TimeStamp, hasher);

        public override string ToString() => ToByteArray().ToHex();

        // Static methods.
        public static byte[] BuildSignDigest(
            SwarmHash hash,
            PostageBatchId batchId,
            PostageBucketIndex bucketIndex,
            DateTimeOffset timeStamp,
            Hasher hasher)
        {
            ArgumentNullException.ThrowIfNull(hasher, nameof(hasher));
            ArgumentNullException.ThrowIfNull(bucketIndex, nameof(bucketIndex));

            return hasher.ComputeHash(
            [
                hash.ToReadOnlyMemory(),
                batchId.ToReadOnlyMemory(),
                bucketIndex.ToByteArray(),
                timeStamp.ToUnixTimeNanosecondsByteArray()
            ]);
        }
        public static PostageStamp FromByteArray(byte[] value) => FromByteArray(value.AsMemory());
        public static PostageStamp FromByteArray(ReadOnlyMemory<byte> value)
        {
            if (value.Length != StampSize)
                throw new ArgumentOutOfRangeException(nameof(value), "Invalid stamp length");

            var batchId = new PostageBatchId(value[..32]);
            var stampBucketIndex = PostageBucketIndex.BuildFromByteArray(value.Span[32..40]);
            var timeStamp = value.Span[40..48].UnixTimeNanosecondsToDateTimeOffset();
            var signature = value[48..];

            return new PostageStamp(batchId, stampBucketIndex, timeStamp, signature);
        }
        public static PostageStamp FromString(string value) => FromByteArray(value.HexToByteArray());
        public static PostageStamp Parse(string s, IFormatProvider? provider) => FromString(s);
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out PostageStamp result)
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
        public static bool operator ==(PostageStamp left, PostageStamp right) => left.Equals(right);
        public static bool operator !=(PostageStamp left, PostageStamp right) => !(left == right);
    }
}