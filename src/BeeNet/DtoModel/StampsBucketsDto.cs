using System.Collections.Generic;

namespace Etherna.BeeNet.DtoModel
{
    public class StampsBucketsDto
    {
        public StampsBucketsDto(
            int depth, 
            int bucketDepth, 
            int bucketUpperBound, 
            ICollection<BucketsDto>? buckets)
        {
            Depth = depth;
            BucketDepth = bucketDepth;
            BucketUpperBound = bucketUpperBound;
            Buckets = buckets;
        }

        public int Depth { get; set; } = default!;
        public int BucketDepth { get; set; } = default!;
        public int BucketUpperBound { get; set; } = default!;
        public ICollection<BucketsDto>? Buckets { get; set; } = default!;
    }
    public class BucketsDto
    {
        public int BucketId { get; set; } = default!;

        public BucketsDto(int bucketId, int collisions)
        {
            BucketId = bucketId;
            Collisions = collisions;
        }

        public int Collisions { get; set; } = default!;

    }
}
