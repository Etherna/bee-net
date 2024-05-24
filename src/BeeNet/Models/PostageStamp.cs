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

using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Models
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public class PostageStamp
    {
        public PostageStamp(byte[] batchId, byte[] index, byte[] timeStamp, byte[] sig)
        {
            BatchId = batchId;
            Index = index;
            TimeStamp = timeStamp;
            Sig = sig;
        }

        /// <summary>
        /// Postage batch ID
        /// </summary>
        public byte[] BatchId { get; }

        /// <summary>
        /// index of the batch
        /// </summary>
        public byte[] Index { get; }

        /// <summary>
        /// to signal order when assigning the indexes to multiple chunks
        /// </summary>
        public byte[] TimeStamp { get; }

        /// <summary>
        /// common r[32]s[32]v[1]-style 65 byte ECDSA signature of batchID|index|address by owner or grantee
        /// </summary>
        public byte[] Sig { get; }
    }
}