// Copyright 2021-present Etherna SA
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
using Etherna.BeeNet.Hashing.Store;
using Etherna.BeeNet.Manifest;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Services;
using Etherna.BeeNet.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FileResponse = Etherna.BeeNet.Models.FileResponse;
using Loggers = Etherna.BeeNet.Models.Loggers;
using PostageProof = Etherna.BeeNet.Models.PostageProof;
using SocProof = Etherna.BeeNet.Clients.SocProof;

#if NET7_0_OR_GREATER
using System.Formats.Tar;
#endif

namespace Etherna.BeeNet
{
    [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings")]
    public class BeeClient : IBeeClient, IDisposable
    {
        // Consts.
        public const int ChunkStreamWSInternalBufferSize = 2 * ChunkStreamWSReceiveBufferSize + ChunkStreamWSSendBufferSize + 256 + 20;
        public const int ChunkStreamWSReceiveBufferSize = SwarmFeedChunk.MaxChunkSize;
        public const int ChunkStreamWSSendBufferSize = SwarmFeedChunk.MaxChunkSize;
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
            (await generatedClient.AccountingAsync(cancellationToken).ConfigureAwait(false)).PeerData.ToDictionary(
                i => i.Key,
                i => new Account(
                    balance: BzzBalance.FromPlurString(i.Value.Balance),
                    thresholdReceived: BzzBalance.FromPlurString(i.Value.ThresholdReceived),
                    thresholdGiven: BzzBalance.FromPlurString(i.Value.ThresholdGiven),
                    surplusBalance: BzzBalance.FromPlurString(i.Value.SurplusBalance),
                    reservedBalance: BzzBalance.FromPlurString(i.Value.ReservedBalance),
                    shadowReservedBalance: BzzBalance.FromPlurString(i.Value.ShadowReservedBalance),
                    ghostBalance: BzzBalance.FromPlurString(i.Value.GhostBalance)));
        
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
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await generatedClient.ChunksHeadAsync(
                    hash.ToString(),
                    swarmActTimestamp,
                    swarmActPublisher,
                    swarmActHistoryAddress,
                    cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (BeeNetApiException)
            {
                return false;
            }
        }

        public async Task<CheckPinsResult> CheckPinsAsync(
            SwarmHash? hash,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.PinsCheckAsync(
                    hash?.ToString(),
                    cancellationToken).ConfigureAwait(false);
            return new CheckPinsResult(
                hash: response.Reference,
                invalid: response.Invalid,
                missing: response.Missing,
                total: response.Total);
        }

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
            bool? swarmAct = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.FeedsPostAsync(
                owner,
                topic,
                type,
                swarmPin,
                batchId.ToString(),
                swarmAct,
                swarmActHistoryAddress,
                cancellationToken).ConfigureAwait(false)).Reference;

        public Task CreatePinAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default) =>
            generatedClient.PinsPostAsync((string)hash, cancellationToken);

        public async Task<TagInfo> CreateTagAsync(
            PostageBatchId postageBatchId,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.TagsPostAsync(
                postageBatchId.ToString(),
                cancellationToken).ConfigureAwait(false);
            return new TagInfo(
                id: new TagId(response.Uid),
                startedAt: response.StartedAt,
                split: response.Split,
                seen: response.Seen,
                stored: response.Stored,
                sent: response.Sent,
                synced: response.Synced);
        }

