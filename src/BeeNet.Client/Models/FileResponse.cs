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
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    public sealed class FileResponse(
        HttpContentHeaders? contentHeaders,
        IReadOnlyDictionary<string, IEnumerable<string>> headers,
        Stream stream)
        : IDisposable, IAsyncDisposable
    {
        // Dispose.
        public void Dispose() => Stream.Dispose();
        public ValueTask DisposeAsync() => Stream.DisposeAsync();

        // Properties.
        public bool IsFeed => Headers.ContainsKey("Swarm-Feed-Index");
        public HttpContentHeaders? ContentHeaders { get; } = contentHeaders;
        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; } = headers;
        public Stream Stream { get; } = stream;
    }
}
