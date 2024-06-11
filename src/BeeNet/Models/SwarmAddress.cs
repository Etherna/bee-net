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
    public class SwarmAddress
    {
        // Constructor.
        public SwarmAddress(SwarmHash hash, Uri? relativePath = null)
        {
            if (relativePath is not null &&
                relativePath.IsAbsoluteUri)
                throw new ArgumentException("Path needs to be relative", nameof(relativePath));
                
            Hash = hash;
            Path = relativePath;
        }
        
        // Properties.
        public SwarmHash Hash { get; }
        public Uri? Path { get; }
        
        // Methods.
        public override string ToString()
        {
            if (Path is null)
                return Hash + "/";
            
            var pathString = Path.ToString();
            if (System.IO.Path.IsPathRooted(pathString))
                return Hash + pathString;

            return Hash + "/" + pathString;
        }
    }
}