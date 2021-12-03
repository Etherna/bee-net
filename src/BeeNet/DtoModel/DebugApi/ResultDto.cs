#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class ResultDto : BaseDto
    {
        public ResultDto(
            string recipient, 
            string lastPayout, 
            bool bounced, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Recipient = recipient;
            LastPayout = lastPayout;
            Bounced = bounced;
        }

        public string Recipient { get; set; }

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string LastPayout { get; set; }

        public bool Bounced { get; set; }
    }
}

#pragma warning restore CA2227