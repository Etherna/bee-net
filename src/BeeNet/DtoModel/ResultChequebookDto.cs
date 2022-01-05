using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel
{
    public class ResultChequeBookDto
    {
        // Constructors.
        public ResultChequeBookDto(Clients.v1_4_1.DebugApi.Result result)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            Recipient = result.Recipient;
            LastPayout = result.LastPayout;
            Bounced = result.Bounced;
        }


        // Properties.
        public string Recipient { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string LastPayout { get; }
        public bool Bounced { get; }
    }
}
