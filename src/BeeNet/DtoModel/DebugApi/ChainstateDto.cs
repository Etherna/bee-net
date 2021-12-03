#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class ChainstateDto : BaseDto
    {
        public ChainstateDto(
            int block, 
            int totalAmount, 
            int currentPrice, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Block = block;
            TotalAmount = totalAmount;
            CurrentPrice = currentPrice;
        }

        public int Block { get; set; }

        public int TotalAmount { get; set; }

        public int CurrentPrice { get; set; }
    }
}

#pragma warning restore CA2227