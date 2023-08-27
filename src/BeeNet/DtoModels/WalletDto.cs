using System;

namespace Etherna.BeeNet.DtoModels
{
    public class WalletDto
    {
        internal WalletDto(Clients.DebugApi.V4_0_0.Response32 response32)
        {
            if (response32 is null)
                throw new ArgumentNullException(nameof(response32));

            Bzz = response32.BzzBalance;
            NativeTokenBalance = response32.NativeTokenBalance;
        }

        public string Bzz { get; }
        public string NativeTokenBalance { get; }
    }
}
