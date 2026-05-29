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

using System.Collections.Generic;

namespace Etherna.BeeNet.Models
{
    public sealed class ReserveCommitmentProof(
        int chunkSpan,
        PostageProof postageProof,
        IEnumerable<string> proofSegments,
        IEnumerable<string> proofSegments2,
        IEnumerable<string> proofSegments3,
        string proveSegment,
        string proveSegment2,
        IEnumerable<SocProof> socProof)
    {
        // Properties.
        public int ChunkSpan { get; } = chunkSpan;
        public PostageProof PostageProof { get; } = postageProof;
        public IEnumerable<string> ProofSegments { get; } = proofSegments;
        public IEnumerable<string> ProofSegments2 { get; } = proofSegments2;
        public IEnumerable<string> ProofSegments3 { get; } = proofSegments3;
        public string ProveSegment { get; } = proveSegment;
        public string ProveSegment2 { get; } = proveSegment2;
        public IEnumerable<SocProof> SocProof { get; } = socProof;
    }
}