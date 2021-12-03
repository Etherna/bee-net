#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class SettlementsDto : BaseDto
    {
        public SettlementsDto(
            string peer, 
            int received, 
            int sent, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Peer = peer;
            Received = received;
            Sent = sent;
        }

        public string Peer { get; set; }

        public int Received { get; set; }

        public int Sent { get; set; }
    }
}

#pragma warning restore CA2227