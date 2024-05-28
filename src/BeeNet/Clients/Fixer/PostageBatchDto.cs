//   Copyright 2021-present Etherna SA
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Etherna.BeeNet.Clients.Fixer
{
    public class PostageBatchDto
    {
        /// <summary>
        /// Internal debugging property. It indicates if the batch is expired.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("exists")]
        public bool Exists { get; set; } = default!;

        /// <summary>
        /// The time (in seconds) remaining until the batch expires; -1 signals that the batch never expires; 0 signals that the batch has already expired.
        /// </summary>

        [System.Text.Json.Serialization.JsonPropertyName("batchTTL")]
        public long BatchTTL { get; set; } = default!;

        [System.Text.Json.Serialization.JsonPropertyName("batchID")]
        public string BatchID { get; set; } = default!;

        [System.Text.Json.Serialization.JsonPropertyName("utilization")]
        public int Utilization { get; set; } = default!;

        /// <summary>
        /// Indicate that the batch was discovered by the Bee node, but it awaits enough on-chain confirmations before declaring the batch as usable.
        /// </summary>

        [System.Text.Json.Serialization.JsonPropertyName("usable")]
        public bool Usable { get; set; } = default!;

        [System.Text.Json.Serialization.JsonPropertyName("label")]
        public string Label { get; set; } = default!;

        [System.Text.Json.Serialization.JsonPropertyName("depth")]
        public int Depth { get; set; } = default!;

        /// <summary>
        /// Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)
        /// </summary>

        [System.Text.Json.Serialization.JsonPropertyName("amount")]
        public string Amount { get; set; } = default!;

        [System.Text.Json.Serialization.JsonPropertyName("bucketDepth")]
        public int BucketDepth { get; set; } = default!;

        [System.Text.Json.Serialization.JsonPropertyName("blockNumber")]
        public int BlockNumber { get; set; } = default!;

        [System.Text.Json.Serialization.JsonPropertyName("immutableFlag")]
        public bool ImmutableFlag { get; set; } = default!;
    }
}
