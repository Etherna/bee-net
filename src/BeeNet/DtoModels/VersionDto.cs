//   Copyright 2021-present Etherna Sagl
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
        public VersionDto(Clients.DebugApi.V2_0_1.Response15 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Status = response.Status;
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        public VersionDto(Clients.DebugApi.V2_0_1.Response19 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Status = response.Status;
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        public VersionDto(Clients.DebugApi.V2_0_1.Response25 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Status = response.Status;
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        public VersionDto(Clients.GatewayApi.V3_0_2.Response4 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Status = response.Status;
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
            if (response.AdditionalProperties.ContainsKey("reference"))
            {
                Reference = response.AdditionalProperties["reference"].ToString();
            }
        }

        public VersionDto(Clients.GatewayApi.V3_0_2.Response9 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Status = response.Status;
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        public VersionDto(Clients.GatewayApi.V3_0_2.Response19 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Status = response.Status;
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        public VersionDto(Clients.GatewayApi.V3_0_2.Response41 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Status = response.Status;
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
            if (response.AdditionalProperties.ContainsKey("reference"))
            {
                Reference = response.AdditionalProperties["reference"].ToString();
            }
        }

        // Properties.
        public string Status { get; }
        public string Version { get; }
        public string ApiVersion { get; }
        public string DebugApiVersion { get; }
        public string? Reference { get; }
    }
}
