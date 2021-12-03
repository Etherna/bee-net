using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Etherna.BeeNet.DtoInput.DebugApi;
using Etherna.BeeNet.DtoModel.Debug;

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace Etherna.BeeNet.Clients.v_1_4.DebugApi
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    public class AdapterBeeDebugVersion_1_4 : IBeeNodeDebugClient
    {
        readonly IBeeDebugClient_1_4 _beeDebugClient;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:URI-like parameters should not be strings", Justification = "<Pending>")]
        public AdapterBeeDebugVersion_1_4(HttpClient httpClient, Uri baseUrl)
        {
            _beeDebugClient = new BeeDebugClient_1_4(httpClient) { BaseUrl = baseUrl.ToString() };
        }

        public async Task<AddressDto> AddressAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.AddressAsync(cancellationToken.Value) : await _beeDebugClient.AddressAsync();

            return new AddressDto(response.ChequebookAddress, response.AdditionalProperties);
        }

        public async Task<AddressesDto> AddressesAsync(CancellationToken? cancellationToken = null)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.AddressesAsync(cancellationToken.Value) : await _beeDebugClient.AddressesAsync();

            return new AddressesDto(response.Overlay, response.Underlay, response.Ethereum, response.PublicKey, response.PssPublicKey);
        }

        public async Task<BalanceDto> BalanceAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.BalanceAsync(cancellationToken.Value) : await _beeDebugClient.BalanceAsync();

            return new BalanceDto(response.TotalBalance, response.AvailableBalance, response.AdditionalProperties);
        }

        public async Task<Balances2Dto> Balances2Async(string address, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.Balances2Async(address, cancellationToken.Value) : await _beeDebugClient.Balances2Async(address);

            return new Balances2Dto(response.Peer, response.Balance, response.AdditionalProperties);
        }

        public async Task<Balances3Dto> BalancesAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.BalancesAsync(cancellationToken.Value) : await _beeDebugClient.BalancesAsync();

            var balances = response.Balances?
                .Select(i => new BalancesDto(i.Peer, i.Balance, i.AdditionalProperties))
                .ToList();
            return new Balances3Dto(balances, response.AdditionalProperties);
        }

        public async Task<BlocklistDto> BlocklistAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.BlocklistAsync(cancellationToken.Value) : await _beeDebugClient.BlocklistAsync();

            var peers = response.Peers?
                .Select(i => new PeersDto(i.Address, i.AdditionalProperties))
                .ToList();
            return new BlocklistDto(peers, response.AdditionalProperties);
        }

        public async Task<Buckets2Dto> BucketsAsync(object id, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.BucketsAsync(id, cancellationToken.Value) : await _beeDebugClient.BucketsAsync(id);

            var buckets = response.Buckets?
                .Select(i => new BucketsDto(i.BucketID, i.Collisions, i.AdditionalProperties))
                .ToList();

            return new Buckets2Dto(response.Depth, response.BucketDepth, response.BucketUpperBound, buckets, response.AdditionalProperties);
        }

        public async Task<CashoutGetDto> CashoutGETAsync(string peerId, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.CashoutGETAsync(peerId, cancellationToken.Value) : await _beeDebugClient.CashoutGETAsync(peerId);

            var lastCashedCheque = response.LastCashedCheque != null ?
                new LastCashedChequeDto(beneficiary: response.LastCashedCheque.Beneficiary, 
                    chequeBook: response.LastCashedCheque.Chequebook, 
                    payout: response.LastCashedCheque.Payout,
                    additionalProperties: response.LastCashedCheque.AdditionalProperties)
                : null;

            var result = response.Result != null 
                ? new ResultDto(response.Result.Recipient,
                    response.Result.LastPayout,
                    response.Result.Bounced,
                    response.Result.AdditionalProperties)
                : null;

            return new CashoutGetDto(response.Peer, lastCashedCheque, response.TransactionHash, result, response.UncashedAmount, response.AdditionalProperties);
        }

        public async Task<CashoutPostDto> CashoutPOSTAsync(string peerId, int? gasPrice, long? gasLimit, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.CashoutPOSTAsync(peerId, gasPrice, gasLimit, cancellationToken.Value) : await _beeDebugClient.CashoutPOSTAsync(peerId, gasPrice, gasLimit);

            return new CashoutPostDto(response.TransactionHash, response.AdditionalProperties);
        }

        public async Task<ChainstateDto> ChainstateAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.ChainstateAsync(cancellationToken.Value) : await _beeDebugClient.ChainstateAsync();

            return new ChainstateDto(response.Block, response.TotalAmount, response.CurrentPrice, response.AdditionalProperties);
        }

        public async Task<Cheque2Dto> Cheque2Async(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.Cheque2Async(cancellationToken.Value) : await _beeDebugClient.Cheque2Async();

            var lastcheques = response.Lastcheques?.Select(
                i => new LastchequesDto(i.Peer,
                i.Lastreceived != null
                ? new LastReceived2Dto(i.Lastreceived.Beneficiary, i.Lastreceived.Chequebook, i.Lastreceived.Payout, i.Lastreceived.AdditionalProperties)
                : null,
                i.Lastsent != null
                ? new Lastsent2Dto(i.Lastsent.Beneficiary, i.Lastsent.Chequebook, i.Lastsent.Payout, i.Lastsent.AdditionalProperties)
                : null,
                i.AdditionalProperties
                )).ToList();

            return new Cheque2Dto(lastcheques, response.AdditionalProperties);
        }

        public async Task<ChequeDto> ChequeAsync(string peerId, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.ChequeAsync(peerId, cancellationToken.Value) : await _beeDebugClient.ChequeAsync(peerId);

            return new ChequeDto(
                response.Peer,
                response.Lastreceived != null
                ? new LastreceivedDto(response.Lastreceived.Beneficiary, response.Lastreceived.Chequebook, response.Lastreceived.Payout, response.Lastreceived.AdditionalProperties)
                : null,
                response.Lastsent != null
                ? new LastsentDto(response.Lastsent.Beneficiary, response.Lastsent.Chequebook, response.Lastsent.Payout, response.Lastsent.AdditionalProperties)
                : null,
                response.AdditionalProperties
                );
        }

        public async Task<ChunksDeleteDto> ChunksDELETEAsync(string address, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.ChunksDELETEAsync(address, cancellationToken.Value) : await _beeDebugClient.ChunksDELETEAsync(address);

            return new ChunksDeleteDto(response.Message, response.Code, response.AdditionalProperties);
        }

        public async Task<ChunksGetDto> ChunksGETAsync(string address, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.ChunksGETAsync(address, cancellationToken.Value) : await _beeDebugClient.ChunksGETAsync(address);

            return new ChunksGetDto(response.Message, response.Code, response.AdditionalProperties);
        }

        public async Task<ConnectDto> ConnectAsync(string multiAddress, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.ConnectAsync(multiAddress, cancellationToken.Value) : await _beeDebugClient.ConnectAsync(multiAddress);

            return new ConnectDto(response.Address, response.AdditionalProperties);
        }

        public async Task<Consumed2Dto> Consumed2Async(string address, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.Consumed2Async(address, cancellationToken.Value) : await _beeDebugClient.Consumed2Async(address);

            return new Consumed2Dto(response.Peer, response.Balance, response.AdditionalProperties);
        }

        public async Task<ConsumedDto> ConsumedAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.ConsumedAsync(cancellationToken.Value) : await _beeDebugClient.ConsumedAsync();

            var balances = response.Balances?
                .Select(i => new Balances2Dto(i.Balance, i.Peer, i.AdditionalProperties))
                .ToList();
            return new ConsumedDto(balances, response.AdditionalProperties);
        }

        public async Task<DepositDto> DepositAsync(int amount, int? gasPrice, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.DepositAsync(amount, gasPrice, cancellationToken.Value) : await _beeDebugClient.DepositAsync(amount, gasPrice);

            return new DepositDto(response.TransactionHash, response.AdditionalProperties);
        }

        public async Task<DiluteDto> DiluteAsync(object id, int depth, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.DiluteAsync(id, depth, cancellationToken.Value) : await _beeDebugClient.DiluteAsync(id, depth);

            return new DiluteDto(response.BatchID, response.AdditionalProperties);
        }

        public async Task<HealthDto> HealthAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.HealthAsync(cancellationToken.Value) : await _beeDebugClient.HealthAsync();

            return new HealthDto(response.Status, response.Version, response.ApiVersion, response.DebugApiVersion, response.AdditionalProperties);
        }

        public async Task<PeersDeleteDto> PeersDELETEAsync(string address, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.PeersDELETEAsync(address, cancellationToken.Value) : await _beeDebugClient.PeersDELETEAsync(address);

            return new PeersDeleteDto(response.Message, response.Code, response.AdditionalProperties);
        }

        public async Task<PeersGetDto> PeersGETAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.PeersGETAsync(cancellationToken.Value) : await _beeDebugClient.PeersGETAsync();

            var peers = response.Peers?
                .Select(i => new Peers2Dto(i.Address, i.AdditionalProperties))
                .ToList();
            return new PeersGetDto(peers, response.AdditionalProperties);
        }

        public async Task<PingpongDto> PingpongAsync(string peerId, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.PingpongAsync(peerId, cancellationToken.Value) : await _beeDebugClient.PingpongAsync(peerId);

            return new PingpongDto(response.Rtt, response.AdditionalProperties);
        }

        public async Task<ReadinessDto> ReadinessAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.ReadinessAsync(cancellationToken.Value) : await _beeDebugClient.ReadinessAsync();

            return new ReadinessDto(response.Status, response.Version, response.ApiVersion, response.DebugApiVersion, response.AdditionalProperties);
        }

        public async Task<ReserveStateDto> ReservestateAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.ReservestateAsync(cancellationToken.Value) : await _beeDebugClient.ReservestateAsync();

            return new ReserveStateDto(response.Radius, response.Available, response.Outer, response.Inner, response.AdditionalProperties);
        }

        public async Task<Settlements3Dto> Settlements2Async(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.Settlements2Async(cancellationToken.Value) : await _beeDebugClient.Settlements2Async();

            var settlements = response.Settlements?
                .Select(i => new SettlementsDto(i.Peer, i.Received, i.Sent, i.AdditionalProperties))
                .ToList();
            return new Settlements3Dto(response.TotalReceived, response.TotalSent, settlements, response.AdditionalProperties);
        }

        public async Task<SettlementsDto> SettlementsAsync(string address, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.SettlementsAsync(address, cancellationToken.Value) : await _beeDebugClient.SettlementsAsync(address);

            return new SettlementsDto(response.Peer, response.Received, response.Sent, response.AdditionalProperties);
        }

        public async Task<StampsGet2Dto> StampsGET2Async(object id, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.StampsGET2Async(id, cancellationToken.Value) : await _beeDebugClient.StampsGET2Async(id);

            return new StampsGet2Dto(response.Exists, response.BatchTTL, response.BatchID, response.Utilization, response.Usable, response.Label, response.Depth, response.Amount, response.BucketDepth, response.BlockNumber, response.ImmutableFlag, response.AdditionalProperties);
        }

        public async Task<StampsGetDto> StampsGETAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.StampsGETAsync(cancellationToken.Value) : await _beeDebugClient.StampsGETAsync();

            var stamps = response.Stamps?
                .Select(i => new StampsDto(i.Exists, i.BatchTTL, i.BatchID, i.Utilization, i.Usable, i.Label, i.Depth, i.Amount, i.BucketDepth, i.BlockNumber, i.ImmutableFlag, i.AdditionalProperties))
                .ToList();
            return new StampsGetDto(stamps, response.AdditionalProperties);
        }

        public async Task<StampsPostDto> StampsPOSTAsync(int amount, int depth, string label, bool? immutable, int? gasPrice, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.StampsPOSTAsync(amount, depth, label, immutable, gasPrice, cancellationToken.Value) : await _beeDebugClient.StampsPOSTAsync(amount, depth, label, immutable, gasPrice);

            return new StampsPostDto(response.BatchID, response.AdditionalProperties);
        }

        public async Task<TagsDto> TagsAsync(int uid, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.TagsAsync(uid, cancellationToken.Value) : await _beeDebugClient.TagsAsync(uid);

            return new TagsDto(response.Total, response.Split, response.Seen, response.Stored, response.Sent, response.Synced, response.Uid, response.Address, response.StartedAt, response.AdditionalProperties);
        }

        public async Task<TimeSettlementsDto> TimesettlementsAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.TimesettlementsAsync(cancellationToken.Value) : await _beeDebugClient.TimesettlementsAsync();

            var settlements = response.Settlements?
                .Select(i => new SettlementsDto(i.Peer, i.Received, i.Sent, i.AdditionalProperties))
                .ToList();

            return new TimeSettlementsDto(response.TotalReceived, response.TotalSent, settlements, response.AdditionalProperties);
        }

        public async Task<TopologyDto> TopologyAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.TopologyAsync(cancellationToken.Value) : await _beeDebugClient.TopologyAsync();

            var bins = response.Bins?.ToDictionary(
                i => i.Key, 
                i => new AnonymousDto(
                    i.Value.Population, 
                    i.Value.Connected,
                    i.Value.DisconnectedPeers?.Select(k => new DisconnectedPeersDto(k.Address, new MetricsDto(k.Metrics.LastSeenTimestamp, k.Metrics.SessionConnectionRetry, k.Metrics.ConnectionTotalDuration, k.Metrics.SessionConnectionDuration, k.Metrics.SessionConnectionDirection, k.Metrics.LatencyEWMA, k.Metrics.AdditionalProperties), k.AdditionalProperties)).ToList(),
                    i.Value.ConnectedPeers?.Select(k => new ConnectedPeersDto(k.Address, new MetricsDto(k.Metrics.LastSeenTimestamp, k.Metrics.SessionConnectionRetry, k.Metrics.ConnectionTotalDuration, k.Metrics.SessionConnectionDuration, k.Metrics.SessionConnectionDirection, k.Metrics.LatencyEWMA, k.Metrics.AdditionalProperties), k.AdditionalProperties)).ToList(),
                    i.Value.AdditionalProperties));

            return new TopologyDto(response.BaseAddr, response.Population, response.Connected, response.Timestamp, response.NnLowWatermark, response.Depth, bins, response.AdditionalProperties);
        }

        public async Task<TopUpDto> TopupAsync(object id, int amount, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.TopupAsync(id, amount, cancellationToken.Value) : await _beeDebugClient.TopupAsync(id, amount);

            return new TopUpDto(response.BatchID, response.AdditionalProperties);
        }

        public async Task<TransactionsDeleteDto> TransactionsDELETEAsync(string txHash, int? gasPrice, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.TransactionsDELETEAsync(txHash, gasPrice, cancellationToken.Value) : await _beeDebugClient.TransactionsDELETEAsync(txHash, gasPrice);

            return new TransactionsDeleteDto(response.TransactionHash, response.AdditionalProperties);
        }

        public async Task<TransactionsGet2Dto> TransactionsGET2Async(string txHash, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.TransactionsGET2Async(txHash, cancellationToken.Value) : await _beeDebugClient.TransactionsGET2Async(txHash);

            return new TransactionsGet2Dto(response.TransactionHash, response.To, response.Nonce, response.GasPrice, response.GasLimit, response.Data, response.Created, response.Description, response.Value, response.AdditionalProperties);
        }

        public async Task<TransactionsDto> TransactionsGETAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.TransactionsGETAsync(cancellationToken.Value) : await _beeDebugClient.TransactionsGETAsync();

            var pendingTransactions = response.PendingTransactions?
                .Select(i => new PendingTransactionsDto(i.TransactionHash, i.To, i.Nonce, i.GasPrice, i.GasLimit, i.Data, i.Created, i.Description, i.Value, i.AdditionalProperties))
                .ToList();
            return new TransactionsDto(pendingTransactions, response.AdditionalProperties);
        }

        public async Task<TransactionsPostDto> TransactionsPOSTAsync(string txHash, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.TransactionsPOSTAsync(txHash, cancellationToken.Value) : await _beeDebugClient.TransactionsPOSTAsync(txHash);

            return new TransactionsPostDto(response.TransactionHash, response.AdditionalProperties);
        }

        public async Task<WelcomeMessageGetDto> WelcomeMessageGETAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.WelcomeMessageGETAsync(cancellationToken.Value) : await _beeDebugClient.WelcomeMessageGETAsync();

            return new WelcomeMessageGetDto(response.WelcomeMessage, response.AdditionalProperties);
        }

        public async Task<WelcomeMessagePostDto> WelcomeMessagePOSTAsync(BodyDto body, CancellationToken? cancellationToken)
        {
            var bodeRequest = new Body
            {
                AdditionalProperties = body?.AdditionalProperties,
                WelcomeMessage = body?.WelcomeMessage
            };

            var response = cancellationToken.HasValue ? await _beeDebugClient.WelcomeMessagePOSTAsync(bodeRequest, cancellationToken.Value) : await _beeDebugClient.WelcomeMessagePOSTAsync(bodeRequest);

            return new WelcomeMessagePostDto(response.Status, response.Version, response.ApiVersion, response.DebugApiVersion, response.AdditionalProperties);
        }

        public async Task<WithdrawDto> WithdrawAsync(int amount, int? gasPrice, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.WithdrawAsync(amount, gasPrice, cancellationToken.Value) : await _beeDebugClient.WithdrawAsync(amount, gasPrice);

            return new WithdrawDto(response.TransactionHash, response.AdditionalProperties);
        }

    }
}
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
#pragma warning restore CA1707 // Identifiers should not contain underscores