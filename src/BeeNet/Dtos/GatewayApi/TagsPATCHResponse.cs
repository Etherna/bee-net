using System;
using System.Collections.Generic;
using System.Text;

namespace TestAdapter.Dtos.GatewayApi
{
    public class TagsPATCHResponse : BaseResponse
    {
        public string Status { get; set; }

        public string Version { get; set; }

        /// <summary>The default value is set in case the bee binary was not build correctly.</summary>
        public string ApiVersion { get; set; } = "0.0.0";

        /// <summary>The default value is set in case the bee binary was not build correctly.</summary>
        public string DebugApiVersion { get; set; } = "0.0.0";
    }
}
