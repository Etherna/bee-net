#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class BalanceDto : BaseDto
    {
        public BalanceDto(string totalBalance, string availableBalance, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            TotalBalance = totalBalance;
            AvailableBalance = availableBalance;
        }

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string TotalBalance { get; set; }

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string AvailableBalance { get; set; }
    }
}

#pragma warning restore CA2227