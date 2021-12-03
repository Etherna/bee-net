#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class ChequeDto : BaseDto
    {
        public ChequeDto(
            string peer, 
            LastreceivedDto? lastReceived, 
            LastsentDto? lastSent, 
            IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Peer = peer;
            LastReceived = lastReceived;
            LastSent = lastSent;
        }

        public string Peer { get; set; }

        public LastreceivedDto? LastReceived { get; set; }

        public LastsentDto? LastSent { get; set; }
    }
}

#pragma warning restore CA2227