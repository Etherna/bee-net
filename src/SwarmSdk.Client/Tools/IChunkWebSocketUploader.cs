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
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.SwarmSdk.Tools
{
    public interface IChunkWebSocketUploader : IDisposable
    {
        Task CloseAsync();

        Task SendChunkAsync(
            ReadOnlyMemory<byte> chunkPayload,
            CancellationToken cancellationToken);

        Task SendChunkAsync(
            SwarmChunk chunk,
            CancellationToken cancellationToken);

        Task SendChunkBatchAsync(
            SwarmChunk[] chunkBatch,
            bool isLastBatch,
            CancellationToken cancellationToken = default);
    }
}