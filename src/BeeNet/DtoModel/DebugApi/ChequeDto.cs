#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class ChequeDto : BaseDto
    {
        public ChequeDto(string peer, LastreceivedDto lastreceived, LastsentDto lastsent, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Peer = peer;
            Lastreceived = lastreceived;
            Lastsent = lastsent;
        }

        public string Peer { get; set; }

        public LastreceivedDto Lastreceived { get; set; }

        public LastsentDto Lastsent { get; set; }
    }
}

#pragma warning restore CA2227