namespace Etherna.BeeNet.DtoModel
{
    public class SettlementDataDto
    {
        public SettlementDataDto(Clients.v1_4.DebugApi.Settlements settlement)
        {
            if (settlement == null)
            {
                return;
            }

            Peer = settlement.Peer;
            Received = settlement.Received;
            Sent = settlement.Sent;
        }

        public SettlementDataDto(Clients.v1_4.DebugApi.Settlements2 settlements2)
        {
            if (settlements2 == null)
            {
                return;
            }

            Peer = settlements2.Peer;
            Received = settlements2.Received;
            Sent = settlements2.Sent;
        }
        

        public string Peer { get; set; } = default!;
        
        public int Received { get; set; } = default!;
        
        public int Sent { get; set; } = default!;
    }
}
