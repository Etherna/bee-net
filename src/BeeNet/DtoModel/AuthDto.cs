using System;

namespace Etherna.BeeNet.DtoModel
{
    public class AuthDto
    {
        // Constructors.
        public AuthDto(Clients.v1_4_1.GatewayApi.Response response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Key = response.Key;
        }


        // Properties.
        public string Key { get; }
    }
}
