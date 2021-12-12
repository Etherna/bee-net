using System;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModel
{
    public class AddressDetailDto
    {
        // Constructors.
        public AddressDetailDto(Clients.v1_4.DebugApi.Response response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Underlay = response.Underlay.Where(i => !string.IsNullOrWhiteSpace(i));
            Overlay = response.Overlay;
            Ethereum = response.Ethereum;
            PublicKey = response.PublicKey;
            PssPublicKey = response.PssPublicKey;
        }


        // Properties.
        public string Overlay { get; }
        public IEnumerable<string> Underlay { get; }
        public string Ethereum { get; }
        public string PublicKey { get; }
        public string PssPublicKey { get; }
    }
}
