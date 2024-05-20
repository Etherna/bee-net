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

using System;

namespace Etherna.BeeNet.Models
{
    public class SwarmChunk
    {
        // Fields.
        private readonly byte[] _data;
        
        // Consts.
        public const int ChunkWithSpanSize = Size + SpanSize;
        public const int BmtSegments = 128;
        public const int BmtSegmentSize = 32; //Keccak hash size
        public const int Size = BmtSegmentSize * BmtSegments;
        public const int SpanSize = 8;
        
        // Constructor.
        public SwarmChunk(SwarmAddress address, byte[] data)
        {
            Address = address;
            _data = data;
        }
        
        // Properties.
        public SwarmAddress Address { get; }
        public ReadOnlySpan<byte> Data => _data;
    }
}