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

namespace Etherna.BeeNet.DtoModel
{
    public class VersionDto
    {
        // Constructors.
        public VersionDto(Clients.DebugApi.v1_2_0.Response14 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Status = response.Status;
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }
        public VersionDto(Clients.DebugApi.v1_2_0.Response18 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Status = response.Status;
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        public VersionDto(Clients.DebugApi.v1_2_0.Response24 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Status = response.Status;
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        public VersionDto(Clients.GatewayApi.v2_0_0.Response4 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Status = response.Status;
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        public VersionDto(Clients.GatewayApi.v2_0_0.Response9 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Status = response.Status;
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }


        // Properties.
        public string Status { get; }
        public string Version { get; }
        public string ApiVersion { get; }
        public string DebugApiVersion { get; }
    }
}
