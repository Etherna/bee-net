using System;

namespace Etherna.BeeNet.DtoModel
{
    public class AddressDto
    {
        // Constructors.
        public AddressDto(Clients.v1_4.DebugApi.Peers? peer)
        {
            if (peer is null)
                throw new ArgumentNullException(nameof(peer));

            Address = peer.Address;
        }

        public AddressDto(Clients.v1_4.DebugApi.Peers2? peers2)
        {
            if (peers2 is null)
                throw new ArgumentNullException(nameof(peers2));

            Address = peers2.Address;
        }

        public AddressDto(string addresses)
        {
            if (addresses is null)
                throw new ArgumentNullException(nameof(addresses));

            Address = addresses;
        }


        // Properties.
        public string Address { get; }
    }
}
