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

namespace Etherna.BeeNet.DtoModels
{
    public class VersionDto
    {
        // Constructors.
        internal VersionDto(Clients.DebugApi.Response18 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Status = response.Status switch
            {
                Clients.DebugApi.Response18Status.Ok => StatusEnumDto.Ok,
                Clients.DebugApi.Response18Status.Nok => StatusEnumDto.Nok,
                _ => throw new InvalidOperationException()
            };
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        internal VersionDto(Clients.DebugApi.Response24 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Status = response.Status switch
            {
                Clients.DebugApi.Response24Status.Ok => StatusEnumDto.Ok,
                Clients.DebugApi.Response24Status.Nok => StatusEnumDto.Nok,
                _ => throw new InvalidOperationException()
            };
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        internal VersionDto(Clients.GatewayApi.Response9 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Status = response.Status switch
            {
                Clients.GatewayApi.Response9Status.Ok => StatusEnumDto.Ok,
                Clients.GatewayApi.Response9Status.Nok => StatusEnumDto.Nok,
                _ => throw new InvalidOperationException()
            };
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        internal VersionDto(Clients.GatewayApi.Response20 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Status = response.Status switch
            {
                Clients.GatewayApi.Response20Status.Ok => StatusEnumDto.Ok,
                Clients.GatewayApi.Response20Status.Nok => StatusEnumDto.Nok,
                _ => throw new InvalidOperationException()
            };
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        internal VersionDto(Clients.GatewayApi.Response39 response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));

            Status = response.Status switch
            {
                Clients.GatewayApi.Response39Status.Ok => StatusEnumDto.Ok,
                Clients.GatewayApi.Response39Status.Nok => StatusEnumDto.Nok,
                _ => throw new InvalidOperationException()
            };
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
            if (response.AdditionalProperties.TryGetValue("reference", out object? reference))
                Reference = reference.ToString();
        }

        // Properties.
        public StatusEnumDto Status { get; }
        public string Version { get; }
        public string ApiVersion { get; }
        public string DebugApiVersion { get; }
        public string? Reference { get; }
    }
}
