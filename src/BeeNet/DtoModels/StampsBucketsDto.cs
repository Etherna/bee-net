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

namespace Etherna.BeeNet.DtoModels
{
    public class StampsBucketsDto
    {
        // Constructors.
        public StampsBucketsDto(Clients.DebugApi.V1_2_0.Response38 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Depth = response.Depth;
            BucketDepth = response.BucketDepth;
            BucketUpperBound = response.BucketUpperBound;
            Buckets = (response.Buckets ?? Array.Empty<Clients.DebugApi.V1_2_0.Buckets>()).Select(i => new BucketDto(i));
        }

        public StampsBucketsDto(Clients.DebugApi.V1_2_1.Response39 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Depth = response.Depth;
            BucketDepth = response.BucketDepth;
            BucketUpperBound = response.BucketUpperBound;
            Buckets = (response.Buckets ?? Array.Empty<Clients.DebugApi.V1_2_1.Buckets>()).Select(i => new BucketDto(i));
        }

        public StampsBucketsDto(Clients.DebugApi.V2_0_0.Response39 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Depth = response.Depth;
            BucketDepth = response.BucketDepth;
            BucketUpperBound = response.BucketUpperBound;
            Buckets = response.Buckets.Select(i => new BucketDto(i));
        }

        public StampsBucketsDto(Clients.DebugApi.V2_0_1.Response40 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Depth = response.Depth;
            BucketDepth = response.BucketDepth;
            BucketUpperBound = response.BucketUpperBound;
            Buckets = response.Buckets.Select(i => new BucketDto(i));
        }

        public StampsBucketsDto(Clients.GatewayApi.V3_0_2.Response54 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Depth = response.Depth;
            BucketDepth = response.BucketDepth;
            BucketUpperBound = response.BucketUpperBound;
            Buckets = response.Buckets.Select(i => new BucketDto(i));
        }

        // Properties.
        public int Depth { get; }
        public int BucketDepth { get; }
        public int BucketUpperBound { get; }
        public IEnumerable<BucketDto> Buckets { get; }
    }
}
