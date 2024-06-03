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

using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Manifest
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public class MantarayNodeFork(
        byte[] prefix,
        MantarayNode node)
    {
        /// <summary>
        /// the non-branching part of the subpath
        /// </summary>
        public byte[] Prefix { get; } = prefix;

        /// <summary>
        /// in memory structure that represents the Node
        /// </summary>
        public MantarayNode Node { get; } = node;
    }
}