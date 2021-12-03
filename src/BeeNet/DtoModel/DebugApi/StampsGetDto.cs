#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class StampsGetDto : BaseDto
    {
        public StampsGetDto(ICollection<StampsDto> stamps, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Stamps = stamps;
        }

        public ICollection<StampsDto> Stamps { get; set; }
    }
}

#pragma warning restore CA2227