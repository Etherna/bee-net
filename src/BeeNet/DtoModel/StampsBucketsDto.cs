//   Copyright 2021-present Etherna Sagl
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
