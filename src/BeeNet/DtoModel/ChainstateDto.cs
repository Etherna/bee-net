namespace Etherna.BeeNet.DtoModel
{
    public class ChainstateDto
    {
        public ChainstateDto(
            int block, 
            int totalAmount, 
            int currentPrice)
        {
            Block = block;
            TotalAmount = totalAmount;
            CurrentPrice = currentPrice;
        }

        public int Block { get; set; } = default!;
        public int TotalAmount { get; set; } = default!;
        public int CurrentPrice { get; set; } = default!;
    }
}
