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
        readonly IBeeDebugClient_1_4 _beeDebugClient;

        public AdapterBeeDebugVersion_1_4(HttpClient httpClient, Uri? baseUrl)
        {
            if (baseUrl is null)
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }

            _beeDebugClient = new BeeDebugClient_1_4(httpClient) { BaseUrl = baseUrl.ToString() };
        }

        public async Task<AddressDetailDto> AddressesAsync()
        {
            var response = await _beeDebugClient.AddressesAsync().ConfigureAwait(false);

            return new AddressDetailDto(response);
        }

        public async Task<List<BalanceDto>?> GetBalancesAsync()
        {
            var response = await _beeDebugClient.BalancesGetAsync().ConfigureAwait(false);

            return response.Balances
                ?.Select(i => new BalanceDto(i))
                ?.ToList();
        }

        public async Task<List<BalanceDto>?> GetBalanceAsync(string address)
        {
            var response = await _beeDebugClient.BalancesGetAsync().ConfigureAwait(false);

            return response.Balances
                ?.Select(i => new BalanceDto(i))
                ?.ToList();
        }

        public async Task<List<AddressDto>?> BlocklistAsync()
        {
            var response = await _beeDebugClient.BlocklistAsync().ConfigureAwait(false);

            return response.Peers
                ?.Select(i => new AddressDto(i))
                ?.ToList();
        }

        public async Task<List<BalanceDto>?> ConsumedGetAsync()
        {
            var response = await _beeDebugClient.ConsumedGetAsync().ConfigureAwait(false);

            return response.Balances
                ?.Select(i => new BalanceDto(i))
                ?.ToList();
        }

        public async Task<List<BalanceDto>?> ConsumedGetAsync(string address)
        {
            var response = await _beeDebugClient.ConsumedGetAsync().ConfigureAwait(false);

            return response.Balances
                ?.Select(i => new BalanceDto(i))
                ?.ToList();
        }

        public async Task<ChequebookAddressDto> ChequebookAddressAsync()
        {
            var response = await _beeDebugClient.ChequebookAddressAsync().ConfigureAwait(false);

            return new ChequebookAddressDto(response);
        }

        public async Task<ChequebookBalanceDto> ChequebookBalanceAsync()
        {
            var response = await _beeDebugClient.ChequebookBalanceAsync().ConfigureAwait(false);

            return new ChequebookBalanceDto(response);
        }

        public async Task<MessageResponseDto> ChunksGetAsync(string address)
        {
            var response = await _beeDebugClient.ChunksGetAsync(address).ConfigureAwait(false);

            return new MessageResponseDto(response);
        }

        public async Task<MessageResponseDto> ChunksDeleteAsync(string address)
        {
            var response = await _beeDebugClient.ChunksDeleteAsync(address).ConfigureAwait(false);

            return new MessageResponseDto(response);
        }

        public async Task<ConnectDto> ConnectAsync(string multiAddress)
        {
            var response = await _beeDebugClient.ConnectAsync(multiAddress).ConfigureAwait(false);

            return new ConnectDto(response);
        }

        public async Task<ReservestateDto> ReservestateAsync()
        {
            var response = await _beeDebugClient.ReservestateAsync().ConfigureAwait(false);

            return new ReservestateDto(response);
        }

        public async Task<ChainstateDto> ChainstateAsync()
        {
            var response = await _beeDebugClient.ChainstateAsync().ConfigureAwait(false);

            return new ChainstateDto(response);
        }

        public async Task<VersionDto> HealthAsync()
        {
            var response = await _beeDebugClient.HealthAsync().ConfigureAwait(false);

            return new VersionDto(response);
        }

        public async Task<List<AddressDto>?> PeersGetAsync()
        {
            var response = await _beeDebugClient.PeersGetAsync().ConfigureAwait(false);

            return response.Peers
                ?.Select(i => new AddressDto(i))
                ?.ToList();
        }

        public async Task<MessageResponseDto> PeersDeleteAsync(string address)
        {
            var response = await _beeDebugClient.PeersDeleteAsync(address).ConfigureAwait(false);

            return new MessageResponseDto(response);
        }

        public async Task<PingpongDto> PingpongAsync(string peerId)
        {
            var response = await _beeDebugClient.PingpongAsync(peerId).ConfigureAwait(false);

            return new PingpongDto(response);
        }

        public async Task<VersionDto> ReadinessAsync()
        {
            var response = await _beeDebugClient.ReadinessAsync().ConfigureAwait(false);

            return new VersionDto(response);
        }

        public async Task<List<SettlementDataDto>?> SettlementsGetAsync(string address)
        {
            var response = await _beeDebugClient.SettlementsGetAsync().ConfigureAwait(false);

            return response.Settlements
                ?.Select(i => new SettlementDataDto(i))
                ?.ToList();
        }

        public async Task<SettlementDto> SettlementsGetAsync()
        {
            var response = await _beeDebugClient.SettlementsGetAsync().ConfigureAwait(false);

            return new SettlementDto(response);
        }

        public async Task<TimesettlementsDto> TimesettlementsAsync()
        {
            var response = await _beeDebugClient.TimesettlementsAsync().ConfigureAwait(false);

            return new TimesettlementsDto(response);
        }

        public async Task<TopologyDto> TopologyAsync()
        {
            var response = await _beeDebugClient.TopologyAsync().ConfigureAwait(false);

            return new TopologyDto(response);
        }

        public async Task<string> WelcomeMessageGetAsync()
        {
            var response = await _beeDebugClient.WelcomeMessageGetAsync().ConfigureAwait(false);

            return response.WelcomeMessage;
        }

        public async Task<VersionDto> WelcomeMessagePostAsync(string welcomeMessage)
        {
            var response = await _beeDebugClient.WelcomeMessagePostAsync(new Body{ WelcomeMessage = welcomeMessage }).ConfigureAwait(false);

            return new VersionDto(response);
        }

        public async Task<ChequebookCashoutGetDto> ChequebookCashoutGetAsync(string peerId)
        {
            var response = await _beeDebugClient.ChequebookCashoutGetAsync(peerId).ConfigureAwait(false);

            return new ChequebookCashoutGetDto(response);
        }

        public async Task<TransactionHashDto> ChequebookCashoutPostAsync(string peerId, int? gasPrice = null, long? gasLimit = null)
        {
            var response = await _beeDebugClient.ChequebookCashoutPostAsync(peerId, gasPrice, gasLimit).ConfigureAwait(false);

            return new TransactionHashDto(response);
        }

        public async Task<ChequebookChequeGetDto> ChequebookChequeGetAsync(string peerId)
        {
            var response = await _beeDebugClient.ChequebookChequeGetAsync(peerId).ConfigureAwait(false);

            return new ChequebookChequeGetDto(response);
        }

        public async Task<List<ChequebookChequeGetDto>> ChequebookChequeGetAsync()
        {
            var response = await _beeDebugClient.ChequebookChequeGetAsync().ConfigureAwait(false);

            return response.Lastcheques
                .Select(i => new ChequebookChequeGetDto(i))
                .ToList();
        }

        public async Task<TransactionHashDto> ChequebookDepositAsync(int amount, int? gasPrice = null)
        {
            var response = await _beeDebugClient.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false);

            return new TransactionHashDto(response);
        }

        public async Task<TransactionHashDto> ChequebookWithdrawAsync(int amount, int? gasPrice = null)
        {
            var response = await _beeDebugClient.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false);

            return new TransactionHashDto(response);
        }

        public async Task<TagDto> TagAsync(int uid)
        {
            var response = await _beeDebugClient.TagsAsync(uid).ConfigureAwait(false);

            return new TagDto(response);
        }

        public async Task<List<PendingTransactionDto>> TransactionsGetAsync()
        {
            var response = await _beeDebugClient.TransactionsGetAsync().ConfigureAwait(false);

            return response.PendingTransactions
                .Select(i => new PendingTransactionDto(i))
                .ToList();
        }

        public async Task<TransactionsDto> TransactionsGetAsync(string txHash)
        {
            var response = await _beeDebugClient.TransactionsGetAsync(txHash).ConfigureAwait(false);

            return new TransactionsDto(response);
        }

        public async Task<TransactionHashDto> TransactionsPostAsync(string txHash)
        {
            var response = await _beeDebugClient.TransactionsPostAsync(txHash).ConfigureAwait(false);

            return new TransactionHashDto(response);
        }

        public async Task<TransactionHashDto> TransactionsDeleteAsync(string txHash, int? gasPrice = null)
        {
            var response = await _beeDebugClient.TransactionsDeleteAsync(txHash).ConfigureAwait(false);

            return new TransactionHashDto(response);
        }

        public async Task<List<StampsGetDto>> StampsGetAsync()
        {
            var response = await _beeDebugClient.StampsGetAsync().ConfigureAwait(false);

            return response.Stamps
                .Select(i => new StampsGetDto(i))
                .ToList();
        }

        public async Task<StampsGetDto> StampsGetAsync(object id)
        {
            var response = await _beeDebugClient.StampsGetAsync(id).ConfigureAwait(false);

            return new StampsGetDto(response);
        }

        public async Task<StampsBucketsDto> StampsBucketsAsync(object id)
        {
            var response = await _beeDebugClient.StampsBucketsAsync(id).ConfigureAwait(false);

            return new StampsBucketsDto(response);
        }

        public async Task<BatchDto> StampsPostAsync(int amount, int depth, string? label = null, bool? immutable = null, int? gasPrice = null)
        {
            var response = await _beeDebugClient.StampsPostAsync(amount, depth, label, immutable, gasPrice).ConfigureAwait(false);

            return new BatchDto(response);
        }

        public async Task<BatchDto> StampsTopupAsync(object id, int amount)
        {
            var response = await _beeDebugClient.StampsTopupAsync(id, amount).ConfigureAwait(false);

            return new BatchDto(response);
        }

        public async Task<BatchDto> StampsDiluteAsync(object id, int depth)
        {
            var response = await _beeDebugClient.StampsDiluteAsync(id, depth).ConfigureAwait(false);

            return new BatchDto(response);
        }
    }
}