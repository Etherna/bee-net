namespace Etherna.BeeNet.DtoModel
{
    public class RefreshDto
    {
        public RefreshDto(Clients.v1_4.GatewayApi.Response2 response2)
        {
            if (response2 == null)
            {
                return;
            }

            Key = response2.Key;
        }

        public string Key { get; set; } = default!;
    }
}
