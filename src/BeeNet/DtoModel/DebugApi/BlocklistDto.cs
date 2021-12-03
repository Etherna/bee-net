#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class BlocklistDto: BaseDto
    {
        public BlocklistDto(ICollection<PeersDto> peers, IDictionary<string, object> additionalProperties)
            : base(additionalProperties)
        {
            Peers = peers;
        }

        public ICollection<PeersDto> Peers { get; set; }
    }
}

#pragma warning restore CA2227