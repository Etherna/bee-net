using Etherna.BeeNet.Clients.DebugApi;
using Etherna.BeeNet.Clients.GatewayApi.V3_0_2;
using Etherna.BeeNet.DtoModels;
using Etherna.BeeNet.InputModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Clients.GatewayApi
{
    public class BeeGatewayClient : IBeeGatewayClient
    {
        // Fields.
        private readonly IBeeGatewayClient_3_0_2 beeGatewayApiClient_3_0_2;

        // Constructors.
        public BeeGatewayClient(HttpClient httpClient, Uri baseUrl, GatewayApiVersion apiVersion)
        {
            if (baseUrl is null)
                throw new ArgumentNullException(nameof(baseUrl));

            beeGatewayApiClient_3_0_2 = new BeeGatewayClient_3_0_2(httpClient) { BaseUrl = baseUrl.ToString() };
            CurrentApiVersion = apiVersion;
        }

        // Properties.
        public GatewayApiVersion CurrentApiVersion { get; set; }

        // Methods.
        public async Task<AuthDto> AuthenticateAsync(BeeAuthicationData beeAuthicationData, string role, int expiry)
        {
            if (beeAuthicationData is null)
                throw new ArgumentNullException(nameof(beeAuthicationData));

            return CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new AuthDto(await beeGatewayApiClient_3_0_2.AuthAsync(
                    beeAuthicationData.Username,
                    beeAuthicationData.Password,
                    new V3_0_2.Body
                    {
                        Role = role,
                        Expiry = expiry
                    }).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };
        }
        public async Task<StewardShipGetDto> CheckIsContentAvailableAsync(string reference) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new StewardShipGetDto(await beeGatewayApiClient_3_0_2.StewardshipGetAsync(reference).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> CreateFeedAsync(
            string owner,
            string topic,
            string swarmPostageBatchId,
            string? type = null,
            bool? swarmPin = null) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.FeedsPostAsync(owner, topic, swarmPostageBatchId, type, swarmPin).ConfigureAwait(false)).Reference,
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> CreatePinAsync(string reference) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new MessageResponseDto(await beeGatewayApiClient_3_0_2.PinsPostAsync(reference).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TagInfoDto> CreateTagAsync(string address) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new TagInfoDto(await beeGatewayApiClient_3_0_2.TagsPostAsync(
                    new V3_0_2.Body3
                    {
                        Address = address
                    }).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeletePinAsync(string reference) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new MessageResponseDto(await beeGatewayApiClient_3_0_2.PinsDeleteAsync(reference).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public Task DeleteTagAsync(int uid) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => beeGatewayApiClient_3_0_2.TagsDeleteAsync(uid),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetAllPinsAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.PinsGetAsync().ConfigureAwait(false)).References,
                _ => throw new InvalidOperationException()
            };
        public async Task<Stream> GetChunkStreamAsync(string reference) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.ChunksGetAsync(reference).ConfigureAwait(false)).Stream,
                _ => throw new InvalidOperationException()
            };

        public async Task<Stream> GetDataAsync(string reference) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.BytesGetAsync(reference).ConfigureAwait(false)).Stream,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetFeedAsync(
            string owner,
            string topic,
            int? at = null,
            string? type = null) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.FeedsGetAsync(owner, topic, at, type).ConfigureAwait(false)).Reference,
                _ => throw new InvalidOperationException()
            };

        public async Task<Stream> GetFileWithPathAsync(string reference, string path) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.BzzGetAsync(reference, path).ConfigureAwait(false)).Stream,
                _ => throw new InvalidOperationException()
            };

        public async Task<Stream> GetFileAsync(string reference) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.BzzGetAsync(reference, CancellationToken.None).ConfigureAwait(false)).Stream,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetPinStatusAsync(string reference) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => await beeGatewayApiClient_3_0_2.PinsGetAsync(reference).ConfigureAwait(false),
                _ => throw new InvalidOperationException()
            };

        public async Task<TagInfoDto> GetTagInfoAsync(int uid) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new TagInfoDto(await beeGatewayApiClient_3_0_2.TagsGetAsync(uid).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<TagInfoDto>> GetTagsListAsync(int? offset = null, int? limit = null) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.TagsGetAsync(offset, limit).ConfigureAwait(false)).Tags.Select(i => new TagInfoDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> RefreshAuthAsync(string role, int expiry) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.RefreshAsync(
                    new V3_0_2.Body2
                    {
                        Role = role,
                        Expiry = expiry
                    }).ConfigureAwait(false)).Key,
                _ => throw new InvalidOperationException()
            };

        public Task ReuploadContentAsync(string reference) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => beeGatewayApiClient_3_0_2.StewardshipPutAsync(reference),
                _ => throw new InvalidOperationException()
            };

        public Task SendPssAsync(
            string topic,
            string targets,
            string swarmPostageBatchId,
            string? recipient = null) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => beeGatewayApiClient_3_0_2.PssSendAsync(topic, targets, swarmPostageBatchId, recipient),
                _ => throw new InvalidOperationException()
            };

        public Task SubscribeToPssAsync(string topic) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => beeGatewayApiClient_3_0_2.PssSubscribeAsync(topic),
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> UpdateTagAsync(int uid, string? address = null) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new VersionDto(await beeGatewayApiClient_3_0_2.TagsPatchAsync(
                    uid,
                    address is null ?
                        null :
                        new V3_0_2.Body4 { Address = address }).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> UploadChunkAsync(
            string swarmPostageBatchId,
            int? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmDeferredUpload = null,
            Stream? body = null) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new VersionDto(await beeGatewayApiClient_3_0_2.ChunksPostAsync(
                    swarmPostageBatchId,
                    swarmTag,
                    swarmPin,
                    swarmDeferredUpload,
                    body).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public Task UploadChunksStreamAsync(
            string swarmPostageBatchId,
            int? swarmTag = null,
            bool? swarmPin = null) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => beeGatewayApiClient_3_0_2.ChunksStreamAsync(swarmPostageBatchId, swarmTag, swarmPin),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> UploadDataAsync(
            string swarmPostageBatchId,
            int? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            bool? swarmDeferredUpload = null,
            Stream? body = null) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.BytesPostAsync(
                    swarmPostageBatchId,
                    swarmTag,
                    swarmPin,
                    swarmEncrypt,
                    swarmDeferredUpload,
                    body).ConfigureAwait(false)).Reference,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> UploadFileAsync(
            string swarmPostageBatchId,
            string? name,
            int? swarmTag,
            bool? swarmPin,
            bool? swarmEncrypt,
            string? contentType,
            bool? swarmCollection,
            string? swarmIndexDocument,
            string? swarmErrorDocument,
            bool? swarmDeferredUpload,
            IEnumerable<FileParameterInput>? file) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.BzzPostAsync(
                    swarmPostageBatchId,
                    name,
                    swarmTag,
                    swarmPin,
                    swarmEncrypt,
                    contentType,
                    swarmCollection,
                    swarmIndexDocument,
                    swarmErrorDocument,
                    swarmDeferredUpload,
                    file.Select(f => new V3_0_2.FileParameter(f.Data, f.FileName, f.ContentType))).ConfigureAwait(false)).Reference,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> UploadSocAsync(
            string owner,
            string id,
            string sig,
            bool? swarmPin = null) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.SocAsync(owner, id, sig, swarmPin).ConfigureAwait(false)).Reference,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> BuyPostageBatchAsync(
            long amount,
            int depth,
            string? label = null,
            bool? immutable = null,
            long? gasPrice = null) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.StampsPostAsync(amount.ToString(CultureInfo.InvariantCulture), depth, label, immutable, gasPrice).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> CashoutChequeForPeerAsync(
            string peerId,
            long? gasPrice = null,
            long? gasLimit = null) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.ChequebookCashoutPostAsync(peerId, gasPrice, gasLimit).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> ConnectToPeerAsync(string address) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.ConnectAsync(address).ConfigureAwait(false)).Address,
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeleteChunkAsync(string address) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new MessageResponseDto(await beeGatewayApiClient_3_0_2.ChunksDeleteAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeletePeerAsync(string address) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new MessageResponseDto(await beeGatewayApiClient_3_0_2.PeersDeleteAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DeleteTransactionAsync(
            string txHash,
            long? gasPrice = null) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.TransactionsDeleteAsync(txHash, gasPrice).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DepositIntoChequeBookAsync(
            long amount,
            long? gasPrice = null) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DilutePostageBatchAsync(
            string id,
            int depth) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.StampsDiluteAsync(id, depth).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<AddressDetailDto> GetAddressesAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new AddressDetailDto(await beeGatewayApiClient_3_0_2.AddressesAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetAllBalancesAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.BalancesGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<ChequeBookChequeGetDto>> GetAllChequeBookChequesAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.ChequebookChequeGetAsync().ConfigureAwait(false)).Lastcheques.Select(i => new ChequeBookChequeGetDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetAllConsumedBalancesAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.ConsumedGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetAllPeerAddressesAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.PeersGetAsync().ConfigureAwait(false)).Peers.Select(i => i.Address),
                _ => throw new InvalidOperationException()
            };

        public async Task<SettlementDto> GetAllSettlementsAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new SettlementDto(await beeGatewayApiClient_3_0_2.SettlementsGetAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TimeSettlementsDto> GetAllTimeSettlementsAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new TimeSettlementsDto(await beeGatewayApiClient_3_0_2.TimesettlementsAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PostageBatchShortDto>> GetAllValidPostageBatchesFromAllNodesAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.BatchesAsync().ConfigureAwait(false)).Batches.Select(i => new PostageBatchShortDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<BalanceDto> GetBalanceWithPeerAsync(string address) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new BalanceDto(await beeGatewayApiClient_3_0_2.BalancesGetAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetBlocklistedPeerAddressesAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.BlocklistAsync().ConfigureAwait(false)).Peers.Select(i => i.Address),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChainStateDto> GetChainStateAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new ChainStateDto(await beeGatewayApiClient_3_0_2.ChainstateAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetChequeBookAddressAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.ChequebookAddressAsync().ConfigureAwait(false)).ChequebookAddress,
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookBalanceDto> GetChequeBookBalanceAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new ChequeBookBalanceDto(await beeGatewayApiClient_3_0_2.ChequebookBalanceAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookCashoutGetDto> GetChequeBookCashoutForPeerAsync(string peerId) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new ChequeBookCashoutGetDto(await beeGatewayApiClient_3_0_2.ChequebookCashoutGetAsync(peerId).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookChequeGetDto> GetChequeBookChequeForPeerAsync(string peerId) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new ChequeBookChequeGetDto(await beeGatewayApiClient_3_0_2.ChequebookChequeGetAsync(peerId).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<FileResponseDto> GetChunkAsync(string address) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new FileResponseDto(await beeGatewayApiClient_3_0_2.ChunksGetAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> ChunksHeadAsync(string address) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new MessageResponseDto(await beeGatewayApiClient_3_0_2.ChunksHeadAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };
        
        public async Task<BalanceDto> GetConsumedBalanceWithPeerAsync(string address) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new BalanceDto(await beeGatewayApiClient_3_0_2.ConsumedGetAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> GetHealthAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new VersionDto(await beeGatewayApiClient_3_0_2.HealthAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<NodeInfoDto> GetNodeInfoAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new NodeInfoDto(await beeGatewayApiClient_3_0_2.NodeAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PostageBatchDto>> GetOwnedPostageBatchesByNodeAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.StampsGetAsync().ConfigureAwait(false)).Stamps.Select(i => new PostageBatchDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PendingTransactionDto>> GetPendingTransactionsAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.TransactionsGetAsync().ConfigureAwait(false)).PendingTransactions.Select(i => new PendingTransactionDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<PostageBatchDto> GetPostageBatchAsync(string id) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new PostageBatchDto(await beeGatewayApiClient_3_0_2.StampsGetAsync(id).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ReserveStateDto> GetReserveStateAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new ReserveStateDto(await beeGatewayApiClient_3_0_2.ReservestateAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<SettlementDataDto> GetSettlementsWithPeerAsync(string address) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new SettlementDataDto(await beeGatewayApiClient_3_0_2.SettlementsGetAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<StampsBucketsDto> GetStampsBucketsForBatchAsync(string batchId) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new StampsBucketsDto(await beeGatewayApiClient_3_0_2.StampsBucketsAsync(batchId).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TopologyDto> GetSwarmTopologyAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new TopologyDto(await beeGatewayApiClient_3_0_2.TopologyAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TransactionsDto> GetTransactionInfoAsync(string txHash) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new TransactionsDto(await beeGatewayApiClient_3_0_2.TransactionsGetAsync(txHash).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetWelcomeMessageAsync() =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.WelcomeMessageGetAsync().ConfigureAwait(false)).WelcomeMessage,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> RebroadcastTransactionAsync(string txHash) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.TransactionsPostAsync(txHash).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> SetWelcomeMessageAsync(string welcomeMessage) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => new VersionDto(await beeGatewayApiClient_3_0_2.WelcomeMessagePostAsync(
                    new Body5
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
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.StampsTopupAsync(id, amount).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> TryConnectToPeerAsync(string peerId) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.PingpongAsync(peerId).ConfigureAwait(false)).Rtt,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> WithdrawFromChequeBookAsync(
            long amount,
            long? gasPrice = null) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v3_0_2 => (await beeGatewayApiClient_3_0_2.ChequebookWithdrawAsync(amount, gasPrice).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public void SetAuthToken(string token)
        {
            if (CurrentApiVersion == GatewayApiVersion.v3_0_2)
            {
                beeGatewayApiClient_3_0_2.SetAuthToken(token);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
}
}