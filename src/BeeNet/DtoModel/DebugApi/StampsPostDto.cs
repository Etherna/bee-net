#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class StampsPostDto : BaseDto
    {
        public StampsPostDto(object batchID, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            BatchID = batchID;
        }

        public object BatchID { get; set; }
    }
}

#pragma warning restore CA2227