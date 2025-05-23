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
using Nethereum.Signer;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Etherna.BeeNet.Models
{
    [TypeConverter(typeof(PostageStampTypeConverter))]
    public class PostageStamp(
        PostageBatchId batchId,
        PostageBucketIndex bucketIndex,
        DateTimeOffset timeStamp,
        ReadOnlyMemory<byte> signature)
    {
        // Consts.
        public const int StampSize = 113;
        
        // Static builders.
        public static PostageStamp BuildFromByteArray(ReadOnlyMemory<byte> bytes)
        {
            if (bytes.Length != StampSize)
                throw new ArgumentOutOfRangeException(nameof(bytes), "Invalid stamp length");

            var batchId = new PostageBatchId(bytes[..32]);
            var stampBucketIndex = PostageBucketIndex.BuildFromByteArray(bytes.Span[32..40]);
            var timeStamp = bytes.Span[40..48].UnixTimeNanosecondsToDateTimeOffset();
            var signature = bytes[48..];

            return new PostageStamp(batchId, stampBucketIndex, timeStamp, signature);
        }
        
        // Properties.
        public PostageBatchId BatchId { get; } = batchId;
        public PostageBucketIndex BucketIndex { get; } = bucketIndex;
        public DateTimeOffset TimeStamp { get; } = timeStamp;
        public ReadOnlyMemory<byte> Signature => signature;
        
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

        // Methods.
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
    }
}