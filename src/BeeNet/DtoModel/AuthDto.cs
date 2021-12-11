namespace Etherna.BeeNet.DtoModel
{
    public class AuthDto
    {
        public AuthDto(Clients.v1_4.GatewayApi.Response response)
        {
            if (response is null)
            {
                return;
            }

            Key = response.Key;
        }

        public string Key { get; set; } = default!;
    }
}
