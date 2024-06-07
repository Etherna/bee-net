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

namespace Etherna.BeeNet.Manifest
{
    [System.Flags]
    public enum NodeType
    {
        None = 0,
        Value = 2,
        Edge = 4,
        WithPathSeparator = 8,
        WithMetadata = 16
    }

    public static class NodeTypeFlagExtensions
    {
        public static bool HasFlag(this NodeType value, NodeType flag)
        {
            return (value & flag) != 0;
        }
    }
}