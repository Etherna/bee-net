namespace Etherna.BeeNet.DtoModel
{
    public class ChequebookAddressDto
    {
        public ChequebookAddressDto(Clients.v1_4.DebugApi.Response7 response7)
        {
            if (response7 is null)
            {
                return;
            }

            ChequebookAddress = response7.ChequebookAddress;
        }
        
        public string ChequebookAddress { get; set; } = default!;
    }
}
