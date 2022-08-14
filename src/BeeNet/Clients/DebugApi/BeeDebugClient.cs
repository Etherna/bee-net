using Etherna.BeeNet.Clients.DebugApi.V2_0_1;
using Etherna.BeeNet.DtoModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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
            long? gasPrice = null, 
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.StampsPostAsync(amount.ToString(CultureInfo.InvariantCulture), depth, label, immutable, gasPrice, cancellationToken).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> CashoutChequeForPeerAsync(
            string peerId,
            long? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.ChequebookCashoutPostAsync(peerId, gasPrice, gasLimit, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> ConnectToPeerAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.ConnectAsync(address, cancellationToken).ConfigureAwait(false)).Address,
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeleteChunkAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new MessageResponseDto(await beeDebugClient_2_0_1.ChunksDeleteAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeletePeerAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new MessageResponseDto(await beeDebugClient_2_0_1.PeersDeleteAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DeleteTransactionAsync(
            string txHash,
            long? gasPrice = null,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.TransactionsDeleteAsync(txHash, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DepositIntoChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.ChequebookDepositAsync(amount, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DilutePostageBatchAsync(
            string id,
            int depth,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.StampsDiluteAsync(id, depth, cancellationToken).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<AddressDetailDto> GetAddressesAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new AddressDetailDto(await beeDebugClient_2_0_1.AddressesAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetAllBalancesAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.BalancesGetAsync(cancellationToken).ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<ChequeBookChequeGetDto>> GetAllChequeBookChequesAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.ChequebookChequeGetAsync(cancellationToken).ConfigureAwait(false)).Lastcheques.Select(i => new ChequeBookChequeGetDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetAllConsumedBalancesAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.ConsumedGetAsync(cancellationToken).ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetAllPeerAddressesAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.PeersGetAsync(cancellationToken).ConfigureAwait(false)).Peers.Select(i => i.Address),
                _ => throw new InvalidOperationException()
            };

        public async Task<SettlementDto> GetAllSettlementsAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new SettlementDto(await beeDebugClient_2_0_1.SettlementsGetAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TimeSettlementsDto> GetAllTimeSettlementsAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new TimeSettlementsDto(await beeDebugClient_2_0_1.TimesettlementsAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PostageBatchShortDto>> GetAllValidPostageBatchesFromAllNodesAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.BatchesAsync(cancellationToken).ConfigureAwait(false)).Batches.Select(i => new PostageBatchShortDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<BalanceDto> GetBalanceWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new BalanceDto(await beeDebugClient_2_0_1.BalancesGetAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetBlocklistedPeerAddressesAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.BlocklistAsync(cancellationToken).ConfigureAwait(false)).Peers.Select(i => i.Address),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChainStateDto> GetChainStateAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new ChainStateDto(await beeDebugClient_2_0_1.ChainstateAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetChequeBookAddressAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.ChequebookAddressAsync(cancellationToken).ConfigureAwait(false)).ChequebookAddress,
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookBalanceDto> GetChequeBookBalanceAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new ChequeBookBalanceDto(await beeDebugClient_2_0_1.ChequebookBalanceAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookCashoutGetDto> GetChequeBookCashoutForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new ChequeBookCashoutGetDto(await beeDebugClient_2_0_1.ChequebookCashoutGetAsync(peerId, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookChequeGetDto> GetChequeBookChequeForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new ChequeBookChequeGetDto(await beeDebugClient_2_0_1.ChequebookChequeGetAsync(peerId, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> GetChunkAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new MessageResponseDto(await beeDebugClient_2_0_1.ChunksGetAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<BalanceDto> GetConsumedBalanceWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new BalanceDto(await beeDebugClient_2_0_1.ConsumedGetAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> GetHealthAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new VersionDto(await beeDebugClient_2_0_1.HealthAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<NodeInfoDto> GetNodeInfoAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new NodeInfoDto(await beeDebugClient_2_0_1.NodeAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PostageBatchDto>> GetOwnedPostageBatchesByNodeAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.StampsGetAsync(cancellationToken).ConfigureAwait(false)).Stamps.Select(i => new PostageBatchDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PendingTransactionDto>> GetPendingTransactionsAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.TransactionsGetAsync(cancellationToken).ConfigureAwait(false)).PendingTransactions.Select(i => new PendingTransactionDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<PostageBatchDto> GetPostageBatchAsync(
            string id,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new PostageBatchDto(await beeDebugClient_2_0_1.StampsGetAsync(id, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> GetReadinessAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new VersionDto(await beeDebugClient_2_0_1.ReadinessAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ReserveStateDto> GetReserveStateAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new ReserveStateDto(await beeDebugClient_2_0_1.ReservestateAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<SettlementDataDto>> GetSettlementsWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.SettlementsGetAsync(cancellationToken).ConfigureAwait(false)).Settlements.Select(i => new SettlementDataDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<StampsBucketsDto> GetStampsBucketsForBatchAsync(
            string batchId,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new StampsBucketsDto(await beeDebugClient_2_0_1.StampsBucketsAsync(batchId, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TopologyDto> GetSwarmTopologyAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new TopologyDto(await beeDebugClient_2_0_1.TopologyAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TagDto> GetTagInfoAsync(
            long uid,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new TagDto(await beeDebugClient_2_0_1.TagsAsync(uid, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TransactionsDto> GetTransactionInfoAsync(
            string txHash,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new TransactionsDto(await beeDebugClient_2_0_1.TransactionsGetAsync(txHash, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<WalletDto> GetWalletBalance(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new WalletDto(await beeDebugClient_2_0_1.WalletAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.WelcomeMessageGetAsync(cancellationToken).ConfigureAwait(false)).WelcomeMessage,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> RebroadcastTransactionAsync(
            string txHash,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.TransactionsPostAsync(txHash, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> SetWelcomeMessageAsync(
            string welcomeMessage,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => new VersionDto(await beeDebugClient_2_0_1.WelcomeMessagePostAsync(
                    new V2_0_1.Body
                    {
                        WelcomeMessage = welcomeMessage
                    },
                    cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> TopUpPostageBatchAsync(
            string id,
            long amount,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.StampsTopupAsync(id, amount, cancellationToken).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> TryConnectToPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.PingpongAsync(peerId, cancellationToken).ConfigureAwait(false)).Rtt,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> WithdrawFromChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v2_0_1 => (await beeDebugClient_2_0_1.ChequebookWithdrawAsync(amount, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };
    }
}
