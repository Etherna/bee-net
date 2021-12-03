#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class ReservestateDto : BaseDto
    {
        public ReservestateDto(int radius, int available, string outer, string inner, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Radius = radius;
            Available = available;
            Outer = outer;
            Inner = inner;
        }

        public int Radius { get; set; }

        public int Available { get; set; }

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Outer { get; set; }

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Inner { get; set; }
    }
}

#pragma warning restore CA2227