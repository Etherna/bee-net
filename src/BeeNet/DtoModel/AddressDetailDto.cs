using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModel
{
    public class AddressDetailDto
    {
        public AddressDetailDto(Clients.v1_4.DebugApi.Response response)
        {
            if (response is null)
            {
                return;
            }

            Underlay = response.Underlay.Where(i => !string.IsNullOrWhiteSpace(i)).ToList();
            Overlay = response.Overlay;
            Ethereum = response.Ethereum;
            PublicKey = response.PublicKey;
            PssPublicKey = response.PssPublicKey;
        }

        public string Overlay { get; set; } = default!;
        
        public ICollection<string> Underlay { get; set; } = default!;
        
        public string Ethereum { get; set; } = default!;
        
        public string PublicKey { get; set; } = default!;
        
        public string PssPublicKey { get; set; } = default!;
    }
}
