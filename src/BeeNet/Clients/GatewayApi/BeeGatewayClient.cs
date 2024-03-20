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

using Etherna.BeeNet.DtoModels;
using Etherna.BeeNet.Exceptions;
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
        private readonly BeeGatewayGeneratedClient generatedClient;

        // Constructors.
        public BeeGatewayClient(HttpClient httpClient, Uri baseUrl)
        {
            ArgumentNullException.ThrowIfNull(baseUrl, nameof(baseUrl));

            generatedClient = new BeeGatewayGeneratedClient(httpClient) { BaseUrl = baseUrl.ToString() };
        }

        // Methods.
        public async Task<AuthDto> AuthenticateAsync(string role, int expiry) =>
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

        public async Task<StewardShipGetDto> CheckIsContentAvailableAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.StewardshipGetAsync(reference, cancellationToken).ConfigureAwait(false));

        public async Task<CheckPinsResultDto> CheckPinsAsync(
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

        public async Task<MessageResponseDto> CreatePinAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.PinsPostAsync(reference, cancellationToken).ConfigureAwait(false));

        public async Task<TagInfoDto> CreateTagAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.TagsPostAsync(
                new Body3
                {
                    Address = address
                },
                cancellationToken).ConfigureAwait(false));

        public async Task<MessageResponseDto> DeletePeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.PeersDeleteAsync(address, cancellationToken).ConfigureAwait(false));

        public async Task<MessageResponseDto> DeletePinAsync(
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

        public async Task<AddressDetailDto> GetAddressesAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.AddressesAsync(cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<BalanceDto>> GetAllBalancesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.BalancesGetAsync(cancellationToken).ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i));

        public async Task<IEnumerable<ChequeBookChequeGetDto>> GetAllChequeBookChequesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookChequeGetAsync(cancellationToken).ConfigureAwait(false)).Lastcheques.Select(i => new ChequeBookChequeGetDto(i));

        public async Task<IEnumerable<BalanceDto>> GetAllConsumedBalancesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.ConsumedGetAsync(cancellationToken).ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i));

        public async Task<IEnumerable<string>> GetAllPeerAddressesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.PeersGetAsync(cancellationToken).ConfigureAwait(false)).Peers.Select(i => i.Address);

        public async Task<SettlementDto> GetAllSettlementsAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.SettlementsGetAsync(cancellationToken).ConfigureAwait(false));

        public async Task<TimeSettlementsDto> GetAllTimeSettlementsAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.TimesettlementsAsync(cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<PostageBatchShortDto>> GetAllValidPostageBatchesFromAllNodesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.BatchesAsync(cancellationToken).ConfigureAwait(false)).Batches.Select(i => new PostageBatchShortDto(i));

        public async Task<BalanceDto> GetBalanceWithPeerAsync(
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

        public async Task<ChainStateDto> GetChainStateAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.ChainstateAsync(cancellationToken).ConfigureAwait(false));

        public async Task<string> GetChequeBookAddressAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookAddressAsync(cancellationToken).ConfigureAwait(false)).ChequebookAddress;

        public async Task<ChequeBookBalanceDto> GetChequeBookBalanceAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.ChequebookBalanceAsync(cancellationToken).ConfigureAwait(false));

        public async Task<ChequeBookCashoutGetDto> GetChequeBookCashoutForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.ChequebookCashoutGetAsync(peerId, cancellationToken).ConfigureAwait(false));

        public async Task<ChequeBookChequeGetDto> GetChequeBookChequeForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.ChequebookChequeGetAsync(peerId, cancellationToken).ConfigureAwait(false));

        public async Task<Stream> GetChunkAsync(
            string reference,
            bool? swarmCache = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChunksGetAsync(reference, swarmCache,  cancellationToken).ConfigureAwait(false)).Stream;

        public async Task<BalanceDto> GetConsumedBalanceWithPeerAsync(
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

        public async Task<FileResponseDto> GetFileAsync(
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

        public async Task<FileResponseDto> GetFileWithPathAsync(
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

        public async Task<VersionDto> GetHealthAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.HealthAsync(cancellationToken).ConfigureAwait(false));

        public async Task<NodeInfoDto> GetNodeInfoAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.NodeAsync(cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<PostageBatchDto>> GetOwnedPostageBatchesByNodeAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.StampsGetAsync(cancellationToken).ConfigureAwait(false)).Stamps.Select(i => new PostageBatchDto(i));

        public async Task<IEnumerable<PendingTransactionDto>> GetPendingTransactionsAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.TransactionsGetAsync(cancellationToken).ConfigureAwait(false)).PendingTransactions.Select(i => new PendingTransactionDto(i));

        public async Task<string> GetPinStatusAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.PinsGetAsync(reference, cancellationToken).ConfigureAwait(false)).Reference;

        public async Task<PostageBatchDto> GetPostageBatchAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.StampsGetAsync(id, cancellationToken).ConfigureAwait(false));

        public async Task<ReserveCommitmentDto> GetReserveCommitmentAsync(int depth, string anchor1, string anchor2,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.RchashAsync(depth, anchor1, anchor2, cancellationToken).ConfigureAwait(false));

        public async Task<ReserveStateDto> GetReserveStateAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.ReservestateAsync(cancellationToken).ConfigureAwait(false));

        public async Task<SettlementDataDto> GetSettlementsWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.SettlementsGetAsync(address, cancellationToken).ConfigureAwait(false));

        public async Task<StampsBucketsDto> GetStampsBucketsForBatchAsync(
            string batchId,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.StampsBucketsAsync(batchId, cancellationToken).ConfigureAwait(false));

        public async Task<TopologyDto> GetSwarmTopologyAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.TopologyAsync(cancellationToken).ConfigureAwait(false));

        public async Task<TagInfoDto> GetTagInfoAsync(
            long uid,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.TagsGetAsync(uid, cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<TagInfoDto>> GetTagsListAsync(
            int? offset = null,
            int? limit = null,
            CancellationToken cancellationToken = default) =>
            ((await generatedClient.TagsGetAsync(offset, limit, cancellationToken).ConfigureAwait(false)).Tags ?? Array.Empty<Tags>()).Select(i => new TagInfoDto(i));

        public async Task<TransactionsDto> GetTransactionInfoAsync(
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

        public void SetAuthToken(
            string token) =>
            generatedClient.SetAuthToken(token);
        
        public async Task<VersionDto> SetWelcomeMessageAsync(
            string welcomeMessage,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.WelcomeMessagePostAsync(
                new Body5
                {
                    WelcomeMessage = welcomeMessage
                },
                cancellationToken).ConfigureAwait(false));

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

        public async Task<VersionDto> UpdateTagAsync(
            long uid,
            string? address = null,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.TagsPatchAsync(
                uid,
                address is null ?
                    null :
                    new Body4 { Address = address },
                cancellationToken).ConfigureAwait(false));

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

        public async Task<string> WithdrawFromChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookWithdrawAsync(amount, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash;
    }
}