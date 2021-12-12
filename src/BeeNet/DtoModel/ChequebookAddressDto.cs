using System;

namespace Etherna.BeeNet.DtoModel
{
    public class ChequeBookAddressDto
    {
        // Constructors.
        public ChequeBookAddressDto(Clients.v1_4.DebugApi.Response7 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            ChequebookAddress = response.ChequebookAddress;
        }


        // Properties.
        public string ChequebookAddress { get; }
    }
}
