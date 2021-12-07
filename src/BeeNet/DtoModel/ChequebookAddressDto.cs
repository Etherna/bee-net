namespace Etherna.BeeNet.DtoModel
{
    public class ChequebookAddressDto
    {
        public ChequebookAddressDto(string chequebookAddress)
        {
            ChequebookAddress = chequebookAddress;
        }
        
        public string ChequebookAddress { get; set; } = default!;
    }
}
