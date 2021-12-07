namespace Etherna.BeeNet.DtoModel.DebugApi
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
