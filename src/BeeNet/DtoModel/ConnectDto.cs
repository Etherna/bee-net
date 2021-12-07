namespace Etherna.BeeNet.DtoModel.DebugApi
{
    public class ConnectDto
    {
        public ConnectDto(string address)
        {
            Address = address;
        }
        
        public string Address { get; set; } = default!;
    }
}
