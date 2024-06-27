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

using Epoche;
using Etherna.BeeNet.Extensions;
using Etherna.BeeNet.Hasher.Signer;
using Etherna.BeeNet.Hasher.Store;
using Etherna.BeeNet.Models;
using System;
using System.Linq;

namespace Etherna.BeeNet.Hasher.Postage
{
    internal class PostageStamper(
        ISigner signer,
        IPostageStampIssuer stampIssuer,
        IStampStore stampStore)
        : IPostageStamper
    {
        // Properties.
        public ISigner Signer { get; } = signer;
        public IPostageStampIssuer StampIssuer { get; } = stampIssuer;
        public IStampStore StampStore { get; } = stampStore;

        // Methods.
        public PostageStamp Stamp(SwarmHash hash)
        {
            var item = new StampStoreItem(
                StampIssuer.PostageBatch.Id,
                hash);

            if (StampStore.TryGet(item.StoreKey, out var storedItem))
                item = storedItem;
            else
                item.StampBucketIndex = StampIssuer.IncrementBucketCount(hash);

            if (item.StampBucketIndex is null)
                throw new InvalidOperationException();

            item.BucketTimestamp = DateTimeOffset.UtcNow;
            
            StampStore.Put(item);

            var toSignDigest = ToSignDigest(
                hash,
                StampIssuer.PostageBatch.Id,
                item.StampBucketIndex!,
                item.BucketTimestamp.Value);

            var signature = Signer.Sign(toSignDigest);

            return new PostageStamp(
                StampIssuer.PostageBatch.Id,
                item.StampBucketIndex!,
                item.BucketTimestamp.Value,
                signature);
        }

        // Helpers.
        /// <summary>
        /// Creates a digest to represent the stamp which is to be signed by the owner
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="batchId"></param>
        /// <param name="stampBucketIndex"></param>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        private static byte[] ToSignDigest(SwarmHash hash, PostageBatchId batchId, StampBucketIndex stampBucketIndex, DateTimeOffset timeStamp) =>
            Keccak256.ComputeHash(
                hash.ToByteArray()
                    .Concat(batchId.ToByteArray())
                    .Concat(stampBucketIndex.ToByteArray())
                    .Concat(timeStamp.ToUnixTimeMilliseconds().UnixDateTimeToByteArray()).ToArray());
    }
}