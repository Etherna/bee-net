// Copyright 2021-present Etherna SA
// This file is part of SwarmSDK.
// 
// SwarmSDK is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// SwarmSDK is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with SwarmSDK.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.SwarmSdk.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.SwarmSdk.Manifest
{
    public interface IReadOnlyMantarayNode
    {
        // Properties.
        SwarmReference? EntryReference { get; }
        SwarmReference Reference { get; }
        IReadOnlyDictionary<string, string> Metadata { get; }
        NodeType NodeTypeFlags { get; }
        EncryptionKey256? ObfuscationKey { get; }
        
        // Methods.
        Task<IReadOnlyDictionary<string, string>> GetMetadataAsync(
            string path,
            CancellationToken cancellationToken = default);
        
        Task<MantarayResourceInfo> GetResourceInfoAsync(
            string path,
            CancellationToken cancellationToken = default);
        
        Task<bool> HasPathPrefixAsync(
            string path,
            CancellationToken cancellationToken = default);
        
        Task OnVisitingAsync(CancellationToken cancellationToken = default);
    }
}