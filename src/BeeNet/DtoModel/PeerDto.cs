namespace Etherna.BeeNet.DtoModel.DebugApi
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
