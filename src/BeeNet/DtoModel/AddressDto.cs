namespace Etherna.BeeNet.DtoModel
{
    public class AddressDto
    {
        public AddressDto(string address)
        {
            Address = address;
        }

        public string Address { get; set; }
    }
}
