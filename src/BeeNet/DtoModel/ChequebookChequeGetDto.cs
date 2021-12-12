using System;

namespace Etherna.BeeNet.DtoModel
{
    public class ChequebookChequeGetDto
    {
        // Constructors.
        public ChequebookChequeGetDto(Clients.v1_4.DebugApi.Response27 response27)
        {
            if (response27 is null)
                throw new ArgumentNullException(nameof(response27));

            Peer = response27.Peer;
            Lastreceived = new LastreceivedDto(response27.Lastreceived);
            Lastsent = new LastsentDto(response27.Lastsent);
        }

        public ChequebookChequeGetDto(Clients.v1_4.DebugApi.Lastcheques lastcheques)
        {
            if (lastcheques is null)
                throw new ArgumentNullException(nameof(lastcheques));

            Peer = lastcheques.Peer;
            Lastreceived = new LastreceivedDto(lastcheques.Lastreceived);
            Lastsent = new LastsentDto(lastcheques.Lastsent);
        }


        // Properties.
        public string Peer { get; }
        public LastreceivedDto Lastreceived { get; }
        public LastsentDto Lastsent { get; }
    }
}
