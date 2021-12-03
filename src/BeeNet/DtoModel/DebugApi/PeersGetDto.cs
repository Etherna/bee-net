#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class PeersGetDto : BaseDto
    {
        public PeersGetDto(ICollection<Peers2Dto> peers, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Peers = peers;
        }

        public ICollection<Peers2Dto> Peers { get; set; }
    }
}

#pragma warning restore CA2227