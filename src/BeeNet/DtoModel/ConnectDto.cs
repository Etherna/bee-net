using System;

namespace Etherna.BeeNet.DtoModel
{
    public class ConnectDto
    {
        // Constructors.
        public ConnectDto(Clients.v1_4.DebugApi.Response11 response11)
        {
            if (response11 is null)
                throw new ArgumentNullException(nameof(response11));

            Address = response11.Address;
        }


        // Properties.
        public string Address { get; }
    }
}
