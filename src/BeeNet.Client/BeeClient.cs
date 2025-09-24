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
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Tar;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileResponse = Etherna.BeeNet.Models.FileResponse;
using Loggers = Etherna.BeeNet.Models.Loggers;
using PostageProof = Etherna.BeeNet.Models.PostageProof;
using SocProof = Etherna.BeeNet.Clients.SocProof;

namespace Etherna.BeeNet
{
    [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings")]
    public class BeeClient : IBeeClient, IDisposable
    {
        // Consts.
        public const int ChunkStreamWSInternalBufferSize = 2 * ChunkStreamWSReceiveBufferSize + ChunkStreamWSSendBufferSize + 256 + 20;
        public const int ChunkStreamWSReceiveBufferSize = SwarmSoc.MaxSocSize;
        public const int ChunkStreamWSSendBufferSize = SwarmSoc.MaxSocSize;
        public const int DefaultPort = 1633;
        public readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(10);

        private readonly double TimeSpanMaxSeconds = TimeSpan.MaxValue.TotalSeconds;

        // Fields.
        private readonly BeeGeneratedClient generatedClient;
        private readonly HttpClient httpClient;
        
        private bool disposed;

        // Constructor.
        public BeeClient(
            Uri beeUrl,
            HttpClient? httpClient = null)
        {
            this.httpClient = httpClient ?? new HttpClient { Timeout = DefaultTimeout };

            BeeUrl = beeUrl;
            generatedClient = new BeeGeneratedClient(this.httpClient) { BaseUrl = BeeUrl.ToString() };
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
        public Uri BeeUrl { get; }
        
        // Methods.
        public async Task<Dictionary<string, Account>> AccountingAsync(
            CancellationToken cancellationToken = default) =>
            (await generatedClient.AccountingAsync(cancellationToken).ConfigureAwait(false)).PeerData.ToDictionary(
                i => i.Key,
                i => new Account(
                    balance: BzzValue.FromPlurString(i.Value.Balance),
                    thresholdReceived: BzzValue.FromPlurString(i.Value.ThresholdReceived),
                    thresholdGiven: BzzValue.FromPlurString(i.Value.ThresholdGiven),
                    surplusBalance: BzzValue.FromPlurString(i.Value.SurplusBalance),
                    reservedBalance: BzzValue.FromPlurString(i.Value.ReservedBalance),
                    shadowReservedBalance: BzzValue.FromPlurString(i.Value.ShadowReservedBalance),
                    ghostBalance: BzzValue.FromPlurString(i.Value.GhostBalance)));
        
        public async Task<(PostageBatchId BatchId, EthTxHash TxHash)> BuyPostageBatchAsync(
            BzzValue amount,
            int depth,
            string? label = null,
            bool? immutable = null,
            ulong? gasLimit = null,
            XDaiValue? gasPrice = null,
            CancellationToken cancellationToken = default)
        {
            var result = await generatedClient.StampsPostAsync(
                amount.ToPlurString(),
                depth,
                label,
                immutable,
                gasPrice?.ToWeiLong(),
                gasLimit,
                cancellationToken).ConfigureAwait(false);
            return (result.BatchID, result.TxHash);
        }

        public async Task<string> CashoutChequeForPeerAsync(
            string peerId,
            XDaiValue? gasPrice = null,
            ulong? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookCashoutPostAsync(
                peerId,
                gasPrice?.ToWeiLong(),
                gasLimit,
                cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<CheckPinsResult> CheckPinsAsync(
            SwarmReference? reference,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.PinsCheckAsync(
                    reference?.ToString(),
                    cancellationToken).ConfigureAwait(false);
            return new CheckPinsResult(
                reference: response.Reference,
                invalid: response.Invalid,
                missing: response.Missing,
                total: response.Total);
        }

        public async Task<string> ConnectToPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ConnectAsync(peerAddress, cancellationToken).ConfigureAwait(false)).Address;

        public async Task<bool> CreatePinAsync(
            SwarmReference reference,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await generatedClient.PinsPostAsync(reference.ToString(), cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (BeeNetApiException e) when (e.StatusCode == 404)
            {
                return false;
            }
        }

        public async Task<TagInfo> CreateTagAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.TagsPostAsync(
                new Body3 { Address = hash.ToString() },
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
            SwarmReference reference,
            CancellationToken cancellationToken = default) =>
            generatedClient.PinsDeleteAsync(reference.ToString(), cancellationToken);

        public Task DeleteTagAsync(
            TagId id,
            CancellationToken cancellationToken = default) =>
            generatedClient.TagsDeleteAsync(
                uid: id.Value,
                cancellationToken: cancellationToken);

        public async Task<string> DeleteTransactionAsync(
            EthTxHash txHash,
            XDaiValue? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.TransactionsDeleteAsync(
                txHash.ToString(),
                gasPrice?.ToWeiLong(),
                cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<string> DepositIntoChequebookAsync(
            BzzValue amount,
            XDaiValue? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookDepositAsync(
                amount.ToPlurLong(),
                gasPrice?.ToWeiLong(),
                cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<EthTxHash> DilutePostageBatchAsync(
            PostageBatchId batchId,
            int depth,
            XDaiValue? gasPrice = null,
            ulong? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.StampsDiluteAsync(
                batchId.ToString(),
                depth,
                gasPrice?.ToWeiLong(),
                gasLimit,
                cancellationToken).ConfigureAwait(false)).TxHash;

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

        public async Task<IDictionary<string, BzzValue>> GetAllBalancesAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.BalancesGetAsync(cancellationToken).ConfigureAwait(false);
            return response.Balances.ToDictionary(
                b => b.Peer,
                b => BzzValue.FromPlurString(b.Balance));
        }

        public async Task<ChequebookCheque[]> GetAllChequebookChequesAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.ChequebookChequeGetAsync(cancellationToken).ConfigureAwait(false);
            return response.Lastcheques.Select(c =>
                new ChequebookCheque(
                    peer: c.Peer,
                lastReceived: c.Lastreceived is not null ? new ChequePayment(
                    beneficiary: c.Lastreceived.Beneficiary,
                    chequebook: c.Lastreceived.Chequebook,
                    payout: BzzValue.FromPlurString(c.Lastreceived.Payout)) : null,
                lastSent: c.Lastsent is not null ? new ChequePayment(
                    beneficiary: c.Lastsent.Beneficiary,
                    chequebook: c.Lastsent.Chequebook,
                    payout: BzzValue.FromPlurString(c.Lastsent.Payout)) : null))
                .ToArray();
        }

        public async Task<IDictionary<string, BzzValue>> GetAllConsumedBalancesAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.ConsumedGetAsync(cancellationToken).ConfigureAwait(false);
            return response.Balances.ToDictionary(
                b => b.Peer,
                b => BzzValue.FromPlurString(b.Balance));
        }

        public async Task<string[]> GetAllPeerAddressesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.PeersGetAsync(cancellationToken).ConfigureAwait(false)).Peers.Select(i => i.Address).ToArray();

        public async Task<SwarmReference[]> GetAllPinsAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.PinsGetAsync(cancellationToken).ConfigureAwait(false);
            return (response.References ?? []).Select(h => new SwarmReference(h)).ToArray();
        }

        public async Task<Settlement> GetAllSettlementsAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.SettlementsGetAsync(cancellationToken).ConfigureAwait(false);
            return new Settlement(
                totalReceived: BzzValue.FromPlurString(response.TotalReceived),
                totalSent: BzzValue.FromPlurString(response.TotalSent),
                settlements: response.Settlements
                    .Select(s => new SettlementData(
                        peer: s.Peer,
                        received: BzzValue.FromPlurString(s.Received),
                        sent: BzzValue.FromPlurString(s.Sent))));
        }

        public async Task<Settlement> GetAllTimeSettlementsAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.TimesettlementsAsync(cancellationToken).ConfigureAwait(false);
            return new Settlement(
                totalReceived: BzzValue.FromPlurString(response.TotalReceived),
                totalSent: BzzValue.FromPlurString(response.TotalSent),
                settlements: response.Settlements
                    .Select(s => new SettlementData(
                        peer: s.Peer,
                        received: BzzValue.FromPlurString(s.Received),
                        sent: BzzValue.FromPlurString(s.Sent))));
        }

        public async Task<BzzValue> GetBalanceWithPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default) =>
            BzzValue.FromPlurString(
                (await generatedClient.BalancesGetAsync(peerAddress, cancellationToken).ConfigureAwait(false)).Balance);

        public async Task<Stream> GetBytesAsync(
            SwarmReference reference,
            bool? swarmCache = null,
            RedundancyLevel? swarmRedundancyLevel = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null, 
            CancellationToken cancellationToken = default) =>
            (await generatedClient.BytesGetAsync(
                reference: reference.ToString(),
                swarm_cache: swarmCache,
                swarm_redundancy_level: (SwarmRedundancyLevel?)swarmRedundancyLevel,
                swarm_redundancy_strategy: (SwarmRedundancyStrategy?)swarmRedundancyStrategy,
                swarm_redundancy_fallback_mode: swarmRedundancyFallbackMode,
                swarm_chunk_retrieval_timeout: swarmChunkRetrievalTimeout,
                swarm_act_timestamp: swarmActTimestamp,
                swarm_act_publisher: swarmActPublisher,
                swarm_act_history_address: swarmActHistoryAddress,
                cancellationToken: cancellationToken).ConfigureAwait(false)).Stream;

        public Task GetBytesHeadersAsync(
            SwarmReference reference,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default) =>
            generatedClient.BytesHeadAsync(
                reference.ToString(),
                swarmActTimestamp,
                swarmActPublisher,
                swarmActHistoryAddress,
                cancellationToken);

        public async Task<string[]> GetBlocklistedPeerAddressesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.BlocklistAsync(cancellationToken).ConfigureAwait(false)).Select(i => i.Address.Address1).ToArray();

