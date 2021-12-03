#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class LastchequesDto : BaseDto
    {
        public LastchequesDto(string peer, Lastreceived2Dto lastreceived, Lastsent2Dto lastsent, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Peer = peer;
            Lastreceived = lastreceived;
            Lastsent = lastsent;
        }

        public string Peer { get; set; }

        public Lastreceived2Dto Lastreceived { get; set; }

        public Lastsent2Dto Lastsent { get; set; }
    }
}

#pragma warning restore CA2227