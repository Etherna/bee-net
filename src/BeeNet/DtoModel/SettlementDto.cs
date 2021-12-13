using System;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModel
{
    public class SettlementDto
    {
        // Constructors.
        public SettlementDto(Clients.v1_4.DebugApi.Response20 response20)
        {
            if (response20 is null)
                throw new ArgumentNullException(nameof(response20));

            TotalReceived = response20.TotalReceived;
            TotalSent = response20.TotalSent;
            Settlements = response20.Settlements
                .Select(i => new SettlementDataDto(i));
        }


        // Properties.
        public int TotalReceived { get; }
        public int TotalSent { get; }
        public IEnumerable<SettlementDataDto> Settlements { get; }
    }
}
