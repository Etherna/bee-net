using System;

namespace Etherna.BeeNet.DtoModel
{
    public class ChequeBookChequeGetDto
    {
        // Constructors.
        public ChequeBookChequeGetDto(Clients.v1_4.DebugApi.Response27 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Peer = response.Peer;
            Lastreceived = new LastreceivedDto(response.Lastreceived);
            Lastsent = new LastsentDto(response.Lastsent);
        }

        public ChequeBookChequeGetDto(Clients.v1_4.DebugApi.Lastcheques lastcheques)
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
