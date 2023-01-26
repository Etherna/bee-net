using System;
using System.Globalization;

namespace Etherna.BeeNet.DtoModels
{
    public class WalletDto
    {
        internal WalletDto(Clients.DebugApi.V4_0_0.Response33 response33)
        {
            if (response33 is null)
                throw new ArgumentNullException(nameof(response33));

            Bzz = response33.BzzBalance;
            NativeTokenBalance = response33.NativeTokenBalance;
        }

        public string Bzz { get; }
        public string NativeTokenBalance { get; }
    }
}
