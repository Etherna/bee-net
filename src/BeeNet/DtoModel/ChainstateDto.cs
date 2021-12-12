using System;

namespace Etherna.BeeNet.DtoModel
{
    public class ChainstateDto
    {
        // Constructors.
        public ChainstateDto(Clients.v1_4.DebugApi.Response13 response13)
        {
            if (response13 is null)
                throw new ArgumentNullException(nameof(response13));

            Block = response13.Block;
            TotalAmount = response13.TotalAmount;
            CurrentPrice = response13.CurrentPrice;
        }


        // Properties.
        public int Block { get; }
        public int TotalAmount { get; }
        public int CurrentPrice { get; }
    }
}
