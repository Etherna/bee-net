//   Copyright 2021-present Etherna SA
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    public sealed class FileResponse : IDisposable, IAsyncDisposable
    {
        // Constructors.
        internal FileResponse(Clients.GatewayApi.FileResponse response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Stream = response.Stream;
            IsFeed = response.Headers.ContainsKey("Swarm-Feed-Index");
        }
        
        // Dispose.
        public void Dispose() => Stream.Dispose();
        public ValueTask DisposeAsync() => Stream.DisposeAsync();

        // Properties.
        public bool IsFeed { get; }
        public Stream Stream { get; }

    }
}
