﻿// Copyright 2021-present Etherna SA
// This file is part of Bee.Net.
// 
// Bee.Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Bee.Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Bee.Net.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.BeeNet.Clients;
using Etherna.BeeNet.Exceptions;
using Etherna.BeeNet.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FileResponse = Etherna.BeeNet.Models.FileResponse;

#if NET7_0_OR_GREATER
using System.Formats.Tar;
#endif

namespace Etherna.BeeNet
{
    [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings")]
    public class BeeClient : IBeeClient, IDisposable
    {
        // Consts.
        public const int DefaultPort = 1633;
        public readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(10);

        // Fields.
        private readonly BeeGeneratedClient generatedClient;
        private readonly HttpClient httpClient;
        
        private bool disposed;

        // Constructors.
        public BeeClient(
            string baseUrl = "http://localhost/",
            int port = DefaultPort,
            HttpClient? httpClient = null)
            : this(BuildBaseUrl(baseUrl, port), httpClient)
        { }

        public BeeClient(
            Uri baseUrl,
            HttpClient? httpClient = null)
        {
            this.httpClient = httpClient ?? new HttpClient { Timeout = DefaultTimeout };

            BaseUrl = baseUrl;
            generatedClient = new BeeGeneratedClient(this.httpClient) { BaseUrl = BaseUrl.ToString() };
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
        public Uri BaseUrl { get; }
        
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
        
        public async Task<PostageBatchId> BuyPostageBatchAsync(
            BzzBalance amount,
            int depth,
            string? label = null,
            bool? immutable = null,
            XDaiBalance? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.StampsPostAsync(
                amount.ToPlurString(),
                depth,
                label,
                immutable,
                gasPrice?.ToWeiLong(),
                gasLimit,
                cancellationToken).ConfigureAwait(false)).BatchID;

        public async Task<string> CashoutChequeForPeerAsync(
            string peerId,
            XDaiBalance? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookCashoutPostAsync(
                peerId,
                gasPrice?.ToWeiLong(),
                gasLimit,
                cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<bool> CheckChunkExistsAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await generatedClient.ChunksHeadAsync(hash.ToString(), cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (BeeNetApiException)
            {
                return false;
            }
        }

        public async Task<StewardshipGet> CheckIsContentAvailableAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.StewardshipGetAsync((string)hash, cancellationToken).ConfigureAwait(false));

        public async Task<CheckPinsResult> CheckPinsAsync(
            SwarmHash? hash,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.PinsCheckAsync(hash?.ToString(), cancellationToken).ConfigureAwait(false));

        public async Task<string> ConnectToPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ConnectAsync(peerAddress, cancellationToken).ConfigureAwait(false)).Address;

        public async Task<SwarmHash> CreateFeedAsync(
            string owner,
            string topic,
            PostageBatchId batchId,
            string? type = null,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.FeedsPostAsync(owner, topic, type, swarmPin, batchId.ToString(), cancellationToken).ConfigureAwait(false)).Reference;

        public async Task<MessageResponse> CreatePinAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.PinsPostAsync((string)hash, cancellationToken).ConfigureAwait(false));

        public async Task<TagInfo> CreateTagAsync(
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.TagsPostAsync(cancellationToken).ConfigureAwait(false));

        public async Task<MessageResponse> DeletePeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.PeersDeleteAsync(peerAddress, cancellationToken).ConfigureAwait(false));

        public async Task<MessageResponse> DeletePinAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.PinsDeleteAsync((string)hash, cancellationToken).ConfigureAwait(false));

        public Task DeleteTagAsync(
            long uid,
            CancellationToken cancellationToken = default) =>
            generatedClient.TagsDeleteAsync(uid, cancellationToken);

