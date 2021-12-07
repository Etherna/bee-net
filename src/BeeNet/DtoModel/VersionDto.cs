namespace Etherna.BeeNet.DtoModel
{
    public class VersionDto
    {
        public VersionDto(
            string status, 
            string version, 
            string apiVersion, 
            string debugApiVersion)
        {
            Status = status;
            Version = version;
            ApiVersion = apiVersion;
            DebugApiVersion = debugApiVersion;
        }

        public string Status { get; set; } = default!;

        public string Version { get; set; } = default!;

        /// <summary>The default value is set in case the bee binary was not build correctly.</summary>
        public string ApiVersion { get; set; } = "0.0.0";

        /// <summary>The default value is set in case the bee binary was not build correctly.</summary>
        public string DebugApiVersion { get; set; } = "0.0.0";
    }
}
