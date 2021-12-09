using Etherna.BeeNet.DtoModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace Etherna.BeeNet.Clients.v_1_4.DebugApi
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    public class AdapterBeeDebugVersion_1_4 : IBeeDebugClient
    {
        readonly IBeeDebugClient_1_4 _beeDebugClient;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:URI-like parameters should not be strings", Justification = "<Pending>")]
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

            response.Underlay = response.Underlay.Where(i => !string.IsNullOrWhiteSpace(i)).ToList();

            return new AddressDetailDto(response.Overlay, response.Underlay, response.Ethereum, response.PublicKey, response.PssPublicKey);
        }

        public async Task<List<BalanceDto>?> GetBalancesAsync()
        {
            var response = await _beeDebugClient.BalancesGetAsync().ConfigureAwait(false);

            return response.Balances
                ?.Select(i => new BalanceDto(i.Peer, i.Balance))
                ?.ToList();
        }

        public async Task<List<BalanceDto>?> GetBalanceAsync(string address)
        {
            var response = await _beeDebugClient.BalancesGetAsync().ConfigureAwait(false);

            return response.Balances
                ?.Select(i => new BalanceDto(i.Peer, i.Balance))
                ?.ToList();
        }

        public async Task<List<AddressDto>?> BlocklistAsync()
        {
            var response = await _beeDebugClient.BlocklistAsync().ConfigureAwait(false);

            return response.Peers
                ?.Select(i => new AddressDto(i.Address))
                ?.ToList();
        }

        public async Task<List<BalanceDto>?> ConsumedGetAsync()
        {
            var response = await _beeDebugClient.ConsumedGetAsync().ConfigureAwait(false);

            return response.Balances
                ?.Select(i => new BalanceDto(i.Peer, i.Balance))
                ?.ToList();
        }

        public async Task<List<BalanceDto>?> ConsumedGetAsync(string address)
        {
            var response = await _beeDebugClient.ConsumedGetAsync().ConfigureAwait(false);

            return response.Balances
                ?.Select(i => new BalanceDto(i.Peer, i.Balance))
                ?.ToList();
        }

        public async Task<ChequebookAddressDto> ChequebookAddressAsync()
        {
            var response = await _beeDebugClient.ChequebookAddressAsync().ConfigureAwait(false);

            return new ChequebookAddressDto(response.ChequebookAddress);
        }

        public async Task<ChequebookBalanceDto> ChequebookBalanceAsync()
        {
            var response = await _beeDebugClient.ChequebookBalanceAsync().ConfigureAwait(false);

            return new ChequebookBalanceDto(response.TotalBalance, response.AvailableBalance);
        }

        public async Task<MessageResponseDto> ChunksGetAsync(string address)
        {
            var response = await _beeDebugClient.ChunksGetAsync(address).ConfigureAwait(false);

            return new MessageResponseDto(response.Message, response.Code);
        }

        public async Task<MessageResponseDto> ChunksDeleteAsync(string address)
        {
            var response = await _beeDebugClient.ChunksDeleteAsync(address).ConfigureAwait(false);

            return new MessageResponseDto(response.Message, response.Code);
        }

        public async Task<ConnectDto> ConnectAsync(string multiAddress)
        {
            var response = await _beeDebugClient.ConnectAsync(multiAddress).ConfigureAwait(false);

            return new ConnectDto(response.Address);
        }

        public async Task<ReservestateDto> ReservestateAsync()
        {
            var response = await _beeDebugClient.ReservestateAsync().ConfigureAwait(false);

            return new ReservestateDto(response.Radius, response.Available, response.Outer, response.Inner);
        }

        public async Task<ChainstateDto> ChainstateAsync()
        {
            var response = await _beeDebugClient.ChainstateAsync().ConfigureAwait(false);

            return new ChainstateDto(response.Block, response.TotalAmount, response.CurrentPrice);
        }

        public async Task<VersionDto> HealthAsync()
        {
            var response = await _beeDebugClient.HealthAsync().ConfigureAwait(false);

            return new VersionDto(response.Status, response.Version, response.ApiVersion, response.DebugApiVersion);
        }

        public async Task<List<AddressDto>?> PeersGetAsync()
        {
            var response = await _beeDebugClient.PeersGetAsync().ConfigureAwait(false);

            return response.Peers
                ?.Select(i => new AddressDto(i.Address))
                ?.ToList();
        }

        public async Task<MessageResponseDto> PeersDeleteAsync(string address)
        {
            var response = await _beeDebugClient.PeersDeleteAsync(address).ConfigureAwait(false);

            return new MessageResponseDto(response.Message, response.Code);
        }

        public async Task<PingpongDto> PingpongAsync(string peerId)
        {
            var response = await _beeDebugClient.PingpongAsync(peerId).ConfigureAwait(false);

            return new PingpongDto(response.Rtt);
        }

        public async Task<VersionDto> ReadinessAsync()
        {
            var response = await _beeDebugClient.ReadinessAsync().ConfigureAwait(false);

            return new VersionDto(response.Status, response.Version, response.ApiVersion, response.DebugApiVersion);
        }

        public async Task<List<SettlementDataDto>?> SettlementsGetAsync(string address)
        {
            var response = await _beeDebugClient.SettlementsGetAsync().ConfigureAwait(false);

            return response.Settlements
                ?.Select(i => new SettlementDataDto(i.Peer, i.Received, i.Sent))
                ?.ToList();
        }

        public async Task<SettlementDto> SettlementsGetAsync()
        {
            var response = await _beeDebugClient.SettlementsGetAsync().ConfigureAwait(false);

            var settlements = response.Settlements
                ?.Select(i => new SettlementDataDto(i.Peer, i.Received, i.Sent))
                ?.ToList();

            return new SettlementDto(response.TotalReceived, response.TotalSent, settlements);
        }

        public async Task<TimesettlementsDto> TimesettlementsAsync()
        {
            var response = await _beeDebugClient.TimesettlementsAsync().ConfigureAwait(false);

            var settlements = response.Settlements?
                .Select(i => new SettlementDataDto(i.Peer, i.Received, i.Sent))
                .ToList();

            return new TimesettlementsDto(response.TotalReceived, response.TotalSent, settlements);
        }

        public async Task<TopologyDto> TopologyAsync()
        {
            var response = await _beeDebugClient.TopologyAsync().ConfigureAwait(false);

            var bins = response.Bins.ToDictionary(
                i => i.Key,
                i => new AnonymousDto(
                    i.Value.Population,
                    i.Value.Connected,
                    i.Value?.DisconnectedPeers?.Select(k => new DisconnectedPeersDto(k.Address, new MetricsDto(k.Metrics.LastSeenTimestamp, k.Metrics.SessionConnectionRetry, k.Metrics.ConnectionTotalDuration, k.Metrics.SessionConnectionDuration, k.Metrics.SessionConnectionDirection, k.Metrics.LatencyEWMA)))?.ToList(),
                    i.Value?.ConnectedPeers?.Select(k => new ConnectedPeersDto(k.Address, k.Metrics.LastSeenTimestamp, k.Metrics.SessionConnectionRetry, k.Metrics.ConnectionTotalDuration, k.Metrics.SessionConnectionDuration, k.Metrics.SessionConnectionDirection, k.Metrics.LatencyEWMA))?.ToList()));

            return new TopologyDto(response.BaseAddr, response.Population, response.Connected, response.Timestamp, response.NnLowWatermark, response.Depth, bins);
        }

        public async Task<string> WelcomeMessageGetAsync()
        {
            var response = await _beeDebugClient.WelcomeMessageGetAsync().ConfigureAwait(false);

            return response.WelcomeMessage;
        }

        public async Task<VersionDto> WelcomeMessagePostAsync(string welcomeMessage)
        {
            var response = await _beeDebugClient.WelcomeMessagePostAsync(new Body{ WelcomeMessage = welcomeMessage }).ConfigureAwait(false);

            return new VersionDto(response.Status, response.Version, response.ApiVersion, response.DebugApiVersion);
        }

        public async Task<ChequebookCashoutGetDto> ChequebookCashoutGetAsync(string peerId)
        {
            var response = await _beeDebugClient.ChequebookCashoutGetAsync(peerId).ConfigureAwait(false);

            return new ChequebookCashoutGetDto(
                response.Peer,
                new DtoModel.LastCashedCheque(response.LastCashedCheque.Beneficiary, response.LastCashedCheque.Chequebook, response.LastCashedCheque.Payout),
                response.TransactionHash,
                new DtoModel.Result(
                    response.Result.Recipient,
                    response.Result.LastPayout,
                    response.Result.Bounced),
                response.UncashedAmount);
        }

        public async Task<TransactionHashDto> ChequebookCashoutPostAsync(string peerId, int? gasPrice = null, long? gasLimit = null)
        {
            var response = await _beeDebugClient.ChequebookCashoutPostAsync(peerId, gasPrice, gasLimit).ConfigureAwait(false);

            return new TransactionHashDto(response.TransactionHash);
        }

        public async Task<ChequebookChequeGetDto> ChequebookChequeGetAsync(string peerId)
        {
            var response = await _beeDebugClient.ChequebookChequeGetAsync(peerId).ConfigureAwait(false);

            return new ChequebookChequeGetDto(
                response.Peer,
                new LastreceivedDto(response.Lastreceived.Beneficiary, response.Lastreceived.Chequebook, response.Lastreceived.Payout),
                new LastsentDto(response.Lastsent.Beneficiary, response.Lastsent.Chequebook, response.Lastsent.Payout));
        }

        public async Task<List<ChequebookChequeGetDto>> ChequebookChequeGetAsync()
        {
            var response = await _beeDebugClient.ChequebookChequeGetAsync().ConfigureAwait(false);

            return response.Lastcheques.Select(i =>
                new ChequebookChequeGetDto(
                i.Peer,
                new LastreceivedDto(i.Lastreceived.Beneficiary, i.Lastreceived.Chequebook, i.Lastreceived.Payout),
                new LastsentDto(i.Lastsent.Beneficiary, i.Lastsent.Chequebook, i.Lastsent.Payout)))
                .ToList();
        }

        public async Task<TransactionHashDto> ChequebookDepositAsync(int amount, int? gasPrice = null)
        {
            var response = await _beeDebugClient.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false);

            return new TransactionHashDto(response.TransactionHash);
        }

        public async Task<TransactionHashDto> ChequebookWithdrawAsync(int amount, int? gasPrice = null)
        {
            var response = await _beeDebugClient.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false);

            return new TransactionHashDto(response.TransactionHash);
        }

        public async Task<TagDto> TagAsync(int uid)
        {
            var response = await _beeDebugClient.TagsAsync(uid).ConfigureAwait(false);

            return new TagDto(response.Total, response.Split, response.Seen, response.Stored, response.Sent, response.Synced, response.Uid, response.Address, response.StartedAt);
        }

        public async Task<List<PendingTransactionDto>> TransactionsGetAsync()
        {
            var response = await _beeDebugClient.TransactionsGetAsync().ConfigureAwait(false);

            return response.PendingTransactions
                .Select(i => new PendingTransactionDto(i.TransactionHash, i.To, i.Nonce, i.GasPrice, i.GasLimit, i.Data, i.Created, i.Description, i.Value))
                .ToList();
        }

        public async Task<TransactionsDto> TransactionsGetAsync(string txHash)
        {
            var response = await _beeDebugClient.TransactionsGetAsync(txHash).ConfigureAwait(false);

            return new TransactionsDto(response.TransactionHash, response.To, response.Nonce, response.GasPrice, response.GasLimit, response.Data, response.Created,
                response.Description, response.Value);
        }

        public async Task<TransactionHashDto> TransactionsPostAsync(string txHash)
        {
            var response = await _beeDebugClient.TransactionsPostAsync(txHash).ConfigureAwait(false);

            return new TransactionHashDto(response.TransactionHash);
        }

        public async Task<TransactionHashDto> TransactionsDeleteAsync(string txHash, int? gasPrice = null)
        {
            var response = await _beeDebugClient.TransactionsDeleteAsync(txHash).ConfigureAwait(false);

            return new TransactionHashDto(response.TransactionHash);
        }

        public async Task<List<StampsGetDto>> StampsGetAsync()
        {
            var response = await _beeDebugClient.StampsGetAsync().ConfigureAwait(false);

            return response.Stamps
                .Select(i => new StampsGetDto(i.Exists, i.BatchTTL, i.BatchID, i.Utilization, i.Usable, i.Label, i.Depth, i.Amount, i.BucketDepth, i.BlockNumber, i.ImmutableFlag))
                .ToList();
        }

        public async Task<StampsGetDto> StampsGetAsync(object id)
        {
            var response = await _beeDebugClient.StampsGetAsync(id).ConfigureAwait(false);

            return new StampsGetDto(response.Exists, response.BatchTTL, response.BatchID, response.Utilization, response.Usable, response.Label, response.Depth,
                response.Amount, response.BucketDepth, response.BlockNumber, response.ImmutableFlag);
        }

        public async Task<StampsBucketsDto> StampsBucketsAsync(object id)
        {
            var response = await _beeDebugClient.StampsBucketsAsync(id).ConfigureAwait(false);

            return new StampsBucketsDto(
                response.Depth,
                response.BucketDepth,
                response.BucketUpperBound,
                response.Buckets.Select(i => new BucketDto(i.BucketID, i.Collisions)).ToList());
        }

        public async Task<BatchDto> StampsPostAsync(int amount, int depth, string? label = null, bool? immutable = null, int? gasPrice = null)
        {
            var response = await _beeDebugClient.StampsPostAsync(amount, depth, label, immutable, gasPrice).ConfigureAwait(false);

            return new BatchDto(response.BatchID);
        }

        public async Task<BatchDto> StampsTopupAsync(object id, int amount)
        {
            var response = await _beeDebugClient.StampsTopupAsync(id, amount).ConfigureAwait(false);

            return new BatchDto(response.BatchID);
        }

        public async Task<BatchDto> StampsDiluteAsync(object id, int depth)
        {
            var response = await _beeDebugClient.StampsDiluteAsync(id, depth).ConfigureAwait(false);

            return new BatchDto(response.BatchID);
        }
    }
}
#pragma warning restore CA1707 // Identifiers should not contain underscores