using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel
{
    public class ResultChequebookDto
    {
        public ResultChequebookDto(Clients.v1_4.DebugApi.Result result)
        {
            if (result is null)
            {
                return;
            }

            Recipient = result.Recipient;
            LastPayout = result.LastPayout;
            Bounced = result.Bounced;
        }

        public string Recipient { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string LastPayout { get; set; } = default!;

        public bool Bounced { get; set; } = default!;
    }
}
