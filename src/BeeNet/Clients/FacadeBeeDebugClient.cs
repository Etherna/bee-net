using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TestAdapter.Dtos.Debug;

namespace TestAdapter
{
    public class FacadeBeeDebugClient : IFacadeBeeDebugClient
    {
        readonly IFacadeBeeDebugClient _beeDebugClient;
        readonly BeeVersionEnum _beeVersion;

        public FacadeBeeDebugClient(string version, HttpClient httpClient, string baseUrl)
        {
            switch (version)
            {
                case "1.5": 
                    _beeVersion = BeeVersionEnum.v1_5;
                    //_beeDebugClient = new AdapterBeeVersion_1_5(httpClient, baseUrl);
                    break;
                case "1.4":
                default:
                    _beeVersion = BeeVersionEnum.v1_4;
                    _beeDebugClient = new AdapterBeeDebugVersion_1_4(httpClient, baseUrl);
                    break;
            }
        }

        public async Task<AddressResponse> AddressAsync(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.AddressAsync(cancellationToken);
        }

        public async Task<AddressesResponse> AddressesAsync(CancellationToken? cancellationToken = null)
        {
            return await _beeDebugClient.AddressesAsync(cancellationToken);
        }

        public async Task<BalanceResponse> BalanceAsync(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.BalanceAsync(cancellationToken);
        }

        public async Task<Balances2Response> Balances2Async(string address, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.Balances2Async(address, cancellationToken);
        }

        public async Task<BalancesResponse> BalancesAsync(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.BalancesAsync(cancellationToken);
        }

        public async Task<BlocklistReponse> BlocklistAsync(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.BlocklistAsync(cancellationToken);
        }

        public async Task<BucketsResponse> BucketsAsync(object id, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.BucketsAsync(id, cancellationToken);
        }

        public async Task<CashoutGETResponse> CashoutGETAsync(string peer_id, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.CashoutGETAsync(peer_id, cancellationToken);
        }

        public async Task<CashoutPOSTResponse> CashoutPOSTAsync(string peer_id, int? gas_price, long? gas_limit, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.CashoutPOSTAsync(peer_id, gas_price, gas_limit, cancellationToken);
        }

        public async Task<ChainstateReponse> ChainstateAsync(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.ChainstateAsync(cancellationToken);
        }

        public async Task<Cheque2Response> Cheque2Async(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.Cheque2Async(cancellationToken);
        }

        public async Task<ChequeResponse> ChequeAsync(string peer_id, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.ChequeAsync(peer_id, cancellationToken);
        }

        public async Task<ChunksDELETEResponse> ChunksDELETEAsync(string address, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.ChunksDELETEAsync(address, cancellationToken);
        }

        public async Task<ChunksGETResponse> ChunksGETAsync(string address, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.ChunksGETAsync(address, cancellationToken);
        }

        public async Task<ConnectResponse> ConnectAsync(string multiAddress, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.ConnectAsync(multiAddress, cancellationToken);
        }

        public async Task<Consumed2Response> Consumed2Async(string address, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.Consumed2Async(address, cancellationToken);
        }

        public async Task<ConsumedResponse> ConsumedAsync(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.ConsumedAsync(cancellationToken);
        }

        public async Task<DepositResponse> DepositAsync(int amount, int? gas_price, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.DepositAsync(amount, gas_price, cancellationToken);
        }

        public async Task<DiluteResponse> DiluteAsync(object id, int depth, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.DiluteAsync(id, depth, cancellationToken);
        }

        public async Task<HealthResponse> HealthAsync(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.HealthAsync(cancellationToken);
        }

        public async Task<PeersDELETE> PeersDELETEAsync(string address, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.PeersDELETEAsync(address, cancellationToken);
        }

        public async Task<PeersGETResponse> PeersGETAsync(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.PeersGETAsync(cancellationToken);
        }

        public async Task<PingpongResponse> PingpongAsync(string peer_id, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.PingpongAsync(peer_id, cancellationToken);
        }

        public async Task<ReadinessResponse> ReadinessAsync(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.ReadinessAsync(cancellationToken);
        }

        public async Task<ReservestateResponse> ReservestateAsync(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.ReservestateAsync(cancellationToken);
        }

        public async Task<Settlements2Response> Settlements2Async(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.Settlements2Async(cancellationToken);
        }

        public async Task<SettlementsResponse> SettlementsAsync(string address, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.SettlementsAsync(address, cancellationToken);
        }

        public async Task<StampsGET2Response> StampsGET2Async(object id, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.StampsGET2Async(id, cancellationToken);
        }

        public async Task<StampsGETResponse> StampsGETAsync(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.StampsGETAsync(cancellationToken);
        }

        public async Task<StampsPOSTResponse> StampsPOSTAsync(int amount, int depth, string label, bool? immutable, int? gas_price, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.StampsPOSTAsync(amount, depth, label, immutable, gas_price, cancellationToken);
        }

        public async Task<TagsResponse> TagsAsync(int uid, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.TagsAsync(uid, cancellationToken);
        }

        public async Task<TimesettlementsResponse> TimesettlementsAsync(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.TimesettlementsAsync(cancellationToken);
        }

        public async Task<TopologyResponse> TopologyAsync(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.TopologyAsync(cancellationToken);
        }

        public async Task<TopupResponse> TopupAsync(object id, int amount, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.TopupAsync(id, amount, cancellationToken);
        }

        public async Task<TransactionsDELETEResponse> TransactionsDELETEAsync(string txHash, int? gas_price, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.TransactionsDELETEAsync(txHash, gas_price, cancellationToken);
        }

        public async Task<TransactionsGET2Response> TransactionsGET2Async(string txHash, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.TransactionsGET2Async(txHash, cancellationToken);
        }

        public async Task<TransactionsResponse> TransactionsGETAsync(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.TransactionsGETAsync(cancellationToken);
        }

        public async Task<TransactionsPOSTResponse> TransactionsPOSTAsync(string txHash, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.TransactionsPOSTAsync(txHash, cancellationToken);
        }

        public async Task<WelcomeMessageGETResponse> WelcomeMessageGETAsync(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.WelcomeMessageGETAsync(cancellationToken);
        }

        public async Task<WelcomeMessagePOSTResponse> WelcomeMessagePOSTAsync(BodyDto body, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.WelcomeMessagePOSTAsync(body, cancellationToken);
        }

        public async Task<WithdrawResponse> WithdrawAsync(int amount, int? gas_price, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.WithdrawAsync(amount, gas_price, cancellationToken);
        }
    }
}
