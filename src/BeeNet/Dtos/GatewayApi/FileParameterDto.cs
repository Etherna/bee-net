using System;
using System.Collections.Generic;
using System.Text;

namespace TestAdapter.Dtos.GatewayApi
{
    public class FileParameterDto
    {
        public FileParameterDto(System.IO.Stream data)
            : this(data, null, null)
        {
        }

        public FileParameterDto(System.IO.Stream data, string fileName)
            : this(data, fileName, null)
        {
        }

        public FileParameterDto(System.IO.Stream data, string fileName, string contentType)
        {
            Data = data;
            FileName = fileName;
            ContentType = contentType;
        }

        public System.IO.Stream Data { get; private set; }

        public string FileName { get; private set; }

        public string ContentType { get; private set; }
    }
}
