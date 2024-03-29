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

namespace Etherna.BeeNet.DtoModels
{
    public class PostageProofDto
    {
        internal PostageProofDto(PostageProof postageProof)
        {
            ArgumentNullException.ThrowIfNull(postageProof, nameof(postageProof));
            
            Index = postageProof.Index;
            PostageId = postageProof.PostageId;
            Signature = postageProof.Signature;
            TimeStamp = postageProof.TimeStamp;
        }

        internal PostageProofDto(PostageProof2 postageProof)
        {
            ArgumentNullException.ThrowIfNull(postageProof, nameof(postageProof));
            
            Index = postageProof.Index;
            PostageId = postageProof.PostageId;
            Signature = postageProof.Signature;
            TimeStamp = postageProof.TimeStamp;
        }

        internal PostageProofDto(PostageProof3 postageProof)
        {
            ArgumentNullException.ThrowIfNull(postageProof, nameof(postageProof));
            
            Index = postageProof.Index;
            PostageId = postageProof.PostageId;
            Signature = postageProof.Signature;
            TimeStamp = postageProof.TimeStamp;
        }

        // Properties.
        public string Index { get; }
        public string PostageId { get; }
        public string Signature { get; }
        public string TimeStamp { get; }
    }
}