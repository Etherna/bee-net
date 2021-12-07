using System.Collections.Generic;

namespace Etherna.BeeNet.DtoModel
{
    public class SettlementDto
    {
        public SettlementDto(
            int totalReceived, 
            int totalSent, 
            ICollection<SettlementDataDto>? settlements)
        {
            TotalReceived = totalReceived;
            TotalSent = totalSent;
            Settlements = settlements;
        }

        public int TotalReceived { get; set; } = default!;
        
        public int TotalSent { get; set; } = default!;
        
        public ICollection<SettlementDataDto>? Settlements { get; set; } = default!;
    }
}
