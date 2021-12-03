#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class Balances2Dto : BaseDto
    {
        public Balances2Dto(string peer, string balance, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Peer = peer;
            Balance = balance;
        }

        public string Peer { get; set; }

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Balance { get; set; }
    }
}

#pragma warning restore CA2227