        public async Task<string> DeleteTransactionAsync(
            string txHash,
            XDaiBalance? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.TransactionsDeleteAsync(
                txHash,
                gasPrice?.ToWeiLong(),
                cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<string> DepositIntoChequeBookAsync(
            BzzBalance amount,
            XDaiBalance? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookDepositAsync(
                amount.ToPlurLong(),
                gasPrice?.ToWeiLong(),
                cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<PostageBatchId> DilutePostageBatchAsync(
            PostageBatchId batchId,
            int depth,
            XDaiBalance? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.StampsDiluteAsync(
                batchId.ToString(),
                depth,
                gasPrice?.ToWeiLong(),
                gasLimit,
                cancellationToken).ConfigureAwait(false)).BatchID;

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

        public async Task<IEnumerable<SwarmHash>> GetAllPinsAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.PinsGetAsync(cancellationToken).ConfigureAwait(false)).Reference
            .Select(h => new SwarmHash(h));

        public async Task<Settlement> GetAllSettlementsAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.SettlementsGetAsync(cancellationToken).ConfigureAwait(false));

        public async Task<Settlement> GetAllTimeSettlementsAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.TimesettlementsAsync(cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<PostageBatchShort>> GetAllValidPostageBatchesFromAllNodesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.BatchesAsync(cancellationToken).ConfigureAwait(false)).Batches.Select(i => new PostageBatchShort(i));

        public async Task<PeerBalance> GetBalanceWithPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.BalancesGetAsync(peerAddress, cancellationToken).ConfigureAwait(false));

        public async Task<Stream> GetBytesAsync(
            SwarmHash hash,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.BytesGetAsync(
                (string)hash,
                swarmCache,
                (SwarmRedundancyStrategy?)swarmRedundancyStrategy,
                swarmRedundancyFallbackMode,
                swarmChunkRetrievalTimeout,
                cancellationToken).ConfigureAwait(false)).Stream;

        public Task GetBytesHeadAsync(SwarmHash hash, CancellationToken cancellationToken = default) =>
            generatedClient.BytesHeadAsync(hash.ToString(), cancellationToken);

        public async Task<IEnumerable<string>> GetBlocklistedPeerAddressesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.BlocklistAsync(cancellationToken).ConfigureAwait(false)).Select(i => i.Address.Address1);

