namespace Etherna.BeeNet.DtoModel.DebugApi
{
    public class ChequebookChequeGetDto
    {
        public ChequebookChequeGetDto(
            string peer, 
            LastreceivedDto lastreceived, 
            LastsentDto lastsent)
        {
            Peer = peer;
            Lastreceived = lastreceived;
            Lastsent = lastsent;
        }

        public string Peer { get; set; } = default!;
        
        public LastreceivedDto Lastreceived { get; set; } = default!;
        
        public LastsentDto Lastsent { get; set; } = default!;
    }
}
