using System;
using System.Collections.Generic;
using System.Text;

namespace TestAdapter.Dtos.GatewayApi
{
    public class FileResponse
    {

        public int StatusCode { get; private set; }

        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; private set; }

        public System.IO.Stream Stream { get; private set; }

        public bool IsPartial
        {
            get { return StatusCode == 206; }
        }

        public FileResponse(int statusCode, IReadOnlyDictionary<string, IEnumerable<string>> headers, System.IO.Stream stream)
        {
            StatusCode = statusCode;
            Headers = headers;
            Stream = stream;
        }
    }
}
