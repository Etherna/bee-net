using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestAdapter.Clients;
using TestAdapter.Clients.DebugApi;
using TestAdapter.Dtos;
using TestAdapter.Dtos.Debug;

namespace TestAdapter
{
    public class AdapterBeeDebugVersion_1_4 : IFacadeBeeDebugClient
    {
        readonly IBeeDebugClient_1_4 _beeDebugClient;

        public AdapterBeeDebugVersion_1_4(HttpClient httpClient, string baseUrl)
        {
            _beeDebugClient = new BeeDebugClient_1_4(httpClient) { BaseUrl = baseUrl };
        }

        public async Task<AddressResponse> AddressAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.AddressAsync(cancellationToken.Value) : await _beeDebugClient.AddressAsync();

            return new AddressResponse
            {
                ChequebookAddress = response.ChequebookAddress,
                AdditionalProperties = response.AdditionalProperties
            };
        }

        public async Task<AddressesResponse> AddressesAsync(CancellationToken? cancellationToken = null)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.AddressesAsync(cancellationToken.Value) : await _beeDebugClient.AddressesAsync();

            return new AddressesResponse
            {
                Ethereum = response.Ethereum,
                Overlay = response.Overlay,
                PssPublicKey = response.PssPublicKey,
                PublicKey = response.PublicKey,
                Underlay = response.Underlay
            };
        }

        public async Task<BalanceResponse> BalanceAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.BalanceAsync(cancellationToken.Value) : await _beeDebugClient.BalanceAsync();

            return new BalanceResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                AvailableBalance = response.AvailableBalance,
                TotalBalance = response.TotalBalance
            };
        }

        public async Task<Balances2Response> Balances2Async(string address, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.Balances2Async(address, cancellationToken.Value) : await _beeDebugClient.Balances2Async(address);

            return new Balances2Response
            {
                AdditionalProperties = response.AdditionalProperties,
                Balance = response.Balance,
                Peer = response.Peer
            };
        }

        public async Task<BalancesResponse> BalancesAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.BalancesAsync(cancellationToken.Value) : await _beeDebugClient.BalancesAsync();

            return new BalancesResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Balances = response.Balances?.Select(i => new BalancesDto { Balance = i.Balance, Peer = i.Peer, AdditionalProperties = i.AdditionalProperties }).ToList()
            };
        }

        public async Task<BlocklistReponse> BlocklistAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.BlocklistAsync(cancellationToken.Value) : await _beeDebugClient.BlocklistAsync();

            return new BlocklistReponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Peers = response.Peers?.Select(i => new PeersDto { Address = i.Address, AdditionalProperties = i.AdditionalProperties }).ToList()
            };
        }

        public async Task<BucketsResponse> BucketsAsync(object id, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.BucketsAsync(id, cancellationToken.Value) : await _beeDebugClient.BucketsAsync(id);

            return new BucketsResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                BucketDepth = response.BucketDepth,
                Buckets = response.Buckets?.Select(i => new BucketsDto { AdditionalProperties = i.AdditionalProperties, BucketID = i.BucketID, Collisions = i.Collisions }).ToList(),
                BucketUpperBound = response.BucketUpperBound,
                Depth = response.Depth
            };
        }

        public async Task<CashoutGETResponse> CashoutGETAsync(string peer_id, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.CashoutGETAsync(peer_id, cancellationToken.Value) : await _beeDebugClient.CashoutGETAsync(peer_id);

            return new CashoutGETResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                LastCashedCheque = response.LastCashedCheque != null ? new LastCashedChequeDto
                {
                    AdditionalProperties = response.LastCashedCheque?.AdditionalProperties,
                    Beneficiary = response.LastCashedCheque?.Beneficiary,
                    Chequebook = response.LastCashedCheque?.Chequebook,
                    Payout = response.LastCashedCheque?.Payout
                } : null,
                Peer = response.Peer,
                Result = response.Result != null ? new ResultDto
                {
                    AdditionalProperties = response.Result.AdditionalProperties,
                    Bounced = response.Result.Bounced,
                    LastPayout = response.Result.LastPayout,
                    Recipient = response.Result.Recipient
                } : null,
                TransactionHash = response.TransactionHash,
                UncashedAmount = response.UncashedAmount
            };
        }

        public async Task<CashoutPOSTResponse> CashoutPOSTAsync(string peer_id, int? gas_price, long? gas_limit, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.CashoutPOSTAsync(peer_id, gas_price, gas_limit, cancellationToken.Value) : await _beeDebugClient.CashoutPOSTAsync(peer_id, gas_price, gas_limit);

            return new CashoutPOSTResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                TransactionHash = response.TransactionHash
            };
        }

        public async Task<ChainstateReponse> ChainstateAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.ChainstateAsync(cancellationToken.Value) : await _beeDebugClient.ChainstateAsync();

            return new ChainstateReponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Block = response.Block,
                CurrentPrice = response.CurrentPrice,
                TotalAmount = response.TotalAmount
            };
        }

        public async Task<Cheque2Response> Cheque2Async(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.Cheque2Async(cancellationToken.Value) : await _beeDebugClient.Cheque2Async();

            return new Cheque2Response
            {
                AdditionalProperties = response.AdditionalProperties,
                Lastcheques = response.Lastcheques?.Select(i => new LastchequesDto
                {
                    AdditionalProperties = i.AdditionalProperties,
                    Lastreceived = i.Lastreceived != null ? new Lastreceived2Dto
                    {
                        AdditionalProperties = i.Lastreceived.AdditionalProperties,
                        Beneficiary = i.Lastreceived.Beneficiary,
                        Chequebook = i.Lastreceived.Chequebook,
                        Payout = i.Lastreceived.Payout
                    } : null,
                    Lastsent = i.Lastsent != null ? new Lastsent2Dto
                    {
                        AdditionalProperties = i.Lastsent.AdditionalProperties,
                        Beneficiary = i.Lastsent.Beneficiary,
                        Chequebook = i.Lastsent.Chequebook,
                        Payout = i.Lastsent.Payout,
                    } : null,
                    Peer = i.Peer
                }).ToList()
            };
        }

        public async Task<ChequeResponse> ChequeAsync(string peer_id, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.ChequeAsync(peer_id, cancellationToken.Value) : await _beeDebugClient.ChequeAsync(peer_id);

            return new ChequeResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Lastreceived = response.Lastreceived != null ? new LastreceivedDto
                {
                    AdditionalProperties = response.Lastreceived.AdditionalProperties,
                    Beneficiary = response.Lastreceived.Beneficiary,
                    Chequebook = response.Lastreceived.Chequebook,
                    Payout = response.Lastreceived.Payout
                } : null,
                Lastsent = response.Lastsent != null ? new LastsentDto
                {
                    AdditionalProperties = response.Lastsent.AdditionalProperties,
                    Beneficiary = response.Lastsent.Beneficiary,
                    Chequebook = response.Lastsent.Chequebook
                } : null,
                Peer = response.Peer
            };
        }

        public async Task<ChunksDELETEResponse> ChunksDELETEAsync(string address, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.ChunksDELETEAsync(address, cancellationToken.Value) : await _beeDebugClient.ChunksDELETEAsync(address);

            return new ChunksDELETEResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Code = response.Code,
                Message = response.Message
            };
        }

        public async Task<ChunksGETResponse> ChunksGETAsync(string address, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.ChunksGETAsync(address, cancellationToken.Value) : await _beeDebugClient.ChunksGETAsync(address);

            return new ChunksGETResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Code = response.Code,
                Message = response.Message
            };
        }

        public async Task<ConnectResponse> ConnectAsync(string multiAddress, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.ConnectAsync(multiAddress, cancellationToken.Value) : await _beeDebugClient.ConnectAsync(multiAddress);

            return new ConnectResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Address = response.Address
            };
        }

        public async Task<Consumed2Response> Consumed2Async(string address, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.Consumed2Async(address, cancellationToken.Value) : await _beeDebugClient.Consumed2Async(address);

            return new Consumed2Response
            {
                AdditionalProperties = response.AdditionalProperties,
                Balance = response.Balance,
                Peer = response.Peer
            };
        }

        public async Task<ConsumedResponse> ConsumedAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.ConsumedAsync(cancellationToken.Value) : await _beeDebugClient.ConsumedAsync();

            return new ConsumedResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Balances = response.Balances?.Select(i => new Balances2Dto { Balance = i.Balance, Peer = i.Peer, AdditionalProperties = i.AdditionalProperties }).ToList()
            };
        }

        public async Task<DepositResponse> DepositAsync(int amount, int? gas_price, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.DepositAsync(amount, gas_price, cancellationToken.Value) : await _beeDebugClient.DepositAsync(amount, gas_price);

            return new DepositResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                TransactionHash = response.TransactionHash
            };
        }

        public async Task<DiluteResponse> DiluteAsync(object id, int depth, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.DiluteAsync(id, depth, cancellationToken.Value) : await _beeDebugClient.DiluteAsync(id, depth);

            return new DiluteResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                BatchID = response.BatchID
            };
        }

        public async Task<HealthResponse> HealthAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.HealthAsync(cancellationToken.Value) : await _beeDebugClient.HealthAsync();

            return new HealthResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                ApiVersion = response.ApiVersion,
                DebugApiVersion = response.DebugApiVersion,
                Status = response.Status,
                Version = response.Version
            };
        }

        public async Task<PeersDELETE> PeersDELETEAsync(string address, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.PeersDELETEAsync(address, cancellationToken.Value) : await _beeDebugClient.PeersDELETEAsync(address);

            return new PeersDELETE
            {
                AdditionalProperties = response.AdditionalProperties,
                Code = response.Code,
                Message = response.Message
            };
        }

        public async Task<PeersGETResponse> PeersGETAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.PeersGETAsync(cancellationToken.Value) : await _beeDebugClient.PeersGETAsync();

            return new PeersGETResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Peers = response.Peers?.Select(i => new Peers2Dto { AdditionalProperties = i.AdditionalProperties, Address = i.Address }).ToList()
            };
        }

        public async Task<PingpongResponse> PingpongAsync(string peer_id, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.PingpongAsync(peer_id, cancellationToken.Value) : await _beeDebugClient.PingpongAsync(peer_id);

            return new PingpongResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Rtt = response.Rtt
            };
        }

        public async Task<ReadinessResponse> ReadinessAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.ReadinessAsync(cancellationToken.Value) : await _beeDebugClient.ReadinessAsync();

            return new ReadinessResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                ApiVersion = response.ApiVersion,
                DebugApiVersion = response.DebugApiVersion,
                Status = response.Status,
                Version = response.Version
            };
        }

        public async Task<ReservestateResponse> ReservestateAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.ReservestateAsync(cancellationToken.Value) : await _beeDebugClient.ReservestateAsync();

            return new ReservestateResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Available = response.Available,
                Inner = response.Inner,
                Outer = response.Outer,
                Radius = response.Radius
            };
        }

        public async Task<Settlements2Response> Settlements2Async(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.Settlements2Async(cancellationToken.Value) : await _beeDebugClient.Settlements2Async();

            return new Settlements2Response
            {
                AdditionalProperties = response.AdditionalProperties,
                Settlements = response.Settlements?.Select(i => new SettlementsDto
                {
                    AdditionalProperties = i.AdditionalProperties,
                    Peer = i.Peer,
                    Received = i.Received,
                    Sent = i.Sent
                }).ToList(),
                TotalReceived = response.TotalReceived,
                TotalSent = response.TotalSent
            };
        }

        public async Task<SettlementsResponse> SettlementsAsync(string address, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.SettlementsAsync(address, cancellationToken.Value) : await _beeDebugClient.SettlementsAsync(address);

            return new SettlementsResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Peer = response.Peer,
                Received = response.Received,
                Sent = response.Sent
            };
        }

        public async Task<StampsGET2Response> StampsGET2Async(object id, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.StampsGET2Async(id, cancellationToken.Value) : await _beeDebugClient.StampsGET2Async(id);

            return new StampsGET2Response
            {
                AdditionalProperties = response.AdditionalProperties,
                Amount = response.Amount,
                BatchID = response.BatchID,
                BatchTTL = response.BatchTTL,
                BlockNumber = response.BlockNumber,
                BucketDepth = response.BucketDepth,
                Depth = response.Depth,
                Exists = response.Exists,
                ImmutableFlag = response.ImmutableFlag,
                Label = response.Label,
                Usable = response.Usable,
                Utilization = response.Utilization
            };
        }

        public async Task<StampsGETResponse> StampsGETAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.StampsGETAsync(cancellationToken.Value) : await _beeDebugClient.StampsGETAsync();

            return new StampsGETResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Stamps = response.Stamps?.Select(i => new StampsDto
                {
                    AdditionalProperties = response.AdditionalProperties,
                    Amount = i.Amount,
                    BatchID = i.BatchID,
                    BatchTTL = i.BatchTTL,
                    BlockNumber = i.BlockNumber,
                    BucketDepth = i.BucketDepth,
                    Depth = i.Depth,
                    Exists = i.Exists,
                    ImmutableFlag = i.ImmutableFlag,
                    Label = i.Label,
                    Usable = i.Usable,
                    Utilization = i.Utilization
                }).ToList()
            };
        }

        public async Task<StampsPOSTResponse> StampsPOSTAsync(int amount, int depth, string label, bool? immutable, int? gas_price, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.StampsPOSTAsync(amount, depth, label, immutable, gas_price, cancellationToken.Value) : await _beeDebugClient.StampsPOSTAsync(amount, depth, label, immutable, gas_price);

            return new StampsPOSTResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                BatchID = response.BatchID
            };
        }

        public async Task<TagsResponse> TagsAsync(int uid, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.TagsAsync(uid, cancellationToken.Value) : await _beeDebugClient.TagsAsync(uid);

            return new TagsResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Address = response.Address,
                Seen = response.Seen,
                Sent = response.Sent,
                Split = response.Split,
                StartedAt = response.StartedAt,
                Stored = response.Stored,
                Synced = response.Synced,
                Total = response.Total,
                Uid = response.Uid
            };
        }

        public async Task<TimesettlementsResponse> TimesettlementsAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.TimesettlementsAsync(cancellationToken.Value) : await _beeDebugClient.TimesettlementsAsync();

            return new TimesettlementsResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Settlements = response.Settlements?.Select(i => new Settlements2Dto
                {
                    AdditionalProperties = i.AdditionalProperties,
                    Peer = i.Peer,
                    Received = i.Received,
                    Sent = i.Sent
                }).ToList(),
                TotalReceived = response.TotalReceived,
                TotalSent = response.TotalSent
            };
        }

        public async Task<TopologyResponse> TopologyAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.TopologyAsync(cancellationToken.Value) : await _beeDebugClient.TopologyAsync();

            return new TopologyResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                BaseAddr = response.BaseAddr,
                Bins = response.Bins?.ToDictionary(i => i.Key, i => new AnonymousDto
                {
                    AdditionalProperties = i.Value.AdditionalProperties,
                    Connected = i.Value.Connected,
                    ConnectedPeers = i.Value.ConnectedPeers?.Select(k => new ConnectedPeersDto
                    {
                        Address = k.Address,
                        AdditionalProperties = k.AdditionalProperties,
                        Metrics = new Metrics2Dto
                        {
                            AdditionalProperties = k.Metrics.AdditionalProperties,
                            ConnectionTotalDuration = k.Metrics.ConnectionTotalDuration,
                            LastSeenTimestamp = k.Metrics.LastSeenTimestamp,
                            LatencyEWMA = k.Metrics.LatencyEWMA,
                            SessionConnectionDirection = k.Metrics.SessionConnectionDirection,
                            SessionConnectionRetry = k.Metrics.SessionConnectionRetry,
                            SessionConnectionDuration = k.Metrics.SessionConnectionDuration
                        }
                    }).ToList(),
                    DisconnectedPeers = i.Value.DisconnectedPeers?.Select(k => new DisconnectedPeersResponse
                    {
                        Address = k.Address,
                        AdditionalProperties = k.AdditionalProperties,
                        Metrics = new MetricsDto
                        {
                            AdditionalProperties = k.Metrics.AdditionalProperties,
                            ConnectionTotalDuration = k.Metrics.ConnectionTotalDuration,
                            LastSeenTimestamp = k.Metrics.LastSeenTimestamp,
                            LatencyEWMA = k.Metrics.LatencyEWMA,
                            SessionConnectionDirection = k.Metrics.SessionConnectionDirection,
                            SessionConnectionRetry = k.Metrics.SessionConnectionRetry,
                            SessionConnectionDuration = k.Metrics.SessionConnectionDuration
                        }
                    }).ToList(),
                    Population = i.Value.Population,
                }),
                Connected = response.Connected,
                Depth = response.Depth,
                NnLowWatermark = response.NnLowWatermark,
                Population = response.Population,
                Timestamp = response.Timestamp
            };
        }

        public async Task<TopupResponse> TopupAsync(object id, int amount, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.TopupAsync(id, amount, cancellationToken.Value) : await _beeDebugClient.TopupAsync(id, amount);

            return new TopupResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                BatchID = response.BatchID
            };
        }

        public async Task<TransactionsDELETEResponse> TransactionsDELETEAsync(string txHash, int? gas_price, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.TransactionsDELETEAsync(txHash, gas_price, cancellationToken.Value) : await _beeDebugClient.TransactionsDELETEAsync(txHash, gas_price);

            return new TransactionsDELETEResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                TransactionHash = response.TransactionHash
            };
        }

        public async Task<TransactionsGET2Response> TransactionsGET2Async(string txHash, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.TransactionsGET2Async(txHash, cancellationToken.Value) : await _beeDebugClient.TransactionsGET2Async(txHash);

            return new TransactionsGET2Response
            {
                AdditionalProperties = response.AdditionalProperties,
                Created = response.Created,
                Data = response.Data,
                Description = response.Description,
                GasLimit = response.GasLimit,
                GasPrice = response.GasPrice,
                Nonce = response.Nonce,
                To = response.To,
                TransactionHash = response.TransactionHash,
                Value = response.Value
            };
        }

        public async Task<TransactionsResponse> TransactionsGETAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.TransactionsGETAsync(cancellationToken.Value) : await _beeDebugClient.TransactionsGETAsync();

            return new TransactionsResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                PendingTransactions = response.PendingTransactions?.Select(i => new PendingTransactionsDto
                {
                    Created = i.Created,
                    Data = i.Data,
                    Description = i.Description,
                    GasLimit = i.GasLimit,
                    GasPrice = i.GasPrice,
                    Nonce = i.Nonce,
                    To = i.To,
                    TransactionHash = i.TransactionHash,
                    Value = i.Value
                }).ToList()
            };
        }

        public async Task<TransactionsPOSTResponse> TransactionsPOSTAsync(string txHash, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.TransactionsPOSTAsync(txHash, cancellationToken.Value) : await _beeDebugClient.TransactionsPOSTAsync(txHash);

            return new TransactionsPOSTResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                TransactionHash = response.TransactionHash
            };
        }

        public async Task<WelcomeMessageGETResponse> WelcomeMessageGETAsync(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.WelcomeMessageGETAsync(cancellationToken.Value) : await _beeDebugClient.WelcomeMessageGETAsync();

            return new WelcomeMessageGETResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                WelcomeMessage = response.WelcomeMessage
            };
        }

        public async Task<WelcomeMessagePOSTResponse> WelcomeMessagePOSTAsync(BodyDto body, CancellationToken? cancellationToken)
        {
            var bodeRequest = new Body
            {
                AdditionalProperties = body.AdditionalProperties,
                WelcomeMessage = body.WelcomeMessage
            };

            var response = cancellationToken.HasValue ? await _beeDebugClient.WelcomeMessagePOSTAsync(bodeRequest, cancellationToken.Value) : await _beeDebugClient.WelcomeMessagePOSTAsync(bodeRequest);

            return new WelcomeMessagePOSTResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                ApiVersion = response.ApiVersion,
                DebugApiVersion = response.DebugApiVersion,
                Status = response.Status,
                Version = response.Version
            };
        }

        public async Task<WithdrawResponse> WithdrawAsync(int amount, int? gas_price, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeDebugClient.WithdrawAsync(amount, gas_price, cancellationToken.Value) : await _beeDebugClient.WithdrawAsync(amount, gas_price);

            return new WithdrawResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                TransactionHash = response.TransactionHash
            };
        }

    }
}
