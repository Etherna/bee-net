//   Copyright 2021-present Etherna SA
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Etherna.BeeNet.Clients;
using Etherna.BeeNet.Exceptions;
using Etherna.BeeNet.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet
{
    [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings")]
    public class BeeClient : IBeeClient, IDisposable
    {
        // Consts.
        public readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(10);

        // Fields.
        private readonly BeeGeneratedClient generatedClient;
        private readonly HttpClient httpClient;
        
        private bool disposed;

        // Constructors.
        public BeeClient(
            string baseUrl = "http://localhost/",
            int gatewayApiPort = 1633,
            HttpClient? customHttpClient = null)
        {
            httpClient = customHttpClient ?? new HttpClient { Timeout = DefaultTimeout };

            GatewayApiUrl = new Uri(BuildBaseUrl(baseUrl, gatewayApiPort));
            generatedClient = new BeeGeneratedClient(httpClient) { BaseUrl = GatewayApiUrl.ToString() };
        }

        // Dispose.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            // Dispose managed resources.
            if (disposing)
                httpClient.Dispose();

            disposed = true;
        }


        // Properties.
        public Uri GatewayApiUrl { get; }
        
        // Methods.
        public async Task<Dictionary<string, Account>> AccountingAsync(
            CancellationToken cancellationToken = default) =>
            (await generatedClient.AccountingAsync(cancellationToken).ConfigureAwait(false)).PeerData.ToDictionary(i => i.Key, i => new Account(i.Value));

        public async Task<Auth> AuthenticateAsync(string role, int expiry) =>
            new(await generatedClient.AuthAsync(
                new Body
                {
                    Role = role,
                    Expiry = expiry
                }).ConfigureAwait(false));
        
        public async Task<string> BuyPostageBatchAsync(
            long amount,
            int depth,
            string? label = null,
            bool? immutable = null,
            long? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.StampsPostAsync(amount.ToString(CultureInfo.InvariantCulture), depth, label, immutable, gasPrice, gasLimit, cancellationToken).ConfigureAwait(false)).BatchID;

        public async Task<string> CashoutChequeForPeerAsync(
            string peerId,
            long? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookCashoutPostAsync(peerId, gasPrice, gasLimit, cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<bool> CheckChunkExistsAsync(
            string address,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await generatedClient.ChunksHeadAsync(address, cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (BeeNetApiException)
            {
                return false;
            }
        }

        public async Task<StewardshipGet> CheckIsContentAvailableAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.StewardshipGetAsync(reference, cancellationToken).ConfigureAwait(false));

        public async Task<CheckPinsResult> CheckPinsAsync(
            string? reference,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.PinsCheckAsync(reference, cancellationToken).ConfigureAwait(false));

        public async Task<string> ConnectToPeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ConnectAsync(address, cancellationToken).ConfigureAwait(false)).Address;

        public async Task<string> CreateFeedAsync(
            string owner,
            string topic,
            string swarmPostageBatchId,
            string? type = null,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.FeedsPostAsync(owner, topic, type, swarmPin, swarmPostageBatchId, cancellationToken).ConfigureAwait(false)).Reference;

        public async Task<MessageResponse> CreatePinAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.PinsPostAsync(reference, cancellationToken).ConfigureAwait(false));

        public async Task<TagInfo> CreateTagAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.TagsPostAsync(
                new Body3
                {
                    Address = address
                },
                cancellationToken).ConfigureAwait(false));

        public async Task<MessageResponse> DeletePeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.PeersDeleteAsync(address, cancellationToken).ConfigureAwait(false));

        public async Task<MessageResponse> DeletePinAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.PinsDeleteAsync(reference, cancellationToken).ConfigureAwait(false));

        public Task DeleteTagAsync(
            long uid,
            CancellationToken cancellationToken = default) =>
            generatedClient.TagsDeleteAsync(uid, cancellationToken);

        public async Task<IEnumerable<string>> GetAllPinsAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.PinsGetAsync(cancellationToken).ConfigureAwait(false)).Reference;

        public async Task<string> DeleteTransactionAsync(
            string txHash,
            long? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.TransactionsDeleteAsync(txHash, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<string> DepositIntoChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookDepositAsync(amount, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<string> DilutePostageBatchAsync(
            string id,
            int depth,
            long? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.StampsDiluteAsync(id, depth, gasPrice, gasLimit, cancellationToken).ConfigureAwait(false)).BatchID;

        public async Task<AddressDetail> GetAddressesAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.AddressesAsync(cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<PeerBalance>> GetAllBalancesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.BalancesGetAsync(cancellationToken).ConfigureAwait(false)).Balances.Select(i => new PeerBalance(i));

        public async Task<IEnumerable<ChequebookChequeGet>> GetAllChequeBookChequesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookChequeGetAsync(cancellationToken).ConfigureAwait(false)).Lastcheques.Select(i => new ChequebookChequeGet(i));

        public async Task<IEnumerable<PeerBalance>> GetAllConsumedBalancesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.ConsumedGetAsync(cancellationToken).ConfigureAwait(false)).Balances.Select(i => new PeerBalance(i));

        public async Task<IEnumerable<string>> GetAllPeerAddressesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.PeersGetAsync(cancellationToken).ConfigureAwait(false)).Peers.Select(i => i.Address);

        public async Task<Settlement> GetAllSettlementsAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.SettlementsGetAsync(cancellationToken).ConfigureAwait(false));

        public async Task<Settlement> GetAllTimeSettlementsAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.TimesettlementsAsync(cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<PostageBatchShort>> GetAllValidPostageBatchesFromAllNodesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.BatchesAsync(cancellationToken).ConfigureAwait(false)).Batches.Select(i => new PostageBatchShort(i));

        public async Task<PeerBalance> GetBalanceWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.BalancesGetAsync(address, cancellationToken).ConfigureAwait(false));

        public async Task<Stream> GetBytesAsync(
            string reference,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.BytesGetAsync(
                reference,
                swarmCache,
                (SwarmRedundancyStrategy?)swarmRedundancyStrategy,
                swarmRedundancyFallbackMode,
                swarmChunkRetrievalTimeout,
                cancellationToken).ConfigureAwait(false)).Stream;

        public Task GetBytesHeadAsync(string address, CancellationToken cancellationToken = default) =>
            generatedClient.BytesHeadAsync(address, cancellationToken);

        public async Task<IEnumerable<string>> GetBlocklistedPeerAddressesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.BlocklistAsync(cancellationToken).ConfigureAwait(false)).Select(i => i.Address.Address1);

        public async Task<ChainState> GetChainStateAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.ChainstateAsync(cancellationToken).ConfigureAwait(false));

        public async Task<string> GetChequeBookAddressAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookAddressAsync(cancellationToken).ConfigureAwait(false)).ChequebookAddress;

        public async Task<ChequebookBalance> GetChequeBookBalanceAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.ChequebookBalanceAsync(cancellationToken).ConfigureAwait(false));

        public async Task<ChequebookCashoutGet> GetChequeBookCashoutForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.ChequebookCashoutGetAsync(peerId, cancellationToken).ConfigureAwait(false));

        public async Task<ChequebookChequeGet> GetChequeBookChequeForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.ChequebookChequeGetAsync(peerId, cancellationToken).ConfigureAwait(false));

        public async Task<Stream> GetChunkAsync(
            string reference,
            bool? swarmCache = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChunksGetAsync(reference, swarmCache,  cancellationToken).ConfigureAwait(false)).Stream;

        public async Task<PeerBalance> GetConsumedBalanceWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.ConsumedGetAsync(address, cancellationToken).ConfigureAwait(false));

        public async Task<string> GetFeedAsync(
            string owner,
            string topic,
            int? at = null,
            int? after = null,
            string? type = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.FeedsGetAsync(owner, topic, at, after, type, cancellationToken).ConfigureAwait(false)).Reference;

        public async Task<Models.FileResponse> GetFileAsync(
            string reference,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.BzzGetAsync(
                reference,
                swarmCache,
                (SwarmRedundancyStrategy2?)swarmRedundancyStrategy,
                swarmRedundancyFallbackMode,
                swarmChunkRetrievalTimeout,
                cancellationToken).ConfigureAwait(false));

        public Task GetFileHeadAsync(string address, CancellationToken cancellationToken = default) =>
            generatedClient.BzzHeadAsync(address, cancellationToken);

        public async Task<Models.FileResponse> GetFileWithPathAsync(
            string reference,
            string path,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.BzzGetAsync(
                reference,
                path,
                (SwarmRedundancyStrategy3?)swarmRedundancyStrategy,
                swarmRedundancyFallbackMode,
                swarmChunkRetrievalTimeout,
                cancellationToken).ConfigureAwait(false));

        public async Task<Health> GetHealthAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.HealthAsync(cancellationToken).ConfigureAwait(false));

        public async Task<NodeInfo> GetNodeInfoAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.NodeAsync(cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<PostageBatch>> GetOwnedPostageBatchesByNodeAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.StampsGetAsync(cancellationToken).ConfigureAwait(false)).Stamps.Select(i => new PostageBatch(i));

        public async Task<IEnumerable<PendingTransaction>> GetPendingTransactionsAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.TransactionsGetAsync(cancellationToken).ConfigureAwait(false)).PendingTransactions.Select(i => new PendingTransaction(i));

        public async Task<string> GetPinStatusAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.PinsGetAsync(reference, cancellationToken).ConfigureAwait(false)).Reference;

        public async Task<PostageBatch> GetPostageBatchAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.StampsGetAsync(id, cancellationToken).ConfigureAwait(false));
        
        public async Task<bool> GetReadinessAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await generatedClient.ReadinessAsync(cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (BeeNetApiException e) when (e.StatusCode == 400)
            {
                return false;
            }
        }

        public async Task<ReserveCommitment> GetReserveCommitmentAsync(int depth, string anchor1, string anchor2,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.RchashAsync(depth, anchor1, anchor2, cancellationToken).ConfigureAwait(false));

        public async Task<ReserveState> GetReserveStateAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.ReservestateAsync(cancellationToken).ConfigureAwait(false));

        public async Task<SettlementData> GetSettlementsWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.SettlementsGetAsync(address, cancellationToken).ConfigureAwait(false));

        public async Task<StampsBuckets> GetStampsBucketsForBatchAsync(
            string batchId,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.StampsBucketsAsync(batchId, cancellationToken).ConfigureAwait(false));

        public async Task<Topology> GetSwarmTopologyAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.TopologyAsync(cancellationToken).ConfigureAwait(false));

        public async Task<TagInfo> GetTagInfoAsync(
            long uid,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.TagsGetAsync(uid, cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<TagInfo>> GetTagsListAsync(
            int? offset = null,
            int? limit = null,
            CancellationToken cancellationToken = default) =>
            ((await generatedClient.TagsGetAsync(offset, limit, cancellationToken).ConfigureAwait(false)).Tags ?? Array.Empty<Tags>()).Select(i => new TagInfo(i));

        public async Task<TransactionInfo> GetTransactionInfoAsync(
            string txHash,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.TransactionsGetAsync(txHash, cancellationToken).ConfigureAwait(false));

        public async Task<Wallet> GetWalletBalance(CancellationToken cancellationToken = default) =>
            new(await generatedClient.WalletAsync(cancellationToken).ConfigureAwait(false));

        public async Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.WelcomeMessageGetAsync(cancellationToken).ConfigureAwait(false)).WelcomeMessage;

        public async Task<LogData> LoggersGetAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.LoggersGetAsync(cancellationToken).ConfigureAwait(false));

        public async Task<LogData> LoggersGetAsync(string exp, CancellationToken cancellationToken = default) =>
            new(await generatedClient.LoggersGetAsync(exp, cancellationToken).ConfigureAwait(false));

        public async Task LoggersPutAsync(string exp, CancellationToken cancellationToken = default) =>
            await generatedClient.LoggersPutAsync(exp, cancellationToken).ConfigureAwait(false);

        public async Task<string> RebroadcastTransactionAsync(
            string txHash,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.TransactionsPostAsync(txHash, cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<RedistributionState> RedistributionStateAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.RedistributionstateAsync(cancellationToken).ConfigureAwait(false));

        public async Task<string> RefreshAuthAsync(
            string role,
            int expiry,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.RefreshAsync(
                new Body2
                {
                    Role = role,
                    Expiry = expiry
                },
                cancellationToken).ConfigureAwait(false)).Key;

        public Task ReuploadContentAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            generatedClient.StewardshipPutAsync(reference, cancellationToken: cancellationToken);

        public Task SendPssAsync(
            string topic,
            string targets,
            string swarmPostageBatchId,
            string? recipient = null,
            CancellationToken cancellationToken = default) =>
            generatedClient.PssSendAsync(topic, targets, swarmPostageBatchId, recipient, cancellationToken);
        
        public Task SetWelcomeMessageAsync(
            string welcomeMessage,
            CancellationToken cancellationToken = default) =>
            generatedClient.WelcomeMessagePostAsync(
                new Body5
                {
                    WelcomeMessage = welcomeMessage
                },
                cancellationToken);

        public async Task StakeDeleteAsync(long? gasPrice = null, long? gasLimit = null, CancellationToken cancellationToken = default) =>
            await generatedClient.StakeDeleteAsync(gasPrice, gasLimit, cancellationToken).ConfigureAwait(false);

        public async Task StakeGetAsync(CancellationToken cancellationToken = default) =>
            await generatedClient.StakeGetAsync(cancellationToken).ConfigureAwait(false);

        public async Task StakePostAsync(string? amount = null, long? gasPrice = null, long? gasLimit = null, CancellationToken cancellationToken = default) =>
            await generatedClient.StakePostAsync(amount, gasPrice, gasLimit, cancellationToken).ConfigureAwait(false);

        public async Task<StatusNode> StatusNodeAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.StatusAsync(cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<StatusNode>> StatusPeersAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.StatusPeersAsync(cancellationToken).ConfigureAwait(false)).Stamps.Select(p => new StatusNode(p));

        public Task SubscribeToPssAsync(
            string topic,
            CancellationToken cancellationToken = default) =>
            generatedClient.PssSubscribeAsync(topic, cancellationToken);

        public async Task<string> TopUpPostageBatchAsync(
            string id,
            long amount,
            long? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.StampsTopupAsync(id, amount, gasPrice, gasLimit, cancellationToken).ConfigureAwait(false)).BatchID;

        public async Task<string> TryConnectToPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.PingpongAsync(peerId, cancellationToken).ConfigureAwait(false)).Rtt;

        public Task UpdateTagAsync(
            long uid,
            string? address = null,
            CancellationToken cancellationToken = default) =>
            generatedClient.TagsPatchAsync(
                uid,
                address is null ?
                    null :
                    new Body4 { Address = address },
                cancellationToken);

        public async Task<string> UploadChunkAsync(
            string swarmPostageBatchId,
            long? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmDeferredUpload = null,
            Stream? body = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChunksPostAsync(
                swarmTag,
                swarmPostageBatchId,
                body,
                cancellationToken).ConfigureAwait(false)).Reference;

        public Task UploadChunksStreamAsync(
            string swarmPostageBatchId,
            int? swarmTag = null,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default) =>
            generatedClient.ChunksStreamAsync(swarmTag, swarmPostageBatchId, cancellationToken);

        public async Task<string> UploadBytesAsync(
            string swarmPostageBatchId,
            Stream body,
            int? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            bool? swarmDeferredUpload = null,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None0,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.BytesPostAsync(
                swarm_postage_batch_id: swarmPostageBatchId,
                swarm_tag: swarmTag,
                swarm_pin: swarmPin,
                swarm_deferred_upload: swarmDeferredUpload,
                swarm_encrypt: swarmEncrypt,
                swarm_redundancy_level: (int)swarmRedundancyLevel,
                body: body,
                cancellationToken).ConfigureAwait(false)).Reference;

        public async Task<string> UploadFileAsync(string swarmPostageBatchId,
            Stream content,
            string? name = null,
            string? contentType = null,
            int? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            bool? swarmCollection = null,
            string? swarmIndexDocument = null,
            string? swarmErrorDocument = null,
            bool? swarmDeferredUpload = null,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None0,
            CancellationToken cancellationToken = default)
        {
            return (await generatedClient.BzzPostAsync(
                body: content,
                name: name,
                content_Type: contentType,
                swarm_tag: swarmTag,
                swarm_pin: swarmPin,
                swarm_encrypt: swarmEncrypt,
                swarm_collection: swarmCollection,
                swarm_index_document: swarmIndexDocument,
                swarm_error_document: swarmErrorDocument,
                swarm_postage_batch_id: swarmPostageBatchId,
                swarm_deferred_upload: swarmDeferredUpload,
                swarm_redundancy_level: (SwarmRedundancyLevel)swarmRedundancyLevel,
                cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
        }

        public async Task<string> UploadSocAsync(
            string owner,
            string id,
            string sig,
            string swarmPostageBatchId,
            Stream body,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.SocAsync(owner, id, sig, swarmPostageBatchId, body, swarmPin, cancellationToken).ConfigureAwait(false)).Reference;

        public async Task<string> WalletWithdrawAsync(
            long amount,
            string address,
            string coin,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.WalletWithdrawAsync(amount.ToString(CultureInfo.InvariantCulture), address, coin, cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<string> WithdrawFromChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookWithdrawAsync(amount, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash;

        // Helpers.
        private static string BuildBaseUrl(string url, int port)
        {
            var normalizedUrl = url;
            if (normalizedUrl.Last() != '/')
                normalizedUrl += '/';

            var baseUrl = "";

            var urlRegex = new Regex(@"^((?<proto>\w+)://)?(?<host>[^/:]+)",
                RegexOptions.None, TimeSpan.FromMilliseconds(150));
            var urlMatch = urlRegex.Match(normalizedUrl);

            if (!urlMatch.Success)
                throw new ArgumentException("Url is not valid", nameof(url));

            if (!string.IsNullOrEmpty(urlMatch.Groups["proto"].Value))
                baseUrl += urlMatch.Groups["proto"].Value + "://";

            baseUrl += $"{urlMatch.Groups["host"].Value}:{port}";

            return baseUrl;
        }
    }
}