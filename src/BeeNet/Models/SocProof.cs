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

using Etherna.BeeNet.Clients.GatewayApi;
using System;

namespace Etherna.BeeNet.Models
{
    public sealed class SocProof
    {
        // Constructors.
        internal SocProof(Clients.GatewayApi.SocProof socProof)
        {
            ArgumentNullException.ThrowIfNull(socProof, nameof(socProof));
            
            ChunkAddr = socProof.ChunkAddr;
            Identifier = socProof.Identifier;
            Signature = socProof.Signature;
            Signer = socProof.Signer;
        }

        internal SocProof(SocProof2 socProof)
        {
            ArgumentNullException.ThrowIfNull(socProof, nameof(socProof));
            
            ChunkAddr = socProof.ChunkAddr;
            Identifier = socProof.Identifier;
            Signature = socProof.Signature;
            Signer = socProof.Signer;
        }

        internal SocProof(SocProof3 socProof)
        {
            ArgumentNullException.ThrowIfNull(socProof, nameof(socProof));
            
            ChunkAddr = socProof.ChunkAddr;
            Identifier = socProof.Identifier;
            Signature = socProof.Signature;
            Signer = socProof.Signer;
        }

        // Properties.
        public string ChunkAddr { get; set; }
        public string Identifier { get; set; }
        public string Signature { get; set; }
        public string Signer { get; set; }
    }
}