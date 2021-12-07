namespace Etherna.BeeNet.DtoModel.DebugApi
{
    public class SettlementDataDto
    {
        public SettlementDataDto(
            string peer, 
            int received, 
            int sent)
        {
            Peer = peer;
            Received = received;
            Sent = sent;
        }

        public string Peer { get; set; } = default!;
        
        public int Received { get; set; } = default!;
        
        public int Sent { get; set; } = default!;
    }
}
