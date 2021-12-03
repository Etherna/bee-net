#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class TimesettlementsDto : BaseDto
    {
        public TimesettlementsDto(int totalReceived, int totalSent, ICollection<SettlementsDto> settlements, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            TotalReceived = totalReceived;
            TotalSent = totalSent;
            Settlements = settlements;
        }

        public int TotalReceived { get; set; }

        public int TotalSent { get; set; }

        public ICollection<SettlementsDto> Settlements { get; set; }
    }
}

#pragma warning restore CA2227