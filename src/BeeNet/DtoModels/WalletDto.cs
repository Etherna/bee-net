using System;

namespace Etherna.BeeNet.DtoModels
{
    public class WalletDto
    {
        public WalletDto(Clients.DebugApi.V3_2_0.Response33 response33)
        {
            if (response33 is null)
                throw new ArgumentNullException(nameof(response33));

            Bzz = response33.Bzz;
            XDai = response33.XDai;
        }

        public string Bzz { get; }
        public string XDai { get; }
    }
}
