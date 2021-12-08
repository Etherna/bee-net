using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel
{
    public class BucketDto
    {
        public BucketDto(int bucketId, int collisions)
        {
            BucketId = bucketId;
            Collisions = collisions;
        }

        public int BucketId { get; set; } = default!;

        public int Collisions { get; set; } = default!;
    }
}
