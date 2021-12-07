namespace Etherna.BeeNet.DtoModel
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
