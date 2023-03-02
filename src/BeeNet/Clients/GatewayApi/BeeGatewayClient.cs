//   Copyright 2021-present Etherna Sagl
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
        private readonly V4_0_0.IBeeGatewayClient_4_0_0 beeGatewayApiClient_4_0_0;

        // Constructors.
        public BeeGatewayClient(HttpClient httpClient, Uri baseUrl, GatewayApiVersion apiVersion)
        {
            if (baseUrl is null)
                throw new ArgumentNullException(nameof(baseUrl));

            beeGatewayApiClient_4_0_0 = new V4_0_0.BeeGatewayClient_4_0_0(httpClient) { BaseUrl = baseUrl.ToString() };
            CurrentApiVersion = apiVersion;
        }

        // Properties.
        public GatewayApiVersion CurrentApiVersion { get; set; }

        // Methods.
        public async Task<AuthDto> AuthenticateAsync(string role, int expiry)
        {
            return CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new AuthDto(await beeGatewayApiClient_4_0_0.AuthAsync(
                    new V4_0_0.Body
                    {
                        Role = role,
                        Expiry = expiry
                    }).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };
        }
        public async Task<StewardShipGetDto> CheckIsContentAvailableAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new StewardShipGetDto(await beeGatewayApiClient_4_0_0.StewardshipGetAsync(reference, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> CreateFeedAsync(
            string owner,
            string topic,
            string swarmPostageBatchId,
            string? type = null,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.FeedsPostAsync(owner, topic, type, swarmPin, swarmPostageBatchId, cancellationToken).ConfigureAwait(false)).Reference,
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> CreatePinAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new MessageResponseDto(await beeGatewayApiClient_4_0_0.PinsPostAsync(reference, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TagInfoDto> CreateTagAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new TagInfoDto(await beeGatewayApiClient_4_0_0.TagsPostAsync(
                    new V4_0_0.Body3
                    {
                        Address = address
                    }, 
                    cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeletePinAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new MessageResponseDto(await beeGatewayApiClient_4_0_0.PinsDeleteAsync(reference, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public Task DeleteTagAsync(
            long uid, 
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => beeGatewayApiClient_4_0_0.TagsDeleteAsync(uid, cancellationToken),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetAllPinsAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.PinsGetAsync(cancellationToken).ConfigureAwait(false)).References,
                _ => throw new InvalidOperationException()
            };
        public async Task<Stream> GetChunkStreamAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.ChunksGetAsync(reference, cancellationToken).ConfigureAwait(false)).Stream,
                _ => throw new InvalidOperationException()
            };

        public async Task<Stream> GetDataAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.BytesGetAsync(reference, cancellationToken).ConfigureAwait(false)).Stream,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetFeedAsync(
            string owner,
            string topic,
            int? at = null,
            string? type = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.FeedsGetAsync(owner, topic, at, type, cancellationToken).ConfigureAwait(false)).Reference,
                _ => throw new InvalidOperationException()
            };

        public async Task<Stream> GetFileWithPathAsync(
            string reference, 
            string path,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.BzzGetAsync(reference, path, cancellationToken).ConfigureAwait(false)).Stream,
                _ => throw new InvalidOperationException()
            };

        public async Task<Stream> GetFileAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.BzzGetAsync(reference, cancellationToken).ConfigureAwait(false)).Stream,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetPinStatusAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.PinsGetAsync(reference, cancellationToken).ConfigureAwait(false)).Reference,
                _ => throw new InvalidOperationException()
            };

        public async Task<TagInfoDto> GetTagInfoAsync(
            long uid,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new TagInfoDto(await beeGatewayApiClient_4_0_0.TagsGetAsync(uid, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<TagInfoDto>> GetTagsListAsync(
            int? offset = null, 
            int? limit = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.TagsGetAsync(offset, limit, cancellationToken).ConfigureAwait(false)).Tags.Select(i => new TagInfoDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> RefreshAuthAsync(
            string role, 
            int expiry,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.RefreshAsync(
                    new V4_0_0.Body2
                    {
                        Role = role,
                        Expiry = expiry
                    }, 
                    cancellationToken).ConfigureAwait(false)).Key,
                _ => throw new InvalidOperationException()
            };

        public Task ReuploadContentAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => beeGatewayApiClient_4_0_0.StewardshipPutAsync(reference, cancellationToken),
                _ => throw new InvalidOperationException()
            };

        public Task SendPssAsync(
            string topic,
            string targets,
            string swarmPostageBatchId,
            string? recipient = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => beeGatewayApiClient_4_0_0.PssSendAsync(topic, targets, swarmPostageBatchId, recipient, cancellationToken),
                _ => throw new InvalidOperationException()
            };

        public Task SubscribeToPssAsync(
            string topic,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => beeGatewayApiClient_4_0_0.PssSubscribeAsync(topic, cancellationToken),
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> UpdateTagAsync(
            long uid, 
            string? address = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new VersionDto(await beeGatewayApiClient_4_0_0.TagsPatchAsync(
                    uid,
                    address is null ?
                        null :
                        new V4_0_0.Body4 { Address = address }, 
                    cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> UploadChunkAsync(
            string swarmPostageBatchId,
            long? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmDeferredUpload = null,
            Stream? body = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.ChunksPostAsync(
                    swarmTag,
                    swarmPin,
                    swarmPostageBatchId,
                    swarmDeferredUpload,
                    body,
                    cancellationToken).ConfigureAwait(false)).Reference,
                _ => throw new InvalidOperationException()
            };

        public Task UploadChunksStreamAsync(
            string swarmPostageBatchId,
            int? swarmTag = null,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => beeGatewayApiClient_4_0_0.ChunksStreamAsync(swarmTag, swarmPin, swarmPostageBatchId, cancellationToken),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> UploadDataAsync(
            string swarmPostageBatchId,
            Stream? body,
            int? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            bool? swarmDeferredUpload = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.BytesPostAsync(
                    swarmPostageBatchId,
                    swarmTag,
                    swarmPin,
                    swarmEncrypt,
                    swarmDeferredUpload,
                    body,
                    cancellationToken).ConfigureAwait(false)).Reference,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> UploadFileAsync(
            string swarmPostageBatchId,
            IEnumerable<FileParameterInput> files,
            string? name,
            int? swarmTag,
            bool? swarmPin,
            bool? swarmEncrypt,
            string? contentType,
            bool? swarmCollection,
            string? swarmIndexDocument,
            string? swarmErrorDocument,
            bool? swarmDeferredUpload,
            CancellationToken cancellationToken = default)
        {
            if (files == null)
            {
                throw new ArgumentNullException(nameof(files));
            }
            if (files.Count() != 1)
            {
                throw new ArgumentOutOfRangeException(nameof(files));
            }

            return CurrentApiVersion switch
            {
                
            GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.BzzPostAsync(
                name,
                swarmTag,
                swarmPin,
                swarmEncrypt,
                contentType,
                swarmCollection,
                swarmIndexDocument,
                swarmErrorDocument,
                swarmPostageBatchId,
                swarmDeferredUpload,
                files.Select(f => new V4_0_0.FileParameter(f.Data, f.FileName, f.ContentType)), 
                cancellationToken).ConfigureAwait(false)).Reference,
                _ => throw new InvalidOperationException()
            };
    }

        public async Task<string> UploadSocAsync(
            string owner,
            string id,
            string sig,
            string swarmPostageBatchId,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.SocAsync(owner, id, sig, swarmPostageBatchId, swarmPin, cancellationToken).ConfigureAwait(false)).Reference,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> BuyPostageBatchAsync(
            long amount,
            int depth,
            string? label = null,
            bool? immutable = null,
            long? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.StampsPostAsync(amount.ToString(CultureInfo.InvariantCulture), depth, label, immutable, gasPrice, gasLimit, cancellationToken).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> CashoutChequeForPeerAsync(
            string peerId,
            long? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.ChequebookCashoutPostAsync(peerId, gasPrice, gasLimit, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> ConnectToPeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.ConnectAsync(address, cancellationToken).ConfigureAwait(false)).Address,
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeleteChunkAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new MessageResponseDto(await beeGatewayApiClient_4_0_0.ChunksDeleteAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeletePeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new MessageResponseDto(await beeGatewayApiClient_4_0_0.PeersDeleteAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DeleteTransactionAsync(
            string txHash,
            long? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.TransactionsDeleteAsync(txHash, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DepositIntoChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.ChequebookDepositAsync(amount, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DilutePostageBatchAsync(
            string id,
            int depth,
            long? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.StampsDiluteAsync(id, depth, gasPrice, gasLimit, cancellationToken).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<AddressDetailDto> GetAddressesAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new AddressDetailDto(await beeGatewayApiClient_4_0_0.AddressesAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetAllBalancesAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.BalancesGetAsync(cancellationToken).ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<ChequeBookChequeGetDto>> GetAllChequeBookChequesAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.ChequebookChequeGetAsync(cancellationToken).ConfigureAwait(false)).Lastcheques.Select(i => new ChequeBookChequeGetDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetAllConsumedBalancesAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.ConsumedGetAsync(cancellationToken).ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetAllPeerAddressesAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.PeersGetAsync(cancellationToken).ConfigureAwait(false)).Peers.Select(i => i.Address),
                _ => throw new InvalidOperationException()
            };

        public async Task<SettlementDto> GetAllSettlementsAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new SettlementDto(await beeGatewayApiClient_4_0_0.SettlementsGetAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TimeSettlementsDto> GetAllTimeSettlementsAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new TimeSettlementsDto(await beeGatewayApiClient_4_0_0.TimesettlementsAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PostageBatchShortDto>> GetAllValidPostageBatchesFromAllNodesAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.BatchesAsync(cancellationToken).ConfigureAwait(false)).Batches.Select(i => new PostageBatchShortDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<BalanceDto> GetBalanceWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new BalanceDto(await beeGatewayApiClient_4_0_0.BalancesGetAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetBlocklistedPeerAddressesAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.BlocklistAsync(cancellationToken).ConfigureAwait(false)).Peers.Select(i => i.Address),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChainStateDto> GetChainStateAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new ChainStateDto(await beeGatewayApiClient_4_0_0.ChainstateAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetChequeBookAddressAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.ChequebookAddressAsync(cancellationToken).ConfigureAwait(false)).ChequebookAddress,
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookBalanceDto> GetChequeBookBalanceAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new ChequeBookBalanceDto(await beeGatewayApiClient_4_0_0.ChequebookBalanceAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookCashoutGetDto> GetChequeBookCashoutForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new ChequeBookCashoutGetDto(await beeGatewayApiClient_4_0_0.ChequebookCashoutGetAsync(peerId, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookChequeGetDto> GetChequeBookChequeForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new ChequeBookChequeGetDto(await beeGatewayApiClient_4_0_0.ChequebookChequeGetAsync(peerId, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<FileResponseDto> GetChunkAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new FileResponseDto(await beeGatewayApiClient_4_0_0.ChunksGetAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> ChunksHeadAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new MessageResponseDto(await beeGatewayApiClient_4_0_0.ChunksHeadAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };
        
        public async Task<BalanceDto> GetConsumedBalanceWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new BalanceDto(await beeGatewayApiClient_4_0_0.ConsumedGetAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> GetHealthAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new VersionDto(await beeGatewayApiClient_4_0_0.HealthAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<NodeInfoDto> GetNodeInfoAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new NodeInfoDto(await beeGatewayApiClient_4_0_0.NodeAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PostageBatchDto>> GetOwnedPostageBatchesByNodeAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.StampsGetAllAsync(null, cancellationToken).ConfigureAwait(false)).Stamps.Select(i => new PostageBatchDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PendingTransactionDto>> GetPendingTransactionsAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.TransactionsGetAsync(cancellationToken).ConfigureAwait(false)).PendingTransactions.Select(i => new PendingTransactionDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<PostageBatchDto> GetPostageBatchAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new PostageBatchDto(await beeGatewayApiClient_4_0_0.StampsGetAsync(id, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ReserveStateDto> GetReserveStateAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new ReserveStateDto(await beeGatewayApiClient_4_0_0.ReservestateAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<SettlementDataDto> GetSettlementsWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new SettlementDataDto(await beeGatewayApiClient_4_0_0.SettlementsGetAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<StampsBucketsDto> GetStampsBucketsForBatchAsync(
            string batchId,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new StampsBucketsDto(await beeGatewayApiClient_4_0_0.StampsBucketsAsync(batchId, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TopologyDto> GetSwarmTopologyAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new TopologyDto(await beeGatewayApiClient_4_0_0.TopologyAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TransactionsDto> GetTransactionInfoAsync(
            string txHash,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new TransactionsDto(await beeGatewayApiClient_4_0_0.TransactionsGetAsync(txHash, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.WelcomeMessageGetAsync(cancellationToken).ConfigureAwait(false)).WelcomeMessage,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> RebroadcastTransactionAsync(
            string txHash,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.TransactionsPostAsync(txHash, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> SetWelcomeMessageAsync(
            string welcomeMessage,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => new VersionDto(await beeGatewayApiClient_4_0_0.WelcomeMessagePostAsync(
                    new V4_0_0.Body5
                    {
                        WelcomeMessage = welcomeMessage
                    },
                    cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> TopUpPostageBatchAsync(
            string id,
            long amount,
            long? gasPrice,
            long? gasLimit,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.StampsTopupAsync(id, amount, gasPrice, gasLimit, cancellationToken).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> TryConnectToPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.PingpongAsync(peerId, cancellationToken).ConfigureAwait(false)).Rtt,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> WithdrawFromChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                GatewayApiVersion.v4_0_0 => (await beeGatewayApiClient_4_0_0.ChequebookWithdrawAsync(amount, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public void SetAuthToken(
            string token)
        {
            if (CurrentApiVersion == GatewayApiVersion.v4_0_0)
            {
                beeGatewayApiClient_4_0_0.SetAuthToken(token);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
}
}