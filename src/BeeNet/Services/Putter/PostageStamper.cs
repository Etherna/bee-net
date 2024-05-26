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
using Etherna.BeeNet.Services.Putter.Models;
using System;
using System.Linq;

namespace Etherna.BeeNet.Services.Putter
{
    public class PostageStamper : IPostageStamper
    {
        // Properties.
        public StampIssuer Issuer { get; set; } = default!;
        public CryptoSigner Signer { get; set; } = default!;
        public IStore Store { get; set; } = default!;
        
        // Methods.
        public PostageStamp Stamp(SwarmAddress address)
        {
            var item = new StampItem(
                Issuer.BatchID!,
                address);

            if (!Store.TryGet(item))
                item.BatchIndex = Issuer.Increment(address);
            item.BatchTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().UnixDateTimeToByteArray();
            Store.Put(item);

            var toSign = ToSignDigest(
                address,
                Issuer.BatchID,
                item.BatchIndex!,
                item.BatchTimestamp);

            var sig = Signer.Sign(toSign);

            return new PostageStamp(Issuer.BatchID, item.BatchIndex!, item.BatchTimestamp, sig);
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
        private static byte[] ToSignDigest(SwarmAddress address, byte[] batchId, byte[] index, byte[] timeStamp) =>
            Keccak256.ComputeHash(
                address.ToByteArray()
                    .Concat(batchId)
                    .Concat(index)
                    .Concat(timeStamp).ToArray());
    }
}