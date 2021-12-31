using System;

namespace Etherna.BeeNet.DtoModel
{
    public class TransactionHashDto
    {
        // Constructors.
        public TransactionHashDto(Clients.v1_4_1.DebugApi.Response26 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            TransactionHash = response.TransactionHash;
        }

        public TransactionHashDto(Clients.v1_4_1.DebugApi.Response29 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            TransactionHash = response.TransactionHash;
        }

        public TransactionHashDto(Clients.v1_4_1.DebugApi.Response34 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            TransactionHash = response.TransactionHash;
        }

        public TransactionHashDto(Clients.v1_4_1.DebugApi.Response35 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            TransactionHash = response.TransactionHash;
        }


        // Properties.
        public string TransactionHash { get; }
    }
}
