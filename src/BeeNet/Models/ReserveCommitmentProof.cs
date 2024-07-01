// Copyright 2021-present Etherna SA
// This file is part of Bee.Net.
// 
// Bee.Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Bee.Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Bee.Net.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.BeeNet.Clients;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.Models
{
    public sealed class ReserveCommitmentProof
    {
        // Constructors.
        internal ReserveCommitmentProof(Proof1 proof)
        {
            ArgumentNullException.ThrowIfNull(proof, nameof(proof));
            
            ChunkSpan = proof.ChunkSpan;
            PostageProof = new PostageProof(proof.PostageProof);
            ProofSegments = proof.ProofSegments ?? Array.Empty<string>();
            ProofSegments2 = proof.ProofSegments2 ?? Array.Empty<string>();
            ProofSegments3 = proof.ProofSegments3 ?? Array.Empty<string>();
            ProveSegment = proof.ProveSegment;
            ProveSegment2 = proof.ProveSegment2;
            SocProof = (proof.SocProof ?? Array.Empty<Clients.SocProof>()).Select(p => new SocProof(p));
        }

        internal ReserveCommitmentProof(Proof2 proof)
        {
            ArgumentNullException.ThrowIfNull(proof, nameof(proof));
            
            ChunkSpan = proof.ChunkSpan;
            PostageProof = new PostageProof(proof.PostageProof);
            ProofSegments = proof.ProofSegments ?? Array.Empty<string>();
            ProofSegments2 = proof.ProofSegments2 ?? Array.Empty<string>();
            ProofSegments3 = proof.ProofSegments3 ?? Array.Empty<string>();
            ProveSegment = proof.ProveSegment;
            ProveSegment2 = proof.ProveSegment2;
            SocProof = (proof.SocProof ?? Array.Empty<SocProof2>()).Select(p => new SocProof(p));
        }

        internal ReserveCommitmentProof(ProofLast proof)
        {
            ArgumentNullException.ThrowIfNull(proof, nameof(proof));
            
            ChunkSpan = proof.ChunkSpan;
            PostageProof = new PostageProof(proof.PostageProof);
            ProofSegments = proof.ProofSegments ?? Array.Empty<string>();
            ProofSegments2 = proof.ProofSegments2 ?? Array.Empty<string>();
            ProofSegments3 = proof.ProofSegments3 ?? Array.Empty<string>();
            ProveSegment = proof.ProveSegment;
            ProveSegment2 = proof.ProveSegment2;
            SocProof = (proof.SocProof ?? Array.Empty<SocProof3>()).Select(p => new SocProof(p));
        }
        
        // Properties.
        public int ChunkSpan { get; }
        public PostageProof PostageProof { get; }
        public IEnumerable<string> ProofSegments { get; }
        public IEnumerable<string> ProofSegments2 { get; }
        public IEnumerable<string> ProofSegments3 { get; }
        public string ProveSegment { get; }
        public string ProveSegment2 { get; }
        public IEnumerable<SocProof> SocProof { get; }

    }
}