        public Task DeletePeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default) =>
            generatedClient.PeersDeleteAsync(peerAddress, cancellationToken);

        public Task DeletePinAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default) =>
            generatedClient.PinsDeleteAsync((string)hash, cancellationToken);

        public Task DeleteTagAsync(
            TagId id,
            CancellationToken cancellationToken = default) =>
            generatedClient.TagsDeleteAsync(id.Value, cancellationToken);

        public async Task<string> DeleteTransactionAsync(
            string txHash,
            XDaiBalance? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.TransactionsDeleteAsync(
                txHash,
                gasPrice?.ToWeiLong(),
                cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<string> DepositIntoChequebookAsync(
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

        public async Task<EnvelopeResponse> EnvelopeAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.EnvelopeAsync(batchId.ToString(), cancellationToken).ConfigureAwait(false);
            return new(
                response.Issuer,
                response.Index,
                response.Timestamp,
                response.Signature);
        }

        public async Task<AddressDetail> GetAddressesAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.AddressesAsync(cancellationToken).ConfigureAwait(false);
            return new AddressDetail(
                underlay: response.Underlay.Where(i => !string.IsNullOrWhiteSpace(i)),
                overlay: response.Overlay,
                ethereum: response.Ethereum,
                publicKey: response.PublicKey,
                pssPublicKey: response.PssPublicKey);
        }

        public async Task<IDictionary<string, BzzBalance>> GetAllBalancesAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.BalancesGetAsync(cancellationToken).ConfigureAwait(false);
            return response.Balances.ToDictionary(
                b => b.Peer,
                b => BzzBalance.FromPlurString(b.Balance));
        }

        public async Task<IEnumerable<ChequebookCheque>> GetAllChequebookChequesAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.ChequebookChequeGetAsync(cancellationToken).ConfigureAwait(false);
            return response.Lastcheques.Select(c =>
                new ChequebookCheque(
                    peer: c.Peer,
                lastReceived: c.Lastreceived is not null ? new ChequePayment(
                    beneficiary: c.Lastreceived.Beneficiary,
                    chequebook: c.Lastreceived.Chequebook,
                    payout: BzzBalance.FromPlurString(c.Lastreceived.Payout)) : null,
                lastSent: c.Lastsent is not null ? new ChequePayment(
                    beneficiary: c.Lastsent.Beneficiary,
                    chequebook: c.Lastsent.Chequebook,
                    payout: BzzBalance.FromPlurString(c.Lastsent.Payout)) : null));
        }

        public async Task<IDictionary<string, BzzBalance>> GetAllConsumedBalancesAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.ConsumedGetAsync(cancellationToken).ConfigureAwait(false);
            return response.Balances.ToDictionary(
                b => b.Peer,
                b => BzzBalance.FromPlurString(b.Balance));
        }

        public async Task<IEnumerable<string>> GetAllPeerAddressesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.PeersGetAsync(cancellationToken).ConfigureAwait(false)).Peers.Select(i => i.Address);

        public async Task<IEnumerable<SwarmHash>> GetAllPinsAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.PinsGetAsync(cancellationToken).ConfigureAwait(false)).Reference
            .Select(h => new SwarmHash(h));

        public async Task<Settlement> GetAllSettlementsAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.SettlementsGetAsync(cancellationToken).ConfigureAwait(false);
            return new Settlement(
                totalReceived: BzzBalance.FromPlurString(response.TotalReceived),
                totalSent: BzzBalance.FromPlurString(response.TotalSent),
                settlements: response.Settlements
                    .Select(s => new SettlementData(
                        peer: s.Peer,
                        received: BzzBalance.FromPlurString(s.Received),
                        sent: BzzBalance.FromPlurString(s.Sent))));
        }

        public async Task<Settlement> GetAllTimeSettlementsAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.TimesettlementsAsync(cancellationToken).ConfigureAwait(false);
            return new Settlement(
                totalReceived: BzzBalance.FromPlurString(response.TotalReceived),
                totalSent: BzzBalance.FromPlurString(response.TotalSent),
                settlements: response.Settlements
                    .Select(s => new SettlementData(
                        peer: s.Peer,
                        received: BzzBalance.FromPlurString(s.Received),
                        sent: BzzBalance.FromPlurString(s.Sent))));
        }

        public async Task<IDictionary<string, IEnumerable<PostageBatch>>> GetAllValidPostageBatchesFromAllNodesAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.BatchesAsync(cancellationToken).ConfigureAwait(false);
            return response.Batches.GroupBy(b => b.Owner)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(batch => new PostageBatch(
                        id: batch.BatchID,
                        amount: BzzBalance.FromPlurString(batch.Value),
                        blockNumber: batch.Start,
                        depth: batch.Depth,
                        exists: true,
                        isImmutable: batch.ImmutableFlag,
                        isUsable: true,
                        label: null,
                        storageRadius: batch.StorageRadius,
                        ttl: TimeSpan.FromSeconds(batch.BatchTTL),
                        utilization: null)));
        }

        public async Task<BzzBalance> GetBalanceWithPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default) =>
            BzzBalance.FromPlurString(
                (await generatedClient.BalancesGetAsync(peerAddress, cancellationToken).ConfigureAwait(false)).Balance);

        public async Task<Stream> GetBytesAsync(
            SwarmHash hash,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null, 
            CancellationToken cancellationToken = default) =>
            (await generatedClient.BytesGetAsync(
                (string)hash,
                swarmCache,
                (SwarmRedundancyStrategy?)swarmRedundancyStrategy,
                swarmRedundancyFallbackMode,
                swarmChunkRetrievalTimeout,
                swarmActTimestamp,
                swarmActPublisher,
                swarmActHistoryAddress,
                cancellationToken).ConfigureAwait(false)).Stream;

        public Task GetBytesHeadersAsync(
            SwarmHash hash,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default) =>
            generatedClient.BytesHeadAsync(
                hash.ToString(),
                swarmActTimestamp,
                swarmActPublisher,
                swarmActHistoryAddress,
                cancellationToken);

        public async Task<IEnumerable<string>> GetBlocklistedPeerAddressesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.BlocklistAsync(cancellationToken).ConfigureAwait(false)).Select(i => i.Address.Address1);

        public async Task<ChainState> GetChainStateAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.ChainstateAsync(cancellationToken).ConfigureAwait(false);
            return new ChainState(
                block: response.Block,
                chainTip: response.ChainTip,
                currentPrice: BzzBalance.FromPlurString(response.CurrentPrice),
                totalAmount: BzzBalance.FromPlurString(response.TotalAmount));
        }

        public async Task<string> GetChequebookAddressAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookAddressAsync(cancellationToken).ConfigureAwait(false)).ChequebookAddress;

        public async Task<ChequebookBalance> GetChequebookBalanceAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.ChequebookBalanceAsync(cancellationToken).ConfigureAwait(false);
            return new ChequebookBalance(
                totalBalance: BzzBalance.FromPlurString(response.TotalBalance),
                availableBalance: BzzBalance.FromPlurString(response.AvailableBalance));
        }

        public async Task<ChequebookCashout> GetChequebookCashoutForPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.ChequebookCashoutGetAsync(
                peerAddress,
                cancellationToken).ConfigureAwait(false);
            return new ChequebookCashout(
                peer: response.Peer,
                lastCashedCheque: response.LastCashedCheque is not null
                    ? new ChequePayment(
                        response.LastCashedCheque.Beneficiary,
                        response.LastCashedCheque.Chequebook,
                        BzzBalance.FromPlurString(response.LastCashedCheque.Payout))
                    : null,
                transactionHash: response.TransactionHash,
                result: new ResultChequebook(
                    recipient: response.Result.Recipient,
                    lastPayout: BzzBalance.FromPlurString(response.Result.LastPayout),
                    bounced: response.Result.Bounced),
                uncashedAmount: BzzBalance.FromPlurString(response.UncashedAmount));
        }

        public async Task<ChequebookCheque> GetChequebookChequeForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.ChequebookChequeGetAsync(peerId, cancellationToken).ConfigureAwait(false);
            return new ChequebookCheque(
                peer: response.Peer,
                lastReceived: response.Lastreceived is not null ? new ChequePayment(
                    beneficiary: response.Lastreceived.Beneficiary,
                    chequebook: response.Lastreceived.Chequebook,
                    payout: BzzBalance.FromPlurString(response.Lastreceived.Payout)) : null,
                lastSent: response.Lastsent is not null ? new ChequePayment(
                    beneficiary: response.Lastsent.Beneficiary,
                    chequebook: response.Lastsent.Chequebook,
                    payout: BzzBalance.FromPlurString(response.Lastsent.Payout)) : null);
        }

        public async Task<SwarmChunk> GetChunkAsync(
            SwarmHash hash,
            int maxRetryAttempts = 10,
            SwarmHash? rootHash = null,
            bool? swarmCache = null,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default)
        {
            using var stream = await GetChunkStreamAsync(
                (string)hash,
                maxRetryAttempts,
                rootHash,
                swarmCache,
                swarmActTimestamp,
                swarmActPublisher,
                swarmActHistoryAddress,
                cancellationToken).ConfigureAwait(false);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
            var spanAndData = memoryStream.ToArray();
            return SwarmChunk.BuildFromSpanAndData(hash, spanAndData);
        }

        public async Task<Stream> GetChunkStreamAsync(
            SwarmHash hash,
            int maxRetryAttempts = 10,
            SwarmHash? rootHash = null,
            bool? swarmCache = null,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default)
        {
            int i = 0;
            while (true)
            {
                try
                {
                    return (await generatedClient.ChunksGetAsync(
                        hash.ToString(),
                        i == 0 ? rootHash?.ToString() : null, //use rootHash only for first attempt
                        swarmCache,
                        swarmActTimestamp,
                        swarmActPublisher,
                        swarmActHistoryAddress,
                        cancellationToken).ConfigureAwait(false)).Stream;
                }
                catch (BeeNetApiException e) when (e.StatusCode == 404)
                {
                    if (i + 1 == maxRetryAttempts)
                        throw;
                }

                i++;
            }
        }

        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope")]
        public async Task<IChunkWebSocketUploader> GetChunkUploaderWebSocketAsync(
            PostageBatchId batchId,
            TagId? tagId = null,
            CancellationToken cancellationToken = default)
        {
            // Build uploader.
            var webSocket = await OpenChunkUploadWebSocketConnectionAsync(
                "chunks/stream",
                batchId,
                tagId,
                ChunkStreamWSInternalBufferSize,
                ChunkStreamWSReceiveBufferSize,
                ChunkStreamWSSendBufferSize,
                cancellationToken).ConfigureAwait(false);
            return new ChunkWebSocketUploader(webSocket);
        }

        public async Task<BzzBalance> GetConsumedBalanceWithPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default) =>
            BzzBalance.FromPlurString(
                (await generatedClient.ConsumedGetAsync(peerAddress, cancellationToken).ConfigureAwait(false)).Balance);

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
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(address, nameof(address));

            if (!address.HasPath)
            {
                var response = await generatedClient.BzzGetAsync(
                    address.Hash.ToString(),
                    swarmCache,
                    (SwarmRedundancyStrategy2?)swarmRedundancyStrategy,
                    swarmRedundancyFallbackMode,
                    swarmChunkRetrievalTimeout,
                    swarmActTimestamp,
                    swarmActPublisher,
                    swarmActHistoryAddress,
                    cancellationToken).ConfigureAwait(false);
                return new FileResponse(
                    response.ContentHeaders,
                    response.Headers,
                    response.Stream);
            }
            else
            {
                var response = await generatedClient.BzzGetAsync(
                    address.Hash.ToString(),
                    address.Path,
                    (SwarmRedundancyStrategy3?)swarmRedundancyStrategy,
                    swarmRedundancyFallbackMode,
                    swarmChunkRetrievalTimeout,
                    cancellationToken).ConfigureAwait(false);
                return new FileResponse(
                    response.ContentHeaders,
                    response.Headers,
                    response.Stream);
            }
        }

        public async Task<Health> GetHealthAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.HealthAsync(cancellationToken).ConfigureAwait(false);
            return new(
                isStatusOk: response.Status switch
                {
                    Response21Status.Ok => true,
                    Response21Status.Nok => false,
                    _ => throw new InvalidOperationException()
                },
                version: response.Version,
                apiVersion: response.ApiVersion);
        }

        public async Task<NodeInfo> GetNodeInfoAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.NodeAsync(cancellationToken).ConfigureAwait(false);
            return new(
                beeMode: response.BeeMode switch
                {
                    Response32BeeMode.Dev => InfoBeeMode.Dev,
                    Response32BeeMode.Full => InfoBeeMode.Full,
                    Response32BeeMode.Light => InfoBeeMode.Light,
                    _ => throw new InvalidOperationException()
                },
                chequebookEnabled: response.ChequebookEnabled,
                swapEnabled: response.SwapEnabled);
        }

        public async Task<IEnumerable<PostageBatch>> GetOwnedPostageBatchesByNodeAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.StampsGetAsync(cancellationToken).ConfigureAwait(false);
            return response.Stamps.Select(b =>
                new PostageBatch(
                    amount: BzzBalance.FromPlurString(b.Amount),
                    depth: b.Depth,
                    blockNumber: b.BlockNumber,
                    exists: b.Exists,
                    id: b.BatchID,
                    isImmutable: b.ImmutableFlag,
                    label: b.Label,
                    ttl: TimeSpan.FromSeconds(b.BatchTTL),
                    isUsable: b.Usable,
                    utilization: b.Utilization,
                    storageRadius: null));
        }

        public async Task<IEnumerable<TxInfo>> GetPendingTransactionsAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.TransactionsGetAsync(cancellationToken).ConfigureAwait(false);
            return response.PendingTransactions.Select(tx => new TxInfo(
                transactionHash: tx.TransactionHash,
                to: tx.To,
                nonce: tx.Nonce,
                gasPrice: XDaiBalance.FromWeiString(tx.GasPrice),
                gasLimit: tx.GasLimit,
                data: tx.Data,
                created: tx.Created,
                description: tx.Description,
                value: XDaiBalance.FromWeiString(tx.Value)));
        }

        public async Task<string> GetPinStatusAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.PinsGetAsync((string)hash, cancellationToken).ConfigureAwait(false)).Reference;

        public async Task<PostageBatch> GetPostageBatchAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.StampsGetAsync(
                batchId.ToString(),
                cancellationToken).ConfigureAwait(false);
            return new PostageBatch(
                amount: BzzBalance.FromPlurString(response.Amount),
                depth: response.Depth,
                blockNumber: response.BlockNumber,
                exists: response.Exists,
                id: response.BatchID,
                isImmutable: response.ImmutableFlag,
                label: response.Label,
                ttl: TimeSpan.FromMilliseconds(response.BatchTTL),
                isUsable: response.Usable,
                utilization: response.Utilization,
                storageRadius: null);
        }

        public async Task<ReserveCommitment> GetReserveCommitmentAsync(int depth, string anchor1, string anchor2,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.RchashAsync(depth, anchor1, anchor2, cancellationToken).ConfigureAwait(false);
            return new(
                duration: response.Duration,
                hash: response.Hash,
                proof1: new ReserveCommitmentProof(
                    chunkSpan: response.Proofs.Proof1.ChunkSpan,
                    postageProof: new PostageProof(
                        index: response.Proofs.Proof1.PostageProof.Index,
                        postageId: response.Proofs.Proof1.PostageProof.PostageId,
                        signature: response.Proofs.Proof1.PostageProof.Signature,
                        timeStamp:  DateTimeOffset.FromUnixTimeSeconds(
                            long.Parse(response.Proofs.Proof1.PostageProof.TimeStamp, CultureInfo.InvariantCulture))),
                    proofSegments: response.Proofs.Proof1.ProofSegments ?? Array.Empty<string>(),
                    proofSegments2: response.Proofs.Proof1.ProofSegments2 ?? Array.Empty<string>(),
                    proofSegments3: response.Proofs.Proof1.ProofSegments3 ?? Array.Empty<string>(),
                    proveSegment: response.Proofs.Proof1.ProveSegment,
                    proveSegment2: response.Proofs.Proof1.ProveSegment2,
                    socProof: (response.Proofs.Proof1.SocProof ?? Array.Empty<SocProof>()).Select(
                        p => new Models.SocProof(
                            chunkHash: p.ChunkAddr,
                            identifier: p.Identifier,
                            signature: p.Signature,
                            signer: p.Signer))),
                proof2: new ReserveCommitmentProof(
                    chunkSpan: response.Proofs.Proof2.ChunkSpan,
                    postageProof: new PostageProof(
                        index: response.Proofs.Proof2.PostageProof.Index,
                        postageId: response.Proofs.Proof2.PostageProof.PostageId,
                        signature: response.Proofs.Proof2.PostageProof.Signature,
                        timeStamp:  DateTimeOffset.FromUnixTimeSeconds(
                            long.Parse(response.Proofs.Proof2.PostageProof.TimeStamp, CultureInfo.InvariantCulture))),
                    proofSegments: response.Proofs.Proof2.ProofSegments ?? Array.Empty<string>(),
                    proofSegments2: response.Proofs.Proof2.ProofSegments2 ?? Array.Empty<string>(),
                    proofSegments3: response.Proofs.Proof2.ProofSegments3 ?? Array.Empty<string>(),
                    proveSegment: response.Proofs.Proof2.ProveSegment,
                    proveSegment2: response.Proofs.Proof2.ProveSegment2,
                    socProof: (response.Proofs.Proof2.SocProof ?? Array.Empty<SocProof2>()).Select(
                        p => new Models.SocProof(
                            chunkHash: p.ChunkAddr,
                            identifier: p.Identifier,
                            signature: p.Signature,
                            signer: p.Signer))),
                proofLast: new ReserveCommitmentProof(
                    chunkSpan: response.Proofs.ProofLast.ChunkSpan,
                    postageProof: new PostageProof(
                        index: response.Proofs.ProofLast.PostageProof.Index,
                        postageId: response.Proofs.ProofLast.PostageProof.PostageId,
                        signature: response.Proofs.ProofLast.PostageProof.Signature,
                        timeStamp:  DateTimeOffset.FromUnixTimeSeconds(
                            long.Parse(response.Proofs.ProofLast.PostageProof.TimeStamp, CultureInfo.InvariantCulture))),
                    proofSegments: response.Proofs.ProofLast.ProofSegments ?? Array.Empty<string>(),
                    proofSegments2: response.Proofs.ProofLast.ProofSegments2 ?? Array.Empty<string>(),
                    proofSegments3: response.Proofs.ProofLast.ProofSegments3 ?? Array.Empty<string>(),
                    proveSegment: response.Proofs.ProofLast.ProveSegment,
                    proveSegment2: response.Proofs.ProofLast.ProveSegment2,
                    socProof: (response.Proofs.ProofLast.SocProof ?? Array.Empty<SocProof3>()).Select(
                        p => new Models.SocProof(
                            chunkHash: p.ChunkAddr,
                            identifier: p.Identifier,
                            signature: p.Signature,
                            signer: p.Signer))));
        }

        public async Task<ReserveState> GetReserveStateAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.ReservestateAsync(cancellationToken).ConfigureAwait(false);
            return new(
                commitment: response.Commitment,
                radius: response.Radius,
                storageRadius: response.StorageRadius);
        }

        public async Task<SettlementData> GetSettlementsWithPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.SettlementsGetAsync(peerAddress, cancellationToken).ConfigureAwait(false);
            return new SettlementData(
                peer: response.Peer,
                received: BzzBalance.FromPlurString(response.Received),
                sent: BzzBalance.FromPlurString(response.Sent));
        }

        public async Task<StampsBuckets> GetStampsBucketsForBatchAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.StampsBucketsAsync(
                batchId.ToString(),
                cancellationToken).ConfigureAwait(false);
            return new(
                depth: response.Depth,
                bucketDepth: response.BucketDepth,
                bucketUpperBound: response.BucketUpperBound,
                collisions: response.Buckets.Select(b => b.Collisions));
        }

        public async Task<Topology> GetSwarmTopologyAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.TopologyAsync(cancellationToken).ConfigureAwait(false);
            return new(
                baseAddr: response.BaseAddr,
                bins: response.Bins.ToDictionary(
                    i => i.Key,
                    i => new PeersAggregate(
                        population: i.Value.Population,
                        connected: i.Value.Connected,
                        disconnectedPeers: i.Value.DisconnectedPeers.Select(
                            peer => new Peer(
                                address: peer.Address,
                                lastSeenTimestamp: peer.Metrics.LastSeenTimestamp,
                                sessionConnectionRetry: peer.Metrics.SessionConnectionRetry,
                                connectionTotalDuration: peer.Metrics.ConnectionTotalDuration,
                                sessionConnectionDuration: peer.Metrics.SessionConnectionDuration,
                                sessionConnectionDirection: peer.Metrics.SessionConnectionDirection,
                                latencyEwma: peer.Metrics.LatencyEWMA)),
                        connectedPeers: i.Value.ConnectedPeers.Select(
                            peer => new Peer(
                                address: peer.Address,
                                lastSeenTimestamp: peer.Metrics.LastSeenTimestamp,
                                sessionConnectionRetry: peer.Metrics.SessionConnectionRetry,
                                connectionTotalDuration: peer.Metrics.ConnectionTotalDuration,
                                sessionConnectionDuration: peer.Metrics.SessionConnectionDuration,
                                sessionConnectionDirection: peer.Metrics.SessionConnectionDirection,
                                latencyEwma: peer.Metrics.LatencyEWMA)))),
                connected: response.Connected,
                depth: response.Depth,
                networkAvailability: response.NetworkAvailability switch
                {
                    Response39NetworkAvailability.Unknown => NetworkAvailability.Unknown,
                    Response39NetworkAvailability.Available => NetworkAvailability.Available,
                    Response39NetworkAvailability.Unavailable => NetworkAvailability.Unavailable,
                    _ => throw new InvalidOperationException(),
                },
                nnLowWatermark: response.NnLowWatermark,
                population: response.Population,
                reachability: response.Reachability switch
                {
                    Response39Reachability.Unknown => Reachability.Unknown,
                    Response39Reachability.Public => Reachability.Public,
                    Response39Reachability.Private => Reachability.Private,
                    _ => throw new InvalidOperationException(),
                },
                timestamp: DateTimeOffset.FromUnixTimeSeconds(
                    long.Parse(response.Timestamp, CultureInfo.InvariantCulture)));
        }

        public async Task<TagInfo> GetTagInfoAsync(
            TagId id,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.TagsGetAsync(id.Value, cancellationToken).ConfigureAwait(false);
            return new TagInfo(
                id: new TagId(response.Uid),
                startedAt: response.StartedAt,
                split: response.Split,
                seen: response.Seen,
                stored: response.Stored,
                sent: response.Sent,
                synced: response.Synced);
        }

        public async Task<IEnumerable<TagInfo>> GetTagsListAsync(
            int? offset = null,
            int? limit = null,
            CancellationToken cancellationToken = default)
        {
            var tags =
                (await generatedClient.TagsGetAsync(offset, limit, cancellationToken).ConfigureAwait(false)).Tags ??
                Array.Empty<Tags>();
            return tags.Select(t => new TagInfo(
                id: new TagId(t.Uid),
                startedAt: t.StartedAt,
                split: t.Split,
                seen: t.Seen,
                stored: t.Stored,
                sent: t.Sent,
                synced: t.Synced));
        }

        public async Task<TxInfo> GetTransactionInfoAsync(
            string txHash,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.TransactionsGetAsync(txHash, cancellationToken).ConfigureAwait(false);
            return new TxInfo(
                transactionHash: response.TransactionHash,
                to: response.To,
                nonce: response.Nonce,
                gasPrice: XDaiBalance.FromWeiString(response.GasPrice),
                gasLimit: response.GasLimit,
                data: response.Data,
                created: response.Created,
                description: response.Description,
                value: XDaiBalance.FromWeiString(response.Value));
        }

        public async Task<WalletBalances> GetWalletBalance(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.WalletAsync(cancellationToken).ConfigureAwait(false);
            return new(
                bzzBalance: BzzBalance.FromPlurString(response.BzzBalance),
                xDaiBalance: XDaiBalance.FromWeiString(response.NativeTokenBalance));
        }

        public async Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.WelcomeMessageGetAsync(cancellationToken).ConfigureAwait(false)).WelcomeMessage;

        public Task<ICollection<string>> GranteeGetAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            generatedClient.GranteeGetAsync(reference, cancellationToken);

        public async Task<GranteeResponse> GranteePatchAsync(
            string reference,
            string swarmActHistoryAddress,
            PostageBatchId batchId,
            string[] AddList,
            string[] RevokeList,
            TagId? tagId = null,
            bool? swarmPin = null,
            bool? swarmDeferredUpload = null,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.GranteePatchAsync(
                reference,
                swarmActHistoryAddress,
                batchId.ToString(),
                new Body2() { Add = AddList, Revoke = RevokeList },
                tagId?.ToString(),
                swarmPin,
                swarmDeferredUpload,
                cancellationToken).ConfigureAwait(false);
            return new GranteeResponse(response.Ref, response.Historyref);
        }

        public async Task<GranteeResponse> GranteePostAsync(
            PostageBatchId batchId,
            string[] grantees,
            TagId? tagId = null,
            bool? swarmPin = null,
            bool? swarmDeferredUpload = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.GranteePostAsync(
                batchId.ToString(),
                new Body { Grantees = grantees },
                tagId?.ToString(),
                swarmPin,
                swarmDeferredUpload,
                swarmActHistoryAddress,
                cancellationToken).ConfigureAwait(false);
            return new GranteeResponse(response.Ref, response.Historyref);
        }

        public async Task<bool> IsContentRetrievableAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.StewardshipGetAsync((string)hash, cancellationToken).ConfigureAwait(false))
            .IsRetrievable;

        public async Task<LogData> LoggersGetAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.LoggersGetAsync(cancellationToken).ConfigureAwait(false);
            return new(
                loggers: response.Loggers.Select(
                    i => new Loggers(
                        id: i.Id,
                        logger: i.Logger,
                        subsystem: i.Subsystem,
                        verbosity: i.Verbosity)).ToList(),
                tree: response.Tree.ToDictionary(i => i.Key, i => i.Value?.Plus.ToList() ?? new List<string>()));
        }

        public async Task<LogData> LoggersGetAsync(string exp, CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.LoggersGetAsync(exp, cancellationToken).ConfigureAwait(false);
            return new(
                loggers: response.Loggers.Select(
                    i => new Loggers(
                        id: i.Id,
                        logger: i.Logger,
                        subsystem: i.Subsystem,
                        verbosity: i.Verbosity)).ToList(),
                tree: response.Tree.ToDictionary(i => i.Key, i => i.Value?.Plus.ToList() ?? new List<string>()));
        }

        public async Task LoggersPutAsync(string exp, CancellationToken cancellationToken = default) =>
            await generatedClient.LoggersPutAsync(exp, cancellationToken).ConfigureAwait(false);

        public async Task<WebSocket> OpenChunkUploadWebSocketConnectionAsync(
            string endpointPath,
            PostageBatchId batchId,
            TagId? tagId,
            int internalBufferSize,
            int receiveBufferSize,
            int sendBufferSize,
            CancellationToken cancellationToken)
        {
            // Build protocol upgrade request.
            //url
            var urlBuilder = new StringBuilder();
            urlBuilder.Append(BaseUrl);
            urlBuilder.Append(endpointPath);
            var url = urlBuilder.ToString();
            
            //secret key
            byte[] keyBytes = new byte[16];
            RandomNumberGenerator.Fill(keyBytes);
            
            //request
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Connection", "Upgrade");
            request.Headers.Add("Upgrade", "websocket");
            request.Headers.Add("Sec-WebSocket-Version", "13");
            request.Headers.Add("Sec-WebSocket-Key", Convert.ToBase64String(keyBytes));
            request.Headers.Add("swarm-postage-batch-id", batchId.ToString());
            if (tagId.HasValue)
                request.Headers.Add("swarm-tag", tagId.Value.ToString());

            // Send request.
            var response = await httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken).ConfigureAwait(false);

            // Evaluate response and upgrade.
            if (response.StatusCode != HttpStatusCode.SwitchingProtocols)
                throw new InvalidOperationException($"Failed to upgrade to WebSocket: {response.StatusCode}");
            
            // Create websocket from stream.
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var internalBufferArray = new byte[internalBufferSize];
            var webSocket = WebSocket.CreateClientWebSocket(
                stream,
                null,
                receiveBufferSize,
                sendBufferSize,
                WebSocket.DefaultKeepAliveInterval,
                false,
                internalBufferArray);
            return webSocket;
        }

        public async Task<string> RebroadcastTransactionAsync(
            string txHash,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.TransactionsPostAsync(txHash, cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<RedistributionState> RedistributionStateAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.RedistributionstateAsync(cancellationToken).ConfigureAwait(false);
            return new(
                isFrozen: response.IsFrozen,
                isFullySynced: response.IsFullySynced,
                isHealthy: response.IsHealthy,
                round: response.Round,
                lastWonRound: response.LastWonRound,
                lastPlayedRound: response.LastPlayedRound,
                lastFrozenRound: response.LastFrozenRound,
                block: response.Block,
                reward: BzzBalance.FromPlurString(response.Reward),
                fees: XDaiBalance.FromWeiString(response.Fees));
        }

        public async Task<SwarmChunkReference> ResolveAddressToChunkReferenceAsync(SwarmAddress address)
        {
            var chunkStore = new BeeClientChunkStore(this);
            
            var rootManifest = new ReferencedMantarayManifest(
                chunkStore,
                address.Hash);
            
            return await rootManifest.ResolveAddressToChunkReferenceAsync(address.Path).ConfigureAwait(false);
        }

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

        public Task StakeMigrateAsync(CancellationToken cancellationToken = default) =>
            generatedClient.StakeMigrateAsync(cancellationToken);

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

        public Task StakeWithdrawableDeleteAsync(
            XDaiBalance? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            generatedClient.StakeWithdrawableDeleteAsync(gasPrice?.ToWeiLong(), gasLimit, cancellationToken);

        public Task StakeWithdrawableGetAsync(CancellationToken cancellationToken = default) =>
            generatedClient.StakeWithdrawableGetAsync(cancellationToken);

        public async Task<StatusNode> StatusNodeAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.StatusAsync(cancellationToken).ConfigureAwait(false);
            return new(
                overlay: response.Overlay,
                beeMode: response.BeeMode switch
                {
                    Response66BeeMode.Light => StatusBeeMode.Light,
                    Response66BeeMode.Full => StatusBeeMode.Full,
                    Response66BeeMode.UltraLight => StatusBeeMode.UltraLight,
                    Response66BeeMode.Unknown => StatusBeeMode.Unknown,
                    _ => throw new InvalidOperationException()
                },
                proximity: response.Proximity,
                reserveSize: response.ReserveSize,
                reserveSizeWithinRadius: response.ReserveSizeWithinRadius,
                pullsyncRate: response.PullsyncRate,
                storageRadius: response.StorageRadius,
                connectedPeers: response.ConnectedPeers,
                neighborhoodSize: response.NeighborhoodSize,
                requestFailed: response.RequestFailed,
                batchCommitment: response.BatchCommitment,
                isReachable: response.IsReachable,
                lastSyncedBlock: response.LastSyncedBlock);
        }

        public async Task<IEnumerable<StatusNode>> StatusPeersAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.StatusPeersAsync(cancellationToken).ConfigureAwait(false);
            return response.Stamps.Select(
                s => new StatusNode(
                    beeMode: s.BeeMode switch
                    {
                        StampsBeeMode.Light => StatusBeeMode.Light,
                        StampsBeeMode.Full => StatusBeeMode.Full,
                        StampsBeeMode.UltraLight => StatusBeeMode.UltraLight,
                        StampsBeeMode.Unknown => StatusBeeMode.Unknown,
                        _ => throw new InvalidOperationException()
                    },
                    batchCommitment: s.BatchCommitment,
                    connectedPeers: s.ConnectedPeers,
                    isReachable: s.IsReachable,
                    lastSyncedBlock: s.LastSyncedBlock,
                    neighborhoodSize: s.NeighborhoodSize,
                    overlay: s.Overlay,
                    proximity: s.Proximity,
                    pullsyncRate: s.PullsyncRate,
                    reserveSize: s.ReserveSize,
                    reserveSizeWithinRadius: (int)s.ReserveSizeWithinRadius,
                    requestFailed: s.RequestFailed,
                    storageRadius: s.StorageRadius));
        }

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

        public async Task<HttpContentHeaders?> TryGetFileHeadersAsync(
            SwarmAddress address,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null, 
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(address, nameof(address));

            return address.HasPath ?
                await generatedClient.BzzHeadAsync(
                    address.Hash.ToString(),
                    address.Path,
                    cancellationToken).ConfigureAwait(false) :
                
                await generatedClient.BzzHeadAsync(
                    address.Hash.ToString(),
                    swarmActTimestamp,
                    swarmActPublisher,
                    swarmActHistoryAddress,
                    cancellationToken).ConfigureAwait(false);
        }

        public async Task<string?> TryGetFileNameAsync(
            SwarmAddress address,
            CancellationToken cancellationToken = default)
        {
            var chunkService = new ChunkService();
            var metadata = await chunkService.GetFileMetadataFromChunksAsync(
                address,
                new BeeClientChunkStore(this)).ConfigureAwait(false);
            return metadata.GetValueOrDefault(ManifestEntry.FilenameKey);
        }

        public async Task<long?> TryGetFileSizeAsync(
            SwarmAddress address,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null, 
            CancellationToken cancellationToken = default)
        {
            var headers = await TryGetFileHeadersAsync(
                address,
                swarmActTimestamp,
                swarmActPublisher,
                swarmActHistoryAddress,
                cancellationToken).ConfigureAwait(false);
            return headers?.ContentLength;
        }

        public Task UpdateTagAsync(
            TagId id,
            SwarmHash? hash = null,
            CancellationToken cancellationToken = default) =>
            generatedClient.TagsPatchAsync(
                id.Value,
                hash.HasValue ?
                    new Body3 { Address = hash.Value.ToString() } :
                    null,
                cancellationToken);

        public async Task<SwarmHash> UploadChunkAsync(PostageBatchId batchId,
            Stream chunkData,
            bool swarmPin = false,
            bool swarmDeferredUpload = true,
            TagId? tagId = null,
            string? swarmPostageStamp = null,
            bool? swarmAct = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChunksPostAsync(
                tagId?.Value,
                batchId.ToString(),
                swarmPostageStamp,
                swarmAct,
                swarmActHistoryAddress,
                chunkData,
                cancellationToken).ConfigureAwait(false)).Reference;

        public async Task<SwarmHash> UploadBytesAsync(
            PostageBatchId batchId,
            Stream body,
            TagId? tagId = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            bool? swarmDeferredUpload = null,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.BytesPostAsync(
                swarm_postage_batch_id: batchId.ToString(),
                swarm_tag: tagId?.Value,
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
            TagId? tagId = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            string? swarmIndexDocument = null,
            string? swarmErrorDocument = null,
            bool? swarmDeferredUpload = null,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            bool? swarmAct = null,
            string? swarmActHistoryAddress = null,
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
                swarm_tag: tagId?.Value,
                swarm_pin: swarmPin,
                swarm_encrypt: swarmEncrypt,
                swarm_collection: true,
                swarm_index_document: swarmIndexDocument,
                swarm_error_document: swarmErrorDocument,
                swarm_postage_batch_id: batchId.ToString(),
                swarm_deferred_upload: swarmDeferredUpload,
                swarm_redundancy_level: (SwarmRedundancyLevel)swarmRedundancyLevel,
                swarm_act: swarmAct,
                swarm_act_history_address: swarmActHistoryAddress,
                cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
        }
#endif

        public async Task<SwarmHash> UploadFileAsync(
            PostageBatchId batchId,
            Stream content,
            string? name = null,
            string? contentType = null,
            bool isFileCollection = false,
            TagId? tagId = null,
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
                swarm_tag: tagId?.Value,
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
            string? swarmPostageStamp = null,
            bool? swarmAct = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.SocAsync(
                owner,
                id,
                sig,
                body,
                swarmPin,
                batchId.ToString(),
                swarmPostageStamp,
                swarmAct,
                swarmActHistoryAddress,
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

        public async Task<string> WithdrawFromChequebookAsync(
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