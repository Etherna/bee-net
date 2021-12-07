namespace Etherna.BeeNet.DtoModel
{
    public class BalanceDto
    {
        public BalanceDto(
            string peer, 
            string balance)
        {
            Peer = peer;
            Balance = balance;
        }
        
        public string Peer { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Balance { get; set; } = default!;
    }
}
