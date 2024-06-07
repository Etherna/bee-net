// Copyright 2021-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
        IStore store)
        : IPostageStamper
    {
        // Properties.
        public ISigner Signer { get; } = signer;
        public IPostageStampIssuer StampIssuer { get; } = stampIssuer;
        public IStore Store { get; } = store;

        // Methods.
        public PostageStamp Stamp(SwarmAddress address)
        {
            StoreItemBase item = new StampStoreItem(
                StampIssuer.PostageBatch.Id,
                address);

            if (Store.TryGet(item.StoreKey, out var storedItem))
                item = storedItem;
            else
                item.StampBucketIndex = StampIssuer.IncrementBucketCount(address);

            if (item.StampBucketIndex is null)
                throw new InvalidOperationException();

            item.BucketTimestamp = DateTimeOffset.UtcNow;
            
            Store.Put(item);

            var toSignDigest = ToSignDigest(
                address,
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
        /// <param name="address"></param>
        /// <param name="batchId"></param>
        /// <param name="index"></param>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        private static byte[] ToSignDigest(SwarmAddress address, PostageBatchId batchId, StampBucketIndex stampBucketIndex, DateTimeOffset timeStamp) =>
            Keccak256.ComputeHash(
                address.ToByteArray()
                    .Concat(batchId.ToByteArray())
                    .Concat(stampBucketIndex.ToByteArray())
                    .Concat(timeStamp.ToUnixTimeMilliseconds().UnixDateTimeToByteArray()).ToArray());
    }
}