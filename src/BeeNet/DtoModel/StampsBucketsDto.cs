using System;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModel
{
    public class StampsBucketsDto
    {
        // Constructors.
        public StampsBucketsDto(Clients.v1_4_1.DebugApi.Response38 response38)
        {
            if (response38 is null)
                throw new ArgumentNullException(nameof(response38));

            Depth = response38.Depth;
            BucketDepth = response38.BucketDepth;
            BucketUpperBound = response38.BucketUpperBound;
            Buckets = response38.Buckets?.Select(i => new BucketDto(i));
        }


        // Properties.
        public int Depth { get; }
        public int BucketDepth { get; }
        public int BucketUpperBound { get; }
        public IEnumerable<BucketDto>? Buckets { get; }
    }
}
