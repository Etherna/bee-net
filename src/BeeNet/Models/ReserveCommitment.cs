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
    public class ReserveCommitment
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
        public string Hash { get; set; }
        public ReserveCommitmentProof Proof1 { get; set; }
        public ReserveCommitmentProof Proof2 { get; set; }
        public ReserveCommitmentProof ProofLast { get; set; }
    }
}