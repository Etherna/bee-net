using System;

namespace Etherna.BeeNet.DtoModel
{
    public class ReferenceDto
    {
        // Constructors.
        public ReferenceDto(Clients.v1_4.GatewayApi.Response3 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Reference = response.Reference;
        }

        public ReferenceDto(Clients.v1_4.GatewayApi.Response5 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Reference = response.Reference;
        }

        public ReferenceDto(Clients.v1_4.GatewayApi.Response14 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Reference = response.Reference;
        }

        public ReferenceDto(Clients.v1_4.GatewayApi.Response15 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Reference = response.Reference;
        }

        public ReferenceDto(Clients.v1_4.GatewayApi.Response16 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Reference = response.Reference;
        }


        // Properties.
        public string Reference { get; } 
    }
}
