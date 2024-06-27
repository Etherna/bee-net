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

namespace Etherna.BeeNet.Models
{
    public sealed class ReserveCommitment
    {
        // Constructors.
        internal ReserveCommitment(Response58 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Duration = response.Duration;
            Hash = response.Hash;
            Proof1 = new ReserveCommitmentProof(response.Proofs.Proof1);
            Proof2 = new ReserveCommitmentProof(response.Proofs.Proof2);
            ProofLast = new ReserveCommitmentProof(response.Proofs.ProofLast);
        }
        
        // Properties.
        public int Duration { get; set; }
        public SwarmHash Hash { get; set; }
        public ReserveCommitmentProof Proof1 { get; set; }
        public ReserveCommitmentProof Proof2 { get; set; }
        public ReserveCommitmentProof ProofLast { get; set; }
    }
}