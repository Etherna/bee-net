using System;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModel
{
    public class TimeSettlementsDto
    {
        // Constructors.
        public TimeSettlementsDto(Clients.v1_4.DebugApi.Response21 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            TotalReceived = response.TotalReceived;
            TotalSent = response.TotalSent;
            Settlements = response.Settlements
                .Select(i => new SettlementDataDto(i));
        }


        // Properties.
        public int TotalReceived { get; }
        public int TotalSent { get; }
        public IEnumerable<SettlementDataDto> Settlements { get; }
    }
}
