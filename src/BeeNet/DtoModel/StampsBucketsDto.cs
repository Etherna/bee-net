using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModel
{
    public class StampsBucketsDto
    {
        public StampsBucketsDto(Clients.v1_4.DebugApi.Response38 response38)
        {
            if (response38 == null)
            {
                return;
            }

            Depth = response38.Depth;
            BucketDepth = response38.BucketDepth;
            BucketUpperBound = response38.BucketUpperBound;
            Buckets = response38.Buckets?.Select(i => new BucketDto(i))?.ToList();
        }

        public int Depth { get; set; } = default!;
        public int BucketDepth { get; set; } = default!;
        public int BucketUpperBound { get; set; } = default!;
        public ICollection<BucketDto>? Buckets { get; set; } = default!;
    }
}
