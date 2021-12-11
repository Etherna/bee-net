namespace Etherna.BeeNet.DtoModel
{
    public class ChainstateDto
    {
        public ChainstateDto(Clients.v1_4.DebugApi.Response13 response13)
        {
            if (response13 is null)
            {
                return;
            }

            Block = response13.Block;
            TotalAmount = response13.TotalAmount;
            CurrentPrice = response13.CurrentPrice;
        }

        public int Block { get; set; } = default!;
        public int TotalAmount { get; set; } = default!;
        public int CurrentPrice { get; set; } = default!;
    }
}
