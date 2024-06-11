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
        public PostageStamp Stamp(SwarmHash hash)
        {
            StoreItemBase item = new StampStoreItem(
                StampIssuer.PostageBatch.Id,
                hash);

            if (Store.TryGet(item.StoreKey, out var storedItem))
                item = storedItem;
            else
                item.StampBucketIndex = StampIssuer.IncrementBucketCount(hash);

            if (item.StampBucketIndex is null)
                throw new InvalidOperationException();

            item.BucketTimestamp = DateTimeOffset.UtcNow;
            
            Store.Put(item);

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