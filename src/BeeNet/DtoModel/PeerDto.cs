namespace Etherna.BeeNet.DtoModel
{
    public class PeerDto
    {
        public PeerDto(string address)
        {
            Address = address;
        }
        
        public string Address { get; set; } = default!;
    }
}
