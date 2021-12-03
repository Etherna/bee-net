#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class BucketsDto : BaseDto
    {
        public BucketsDto(
            int bucketId, 
            int collisions, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            BucketId = bucketId;
            Collisions = collisions;
        }

        public int BucketId { get; set; }

        public int Collisions { get; set; }
    }
}

#pragma warning restore CA2227