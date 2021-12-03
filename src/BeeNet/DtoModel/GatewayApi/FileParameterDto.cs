#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.GatewayApi
{
    public class FileParameterDto : BaseDto
    {
        public FileParameterDto(System.IO.Stream data, IDictionary<string, object> additionalProperties)
            : this(data, null, null, additionalProperties)
        {
        }

        public FileParameterDto(System.IO.Stream data, string fileName, IDictionary<string, object> additionalProperties)
            : this(data, fileName, null, additionalProperties)
        {
        }

        public FileParameterDto(System.IO.Stream data, string fileName, string contentType, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
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

#pragma warning restore CA2227
