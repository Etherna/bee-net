using System;
using System.Globalization;

namespace Etherna.BeeNet.DtoModels
{
    public class WalletDto
    {
        internal WalletDto(Clients.DebugApi.V4_0_0.Response34 response33)
        {
            if (response33 is null)
                throw new ArgumentNullException(nameof(response33));

            Bzz = long.Parse(response33.BzzBalance, CultureInfo.InvariantCulture);
            NativeTokenBalance = long.Parse(response33.NativeTokenBalance, CultureInfo.InvariantCulture);
        }

        public long Bzz { get; }
        public long NativeTokenBalance { get; }
    }
}
