using System;

namespace Etherna.BeeNet.DtoModel
{
    public class ConnectDto
    {
        // Constructors.
        public ConnectDto(Clients.v1_4.DebugApi.Response11 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Address = response.Address;
        }


        // Properties.
        public string Address { get; }
    }
}