        public async Task<ChainState> GetChainStateAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.ChainstateAsync(cancellationToken).ConfigureAwait(false);
            return new ChainState(
                block: response.Block,
                chainTip: response.ChainTip,
                currentPrice: BzzValue.FromPlurLong(Math.Max(long.Parse(response.CurrentPrice, CultureInfo.InvariantCulture), 1)), //force price >= 1
                totalAmount: BzzValue.FromPlurString(response.TotalAmount));
        }

        public async Task<EthAddress> GetChequebookAddressAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookAddressAsync(cancellationToken).ConfigureAwait(false)).ChequebookAddress;

        public async Task<ChequebookBalance> GetChequebookBalanceAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.ChequebookBalanceAsync(cancellationToken).ConfigureAwait(false);
            return new ChequebookBalance(
                totalBalance: BzzValue.FromPlurString(response.TotalBalance),
                availableBalance: BzzValue.FromPlurString(response.AvailableBalance));
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
                        BzzValue.FromPlurString(response.LastCashedCheque.Payout))
                    : null,
                transactionHash: response.TransactionHash,
                result: response.Result is not null
                    ? new ResultChequebook(
                        recipient: response.Result.Recipient,
                        lastPayout: BzzValue.FromPlurString(response.Result.LastPayout),
                        bounced: response.Result.Bounced)
                    : null,
                uncashedAmount: BzzValue.FromPlurString(response.UncashedAmount));
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
                    payout: BzzValue.FromPlurString(response.Lastreceived.Payout)) : null,
                lastSent: response.Lastsent is not null ? new ChequePayment(
                    beneficiary: response.Lastsent.Beneficiary,
                    chequebook: response.Lastsent.Chequebook,
                    payout: BzzValue.FromPlurString(response.Lastsent.Payout)) : null);
        }

        public async Task<SwarmChunk> GetChunkAsync(
            SwarmReference reference,
            SwarmChunkBmt swarmChunkBmt,
            bool? swarmCache = null,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default)
        {
            using var memoryStream = new MemoryStream();
            var stream = await GetChunkStreamAsync(
                reference.ToString(),
                swarmCache,
                swarmActTimestamp,
                swarmActPublisher,
                swarmActHistoryAddress,
                cancellationToken).ConfigureAwait(false);
            await using (stream.ConfigureAwait(false))
            {
                await stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
            }
            var chunkData = memoryStream.ToArray();

            return SwarmChunk.BuildChunkFromHashAndData(reference.Hash, chunkData, swarmChunkBmt);
        }

        public async Task<Stream> GetChunkStreamAsync(
            SwarmReference reference,
            bool? swarmCache = null,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChunksGetAsync(
                reference.ToString(),
                swarmCache,
                swarmActTimestamp,
                swarmActPublisher,
                swarmActHistoryAddress,
                cancellationToken).ConfigureAwait(false)).Stream;

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

        public async Task<BzzValue> GetConsumedBalanceWithPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default) =>
            BzzValue.FromPlurString(
                (await generatedClient.ConsumedGetAsync(peerAddress, cancellationToken).ConfigureAwait(false)).Balance);

        public async Task<FileResponse> GetFileAsync(
            SwarmAddress address,
            bool? swarmCache = null,
            RedundancyLevel? swarmRedundancyLevel = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default)
        {
            if (!address.HasPath)
            {
                var response = await generatedClient.BzzGetAsync(
                    reference: address.Reference.ToString(),
                    swarm_cache: swarmCache,
                    swarm_redundancy_level: (SwarmRedundancyLevel3?)swarmRedundancyLevel,
                    swarm_redundancy_strategy: (SwarmRedundancyStrategy2?)swarmRedundancyStrategy,
                    swarm_redundancy_fallback_mode: swarmRedundancyFallbackMode,
                    swarm_chunk_retrieval_timeout: swarmChunkRetrievalTimeout,
                    swarm_act_timestamp: swarmActTimestamp,
                    swarm_act_publisher: swarmActPublisher,
                    swarm_act_history_address: swarmActHistoryAddress,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
                return new FileResponse(
                    response.ContentHeaders,
                    response.Headers,
                    response.Stream);
            }
            else
            {
                var response = await generatedClient.BzzGetAsync(
                    reference: address.Reference.ToString(),
                    path: address.Path,
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

        public async Task<(PostageBatch PostageBatch, EthAddress Owner)[]> GetGlobalValidPostageBatchesAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.BatchesAsync(cancellationToken).ConfigureAwait(false);
            return response.Batches.Select(batch => (
                    new PostageBatch(
                        id: batch.BatchID,
                        amount: BzzValue.FromPlurString(batch.Value),
                        blockNumber: batch.Start,
                        depth: batch.Depth,
                        exists: true,
                        isImmutable: batch.Immutable,
                        isUsable: true,
                        label: null,
                        ttl: TimeSpan.FromSeconds(Math.Min(batch.BatchTTL, TimeSpanMaxSeconds)),
                        utilization: 0),
                    EthAddress.FromString(batch.Owner)))
                .ToArray();
        }

        public async Task<Health> GetHealthAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.HealthAsync(cancellationToken).ConfigureAwait(false);
            return new(
                isStatusOk: response.Status switch
                {
                    Response20Status.Ok => true,
                    Response20Status.Nok => false,
                    _ => throw new InvalidOperationException()
                },
                version: response.Version,
                apiVersion: response.ApiVersion);
        }

        public async Task<NeighborhoodStatus[]> GetNeighborhoodsStatus(
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.StatusNeighborhoodsAsync(cancellationToken).ConfigureAwait(false);
            return response.Neighborhoods.Select(s => new NeighborhoodStatus(
                s.Neighborhood,
                s.Proximity,
                s.ReserveSizeWithinRadius)).ToArray();
        }

        public async Task<SwarmNodeAddresses> GetNodeAddressesAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.AddressesAsync(cancellationToken).ConfigureAwait(false);
            return new SwarmNodeAddresses(
                underlay: response.Underlay.Where(i => !string.IsNullOrWhiteSpace(i)),
                overlay: response.Overlay,
                ethereum: response.Ethereum,
                chainAddress: response.Chain_address,
                publicKey: response.PublicKey,
                pssPublicKey: response.PssPublicKey);
        }

        public async Task<NodeInfo> GetNodeInfoAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.NodeAsync(cancellationToken).ConfigureAwait(false);
            return new(
                beeMode: response.BeeMode switch
                {
                    Response31BeeMode.Dev => InfoBeeMode.Dev,
                    Response31BeeMode.Full => InfoBeeMode.Full,
                    Response31BeeMode.Light => InfoBeeMode.Light,
                    _ => throw new InvalidOperationException()
                },
                chequebookEnabled: response.ChequebookEnabled,
                swapEnabled: response.SwapEnabled);
        }

        public async Task<PostageBatch[]> GetOwnedPostageBatchesAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.StampsGetAsync(cancellationToken).ConfigureAwait(false);
            return response.Stamps.Select(b =>
                new PostageBatch(
                    amount: b.Amount != null ? BzzValue.FromPlurString(b.Amount) : null,
                    depth: b.Depth,
                    blockNumber: b.BlockNumber,
                    exists: b.Exists,
                    id: b.BatchID,
                    isImmutable: b.ImmutableFlag,
                    label: b.Label,
                    ttl: TimeSpan.FromSeconds(Math.Min(b.BatchTTL, TimeSpanMaxSeconds)),
                    isUsable: b.Usable,
                    utilization: b.Utilization))
                .ToArray();
        }

        public async Task<EthTx[]> GetPendingTransactionsAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.TransactionsGetAsync(cancellationToken).ConfigureAwait(false);
            return response.PendingTransactions.Select(tx => new EthTx(
                transactionHash: tx.TransactionHash,
                to: tx.To,
                nonce: tx.Nonce,
                gasPrice: XDaiValue.FromWeiString(tx.GasPrice),
                gasLimit: tx.GasLimit,
                data: tx.Data,
                created: tx.Created,
                description: tx.Description,
                value: XDaiValue.FromWeiString(tx.Value)))
                .ToArray();
        }

        public async Task<bool> GetPinStatusAsync(
            SwarmReference reference,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return (await generatedClient.PinsGetAsync(reference.ToString(), cancellationToken).ConfigureAwait(false))
                    .Reference == reference;
            }
            catch (BeeNetApiException e) when (e.StatusCode == 404)
            {
                return false;
            }
        }

        public async Task<PostageBatch> GetPostageBatchAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.StampsGetAsync(
                batchId.ToString(),
                cancellationToken).ConfigureAwait(false);
            return new PostageBatch(
                amount: response.Amount != null ? BzzValue.FromPlurString(response.Amount) : null,
                depth: response.Depth,
                blockNumber: response.BlockNumber,
                exists: response.Exists,
                id: response.BatchID,
                isImmutable: response.ImmutableFlag,
                label: response.Label,
                ttl: TimeSpan.FromSeconds(Math.Min(response.BatchTTL, TimeSpanMaxSeconds)),
                isUsable: response.Usable,
                utilization: response.Utilization);
        }

        public async Task<PostageBucketsStatus> GetPostageBatchBucketsAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.StampsBucketsAsync(
                batchId.ToString(),
                cancellationToken).ConfigureAwait(false);
            return new(
                bucketDepth: response.BucketDepth,
                bucketUpperBound: (uint)response.BucketUpperBound,
                collisions: response.Buckets.Select(b => (uint)b.Collisions),
                depth: response.Depth);
        }

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
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.RchashAsync(depth, anchor1, anchor2, cancellationToken).ConfigureAwait(false);
            return new(
                duration: TimeSpan.FromSeconds(Math.Min(response.DurationSeconds, TimeSpanMaxSeconds)),
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
                received: BzzValue.FromPlurString(response.Received),
                sent: BzzValue.FromPlurString(response.Sent));
        }

        public async Task<FileResponse> GetSocDataAsync(
            EthAddress owner,
            string id,
            bool? swarmOnlyRootChunk = null,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.SocGetAsync(
                owner: owner.ToString(),
                id: id,
                swarm_only_root_chunk: swarmOnlyRootChunk,
                swarm_cache: swarmCache,
                swarm_redundancy_strategy: (SwarmRedundancyStrategy4?)swarmRedundancyStrategy,
                swarm_redundancy_fallback_mode: swarmRedundancyFallbackMode,
                swarm_chunk_retrieval_timeout: swarmChunkRetrievalTimeout,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            return new FileResponse(
                response.ContentHeaders,
                response.Headers,
                response.Stream);
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
                    Response38NetworkAvailability.Unknown => NetworkAvailability.Unknown,
                    Response38NetworkAvailability.Available => NetworkAvailability.Available,
                    Response38NetworkAvailability.Unavailable => NetworkAvailability.Unavailable,
                    _ => throw new InvalidOperationException(),
                },
                nnLowWatermark: response.NnLowWatermark,
                population: response.Population,
                reachability: response.Reachability switch
                {
                    Response38Reachability.Unknown => Reachability.Unknown,
                    Response38Reachability.Public => Reachability.Public,
                    Response38Reachability.Private => Reachability.Private,
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

        public async Task<TagInfo[]> GetTagsListAsync(
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
                synced: t.Synced))
                .ToArray();
        }

        public async Task<EthTx> GetTransactionInfoAsync(
            EthTxHash txHash,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.TransactionsGetAsync(txHash.ToString(), cancellationToken).ConfigureAwait(false);
            return new EthTx(
                transactionHash: response.TransactionHash,
                to: response.To,
                nonce: response.Nonce,
                gasPrice: XDaiValue.FromWeiString(response.GasPrice),
                gasLimit: response.GasLimit,
                data: response.Data,
                created: response.Created,
                description: response.Description,
                value: XDaiValue.FromWeiString(response.Value));
        }

        public async Task<WalletBalances> GetWalletBalance(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.WalletAsync(cancellationToken).ConfigureAwait(false);
            return new(
                bzzBalance: BzzValue.FromPlurString(response.BzzBalance),
                xDaiBalance: XDaiValue.FromWeiString(response.NativeTokenBalance));
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
            string[] addList,
            string[] revokeList,
            TagId? tagId = null,
            bool? swarmPin = null,
            bool? swarmDeferredUpload = null,
            CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.GranteePatchAsync(
                reference,
                swarmActHistoryAddress,
                batchId.ToString(),
                new Body2 { Add = addList, Revoke = revokeList },
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

        public async Task<bool> IsChunkExistingAsync(
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

        public async Task<bool> IsContentRetrievableAsync(
            SwarmReference reference,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.StewardshipGetAsync(reference.ToString(), cancellationToken).ConfigureAwait(false))
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
            urlBuilder.Append(BeeUrl);
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
            EthTxHash txHash,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.TransactionsPostAsync(txHash.ToString(), cancellationToken).ConfigureAwait(false)).TransactionHash;

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
                reward: BzzValue.FromPlurString(response.Reward),
                fees: XDaiValue.FromWeiString(response.Fees));
        }

        public Task ReuploadContentAsync(
            SwarmReference reference,
            CancellationToken cancellationToken = default) =>
            generatedClient.StewardshipPutAsync(reference.ToString(), cancellationToken: cancellationToken);

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
                new Body5 { WelcomeMessage = welcomeMessage },
                cancellationToken);

        public async Task StakeDeleteAsync(
            XDaiValue? gasPrice = null,
            ulong? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            await generatedClient.StakeDeleteAsync(
                gasPrice?.ToWeiLong(),
                gasLimit,
                cancellationToken).ConfigureAwait(false);

        public async Task StakeGetAsync(CancellationToken cancellationToken = default) =>
            await generatedClient.StakeGetAsync(cancellationToken).ConfigureAwait(false);

        public async Task StakePostAsync(
            BzzValue amount,
            XDaiValue? gasPrice = null,
            ulong? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            await generatedClient.StakePostAsync(
                amount.ToPlurString(),
                gasPrice?.ToWeiLong(),
                gasLimit,
                cancellationToken).ConfigureAwait(false);

        public Task StakeWithdrawableDeleteAsync(
            XDaiValue? gasPrice = null,
            ulong? gasLimit = null,
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
                    Response70BeeMode.Light => StatusBeeMode.Light,
                    Response70BeeMode.Full => StatusBeeMode.Full,
                    Response70BeeMode.UltraLight => StatusBeeMode.UltraLight,
                    Response70BeeMode.Unknown => StatusBeeMode.Unknown,
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
                lastSyncedBlock: response.LastSyncedBlock,
                committedDepth: response.CommittedDepth,
                isWarmingUp: response.IsWarmingUp);
        }

        public async Task<StatusNode[]> StatusPeersAsync(CancellationToken cancellationToken = default)
        {
            var response = await generatedClient.StatusPeersAsync(cancellationToken).ConfigureAwait(false);
            return response.Snapshots.Select(
                s => new StatusNode(
                    beeMode: s.BeeMode switch
                    {
                        SnapshotsBeeMode.Light => StatusBeeMode.Light,
                        SnapshotsBeeMode.Full => StatusBeeMode.Full,
                        SnapshotsBeeMode.Dev => StatusBeeMode.Dev,
                        SnapshotsBeeMode.UltraLight => StatusBeeMode.UltraLight,
                        SnapshotsBeeMode.Unknown => StatusBeeMode.Unknown,
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
                    reserveSizeWithinRadius: s.ReserveSizeWithinRadius,
                    requestFailed: s.RequestFailed,
                    storageRadius: s.StorageRadius,
                    committedDepth: s.CommittedDepth,
                    isWarmingUp: s.IsWarmingUp))
                .ToArray();
        }

        public Task SubscribeToGsocAsync(
            string reference,
            CancellationToken cancellationToken = default) =>
            generatedClient.GsocSubscribeAsync(reference, cancellationToken);

        public Task SubscribeToPssAsync(
            string topic,
            CancellationToken cancellationToken = default) =>
            generatedClient.PssSubscribeAsync(topic, cancellationToken);

        public async Task<EthTxHash> TopUpPostageBatchAsync(
            PostageBatchId batchId,
            BzzValue amount,
            XDaiValue? gasPrice = null,
            ulong? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.StampsTopupAsync(
                batchId.ToString(),
                amount.ToPlurLong(),
                gasPrice?.ToWeiLong(),
                gasLimit,
                cancellationToken).ConfigureAwait(false)).TxHash;

        public async Task<string> TryConnectToPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.PingpongAsync(peerId, cancellationToken).ConfigureAwait(false)).Rtt;

        public async Task<FileResponse?> TryGetFeedAsync(
            EthAddress owner,
            SwarmFeedTopic topic,
            long? at = null,
            ulong? after = null,
            SwarmFeedType type = SwarmFeedType.Sequence,
            bool? swarmOnlyRootChunk = null,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await generatedClient.FeedsGetAsync(
                    owner: owner.ToString(),
                    topic: topic.ToString(),
                    at: at,
                    after: after,
                    type: type.ToString(),
                    swarm_only_root_chunk: swarmOnlyRootChunk,
                    swarm_cache: swarmCache,
                    swarm_redundancy_strategy: (SwarmRedundancyStrategy5?)swarmRedundancyStrategy,
                    swarm_redundancy_fallback_mode: swarmRedundancyFallbackMode,
                    swarm_chunk_retrieval_timeout: swarmChunkRetrievalTimeout,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
                return new FileResponse(
                    response.ContentHeaders,
                    response.Headers,
                    response.Stream);
            }
            catch (BeeNetApiException e) when (e.StatusCode == 404)
            {
                return null;
            }
        }

        public async Task<HttpContentHeaders?> TryGetFileHeadersAsync(
            SwarmAddress address,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null, 
            CancellationToken cancellationToken = default)
        {
            return address.HasPath ?
                await generatedClient.BzzHeadAsync(
                    address.Reference.ToString(),
                    address.Path,
                    cancellationToken).ConfigureAwait(false) :
                
                await generatedClient.BzzHeadAsync(
                    address.Reference.ToString(),
                    swarmActTimestamp,
                    swarmActPublisher,
                    swarmActHistoryAddress,
                    cancellationToken).ConfigureAwait(false);
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
                uid: id.Value,
                body: hash.HasValue ?
                    new Body4 { Address = hash.Value.ToString() } :
                    null,
                cancellationToken: cancellationToken);

        public async Task<SwarmReference> UploadBytesAsync(
            Stream body,
            PostageBatchId batchId,
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

        public Task<SwarmReference> UploadChunkAsync(
            SwarmCac chunk,
            PostageBatchId? batchId,
            bool pinChunk = false,
            TagId? tagId = null,
            PostageStamp? presignedPostageStamp = null,
            bool? swarmAct = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(chunk, nameof(chunk));
            using var memoryStream = new MemoryStream(chunk.GetFullPayloadToByteArray());
            return UploadChunkAsync(
                memoryStream,
                batchId,
                pinChunk,
                tagId,
                presignedPostageStamp,
                swarmAct,
                swarmActHistoryAddress,
                cancellationToken);
        }

        public async Task<SwarmReference> UploadChunkAsync(
            Stream chunkData,
            PostageBatchId? batchId,
            bool pinChunk = false,
            TagId? tagId = null,
            PostageStamp? presignedPostageStamp = null,
            bool? swarmAct = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChunksPostAsync(
                swarm_tag: tagId?.Value,
                swarm_postage_batch_id: batchId?.ToString(),
                swarm_postage_stamp: presignedPostageStamp?.ToString(),
                swarm_act: swarmAct,
                swarm_act_history_address: swarmActHistoryAddress,
                body: chunkData,
                cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;

        public async Task<SwarmReference> UploadDirectoryAsync(
            string directoryPath,
            PostageBatchId batchId,
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
                swarm_redundancy_level: (SwarmRedundancyLevel2)swarmRedundancyLevel,
                swarm_act: swarmAct,
                swarm_act_history_address: swarmActHistoryAddress,
                cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
        }

        public async Task<SwarmHash> UploadFeedManifestAsync(
            SwarmFeedBase feed,
            PostageBatchId batchId,
            bool swarmPin = false,
            bool? swarmAct = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(feed, nameof(feed));
            
            return (await generatedClient.FeedsPostAsync(
                owner: feed.Owner.ToString(),
                topic: feed.Topic.ToString(),
                type: feed.Type.ToString(),
                swarm_pin: swarmPin,
                swarm_postage_batch_id: batchId.ToString(),
                swarm_act: swarmAct,
                swarm_act_history_address: swarmActHistoryAddress,
                cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
        }

        public async Task<SwarmReference> UploadFileAsync(
            Stream content,
            PostageBatchId batchId,
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
                swarm_redundancy_level: (SwarmRedundancyLevel2)swarmRedundancyLevel,
                cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
        }

        public async Task<SwarmHash> UploadSocAsync(
            SwarmSoc soc,
            PostageBatchId? batchId,
            PostageStamp? presignedPostageStamp = null,
            bool? swarmAct = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(soc, nameof(soc));
            if (!soc.Signature.HasValue)
                throw new InvalidOperationException("SOC is not signed");

            using var bodyMemoryStream = new MemoryStream(soc.InnerChunk.SpanData.ToArray());
            return (await generatedClient.SocPostAsync(
                owner: soc.Owner.ToString(false),
                id: soc.Identifier.ToString(),
                sig: soc.Signature.Value.ToString(),
                swarm_postage_batch_id: batchId?.ToString(),
                body: bodyMemoryStream,
                swarm_postage_stamp: presignedPostageStamp?.ToString(),
                swarm_act: swarmAct,
                swarm_act_history_address: swarmActHistoryAddress,
                cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
        }

        public async Task<string> WalletBzzWithdrawAsync(
            BzzValue amount,
            EthAddress address,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.WalletWithdrawAsync(
                amount.ToPlurString(),
                address.ToString(),
                Coin.Bzz,
                cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<string> WalletNativeCoinWithdrawAsync(
            XDaiValue amount,
            EthAddress address,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.WalletWithdrawAsync(
                amount.ToWeiString(),
                address.ToString(),
                Coin.Nativetoken,
                cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<string> WithdrawFromChequebookAsync(
            BzzValue amount,
            XDaiValue? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookWithdrawAsync(
                amount.ToPlurLong(),
                gasPrice?.ToWeiLong(),
                cancellationToken).ConfigureAwait(false)).TransactionHash;
    }
}