using System;

namespace Etherna.BeeNet.DtoModel
{
    public class VersionDto
    {
        // Constructors.
        public VersionDto(Clients.v1_4.DebugApi.Response14 response14)
        {
            if (response14 is null)
                throw new ArgumentNullException(nameof(response14));

            Status = response14.Status;
            Version = response14.Version;
            ApiVersion = response14.ApiVersion;
            DebugApiVersion = response14.DebugApiVersion;
        }
        public VersionDto(Clients.v1_4.DebugApi.Response18 response18)
        {
            if (response18 is null)
                throw new ArgumentNullException(nameof(response18));

            Status = response18.Status;
            Version = response18.Version;
            ApiVersion = response18.ApiVersion;
            DebugApiVersion = response18.DebugApiVersion;
        }

        public VersionDto(Clients.v1_4.DebugApi.Response24 response24)
        {
            if (response24 is null)
                throw new ArgumentNullException(nameof(response24));

            Status = response24.Status;
            Version = response24.Version;
            ApiVersion = response24.ApiVersion;
            DebugApiVersion = response24.DebugApiVersion;
        }

        public VersionDto(Clients.v1_4.GatewayApi.Response4 response4)
        {
            if (response4 is null)
                throw new ArgumentNullException(nameof(response4));

            Status = response4.Status;
            Version = response4.Version;
            ApiVersion = response4.ApiVersion;
            DebugApiVersion = response4.DebugApiVersion;
        }

        public VersionDto(Clients.v1_4.GatewayApi.Response9 response9)
        {
            if (response9 is null)
                throw new ArgumentNullException(nameof(response9));

            Status = response9.Status;
            Version = response9.Version;
            ApiVersion = response9.ApiVersion;
            DebugApiVersion = response9.DebugApiVersion;
        }


        // Properties.
        public string Status { get; }
        public string Version { get; }
        public string ApiVersion { get; }
        public string DebugApiVersion { get; }
    }
}
