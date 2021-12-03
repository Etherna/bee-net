#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class DiluteDto : BaseDto
    {
        public DiluteDto(
            object batchId, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            BatchId = batchId;
        }

        public object BatchId { get; set; }
    }
}

#pragma warning restore CA2227