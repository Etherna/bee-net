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
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Signer;
using Etherna.BeeNet.Store;
using System;
using System.Linq;

namespace Etherna.BeeNet.Postage
{
    public class PostageStamper : IPostageStamper
    {
        // Constructor.
        public PostageStamper(
            StampIssuerBase issuer,
            ISigner signer,
            IStore store)
        {
            Issuer = issuer;
            Signer = signer;
            Store = store;
        }
        
        // Properties.
        public StampIssuerBase Issuer { get; set; }
        public ISigner Signer { get; set; }
        public IStore Store { get; set; }
        
        // Methods.
        public PostageStamp Stamp(SwarmAddress address)
        {
            var item = new StampStoreItem(
                Issuer.BatchId,
                address);

            if (!Store.TryGet(item))
                item.BatchIndex = Issuer.Increment(address);
            item.BatchTimestamp = DateTimeOffset.UtcNow;
            Store.Put(item);

            var toSign = ToSignDigest(
                address,
                Issuer.BatchId,
                item.BatchIndex!,
                item.BatchTimestamp.Value);

            var sig = Signer.Sign(toSign);

            return new PostageStamp(Issuer.BatchId, item.BatchIndex!, item.BatchTimestamp.Value, sig);
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
        private static byte[] ToSignDigest(SwarmAddress address, PostageBatchId batchId, byte[] index, DateTimeOffset timeStamp) =>
            Keccak256.ComputeHash(
                address.ToByteArray()
                    .Concat(batchId.ToByteArray())
                    .Concat(index)
                    .Concat(timeStamp.ToUnixTimeMilliseconds().UnixDateTimeToByteArray()).ToArray());
    }
}