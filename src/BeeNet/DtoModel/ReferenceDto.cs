namespace Etherna.BeeNet.DtoModel
{
    public class ReferenceDto
    {
        public ReferenceDto(Clients.v1_4.GatewayApi.Response3 response3)
        {
            if (response3 is null)
            {
                return;
            }

            Reference = response3.Reference;
        }

        public ReferenceDto(Clients.v1_4.GatewayApi.Response5 response5)
        {
            if (response5 is null)
            {
                return;
            }

            Reference = response5.Reference;
        }

        public ReferenceDto(Clients.v1_4.GatewayApi.Response14 response14)
        {
            if (response14 is null)
            {
                return;
            }

            Reference = response14.Reference;
        }

        public ReferenceDto(Clients.v1_4.GatewayApi.Response15 response15)
        {
            if (response15 is null)
            {
                return;
            }

            Reference = response15.Reference;
        }

        public ReferenceDto(Clients.v1_4.GatewayApi.Response16 response16)
        {
            if (response16 is null)
            {
                return;
            }

            Reference = response16.Reference;
        }
        



        public string Reference { get; set; } = default!;
    }
}
