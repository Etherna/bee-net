namespace Etherna.BeeNet.DtoModel
{
    public class StewardshipGetDto
    {
        public StewardshipGetDto(Clients.v1_4.GatewayApi.Response17 response17)
        {
            if (response17 is null)
            {
                return;
            }

            IsRetrievable = response17.IsRetrievable;
        }

        public bool IsRetrievable { get; set; } = default!;
    }
}
