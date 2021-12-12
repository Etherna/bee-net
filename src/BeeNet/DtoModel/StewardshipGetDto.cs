using System;

namespace Etherna.BeeNet.DtoModel
{
    public class StewardShipGetDto
    {
        // Constructors.
        public StewardShipGetDto(Clients.v1_4.GatewayApi.Response17 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            IsRetrievable = response.IsRetrievable;
        }


        // Properties.
        public bool IsRetrievable { get; }
    }
}
