#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class ReadinessDto : BaseDto
    {
        public ReadinessDto(string status, string version, string apiVersion, string debugApiVersion, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Status = status;
            Version = version;
            ApiVersion = apiVersion;
            DebugApiVersion = debugApiVersion;
        }

        public string Status { get; set; }

        public string Version { get; set; }

        /// <summary>The default value is set in case the bee binary was not build correctly.</summary>
        public string ApiVersion { get; set; } = "0.0.0";

        /// <summary>The default value is set in case the bee binary was not build correctly.</summary>
        public string DebugApiVersion { get; set; } = "0.0.0";

    }
}

#pragma warning restore CA2227