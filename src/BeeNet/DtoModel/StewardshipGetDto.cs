using System;

namespace Etherna.BeeNet.DtoModel
{
    public class StewardshipGetDto
    {
        // Constructors.
        public StewardshipGetDto(Clients.v1_4.GatewayApi.Response17 response17)
        {
            if (response17 is null)
                throw new ArgumentNullException(nameof(response17));

            IsRetrievable = response17.IsRetrievable;
        }


        // Properties.
        public bool IsRetrievable { get; }
    }
}
