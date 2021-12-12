using System;

namespace Etherna.BeeNet.DtoModel
{
    public class TransactionHashDto
    {
        // Constructors.
        public TransactionHashDto(Clients.v1_4.DebugApi.Response26 response26)
        {
            if (response26 is null)
                throw new ArgumentNullException(nameof(response26));

            TransactionHash = response26.TransactionHash;
        }

        public TransactionHashDto(Clients.v1_4.DebugApi.Response29 response29)
        {
            if (response29 is null)
                throw new ArgumentNullException(nameof(response29));

            TransactionHash = response29.TransactionHash;
        }

        public TransactionHashDto(Clients.v1_4.DebugApi.Response34 response34)
        {
            if (response34 is null)
                throw new ArgumentNullException(nameof(response34));

            TransactionHash = response34.TransactionHash;
        }

        public TransactionHashDto(Clients.v1_4.DebugApi.Response35 response35)
        {
            if (response35 is null)
                throw new ArgumentNullException(nameof(response35));

            TransactionHash = response35.TransactionHash;
        }


        // Properties.
        public string TransactionHash { get; }
    }
}
