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
using System.Globalization;

namespace Etherna.BeeNet.Models
{
    public sealed class PostageProof
    {
        internal PostageProof(Clients.PostageProof postageProof)
        {
            ArgumentNullException.ThrowIfNull(postageProof, nameof(postageProof));
            
            Index = postageProof.Index;
            PostageId = postageProof.PostageId;
            Signature = postageProof.Signature;
            TimeStamp = DateTimeOffset.FromUnixTimeSeconds(
                long.Parse(postageProof.TimeStamp, CultureInfo.InvariantCulture));
        }

        internal PostageProof(PostageProof2 postageProof)
        {
            ArgumentNullException.ThrowIfNull(postageProof, nameof(postageProof));
            
            Index = postageProof.Index;
            PostageId = postageProof.PostageId;
            Signature = postageProof.Signature;
            TimeStamp = DateTimeOffset.FromUnixTimeSeconds(
                long.Parse(postageProof.TimeStamp, CultureInfo.InvariantCulture));
        }

        internal PostageProof(PostageProof3 postageProof)
        {
            ArgumentNullException.ThrowIfNull(postageProof, nameof(postageProof));
            
            Index = postageProof.Index;
            PostageId = postageProof.PostageId;
            Signature = postageProof.Signature;
            TimeStamp = DateTimeOffset.FromUnixTimeSeconds(
                long.Parse(postageProof.TimeStamp, CultureInfo.InvariantCulture));
        }

        // Properties.
        public string Index { get; }
        public PostageBatchId PostageId { get; }
        public string Signature { get; }
        public DateTimeOffset TimeStamp { get; }
    }
}