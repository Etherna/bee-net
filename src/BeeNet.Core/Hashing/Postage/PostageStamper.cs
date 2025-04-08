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

using Etherna.BeeNet.Hashing.Signer;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.Hashing.Postage
{
    public sealed class PostageStamper : IPostageStamper
    {
        // Fields.
        private readonly Hasher hasher = new Hasher(); //init hasher instance, because stamper will always need its own
        private readonly Dictionary<SwarmHash, PostageStamp> presignedPostageStamps;
        private readonly bool storePresignedPostageStamps;
        
        // Constructor.
        public PostageStamper(
            ISigner signer,
            IPostageStampIssuer stampIssuer,
            IStampStore stampStore,
            IDictionary<SwarmHash, PostageStamp>? presignedPostageStamps = null,
            bool storePresignedPostageStamps = false)
        {
            Signer = signer;
            StampIssuer = stampIssuer;
            StampStore = stampStore;
            this.storePresignedPostageStamps = storePresignedPostageStamps;

            this.presignedPostageStamps = (presignedPostageStamps ?? new Dictionary<SwarmHash, PostageStamp>())
                .Where(s => s.Value.BatchId == stampIssuer.PostageBatch.Id)
                .ToDictionary(s => s.Key, s => s.Value);
        }

        // Properties.
        public ISigner Signer { get; }
        public IPostageStampIssuer StampIssuer { get; }
        public IStampStore StampStore { get; }

        // Methods.
        public PostageStamp Stamp(SwarmHash hash)
        {
            StampStoreItem item;

            // If match with a presigned chunk, verify and take it.
            if (presignedPostageStamps.TryGetValue(hash, out var presignedStamp))
            {
                if (StampIssuer.PostageBatchOwner == null)
                    throw new InvalidOperationException("Batch owner can't be null with presigned stamps");
                
                // Verify signer address.
                EthAddress signerAddress;
                lock (hasher)
                {
                    signerAddress = presignedStamp.RecoverBatchOwner(hash, hasher);
                }
                if (signerAddress != StampIssuer.PostageBatchOwner)
                    throw new ArgumentException("Invalid postage stamp signature");
                
                // Store if required.
                if (storePresignedPostageStamps)
                {
                    lock (StampStore)
                    {
                        if (StampStore.TryGet(StampStoreItem.BuildId(StampIssuer.PostageBatch.Id, hash), out var storedItem))
                            item = storedItem;
                        else
                            item = new StampStoreItem(
                                StampIssuer.PostageBatch.Id,
                                hash,
                                presignedStamp.BucketIndex);

                        item.BucketTimestamp = DateTimeOffset.UtcNow;

                        StampStore.Put(item);
                    }
                }

                return presignedStamp;
            }
            
            lock (StampStore)
            {
                if (StampStore.TryGet(StampStoreItem.BuildId(StampIssuer.PostageBatch.Id, hash), out var storedItem))
                    item = storedItem;
                else
                    item = new StampStoreItem(
                        StampIssuer.PostageBatch.Id,
                        hash,
                        StampIssuer.IncrementBucketCount(hash));

                item.BucketTimestamp = DateTimeOffset.UtcNow;

                StampStore.Put(item);
            }

            byte[] toSignDigest;
            lock (hasher)
            {
                toSignDigest = PostageStamp.BuildSignDigest(
                    hash,
                    StampIssuer.PostageBatch.Id,
                    item.StampBucketIndex,
                    item.BucketTimestamp.Value,
                    hasher);
            }

            var signature = Signer.Sign(toSignDigest);

            return new PostageStamp(
                StampIssuer.PostageBatch.Id,
                item.StampBucketIndex!,
                item.BucketTimestamp.Value,
                signature);
        }
    }
}