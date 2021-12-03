#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class StampsDto : Anonymous3Dto
    {
        public StampsDto(
            bool exists, 
            int batchTTL, 
            object batchId, 
            int utilization, 
            bool usable,
            string label, 
            int depth, 
            string amount,
            int bucketDepth, 
            int blockNumber, 
            bool immutableFlag, 
            IDictionary<string, object> additionalProperties)
            : base (batchId, utilization, usable, label, depth, amount, bucketDepth, blockNumber, immutableFlag, additionalProperties)
        {
            Exists = exists;
            BatchTTL = batchTTL;
        }

        public bool Exists { get; set; }

        /// <summary>The time (in seconds) remaining until the batch expires; -1 signals that the batch never expires; 0 signals that the batch has already expired.</summary>
        public int BatchTTL { get; set; }
    }
}

#pragma warning restore CA2227