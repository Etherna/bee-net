using System;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModel
{
    public class TimesettlementsDto
    {
        // Constructors.
        public TimesettlementsDto(Clients.v1_4.DebugApi.Response21 response21)
        {
            if (response21 is null)
                throw new ArgumentNullException(nameof(response21));

            TotalReceived = response21.TotalReceived;
            TotalSent = response21.TotalSent;
            Settlements = response21.Settlements
                .Select(i => new SettlementDataDto(i))
                .ToList();
        }


        // Properties.
        public int TotalReceived { get; }
        public int TotalSent { get; }
        public IEnumerable<SettlementDataDto> Settlements { get; }
    }
}
