﻿//   Copyright 2021-present Etherna Sagl
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

namespace Etherna.BeeNet.DtoModels
{
    public class BucketDto
    {
        // Constructors.
        public BucketDto(Clients.DebugApi.V1_2_0.Buckets bucket)
        {
            if (bucket is null)
                throw new ArgumentNullException(nameof(bucket));

            BucketId = bucket.BucketID;
            Collisions = bucket.Collisions;
        }

        public BucketDto(Clients.DebugApi.V1_2_1.Buckets bucket)
        {
            if (bucket is null)
                throw new ArgumentNullException(nameof(bucket));

            BucketId = bucket.BucketID;
            Collisions = bucket.Collisions;
        }

        // Properties.
        public int BucketId { get; }
        public int Collisions { get; }
    }
}
