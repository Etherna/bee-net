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

namespace Etherna.BeeNet.Clients.Bee.Fixer
{
    public class PostageBatchDto
    {
        /// <summary>
        /// Internal debugging property. It indicates if the batch is expired.
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("exists")]
        public bool Exists { get; set; }

        /// <summary>
        /// The time (in seconds) remaining until the batch expires; -1 signals that the batch never expires; 0 signals that the batch has already expired.
        /// </summary>

        [System.Text.Json.Serialization.JsonPropertyName("batchTTL")]
        public long BatchTTL { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("batchID")]
        public required string BatchID { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("utilization")]
        public uint Utilization { get; set; }

        /// <summary>
        /// Indicate that the batch was discovered by the Bee node, but it awaits enough on-chain confirmations before declaring the batch as usable.
        /// </summary>

        [System.Text.Json.Serialization.JsonPropertyName("usable")]
        public bool Usable { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("label")]
        public string? Label { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("depth")]
        public int? Depth { get; set; }

        /// <summary>
        /// Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)
        /// </summary>

        [System.Text.Json.Serialization.JsonPropertyName("amount")]
        public string? Amount { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("bucketDepth")]
        public int BucketDepth { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("blockNumber")]
        public ulong BlockNumber { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("immutableFlag")]
        public bool ImmutableFlag { get; set; }
    }
}
