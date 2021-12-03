#pragma warning disable CA2227 // Disable "CS2227 Collection properties should be read only"

using System;
using System.Collections.Generic;
using System.Text;

namespace Etherna.BeeNet.DtoModel.Debug
{
    public class AddressesDto
    {
        public AddressesDto(string overlay, ICollection<string> underlay, string ethereum, string publicKey, string pssPublicKey)
        {
            Overlay = overlay;
            Underlay = underlay;
            Ethereum = ethereum;
            PublicKey = publicKey;
            PssPublicKey = pssPublicKey;
        }

        public string Overlay { get; set; }

        public ICollection<string> Underlay { get; set; }

        public string Ethereum { get; set; }

        public string PublicKey { get; set; }

        public string PssPublicKey { get; set; }
    }
}

#pragma warning restore CA2227