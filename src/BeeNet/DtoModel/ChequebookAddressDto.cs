using System;

namespace Etherna.BeeNet.DtoModel
{
    public class ChequebookAddressDto
    {
        // Constructors.
        public ChequebookAddressDto(Clients.v1_4.DebugApi.Response7 response7)
        {
            if (response7 is null)
                throw new ArgumentNullException(nameof(response7));

            ChequebookAddress = response7.ChequebookAddress;
        }


        // Properties.
        public string ChequebookAddress { get; }
    }
}
