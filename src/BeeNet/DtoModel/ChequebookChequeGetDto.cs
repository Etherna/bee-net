namespace Etherna.BeeNet.DtoModel
{
    public class ChequebookChequeGetDto
    {
        public ChequebookChequeGetDto(Clients.v1_4.DebugApi.Response27 response27)
        {
            if (response27 is null)
            {
                return;
            }

            Peer = response27.Peer;
            Lastreceived = new LastreceivedDto(response27.Lastreceived);
            Lastsent = new LastsentDto(response27.Lastsent);
        }

        public ChequebookChequeGetDto(Clients.v1_4.DebugApi.Lastcheques lastcheques)
        {
            if (lastcheques is null)
            {
                return;
            }

            Peer = lastcheques.Peer;
            Lastreceived = new LastreceivedDto(lastcheques.Lastreceived);
            Lastsent = new LastsentDto(lastcheques.Lastsent);
        }

        public string Peer { get; set; } = default!;
        
        public LastreceivedDto Lastreceived { get; set; } = default!;
        
        public LastsentDto Lastsent { get; set; } = default!;
    }
}
