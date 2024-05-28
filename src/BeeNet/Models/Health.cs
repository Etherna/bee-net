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
            if (response.AdditionalProperties.TryGetValue("reference", out object? reference))
                Reference = reference.ToString();
        }

        // Properties.
        public bool StatusIsOk { get; }
        public string Version { get; }
        public string ApiVersion { get; }
        public string DebugApiVersion { get; }
        public string? Reference { get; }
    }
}
