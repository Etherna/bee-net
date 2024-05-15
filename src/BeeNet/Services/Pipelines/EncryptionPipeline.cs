// Copyright 2021-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Etherna.BeeNet.Models;
using Etherna.BeeNet.Services.Putter;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Etherna.BeeNet.Services.Pipelines
{
    public class EncryptionPipeline : PipelineBase
    {
        public EncryptionPipeline(
            IPutter putter,
            RedundancyLevel redundancyLevel)
            : base(putter, redundancyLevel)
        { }

        public override ChunkFeeder StartStage { get; }

        public override int Write(byte[] bytes)
        {
            throw new System.NotImplementedException();
        }

        public override byte[] Sum()
        {
            throw new System.NotImplementedException();
        }
    }
}