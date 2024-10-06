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

using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    public class ChunkUploaderWebSocket(
        WebSocket webSocket)
        : IDisposable
    {
        // Fields.
        private readonly byte[] responseBuffer = new byte[SwarmHash.HashSize]; //not really used
        
        // Dispose.
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                webSocket.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        // Methods.
        public virtual async Task CloseAsync()
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
        
        public virtual async Task SendChunkAsync(
            byte[] chunkPayload,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(chunkPayload, nameof(chunkPayload));
            
            await webSocket.SendAsync(chunkPayload, WebSocketMessageType.Binary, true, cancellationToken).ConfigureAwait(false);
            var response = await webSocket.ReceiveAsync(responseBuffer, CancellationToken.None).ConfigureAwait(false);

            if (response.MessageType == WebSocketMessageType.Close)
                throw new OperationCanceledException(
                    $"Connection closed by server, message: {response.CloseStatusDescription}");
        }

        public virtual Task SendChunkAsync(
            SwarmChunk chunk,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(chunk, nameof(chunk));
            return SendChunkAsync(chunk.GetSpanAndData(), cancellationToken);
        }

        public virtual async Task SendChunksAsync(
            SwarmChunk[] chunkBatch,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(chunkBatch, nameof(chunkBatch));
            
            foreach (var (chunk, i) in chunkBatch.Select((c, i) => (c, i)))
                await SendChunkAsync(chunk, cancellationToken).ConfigureAwait(false);
        }
    }
}