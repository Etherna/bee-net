using System.Collections.Generic;

namespace Etherna.BeeNet.DtoModel
{
    public class StampsBucketsDto
    {
        public StampsBucketsDto(
            int depth, 
            int bucketDepth, 
            int bucketUpperBound, 
            ICollection<BucketDto>? buckets)
        {
            Depth = depth;
            BucketDepth = bucketDepth;
            BucketUpperBound = bucketUpperBound;
            Buckets = buckets;
        }

        public int Depth { get; set; } = default!;
        public int BucketDepth { get; set; } = default!;
        public int BucketUpperBound { get; set; } = default!;
        public ICollection<BucketDto>? Buckets { get; set; } = default!;
    }
}
