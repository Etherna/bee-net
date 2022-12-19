using System;
using System.Globalization;

namespace Etherna.BeeNet.DtoModels
{
    public class WalletDto
    {
        internal WalletDto(Clients.DebugApi.V3_2_0.Response33 response33)
        {
            if (response33 is null)
                throw new ArgumentNullException(nameof(response33));

            Bzz = long.Parse(response33.Bzz, CultureInfo.InvariantCulture);
            XDai = long.Parse(response33.XDai, CultureInfo.InvariantCulture);
        }

        public long Bzz { get; }
        public long XDai { get; }
    }
}
