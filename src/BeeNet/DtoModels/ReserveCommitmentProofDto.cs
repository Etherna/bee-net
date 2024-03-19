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
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModels
{
    public class ReserveCommitmentProofDto
    {
        // Constructors.
        public ReserveCommitmentProofDto(Proof1 proof)
        {
            ArgumentNullException.ThrowIfNull(proof, nameof(proof));
            
            ChunkSpan = proof.ChunkSpan;
            PostageProof = new PostageProofDto(proof.PostageProof);
            ProofSegments = proof.ProofSegments ?? Array.Empty<string>();
            ProofSegments2 = proof.ProofSegments2 ?? Array.Empty<string>();
            ProofSegments3 = proof.ProofSegments3 ?? Array.Empty<string>();
            ProveSegment = proof.ProveSegment;
            ProveSegment2 = proof.ProveSegment2;
            SocProof = (proof.SocProof ?? Array.Empty<SocProof>()).Select(p => new SocProofDto(p));
        }

        public ReserveCommitmentProofDto(Proof2 proof)
        {
            ArgumentNullException.ThrowIfNull(proof, nameof(proof));
            
            ChunkSpan = proof.ChunkSpan;
            PostageProof = new PostageProofDto(proof.PostageProof);
            ProofSegments = proof.ProofSegments ?? Array.Empty<string>();
            ProofSegments2 = proof.ProofSegments2 ?? Array.Empty<string>();
            ProofSegments3 = proof.ProofSegments3 ?? Array.Empty<string>();
            ProveSegment = proof.ProveSegment;
            ProveSegment2 = proof.ProveSegment2;
            SocProof = (proof.SocProof ?? Array.Empty<SocProof2>()).Select(p => new SocProofDto(p));
        }

        public ReserveCommitmentProofDto(ProofLast proof)
        {
            ArgumentNullException.ThrowIfNull(proof, nameof(proof));
            
            ChunkSpan = proof.ChunkSpan;
            PostageProof = new PostageProofDto(proof.PostageProof);
            ProofSegments = proof.ProofSegments ?? Array.Empty<string>();
            ProofSegments2 = proof.ProofSegments2 ?? Array.Empty<string>();
            ProofSegments3 = proof.ProofSegments3 ?? Array.Empty<string>();
            ProveSegment = proof.ProveSegment;
            ProveSegment2 = proof.ProveSegment2;
            SocProof = (proof.SocProof ?? Array.Empty<SocProof3>()).Select(p => new SocProofDto(p));
        }
        
        // Properties.
        public int ChunkSpan { get; }
        public PostageProofDto PostageProof { get; }
        public IEnumerable<string> ProofSegments { get; }
        public IEnumerable<string> ProofSegments2 { get; }
        public IEnumerable<string> ProofSegments3 { get; }
        public string ProveSegment { get; }
        public string ProveSegment2 { get; }
        public IEnumerable<SocProofDto> SocProof { get; }

    }
}