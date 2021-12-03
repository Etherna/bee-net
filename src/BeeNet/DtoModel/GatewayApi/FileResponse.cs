#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.GatewayApi
{
    public class FileResponse
    {
        public FileResponse(
            int statusCode, 
            IReadOnlyDictionary<string, IEnumerable<string>> headers, 
            System.IO.Stream stream)
        {
            StatusCode = statusCode;
            Headers = headers;
            Stream = stream;
        }

        public int StatusCode { get; private set; }

        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; private set; }

        public System.IO.Stream Stream { get; private set; }

        public bool IsPartial
        {
            get { return StatusCode == 206; }
        }
    }
}

#pragma warning restore CA2227
