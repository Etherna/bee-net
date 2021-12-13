using Etherna.BeeNet.DtoModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Clients.v1_4.DebugApi
{
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Version number should containt underscores")]
    public class AdapterBeeDebugVersion_1_4 : IBeeDebugClient
    {
        // Fields.
        private readonly IBeeDebugClient_1_4 beeDebugClient;


        // Constructors.
        public AdapterBeeDebugVersion_1_4(HttpClient httpClient, Uri baseUrl)
        {
            if (baseUrl is null)
                throw new ArgumentNullException(nameof(baseUrl));

            beeDebugClient = new BeeDebugClient_1_4(httpClient) { BaseUrl = baseUrl.ToString() };
        }

        public async Task<AddressDetailDto> AddressesAsync()
        {
            var response = await beeDebugClient.AddressesAsync().ConfigureAwait(false);

            return new AddressDetailDto(response);
        }

        public async Task<IEnumerable<BalanceDto>> GetBalancesAsync()
        {
            var response = await beeDebugClient.BalancesGetAsync().ConfigureAwait(false);

            return response.Balances
                .Select(i => new BalanceDto(i));
        }

        public async Task<IEnumerable<BalanceDto>> GetBalanceAsync(string address)
        {
            var response = await beeDebugClient.BalancesGetAsync().ConfigureAwait(false);

            return response.Balances
                .Select(i => new BalanceDto(i));
        }

        public async Task<IEnumerable<AddressDto>> BlocklistAsync()
        {
            var response = await beeDebugClient.BlocklistAsync().ConfigureAwait(false);

            return response.Peers
                .Select(i => new AddressDto(i));
        }

        public async Task<IEnumerable<BalanceDto>> ConsumedGetAsync()
        {
            var response = await beeDebugClient.ConsumedGetAsync().ConfigureAwait(false);

            return response.Balances
                .Select(i => new BalanceDto(i));
        }

        public async Task<IEnumerable<BalanceDto>> ConsumedGetAsync(string address)
        {
            var response = await beeDebugClient.ConsumedGetAsync().ConfigureAwait(false);

            return response.Balances
                .Select(i => new BalanceDto(i));
        }

        public async Task<ChequeBookAddressDto> ChequeBookAddressAsync()
        {
            var response = await beeDebugClient.ChequebookAddressAsync().ConfigureAwait(false);

            return new ChequeBookAddressDto(response);
        }

        public async Task<ChequeBookBalanceDto> ChequeBookBalanceAsync()
        {
            var response = await beeDebugClient.ChequebookBalanceAsync().ConfigureAwait(false);

            return new ChequeBookBalanceDto(response);
        }

        public async Task<MessageResponseDto> ChunksGetAsync(string address)
        {
            var response = await beeDebugClient.ChunksGetAsync(address).ConfigureAwait(false);

            return new MessageResponseDto(response);
        }

        public async Task<MessageResponseDto> ChunksDeleteAsync(string address)
        {
            var response = await beeDebugClient.ChunksDeleteAsync(address).ConfigureAwait(false);

            return new MessageResponseDto(response);
        }

        public async Task<ConnectDto> ConnectAsync(string multiAddress)
        {
            var response = await beeDebugClient.ConnectAsync(multiAddress).ConfigureAwait(false);

            return new ConnectDto(response);
        }

        public async Task<ReserveStateDto> ReserveStateAsync()
        {
            var response = await beeDebugClient.ReservestateAsync().ConfigureAwait(false);

            return new ReserveStateDto(response);
        }

        public async Task<ChainStateDto> ChainStateAsync()
        {
            var response = await beeDebugClient.ChainstateAsync().ConfigureAwait(false);

            return new ChainStateDto(response);
        }

        public async Task<VersionDto> HealthAsync()
        {
            var response = await beeDebugClient.HealthAsync().ConfigureAwait(false);

            return new VersionDto(response);
        }

        public async Task<IEnumerable<AddressDto>> PeersGetAsync()
        {
            var response = await beeDebugClient.PeersGetAsync().ConfigureAwait(false);

            return response.Peers
                .Select(i => new AddressDto(i));
        }

        public async Task<MessageResponseDto> PeersDeleteAsync(string address)
        {
            var response = await beeDebugClient.PeersDeleteAsync(address).ConfigureAwait(false);

            return new MessageResponseDto(response);
        }

        public async Task<PingPongDto> PingPongAsync(string peerId)
        {
            var response = await beeDebugClient.PingpongAsync(peerId).ConfigureAwait(false);

            return new PingPongDto(response);
        }

        public async Task<VersionDto> ReadinessAsync()
        {
            var response = await beeDebugClient.ReadinessAsync().ConfigureAwait(false);

            return new VersionDto(response);
        }

        public async Task<IEnumerable<SettlementDataDto>> SettlementsGetAsync(string address)
        {
            var response = await beeDebugClient.SettlementsGetAsync().ConfigureAwait(false);

            return response.Settlements
                .Select(i => new SettlementDataDto(i));
        }

        public async Task<SettlementDto> SettlementsGetAsync()
        {
            var response = await beeDebugClient.SettlementsGetAsync().ConfigureAwait(false);

            return new SettlementDto(response);
        }

        public async Task<TimeSettlementsDto> TimeSettlementsAsync()
        {
            var response = await beeDebugClient.TimesettlementsAsync().ConfigureAwait(false);

            return new TimeSettlementsDto(response);
        }

        public async Task<TopologyDto> TopologyAsync()
        {
            var response = await beeDebugClient.TopologyAsync().ConfigureAwait(false);

            return new TopologyDto(response);
        }

        public async Task<string> WelcomeMessageGetAsync()
        {
            var response = await beeDebugClient.WelcomeMessageGetAsync().ConfigureAwait(false);

            return response.WelcomeMessage;
        }

        public async Task<VersionDto> WelcomeMessagePostAsync(string welcomeMessage)
        {
            var response = await beeDebugClient.WelcomeMessagePostAsync(
                new Body
                {
                    WelcomeMessage = welcomeMessage
                }).ConfigureAwait(false);

            return new VersionDto(response);
        }

        public async Task<ChequeBookCashoutGetDto> ChequeBookCashoutGetAsync(string peerId)
        {
            var response = await beeDebugClient.ChequebookCashoutGetAsync(peerId).ConfigureAwait(false);

            return new ChequeBookCashoutGetDto(response);
        }

        public async Task<TransactionHashDto> ChequeBookCashoutPostAsync(
            string peerId, 
            int? gasPrice = null, 
            long? gasLimit = null)
        {
            var response = await beeDebugClient.ChequebookCashoutPostAsync(peerId, gasPrice, gasLimit).ConfigureAwait(false);

            return new TransactionHashDto(response);
        }

        public async Task<ChequeBookChequeGetDto> ChequeBookChequeGetAsync(string peerId)
        {
            var response = await beeDebugClient.ChequebookChequeGetAsync(peerId).ConfigureAwait(false);

            return new ChequeBookChequeGetDto(response);
        }

        public async Task<IEnumerable<ChequeBookChequeGetDto>> ChequeBookChequeGetAsync()
        {
            var response = await beeDebugClient.ChequebookChequeGetAsync().ConfigureAwait(false);

            return response.Lastcheques
                .Select(i => new ChequeBookChequeGetDto(i));
        }

        public async Task<TransactionHashDto> ChequeBookDepositAsync(
            int amount, 
            int? gasPrice = null)
        {
            var response = await beeDebugClient.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false);

            return new TransactionHashDto(response);
        }

        public async Task<TransactionHashDto> ChequeBookWithdrawAsync(
            int amount, 
            int? gasPrice = null)
        {
            var response = await beeDebugClient.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false);

            return new TransactionHashDto(response);
        }

        public async Task<TagDto> TagAsync(int uid)
        {
            var response = await beeDebugClient.TagsAsync(uid).ConfigureAwait(false);

            return new TagDto(response);
        }

        public async Task<IEnumerable<PendingTransactionDto>> TransactionsGetAsync()
        {
            var response = await beeDebugClient.TransactionsGetAsync().ConfigureAwait(false);

            return response.PendingTransactions
                .Select(i => new PendingTransactionDto(i));
        }

        public async Task<TransactionsDto> TransactionsGetAsync(string txHash)
        {
            var response = await beeDebugClient.TransactionsGetAsync(txHash).ConfigureAwait(false);

            return new TransactionsDto(response);
        }

        public async Task<TransactionHashDto> TransactionsPostAsync(string txHash)
        {
            var response = await beeDebugClient.TransactionsPostAsync(txHash).ConfigureAwait(false);

            return new TransactionHashDto(response);
        }

        public async Task<TransactionHashDto> TransactionsDeleteAsync(
            string txHash, 
            int? gasPrice = null)
        {
            var response = await beeDebugClient.TransactionsDeleteAsync(txHash).ConfigureAwait(false);

            return new TransactionHashDto(response);
        }

        public async Task<IEnumerable<StampsGetDto>> StampsGetAsync()
        {
            var response = await beeDebugClient.StampsGetAsync().ConfigureAwait(false);

            return response.Stamps
                .Select(i => new StampsGetDto(i));
        }

        public async Task<StampsGetDto> StampsGetAsync(object id)
        {
            var response = await beeDebugClient.StampsGetAsync(id).ConfigureAwait(false);

            return new StampsGetDto(response);
        }

        public async Task<StampsBucketsDto> StampsBucketsAsync(object id)
        {
            var response = await beeDebugClient.StampsBucketsAsync(id).ConfigureAwait(false);

            return new StampsBucketsDto(response);
        }

        public async Task<BatchDto> StampsPostAsync(
            int amount, 
            int depth, 
            string? label = null, 
            bool? immutable = null, 
            int? gasPrice = null)
        {
            var response = await beeDebugClient.StampsPostAsync(amount, depth, label, immutable, gasPrice).ConfigureAwait(false);

            return new BatchDto(response);
        }

        public async Task<BatchDto> StampsTopupAsync(
            object id, 
            int amount)
        {
            var response = await beeDebugClient.StampsTopupAsync(id, amount).ConfigureAwait(false);

            return new BatchDto(response);
        }

        public async Task<BatchDto> StampsDiluteAsync(
            object id, 
            int depth)
        {
            var response = await beeDebugClient.StampsDiluteAsync(id, depth).ConfigureAwait(false);

            return new BatchDto(response);
        }
    }
}