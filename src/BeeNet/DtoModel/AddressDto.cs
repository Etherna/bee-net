using System;

namespace Etherna.BeeNet.DtoModel
{
    public class AddressDto
    {
        // Constructors.
        public AddressDto(Clients.v1_4_1.DebugApi.Peers? peer)
        {
            if (peer is null)
                throw new ArgumentNullException(nameof(peer));

            Address = peer.Address;
        }

        public AddressDto(Clients.v1_4_1.DebugApi.Peers2? peers)
        {
            if (peers is null)
                throw new ArgumentNullException(nameof(peers));

            Address = peers.Address;
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
