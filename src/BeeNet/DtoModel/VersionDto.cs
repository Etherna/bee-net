using System;

namespace Etherna.BeeNet.DtoModel
{
    public class VersionDto
    {
        // Constructors.
        public VersionDto(Clients.v1_4_1.DebugApi.Response14 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Status = response.Status;
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }
        public VersionDto(Clients.v1_4_1.DebugApi.Response18 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Status = response.Status;
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        public VersionDto(Clients.v1_4_1.DebugApi.Response24 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Status = response.Status;
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        public VersionDto(Clients.v1_4_1.GatewayApi.Response4 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Status = response.Status;
            Version = response.Version;
            ApiVersion = response.ApiVersion;
            DebugApiVersion = response.DebugApiVersion;
        }

        public VersionDto(Clients.v1_4_1.GatewayApi.Response9 response)
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
