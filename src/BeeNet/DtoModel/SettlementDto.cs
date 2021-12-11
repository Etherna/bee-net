using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModel
{
    public class SettlementDto
    {
        public SettlementDto(Clients.v1_4.DebugApi.Response20 response20)
        {
            if (response20 is null)
            {
                return;
            }

            TotalReceived = response20.TotalReceived;
            TotalSent = response20.TotalSent;
            Settlements = response20.Settlements
                ?.Select(i => new SettlementDataDto(i))
                ?.ToList();
        }

        public int TotalReceived { get; set; } = default!;
        
        public int TotalSent { get; set; } = default!;
        
        public ICollection<SettlementDataDto>? Settlements { get; set; } = default!;
    }
}
