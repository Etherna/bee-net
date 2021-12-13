using System;

namespace Etherna.BeeNet.DtoModel
{
    public class RefreshDto
    {
        // Constructors.
        public RefreshDto(Clients.v1_4.GatewayApi.Response2 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Key = response.Key;
        }


        // Properties.
        public string Key { get; } 
    }
}
