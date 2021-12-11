using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModel
{
    public class TimesettlementsDto
    {
        public TimesettlementsDto(Clients.v1_4.DebugApi.Response21 response21)
        {
            if (response21 is null)
            {
                return;
            }

            TotalReceived = response21.TotalReceived;
            TotalSent = response21.TotalSent;
            Settlements = response21.Settlements?
                .Select(i => new SettlementDataDto(i))
                .ToList();
        }
        public int TotalReceived { get; set; } = default!;

        public int TotalSent { get; set; } = default!;

        public ICollection<SettlementDataDto>? Settlements { get; set; } = default!;
    }
}
