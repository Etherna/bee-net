#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class LastchequesDto : BaseDto
    {
        public LastchequesDto(
            string peer, 
            LastReceived2Dto? lastReceived, 
            Lastsent2Dto? lastSent, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Peer = peer;
            LastReceived = lastReceived;
            LastSent = lastSent;
        }

        public string Peer { get; set; }

        public LastReceived2Dto? LastReceived { get; set; }

        public Lastsent2Dto? LastSent { get; set; }
    }
}

#pragma warning restore CA2227