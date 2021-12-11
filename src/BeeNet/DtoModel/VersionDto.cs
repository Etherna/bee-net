namespace Etherna.BeeNet.DtoModel
{
    public class VersionDto
    {
        public VersionDto(Clients.v1_4.DebugApi.Response14 response14)
        {
            if (response14 == null)
            {
                return;
            }

            Status = response14.Status;
            Version = response14.Version;
            ApiVersion = response14.ApiVersion;
            DebugApiVersion = response14.DebugApiVersion;
        }
        public VersionDto(Clients.v1_4.DebugApi.Response18 response18)
        {
            if (response18 == null)
            {
                return;
            }

            Status = response18.Status;
            Version = response18.Version;
            ApiVersion = response18.ApiVersion;
            DebugApiVersion = response18.DebugApiVersion;
        }

        public VersionDto(Clients.v1_4.DebugApi.Response24 response24)
        {
            if (response24 == null)
            {
                return;
            }

            Status = response24.Status;
            Version = response24.Version;
            ApiVersion = response24.ApiVersion;
            DebugApiVersion = response24.DebugApiVersion;
        }

        public VersionDto(Clients.v1_4.GatewayApi.Response4 response4)
        {
            if (response4 == null)
            {
                return;
            }

            Status = response4.Status;
            Version = response4.Version;
            ApiVersion = response4.ApiVersion;
            DebugApiVersion = response4.DebugApiVersion;
        }

        public VersionDto(Clients.v1_4.GatewayApi.Response9 response9)
        {
            if (response9 == null)
            {
                return;
            }

            Status = response9.Status;
            Version = response9.Version;
            ApiVersion = response9.ApiVersion;
            DebugApiVersion = response9.DebugApiVersion;
        }
        


        public string Status { get; set; } = default!;

        public string Version { get; set; } = default!;

        /// <summary>The default value is set in case the bee binary was not build correctly.</summary>
        public string ApiVersion { get; set; } = "0.0.0";

        /// <summary>The default value is set in case the bee binary was not build correctly.</summary>
        public string DebugApiVersion { get; set; } = "0.0.0";
    }
}
