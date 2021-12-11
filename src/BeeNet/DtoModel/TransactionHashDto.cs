namespace Etherna.BeeNet.DtoModel
{
    public class TransactionHashDto
    {
        public TransactionHashDto(Clients.v1_4.DebugApi.Response26 response26)
        {
            if (response26 == null)
            {
                return;
            }

            TransactionHash = response26.TransactionHash;
        }

        public TransactionHashDto(Clients.v1_4.DebugApi.Response29 response29)
        {
            if (response29 == null)
            {
                return;
            }

            TransactionHash = response29.TransactionHash;
        }

        public TransactionHashDto(Clients.v1_4.DebugApi.Response34 response34)
        {
            if (response34 == null)
            {
                return;
            }

            TransactionHash = response34.TransactionHash;
        }

        public TransactionHashDto(Clients.v1_4.DebugApi.Response35 response35)
        {
            if (response35 == null)
            {
                return;
            }

            TransactionHash = response35.TransactionHash;
        }


        public string TransactionHash { get; set; } = default!;
    }
}
