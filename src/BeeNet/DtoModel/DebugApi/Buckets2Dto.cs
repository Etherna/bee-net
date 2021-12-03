#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class Buckets2Dto : BaseDto
    {
        public Buckets2Dto(int depth, int bucketDepth, int bucketUpperBound, ICollection<BucketsDto> buckets, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Depth = depth;
            BucketDepth = bucketDepth;
            BucketUpperBound = bucketUpperBound;
            Buckets = buckets;
        }

        public int Depth { get; set; }

        public int BucketDepth { get; set; }

        public int BucketUpperBound { get; set; }

        public ICollection<BucketsDto> Buckets { get; set; }
    }
}

#pragma warning restore CA2227