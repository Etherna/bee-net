#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class CashoutGetDto : BaseDto
    {
        public CashoutGetDto(string peer, LastCashedChequeDto lastCashedCheque, string transactionHash, ResultDto result, string uncashedAmount, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Peer = peer;
            LastCashedCheque = lastCashedCheque;
            TransactionHash = transactionHash;
            Result = result;
            UncashedAmount = uncashedAmount;
        }

        public string Peer { get; set; }

        public LastCashedChequeDto LastCashedCheque { get; set; }

        public string TransactionHash { get; set; }

        public ResultDto Result { get; set; }

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string UncashedAmount { get; set; }
    }
}

#pragma warning restore CA2227