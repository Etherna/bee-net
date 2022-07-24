using Etherna.BeeNet.Clients.DebugApi.V2_0_1;
using Etherna.BeeNet.DtoModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Clients.DebugApi
{
    public class BeeDebugClient : IBeeDebugClient
    {
        // Fields.
        private readonly IBeeDebugClient_2_0_1 beeDebugClient_2_0_1;

        // Constructors.
        public BeeDebugClient(HttpClient httpClient, Uri baseUrl, DebugApiVersion apiVersion)
        {
            if (baseUrl is null)
                throw new ArgumentNullException(nameof(baseUrl));

            beeDebugClient_2_0_1 = new BeeDebugClient_2_0_1(httpClient) { BaseUrl = baseUrl.ToString() };
            CurrentApiVersion = apiVersion;
        }

        // Properties.
        public DebugApiVersion CurrentApiVersion { get; set; }

        // Methods.
        public async Task<string> BuyPostageBatchAsync(
            long amount,
            int depth,
            string? label = null,
            bool? immutable = null,
            long? gasPrice = null) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.StampsPostAsync(amount.ToString(CultureInfo.InvariantCulture), depth, label, immutable, gasPrice).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> CashoutChequeForPeerAsync(
            string peerId,
            long? gasPrice = null,
            long? gasLimit = null) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.ChequebookCashoutPostAsync(peerId, gasPrice, gasLimit).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> ConnectToPeerAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.ConnectAsync(address).ConfigureAwait(false)).Address,
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeleteChunkAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new MessageResponseDto(await beeDebugClient_2_0_1.ChunksDeleteAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeletePeerAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new MessageResponseDto(await beeDebugClient_2_0_1.PeersDeleteAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DeleteTransactionAsync(
            string txHash,
            long? gasPrice = null) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.TransactionsDeleteAsync(txHash, gasPrice).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DepositIntoChequeBookAsync(
            long amount,
            long? gasPrice = null) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DilutePostageBatchAsync(
            string id,
            int depth) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.StampsDiluteAsync(id, depth).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<AddressDetailDto> GetAddressesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new AddressDetailDto(await beeDebugClient_2_0_1.AddressesAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetAllBalancesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.BalancesGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<ChequeBookChequeGetDto>> GetAllChequeBookChequesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.ChequebookChequeGetAsync().ConfigureAwait(false)).Lastcheques.Select(i => new ChequeBookChequeGetDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetAllConsumedBalancesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.ConsumedGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetAllPeerAddressesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.PeersGetAsync().ConfigureAwait(false)).Peers.Select(i => i.Address),
                _ => throw new InvalidOperationException()
            };

        public async Task<SettlementDto> GetAllSettlementsAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new SettlementDto(await beeDebugClient_2_0_1.SettlementsGetAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TimeSettlementsDto> GetAllTimeSettlementsAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new TimeSettlementsDto(await beeDebugClient_2_0_1.TimesettlementsAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PostageBatchShortDto>> GetAllValidPostageBatchesFromAllNodesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.BatchesAsync().ConfigureAwait(false)).Batches.Select(i => new PostageBatchShortDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetBalanceWithPeerAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.BalancesGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetBlocklistedPeerAddressesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.BlocklistAsync().ConfigureAwait(false)).Peers.Select(i => i.Address),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChainStateDto> GetChainStateAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new ChainStateDto(await beeDebugClient_2_0_1.ChainstateAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetChequeBookAddressAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.ChequebookAddressAsync().ConfigureAwait(false)).ChequebookAddress,
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookBalanceDto> GetChequeBookBalanceAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new ChequeBookBalanceDto(await beeDebugClient_2_0_1.ChequebookBalanceAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookCashoutGetDto> GetChequeBookCashoutForPeerAsync(string peerId) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new ChequeBookCashoutGetDto(await beeDebugClient_2_0_1.ChequebookCashoutGetAsync(peerId).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookChequeGetDto> GetChequeBookChequeForPeerAsync(string peerId) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new ChequeBookChequeGetDto(await beeDebugClient_2_0_1.ChequebookChequeGetAsync(peerId).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> GetChunkAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new MessageResponseDto(await beeDebugClient_2_0_1.ChunksGetAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetConsumedBalanceWithPeerAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.ConsumedGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> GetHealthAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new VersionDto(await beeDebugClient_2_0_1.HealthAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<NodeInfoDto> GetNodeInfoAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new NodeInfoDto(await beeDebugClient_2_0_1.NodeAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PostageBatchDto>> GetOwnedPostageBatchesByNodeAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.StampsGetAsync().ConfigureAwait(false)).Stamps.Select(i => new PostageBatchDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PendingTransactionDto>> GetPendingTransactionsAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.TransactionsGetAsync().ConfigureAwait(false)).PendingTransactions.Select(i => new PendingTransactionDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<PostageBatchDto> GetPostageBatchAsync(string id) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new PostageBatchDto(await beeDebugClient_2_0_1.StampsGetAsync(id).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> GetReadinessAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new VersionDto(await beeDebugClient_2_0_1.ReadinessAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ReserveStateDto> GetReserveStateAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new ReserveStateDto(await beeDebugClient_2_0_1.ReservestateAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<SettlementDataDto>> GetSettlementsWithPeerAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.SettlementsGetAsync().ConfigureAwait(false)).Settlements.Select(i => new SettlementDataDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<StampsBucketsDto> GetStampsBucketsForBatchAsync(string batchId) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new StampsBucketsDto(await beeDebugClient_2_0_1.StampsBucketsAsync(batchId).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TopologyDto> GetSwarmTopologyAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new TopologyDto(await beeDebugClient_2_0_1.TopologyAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TagDto> GetTagInfoAsync(int uid) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new TagDto(await beeDebugClient_2_0_1.TagsAsync(uid).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TransactionsDto> GetTransactionInfoAsync(string txHash) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new TransactionsDto(await beeDebugClient_2_0_1.TransactionsGetAsync(txHash).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<WalletDto> GetWalletBalance() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new WalletDto(await beeDebugClient_2_0_1.WalletAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetWelcomeMessageAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.WelcomeMessageGetAsync().ConfigureAwait(false)).WelcomeMessage,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> RebroadcastTransactionAsync(string txHash) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.TransactionsPostAsync(txHash).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> SetWelcomeMessageAsync(string welcomeMessage) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new VersionDto(await beeDebugClient_2_0_1.WelcomeMessagePostAsync(
                    new V2_0_1.Body
                    {
                        WelcomeMessage = welcomeMessage
                    }).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> TopUpPostageBatchAsync(
            string id,
            long amount) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.StampsTopupAsync(id, amount).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> TryConnectToPeerAsync(string peerId) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.PingpongAsync(peerId).ConfigureAwait(false)).Rtt,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> WithdrawFromChequeBookAsync(
            long amount,
            long? gasPrice = null) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };
    }
}
