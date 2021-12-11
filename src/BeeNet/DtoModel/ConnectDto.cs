namespace Etherna.BeeNet.DtoModel
{
    public class ConnectDto
    {
        public ConnectDto(Clients.v1_4.DebugApi.Response11 response11)
        {
            if (response11 is null)
            {
                return;
            }

            Address = response11.Address;
        }
        
        public string Address { get; set; } = default!;
    }
}
