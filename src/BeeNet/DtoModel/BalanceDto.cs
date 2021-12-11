namespace Etherna.BeeNet.DtoModel
{
    public class BalanceDto
    {
        public BalanceDto(Clients.v1_4.DebugApi.Balances balance)
        {
            if (balance is null)
            {
                return;
            }

            Peer = balance.Peer;
            Balance = balance.Balance;
        }

        public BalanceDto(Clients.v1_4.DebugApi.Balances2 balance)
        {
            if (balance is null)
            {
                return;
            }

            Peer = balance.Peer;
            Balance = balance.Balance;
        }

        public string Peer { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Balance { get; set; } = default!;
    }
}
