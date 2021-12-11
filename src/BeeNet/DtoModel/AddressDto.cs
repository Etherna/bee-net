namespace Etherna.BeeNet.DtoModel
{
    public class AddressDto
    {
        public AddressDto(Clients.v1_4.DebugApi.Peers? peer)
        {
            if (peer is null)
            {
                return;
            }

            Address = peer.Address;
        }

        public AddressDto(Clients.v1_4.DebugApi.Peers2? peers2)
        {
            if (peers2 is null)
            {
                return;
            }

            Address = peers2.Address;
        }

        public AddressDto(string addresses)
        {
            Address = addresses;
        }


        public string? Address { get; set; }
    }
}
