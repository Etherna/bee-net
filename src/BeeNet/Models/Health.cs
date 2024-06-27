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

namespace Etherna.BeeNet.Models
{
    public sealed class Health
    {
        // Constructors.
        internal Health(Clients.Response9 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            StatusIsOk = response.Status switch
            {
                Clients.Response9Status.Ok => true,
                Clients.Response9Status.Nok => false,
                _ => throw new InvalidOperationException()
            };
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        internal Health(Clients.Response21 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            StatusIsOk = response.Status switch
            {
                Clients.Response21Status.Ok => true,
                Clients.Response21Status.Nok => false,
                _ => throw new InvalidOperationException()
            };
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        internal Health(Clients.Response40 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            StatusIsOk = response.Status switch
            {
                Clients.Response40Status.Ok => true,
                Clients.Response40Status.Nok => false,
                _ => throw new InvalidOperationException()
            };
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        // Properties.
        public bool StatusIsOk { get; }
        public string Version { get; }
        public string ApiVersion { get; }
        public string DebugApiVersion { get; }
    }
}
