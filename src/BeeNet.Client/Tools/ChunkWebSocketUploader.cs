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

using Etherna.BeeNet.Models;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Tools
{
    public sealed class ChunkWebSocketUploader(
        WebSocket webSocket)
        : IChunkWebSocketUploader
    {
        // Fields.
        private readonly byte[] responseBuffer = new byte[SwarmHash.HashSize]; //not really used
        
        // Dispose.
        public void Dispose() =>
            webSocket.Dispose();
        
        // Methods.
        public async Task CloseAsync()
        {
            if (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    await webSocket.CloseOutputAsync(
                        WebSocketCloseStatus.NormalClosure,
                        null,
                        CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception e) when (e is WebSocketException or OperationCanceledException)
                { }
            }
        }
        
        public async Task SendChunkAsync(
            ReadOnlyMemory<byte> chunkPayload,
            CancellationToken cancellationToken)
        {
            await webSocket.SendAsync(chunkPayload, WebSocketMessageType.Binary, true, cancellationToken).ConfigureAwait(false);
            var response = await webSocket.ReceiveAsync(responseBuffer, CancellationToken.None).ConfigureAwait(false);

            if (response.MessageType == WebSocketMessageType.Close)
                throw new OperationCanceledException(
                    $"Connection closed by server, message: {response.CloseStatusDescription}");
        }

        public Task SendChunkAsync(
            SwarmChunk chunk,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(chunk, nameof(chunk));
            return SendChunkAsync(chunk.GetFullPayload(), cancellationToken);
        }

        public async Task SendChunkBatchAsync(
            SwarmChunk[] chunkBatch,
            bool isLastBatch,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(chunkBatch, nameof(chunkBatch));
            
            foreach (var chunk in chunkBatch)
                await SendChunkAsync(chunk, cancellationToken).ConfigureAwait(false);
        }
    }
}