        public async Task<ChainState> GetChainStateAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.ChainstateAsync(cancellationToken).ConfigureAwait(false));

        public async Task<string> GetChequeBookAddressAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookAddressAsync(cancellationToken).ConfigureAwait(false)).ChequebookAddress;

        public async Task<ChequebookBalance> GetChequeBookBalanceAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.ChequebookBalanceAsync(cancellationToken).ConfigureAwait(false));

        public async Task<ChequebookCashoutGet> GetChequeBookCashoutForPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.ChequebookCashoutGetAsync(peerAddress, cancellationToken).ConfigureAwait(false));

        public async Task<ChequebookChequeGet> GetChequeBookChequeForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.ChequebookChequeGetAsync(peerId, cancellationToken).ConfigureAwait(false));

        public async Task<SwarmChunk> GetChunkAsync(
            SwarmHash hash,
            bool? swarmCache = null,
            CancellationToken cancellationToken = default)
        {
            var chunkDto = await generatedClient.ChunksGetAsync((string)hash, swarmCache, cancellationToken).ConfigureAwait(false);
            using var memoryStream = new MemoryStream();
            await chunkDto.Stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
            var data = memoryStream.ToArray();
            return new SwarmChunk(hash, data);
        }

        public async Task<Stream> GetChunkStreamAsync(
            SwarmHash hash,
            bool? swarmCache = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChunksGetAsync(hash.ToString(), swarmCache,  cancellationToken).ConfigureAwait(false)).Stream;

        public async Task<PeerBalance> GetConsumedBalanceWithPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.ConsumedGetAsync(peerAddress, cancellationToken).ConfigureAwait(false));

        public async Task<SwarmHash> GetFeedAsync(
            string owner,
            string topic,
            int? at = null,
            int? after = null,
            string? type = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.FeedsGetAsync(owner, topic, at, after, type, cancellationToken).ConfigureAwait(false)).Reference;

        public async Task<FileResponse> GetFileAsync(
            SwarmAddress address,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(address, nameof(address));

            return address.RelativePath is null
                ? new(await generatedClient.BzzGetAsync(
                    address.Hash.ToString(),
                    swarmCache,
                    (SwarmRedundancyStrategy2?)swarmRedundancyStrategy,
                    swarmRedundancyFallbackMode,
                    swarmChunkRetrievalTimeout,
                    cancellationToken).ConfigureAwait(false))
                : new(await generatedClient.BzzGetAsync(
                    address.Hash.ToString(),
                    address.RelativePath.ToString(),
                    (SwarmRedundancyStrategy3?)swarmRedundancyStrategy,
                    swarmRedundancyFallbackMode,
                    swarmChunkRetrievalTimeout,
                    cancellationToken).ConfigureAwait(false));
        }

        public Task GetFileHeadAsync(SwarmHash hash, CancellationToken cancellationToken = default) =>
            generatedClient.BzzHeadAsync((string)hash, cancellationToken);

        public async Task<Health> GetHealthAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.HealthAsync(cancellationToken).ConfigureAwait(false));

        public async Task<NodeInfo> GetNodeInfoAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.NodeAsync(cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<PostageBatch>> GetOwnedPostageBatchesByNodeAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.StampsGetAsync(cancellationToken).ConfigureAwait(false)).Stamps.Select(i => new PostageBatch(i));

        public async Task<IEnumerable<TxInfo>> GetPendingTransactionsAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.TransactionsGetAsync(cancellationToken).ConfigureAwait(false)).PendingTransactions.Select(i => new TxInfo(i));

        public async Task<string> GetPinStatusAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.PinsGetAsync((string)hash, cancellationToken).ConfigureAwait(false)).Reference;

        public async Task<PostageBatch> GetPostageBatchAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.StampsGetAsync(batchId.ToString(), cancellationToken).ConfigureAwait(false));

        public async Task<ReserveCommitment> GetReserveCommitmentAsync(int depth, string anchor1, string anchor2,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.RchashAsync(depth, anchor1, anchor2, cancellationToken).ConfigureAwait(false));

        public async Task<ReserveState> GetReserveStateAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.ReservestateAsync(cancellationToken).ConfigureAwait(false));

        public async Task<SettlementData> GetSettlementsWithPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.SettlementsGetAsync(peerAddress, cancellationToken).ConfigureAwait(false));

        public async Task<StampsBuckets> GetStampsBucketsForBatchAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.StampsBucketsAsync(batchId.ToString(), cancellationToken).ConfigureAwait(false));

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

        public async Task<TxInfo> GetTransactionInfoAsync(
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
            SwarmHash hash,
            CancellationToken cancellationToken = default) =>
            generatedClient.StewardshipPutAsync((string)hash, cancellationToken: cancellationToken);

        public Task SendPssAsync(
            string topic,
            string targets,
            PostageBatchId batchId,
            string? recipient = null,
            CancellationToken cancellationToken = default) =>
            generatedClient.PssSendAsync(topic, targets, batchId.ToString(), recipient, cancellationToken);
        
        public Task SetWelcomeMessageAsync(
            string welcomeMessage,
            CancellationToken cancellationToken = default) =>
            generatedClient.WelcomeMessagePostAsync(
                new Body4
                {
                    WelcomeMessage = welcomeMessage
                },
                cancellationToken);

        public async Task StakeDeleteAsync(
            XDaiBalance? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            await generatedClient.StakeDeleteAsync(
                gasPrice?.ToWeiLong(),
                gasLimit,
                cancellationToken).ConfigureAwait(false);

        public async Task StakeGetAsync(CancellationToken cancellationToken = default) =>
            await generatedClient.StakeGetAsync(cancellationToken).ConfigureAwait(false);

        public async Task StakePostAsync(
            BzzBalance amount,
            XDaiBalance? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            await generatedClient.StakePostAsync(
                amount.ToPlurString(),
                gasPrice?.ToWeiLong(),
                gasLimit,
                cancellationToken).ConfigureAwait(false);

        public async Task<StatusNode> StatusNodeAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.StatusAsync(cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<StatusNode>> StatusPeersAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.StatusPeersAsync(cancellationToken).ConfigureAwait(false)).Stamps.Select(p => new StatusNode(p));

        public Task SubscribeToPssAsync(
            string topic,
            CancellationToken cancellationToken = default) =>
            generatedClient.PssSubscribeAsync(topic, cancellationToken);

        public async Task<PostageBatchId> TopUpPostageBatchAsync(
            PostageBatchId batchId,
            BzzBalance amount,
            XDaiBalance? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.StampsTopupAsync(
                batchId.ToString(),
                amount.ToPlurLong(),
                gasPrice?.ToWeiLong(),
                gasLimit,
                cancellationToken).ConfigureAwait(false)).BatchID;

        public async Task<string> TryConnectToPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.PingpongAsync(peerId, cancellationToken).ConfigureAwait(false)).Rtt;

        public Task UpdateTagAsync(
            long uid,
            SwarmHash? hash = null,
            CancellationToken cancellationToken = default) =>
            generatedClient.TagsPatchAsync(
                uid,
                hash.HasValue ?
                    new Body3 { Address = hash.Value.ToString() } :
                    null,
                cancellationToken);

        public async Task<SwarmHash> UploadChunkAsync(PostageBatchId batchId,
            Stream chunkData,
            bool swarmPin = false,
            bool swarmDeferredUpload = true,
            long? swarmTag = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChunksPostAsync(
                swarmTag,
                batchId.ToString(),
                chunkData,
                cancellationToken).ConfigureAwait(false)).Reference;

        public Task UploadChunksStreamAsync(
            PostageBatchId batchId,
            int? swarmTag = null,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default) =>
            generatedClient.ChunksStreamAsync(swarmTag, batchId.ToString(), cancellationToken);

        public async Task<SwarmHash> UploadBytesAsync(
            PostageBatchId batchId,
            Stream body,
            int? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            bool? swarmDeferredUpload = null,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.BytesPostAsync(
                swarm_postage_batch_id: batchId.ToString(),
                swarm_tag: swarmTag,
                swarm_pin: swarmPin,
                swarm_deferred_upload: swarmDeferredUpload,
                swarm_encrypt: swarmEncrypt,
                swarm_redundancy_level: (int)swarmRedundancyLevel,
                body: body,
                cancellationToken).ConfigureAwait(false)).Reference;

#if NET7_0_OR_GREATER
        public async Task<SwarmHash> UploadDirectoryAsync(
            PostageBatchId batchId,
            string directoryPath,
            int? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            string? swarmIndexDocument = null,
            string? swarmErrorDocument = null,
            bool? swarmDeferredUpload = null,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            CancellationToken cancellationToken = default)
        {
            // Create tar file.
            using var memoryStream = new MemoryStream();
            await TarFile.CreateFromDirectoryAsync(directoryPath, memoryStream, false, cancellationToken).ConfigureAwait(false);
            memoryStream.Position = 0;
            
            // Try set index document.
            if (swarmIndexDocument is null &&
                File.Exists(Path.Combine(directoryPath, "index.html")))
                swarmIndexDocument = "index.html";

            // Upload directory.
            return (await generatedClient.BzzPostAsync(
                new FileParameter(memoryStream, null, "application/x-tar"),
                swarm_tag: swarmTag,
                swarm_pin: swarmPin,
                swarm_encrypt: swarmEncrypt,
                swarm_collection: true,
                swarm_index_document: swarmIndexDocument,
                swarm_error_document: swarmErrorDocument,
                swarm_postage_batch_id: batchId.ToString(),
                swarm_deferred_upload: swarmDeferredUpload,
                swarm_redundancy_level: (SwarmRedundancyLevel)swarmRedundancyLevel,
                cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
        }
#endif

        public async Task<SwarmHash> UploadFileAsync(
            PostageBatchId batchId,
            Stream content,
            string? name = null,
            string? contentType = null,
            bool isFileCollection = false,
            int? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            string? swarmIndexDocument = null,
            string? swarmErrorDocument = null,
            bool? swarmDeferredUpload = null,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            CancellationToken cancellationToken = default)
        {
            return (await generatedClient.BzzPostAsync(
                new FileParameter(content, name, contentType),
                swarm_tag: swarmTag,
                swarm_pin: swarmPin,
                swarm_encrypt: swarmEncrypt,
                swarm_collection: isFileCollection,
                swarm_index_document: swarmIndexDocument,
                swarm_error_document: swarmErrorDocument,
                swarm_postage_batch_id: batchId.ToString(),
                swarm_deferred_upload: swarmDeferredUpload,
                swarm_redundancy_level: (SwarmRedundancyLevel)swarmRedundancyLevel,
                cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
        }

        public async Task<SwarmHash> UploadSocAsync(
            string owner,
            string id,
            string sig,
            PostageBatchId batchId,
            Stream body,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.SocAsync(
                owner,
                id,
                sig,
                batchId.ToString(),
                body,
                swarmPin,
                cancellationToken).ConfigureAwait(false)).Reference;

        public async Task<string> WalletWithdrawAsync(
            BzzBalance amount,
            string address,
            XDaiBalance coin,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.WalletWithdrawAsync(
                amount.ToPlurString(),
                address,
                coin.ToWeiString(),
                cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<string> WithdrawFromChequeBookAsync(
            BzzBalance amount,
            XDaiBalance? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookWithdrawAsync(
                amount.ToPlurLong(),
                gasPrice?.ToWeiLong(),
                cancellationToken).ConfigureAwait(false)).TransactionHash;

        // Helpers.
        private static Uri BuildBaseUrl(string url, int port)
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

            return new Uri(baseUrl);
        }
    }
}