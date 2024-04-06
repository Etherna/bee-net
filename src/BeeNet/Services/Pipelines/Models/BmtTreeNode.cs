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

namespace Etherna.BeeNet.Services.Pipelines.Models
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public class BmtTreeNode
    {
        // Constructor.
        public BmtTreeNode(int index, BmtTreeNode? parent, BmtHasher hasher)
        {
            IsLeft = index % 2 == 0;
            Parent = parent;
            Hasher = hasher;
        }

        // Properties.
        /// <summary>
        /// Whether it is left side of the parent double segment
        /// </summary>
        public bool IsLeft { get; }
        
        /// <summary>
        /// The parent node in the BMT
        /// </summary>
        public BmtTreeNode? Parent { get; }

        /// <summary>
        /// Atomic increment impl concurrent boolean toggle
        /// </summary>
        public int State { get; private set; }

        public byte[]? Left { get; set; }
        
        public byte[]? Right { get; set; }
        
        /// <summary>
        /// Preconstructed hasher on nodes
        /// </summary>
        public BmtHasher Hasher { get; }
        
        // Methods.
        /// <summary>
        /// Atomic bool toggle implementing a concurrent reusable 2-state object.
        /// Atomic addint with %2 implements atomic bool toggle.
        /// </summary>
        /// <returns>Returns true if the toggler just put it in the active/waiting state</returns>
        public bool Toggle()
        {
#pragma warning disable CA2002
            lock (this)
            {
                State++;
                return State % 2 == 1;
            }
#pragma warning restore CA2002
        }
    }
}