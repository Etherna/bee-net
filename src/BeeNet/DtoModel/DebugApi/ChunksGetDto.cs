#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class ChunksGetDto : BaseDto
    {
        public ChunksGetDto(string message, int code, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Message = message;
            Code = code;
        }

        public string Message { get; set; }

        public int Code { get; set; }
    }
}

#pragma warning restore CA2227