using System;

namespace Etherna.BeeNet.DtoModel
{
    public class BalanceDto
    {
        // Constructors.
        public BalanceDto(Clients.v1_4.DebugApi.Balances balance)
        {
            if (balance is null)
                throw new ArgumentNullException(nameof(balance));

            Peer = balance.Peer;
            Balance = balance.Balance;
        }

        public BalanceDto(Clients.v1_4.DebugApi.Balances2 balance2)
        {
            if (balance2 is null)
                throw new ArgumentNullException(nameof(balance2));

            Peer = balance2.Peer;
            Balance = balance2.Balance;
        }


        // Properties.
        public string Peer { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Balance { get; }
    }
}
