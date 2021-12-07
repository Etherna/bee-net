using System.Collections.Generic;

namespace Etherna.BeeNet.DtoModel.DebugApi
{
    public class AddressesDto
    {
        public AddressesDto(
            string overlay, 
            ICollection<string> underlay, 
            string ethereum, 
            string publicKey, 
            string pssPublicKey)
        {
            Overlay = overlay;
            Underlay = underlay;
            Ethereum = ethereum;
            PublicKey = publicKey;
            PssPublicKey = pssPublicKey;
        }

        public string Overlay { get; set; } = default!;
        
        public ICollection<string> Underlay { get; set; } = default!;
        
        public string Ethereum { get; set; } = default!;
        
        public string PublicKey { get; set; } = default!;
        
        public string PssPublicKey { get; set; } = default!;
    }
}
