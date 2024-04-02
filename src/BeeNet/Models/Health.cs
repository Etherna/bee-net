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
    public class Health
    {
        // Constructors.
        internal Health(Clients.DebugApi.Response18 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Status = response.Status switch
            {
                Clients.DebugApi.Response18Status.Ok => StatusValues.Ok,
                Clients.DebugApi.Response18Status.Nok => StatusValues.Nok,
                _ => throw new InvalidOperationException()
            };
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        internal Health(Clients.DebugApi.Response24 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Status = response.Status switch
            {
                Clients.DebugApi.Response24Status.Ok => StatusValues.Ok,
                Clients.DebugApi.Response24Status.Nok => StatusValues.Nok,
                _ => throw new InvalidOperationException()
            };
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        internal Health(Clients.GatewayApi.Response9 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Status = response.Status switch
            {
                Clients.GatewayApi.Response9Status.Ok => StatusValues.Ok,
                Clients.GatewayApi.Response9Status.Nok => StatusValues.Nok,
                _ => throw new InvalidOperationException()
            };
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        internal Health(Clients.GatewayApi.Response21 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Status = response.Status switch
            {
                Clients.GatewayApi.Response21Status.Ok => StatusValues.Ok,
                Clients.GatewayApi.Response21Status.Nok => StatusValues.Nok,
                _ => throw new InvalidOperationException()
            };
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        internal Health(Clients.GatewayApi.Response40 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Status = response.Status switch
            {
                Clients.GatewayApi.Response40Status.Ok => StatusValues.Ok,
                Clients.GatewayApi.Response40Status.Nok => StatusValues.Nok,
                _ => throw new InvalidOperationException()
            };
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
            if (response.AdditionalProperties.TryGetValue("reference", out object? reference))
                Reference = reference.ToString();
        }

        // Properties.
        public StatusValues Status { get; }
        public string Version { get; }
        public string ApiVersion { get; }
        public string DebugApiVersion { get; }
        public string? Reference { get; }
    }
}
