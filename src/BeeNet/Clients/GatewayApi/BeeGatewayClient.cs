﻿//   Copyright 2021-present Etherna SA
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

using Etherna.BeeNet.Exceptions;
using Etherna.BeeNet.Models;
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
        private readonly BeeGatewayGeneratedClient generatedClient;

        // Constructors.
        public BeeGatewayClient(Uri baseUrl, HttpClient httpClient)
        {
            ArgumentNullException.ThrowIfNull(baseUrl, nameof(baseUrl));

            generatedClient = new BeeGatewayGeneratedClient(httpClient) { BaseUrl = baseUrl.ToString() };
        }

        // Methods.
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
            catch (BeeNetGatewayApiException)
            {
                return false;
            }
        }

        public async Task<StewardshipGet> CheckIsContentAvailableAsync(
            SwarmAddress reference,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.StewardshipGetAsync((string)reference, cancellationToken).ConfigureAwait(false));

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
            SwarmAddress reference,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.PinsPostAsync((string)reference, cancellationToken).ConfigureAwait(false));

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
            SwarmAddress reference,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.PinsDeleteAsync((string)reference, cancellationToken).ConfigureAwait(false));

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
            SwarmAddress reference,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.BytesGetAsync(
                (string)reference,
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
            SwarmAddress reference,
            bool? swarmCache = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChunksGetAsync((string)reference, swarmCache,  cancellationToken).ConfigureAwait(false)).Stream;

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
            SwarmAddress reference,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.BzzGetAsync(
                (string)reference,
                swarmCache,
                (SwarmRedundancyStrategy2?)swarmRedundancyStrategy,
                swarmRedundancyFallbackMode,
                swarmChunkRetrievalTimeout,
                cancellationToken).ConfigureAwait(false));

        public Task GetFileHeadAsync(SwarmAddress address, CancellationToken cancellationToken = default) =>
            generatedClient.BzzHeadAsync((string)address, cancellationToken);

        public async Task<Models.FileResponse> GetFileWithPathAsync(
            SwarmAddress reference,
            string path,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.BzzGetAsync(
                (string)reference,
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
            SwarmAddress reference,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.PinsGetAsync((string)reference, cancellationToken).ConfigureAwait(false)).Reference;

        public async Task<PostageBatch> GetPostageBatchAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.StampsGetAsync(id, cancellationToken).ConfigureAwait(false));

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

        public async Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.WelcomeMessageGetAsync(cancellationToken).ConfigureAwait(false)).WelcomeMessage;

        public async Task<string> RebroadcastTransactionAsync(
            string txHash,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.TransactionsPostAsync(txHash, cancellationToken).ConfigureAwait(false)).TransactionHash;

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
            SwarmAddress reference,
            CancellationToken cancellationToken = default) =>
            generatedClient.StewardshipPutAsync((string)reference, cancellationToken: cancellationToken);

        public Task SendPssAsync(
            string topic,
            string targets,
            string swarmPostageBatchId,
            string? recipient = null,
            CancellationToken cancellationToken = default) =>
            generatedClient.PssSendAsync(topic, targets, swarmPostageBatchId, recipient, cancellationToken);

        public void SetAuthToken(
            string token) =>
            generatedClient.SetAuthToken(token);
        
        public Task SetWelcomeMessageAsync(
            string welcomeMessage,
            CancellationToken cancellationToken = default) =>
            generatedClient.WelcomeMessagePostAsync(
                new Body5
                {
                    WelcomeMessage = welcomeMessage
                },
                cancellationToken);

        public Task SubscribeToPssAsync(
            string topic,
            CancellationToken cancellationToken = default) =>
            generatedClient.PssSubscribeAsync(topic, cancellationToken);

        public async Task<string> TopUpPostageBatchAsync(
            string id,
            long amount,
            long? gasPrice,
            long? gasLimit,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.StampsTopupAsync(id, amount, gasPrice, gasLimit, cancellationToken).ConfigureAwait(false)).BatchID;

        public async Task<string> TryConnectToPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.PingpongAsync(peerId, cancellationToken).ConfigureAwait(false)).Rtt;

        public Task UpdateTagAsync(
            long uid,
            SwarmAddress? address = null,
            CancellationToken cancellationToken = default) =>
            generatedClient.TagsPatchAsync(
                uid,
                address is null ?
                    null :
                    new Body4 { Address = (string)address },
                cancellationToken);

        public async Task<SwarmAddress> UploadChunkAsync(
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

        public async Task<SwarmAddress> UploadBytesAsync(
            string swarmPostageBatchId,
            Stream body,
            int? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            bool? swarmDeferredUpload = null,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
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

        public async Task<SwarmAddress> UploadFileAsync(string swarmPostageBatchId,
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
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
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

        public async Task<SwarmAddress> UploadSocAsync(
            string owner,
            string id,
            string sig,
            string swarmPostageBatchId,
            Stream body,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.SocAsync(
                owner,
                id,
                sig,
                swarmPostageBatchId,
                body,
                swarmPin,
                cancellationToken).ConfigureAwait(false)).Reference;

        public async Task<string> WithdrawFromChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookWithdrawAsync(amount, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash;
    }
}