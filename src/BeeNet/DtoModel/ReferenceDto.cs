using System;

namespace Etherna.BeeNet.DtoModel
{
    public class ReferenceDto
    {
        // Constructors.
        public ReferenceDto(Clients.v1_4.GatewayApi.Response3 response3)
        {
            if (response3 is null)
                throw new ArgumentNullException(nameof(response3));

            Reference = response3.Reference;
        }

        public ReferenceDto(Clients.v1_4.GatewayApi.Response5 response5)
        {
            if (response5 is null)
                throw new ArgumentNullException(nameof(response5));

            Reference = response5.Reference;
        }

        public ReferenceDto(Clients.v1_4.GatewayApi.Response14 response14)
        {
            if (response14 is null)
                throw new ArgumentNullException(nameof(response14));

            Reference = response14.Reference;
        }

        public ReferenceDto(Clients.v1_4.GatewayApi.Response15 response15)
        {
            if (response15 is null)
                throw new ArgumentNullException(nameof(response15));

            Reference = response15.Reference;
        }

        public ReferenceDto(Clients.v1_4.GatewayApi.Response16 response16)
        {
            if (response16 is null)
                throw new ArgumentNullException(nameof(response16));

            Reference = response16.Reference;
        }


        // Properties.
        public string Reference { get; } 
    }
}
