using System;

namespace Etherna.BeeNet.DtoModel
{
    public class ChainStateDto
    {
        // Constructors.
        public ChainStateDto(Clients.v1_4_1.DebugApi.Response13 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Block = response.Block;
            TotalAmount = response.TotalAmount;
            CurrentPrice = response.CurrentPrice;
        }


        // Properties.
        public int Block { get; }
        public int TotalAmount { get; }
        public int CurrentPrice { get; }
    }
}
