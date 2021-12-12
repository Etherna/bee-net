using System;

namespace Etherna.BeeNet.DtoModel
{
    public class RefreshDto
    {
        // Constructors.
        public RefreshDto(Clients.v1_4.GatewayApi.Response2 response2)
        {
            if (response2 is null)
                throw new ArgumentNullException(nameof(response2));

            Key = response2.Key;
        }


        // Properties.
        public string Key { get; } 
    }
}
