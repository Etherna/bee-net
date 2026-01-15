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

using Etherna.BeeNet.Clients.Beehive;
using Etherna.BeeNet.Exceptions;
using Etherna.BeeNet.Extensions;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Tools;
using Nethereum.Hex.HexConvertors.Extensions;
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

namespace Etherna.BeeNet
{
    [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings")]
    public class SwarmClient : ISwarmClient, IDisposable
    {
        // Consts.
        public const int ChunkStreamWSInternalBufferSize = 2 * ChunkStreamWSReceiveBufferSize + ChunkStreamWSSendBufferSize + 256 + 20;
        public const int ChunkStreamWSReceiveBufferSize = SwarmSoc.MaxSocSize;
        public const int ChunkStreamWSSendBufferSize = SwarmSoc.MaxSocSize;
        public const int DefaultPort = 1633;
        public readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(10);

        private readonly double TimeSpanMaxSeconds = TimeSpan.MaxValue.TotalSeconds;

        // Fields.
        private readonly Clients.Bee.BeeGeneratedClient beeGeneratedClient = null!;
        private readonly Clients.Beehive.BeehiveGeneratedClient beehiveGeneratedClient = null!;
        private readonly HttpClient httpClient;
        
        private bool disposed;

        // Constructor.
        public SwarmClient(
            Uri nodeUrl,
            SwarmClients apiCompatibility = SwarmClients.Bee,
            HttpClient? httpClient = null,
            bool isDryMode = false)
        {
            ArgumentNullException.ThrowIfNull(nodeUrl);
            
            this.httpClient = httpClient ?? new HttpClient { Timeout = DefaultTimeout };

            ApiCompatibility = apiCompatibility;
            switch (apiCompatibility)
            {
                case SwarmClients.Bee:
                    beeGeneratedClient = new Clients.Bee.BeeGeneratedClient(this.httpClient) { BaseUrl = nodeUrl.ToString() };
                    break;
                case SwarmClients.Beehive:
                    beehiveGeneratedClient = new Clients.Beehive.BeehiveGeneratedClient(nodeUrl.ToString(), this.httpClient);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(apiCompatibility), apiCompatibility, null);
            }
            IsDryMode = isDryMode;
            NodeUrl = nodeUrl;
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
        public SwarmClients ApiCompatibility { get; }
        public bool IsDryMode { get; }
        public Uri NodeUrl { get; }
        
        // Methods.
        public async Task<Dictionary<string, Account>> AccountingAsync(
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.AccountingAsync(cancellationToken).ConfigureAwait(false)).PeerData
                        .ToDictionary(
                            i => i.Key,
                            i => new Account(
                                balance: BzzValue.FromPlurString(i.Value.Balance),
                                thresholdReceived: BzzValue.FromPlurString(i.Value.ThresholdReceived),
                                thresholdGiven: BzzValue.FromPlurString(i.Value.ThresholdGiven),
                                surplusBalance: BzzValue.FromPlurString(i.Value.SurplusBalance),
                                reservedBalance: BzzValue.FromPlurString(i.Value.ReservedBalance),
                                shadowReservedBalance: BzzValue.FromPlurString(i.Value.ShadowReservedBalance),
                                ghostBalance: BzzValue.FromPlurString(i.Value.GhostBalance)));
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<(PostageBatchId BatchId, EthTxHash TxHash)> BuyPostageBatchAsync(
            BzzValue amount,
            int depth,
            string? label = null,
            bool? immutable = null,
            ulong? gasLimit = null,
            XDaiValue? gasPrice = null,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return (PostageBatchId.Zero, EthTxHash.Zero);

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var result = await beeGeneratedClient.StampsPostAsync(
                        amount: amount.ToPlurString(),
                        depth: depth,
                        label: label,
                        immutable: immutable,
                        gas_limit: gasLimit,
                        gas_price: gasPrice?.ToWeiLong(),
                        cancellationToken: cancellationToken).ConfigureAwait(false);
                    return (result.BatchID, result.TxHash);
                }
                case SwarmClients.Beehive:
                {
                    var result = await beehiveGeneratedClient.StampsPostAsync(
                        amount: amount.ToPlurLong(),
                        depth: depth,
                        label: label,
                        immutable: immutable,
                        gas_Limit: gasLimit,
                        gas_Price: gasPrice?.ToWeiLong(),
                        cancellationToken: cancellationToken).ConfigureAwait(false);
                    return (result.BatchID, result.TxHash);
                }
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<EthTxHash> CashoutChequeForPeerAsync(
            string peerId,
            XDaiValue? gasPrice = null,
            ulong? gasLimit = null,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return EthTxHash.Zero;
            
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    return (await beeGeneratedClient.ChequebookCashoutPostAsync(
                        peerId,
                        gasPrice?.ToWeiLong(),
                        gasLimit,
                        cancellationToken).ConfigureAwait(false)).TransactionHash;
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<CheckPinsResult> CheckPinsAsync(
            SwarmReference? reference,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.PinsCheckAsync(
                        reference?.ToString(),
                        cancellationToken).ConfigureAwait(false);
                    return new CheckPinsResult(
                        reference: response.Reference,
                        invalid: response.Invalid,
                        missing: response.Missing,
                        total: response.Total);
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }
        
        public async Task ChunksBulkUploadAsync(
            SwarmChunk[] chunks,
            PostageBatchId batchId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(chunks);
            
            if (IsDryMode)
                return;
            
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    foreach (var chunk in chunks)
                    {
                        using var memoryStream = new MemoryStream(chunk.GetFullPayloadToByteArray());
                    
                        await UploadChunkAsync(
                            memoryStream,
                            batchId,
                            cancellationToken: cancellationToken).ConfigureAwait(false);
                    }
                    break;
                }
                case SwarmClients.Beehive:
                {
                    // Build payload.
                    List<byte> payload = [];
                    foreach (var chunk in chunks)
                    {
                        var chunkBytes = chunk.GetFullPayload();
                        var chunkSizeByteArray = BitConverter.GetBytes((ushort)chunkBytes.Length);

                        //chunk size
                        payload.AddRange(chunkSizeByteArray);

                        //chunk data
                        payload.AddRange(chunkBytes.Span);
                
                        //check hash
                        payload.AddRange(chunk.Hash.ToByteArray());
                    }
            
                    var byteArrayPayload = payload.ToArray();
                    using var memoryStream = new MemoryStream(byteArrayPayload);
                    
                    // Upload stream.
                    await beehiveGeneratedClient.Ev1ChunksBulkUploadAsync(
                        swarm_Postage_Batch_Id: batchId.ToString(),
                        body: memoryStream,
                        cancellationToken: cancellationToken).ConfigureAwait(false);
                    
                    break;
                }
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<SwarmOverlayAddress> ConnectToPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return SwarmOverlayAddress.Zero;

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    return (await beeGeneratedClient.ConnectAsync(peerAddress, cancellationToken).ConfigureAwait(false))
                        .Address;
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<bool> CreatePinAsync(
            SwarmReference reference,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return false;

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    try
                    {
                        await beeGeneratedClient.PinsPostAsync(reference.ToString(), cancellationToken).ConfigureAwait(false);
                        return true;
                    }
                    catch (BeeNetApiException e) when (e.StatusCode == 404)
                    {
                        return false;
                    }
                }
                case SwarmClients.Beehive:
                {
                    try
                    {
                        await beehiveGeneratedClient.PinsPostAsync(reference.ToString(), cancellationToken).ConfigureAwait(false);
                        return true;
                    }
                    catch (BeeNetApiException e) when (e.StatusCode == 404)
                    {
                        return false;
                    }
                }
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<TagInfo> CreateTagAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return new TagInfo(new TagId(0), DateTimeOffset.UtcNow, 0, 0, 0, 0, 0);
            
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.TagsPostAsync(
                        new Clients.Bee.Body3 { Address = hash.ToString() },
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
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public Task DeletePeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return Task.CompletedTask;
            
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return beeGeneratedClient.PeersDeleteAsync(peerAddress, cancellationToken);
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public Task DeletePinAsync(
            SwarmReference reference,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return Task.CompletedTask;
            
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return beeGeneratedClient.PinsDeleteAsync(reference.ToString(), cancellationToken);
                case SwarmClients.Beehive:
                    return beehiveGeneratedClient.PinsDeleteAsync(reference.ToString(), cancellationToken);
                default:
                    throw new InvalidOperationException();
            }
        }

        public Task DeleteTagAsync(
            TagId id,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return Task.CompletedTask;
            
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return beeGeneratedClient.TagsDeleteAsync(
                        uid: id.Value,
                        cancellationToken: cancellationToken);
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<EthTxHash> DeleteTransactionAsync(
            EthTxHash txHash,
            XDaiValue? gasPrice = null,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return txHash;
            
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.TransactionsDeleteAsync(
                        txHash.ToString(),
                        gasPrice?.ToWeiLong(),
                        cancellationToken).ConfigureAwait(false)).TransactionHash;
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<EthTxHash> DepositIntoChequebookAsync(
            BzzValue amount,
            XDaiValue? gasPrice = null,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return EthTxHash.Zero;
            
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.ChequebookDepositAsync(
                        amount.ToPlurLong(),
                        gasPrice?.ToWeiLong(),
                        cancellationToken).ConfigureAwait(false)).TransactionHash;
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<EthTxHash> DilutePostageBatchAsync(
            PostageBatchId batchId,
            int depth,
            XDaiValue? gasPrice = null,
            ulong? gasLimit = null,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return EthTxHash.Zero;
            
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.StampsDiluteAsync(
                        batch_id: batchId.ToString(),
                        depth: depth,
                        gas_price: gasPrice?.ToWeiLong(),
                        gas_limit: gasLimit,
                        cancellationToken: cancellationToken).ConfigureAwait(false)).TxHash;
                case SwarmClients.Beehive:
                    return (await beehiveGeneratedClient.StampsDiluteAsync(
                        batchId: batchId.ToString(),
                        depth: depth,
                        gas_Limit: gasLimit,
                        gas_Price: gasPrice?.ToWeiLong(),
                        cancellationToken: cancellationToken).ConfigureAwait(false)).TxHash;
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<EnvelopeResponse> EnvelopeAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return new EnvelopeResponse(
                    EthAddress.Zero,
                    new PostageBucketIndex(0, 0),
                    DateTimeOffset.UtcNow,
                    Array.Empty<byte>());
            
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.EnvelopeAsync(batchId.ToString(), cancellationToken).ConfigureAwait(false);
                    return new(
                        response.Issuer,
                        PostageBucketIndex.BuildFromByteArray(response.Index.HexToByteArray()),
                        response.Timestamp.HexToByteArray().UnixTimeNanosecondsToDateTimeOffset(),
                        response.Signature.HexToByteArray());
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<IDictionary<string, BzzValue>> GetAllBalancesAsync(
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.BalancesGetAsync(cancellationToken).ConfigureAwait(false);
                    return response.Balances.ToDictionary(
                        b => b.Peer,
                        b => BzzValue.FromPlurString(b.Balance));
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<ChequebookCheque[]> GetAllChequebookChequesAsync(
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.ChequebookChequeGetAsync(cancellationToken).ConfigureAwait(false);
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
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<IDictionary<string, BzzValue>> GetAllConsumedBalancesAsync(
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.ConsumedGetAsync(cancellationToken).ConfigureAwait(false);
                    return response.Balances.ToDictionary(
                        b => b.Peer,
                        b => BzzValue.FromPlurString(b.Balance));
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<string[]> GetAllPeerAddressesAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    return (await beeGeneratedClient.PeersGetAsync(cancellationToken).ConfigureAwait(false)).Peers
                        .Select(i => i.Address).ToArray();
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<SwarmReference[]> GetAllPinsAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.PinsGetAsync(cancellationToken).ConfigureAwait(false);
                    return (response.References ?? []).Select(h => new SwarmReference(h)).ToArray();
                }
                case SwarmClients.Beehive:
                {
                    var response = await beehiveGeneratedClient.PinsGetAsync(cancellationToken).ConfigureAwait(false);
                    return response.References.Select(h => new SwarmReference(h)).ToArray();
                }
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<Settlement> GetAllSettlementsAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.SettlementsGetAsync(cancellationToken).ConfigureAwait(false);
                    return new Settlement(
                        totalReceived: BzzValue.FromPlurString(response.TotalReceived),
                        totalSent: BzzValue.FromPlurString(response.TotalSent),
                        settlements: response.Settlements
                            .Select(s => new SettlementData(
                                peer: s.Peer,
                                received: BzzValue.FromPlurString(s.Received),
                                sent: BzzValue.FromPlurString(s.Sent))));
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<Settlement> GetAllTimeSettlementsAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.TimesettlementsAsync(cancellationToken).ConfigureAwait(false);
                    return new Settlement(
                        totalReceived: BzzValue.FromPlurString(response.TotalReceived),
                        totalSent: BzzValue.FromPlurString(response.TotalSent),
                        settlements: response.Settlements
                            .Select(s => new SettlementData(
                                peer: s.Peer,
                                received: BzzValue.FromPlurString(s.Received),
                                sent: BzzValue.FromPlurString(s.Sent))));
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<BzzValue> GetBalanceWithPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return BzzValue.FromPlurString(
                        (await beeGeneratedClient.BalancesGetAsync(peerAddress, cancellationToken).ConfigureAwait(false))
                        .Balance);
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

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
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.BytesGetAsync(
                        reference: reference.ToString(),
                        swarm_cache: swarmCache,
                        swarm_redundancy_level: (Clients.Bee.SwarmRedundancyLevel?)swarmRedundancyLevel,
                        swarm_redundancy_strategy: (Clients.Bee.SwarmRedundancyStrategy?)swarmRedundancyStrategy,
                        swarm_redundancy_fallback_mode: swarmRedundancyFallbackMode,
                        swarm_chunk_retrieval_timeout: swarmChunkRetrievalTimeout,
                        swarm_act_timestamp: swarmActTimestamp,
                        swarm_act_publisher: swarmActPublisher,
                        swarm_act_history_address: swarmActHistoryAddress,
                        cancellationToken: cancellationToken).ConfigureAwait(false)).Stream;
                case SwarmClients.Beehive:
                    return (await beehiveGeneratedClient.BytesGetAsync(
                        reference: reference.ToString(),
                        swarm_Redundancy_Level: (Clients.Beehive.SwarmRedundancyLevel?)swarmRedundancyLevel,
                        swarm_Redundancy_Strategy: (Clients.Beehive.SwarmRedundancyStrategy?)swarmRedundancyStrategy,
                        swarm_Redundancy_Fallback_Mode: swarmRedundancyFallbackMode,
                        cancellationToken: cancellationToken).ConfigureAwait(false)).Stream;
                default:
                    throw new InvalidOperationException();
            }
        }

        public Task<HttpContentHeaders?> GetBytesHeadersAsync(
            SwarmReference reference,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return beeGeneratedClient.BytesHeadAsync(
                        reference: reference.ToString(),
                        swarm_act_timestamp: swarmActTimestamp,
                        swarm_act_publisher: swarmActPublisher,
                        swarm_act_history_address: swarmActHistoryAddress,
                        cancellationToken: cancellationToken);
                case SwarmClients.Beehive:
                    return beehiveGeneratedClient.BytesHeadAsync(
                        reference: reference.ToString(),
                        cancellationToken: cancellationToken);
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<string[]> GetBlocklistedPeerAddressesAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.BlocklistAsync(cancellationToken).ConfigureAwait(false))
                        .Select(i => i.Address.Address1).ToArray();
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<ChainState> GetChainStateAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.ChainstateAsync(cancellationToken).ConfigureAwait(false);
                    return new ChainState(
                        block: response.Block,
                        chainTip: response.ChainTip,
                        currentPrice: BzzValue.FromPlurLong(Math.Max(long.Parse(response.CurrentPrice, CultureInfo.InvariantCulture), 1)), //force price >= 1
                        totalAmount: BzzValue.FromPlurString(response.TotalAmount));
                }
                case SwarmClients.Beehive:
                {
                    var response = await beehiveGeneratedClient.ChainstateAsync(cancellationToken).ConfigureAwait(false);
                    return new ChainState(
                        block: response.Block,
                        chainTip: response.ChainTip,
                        currentPrice: BzzValue.FromPlurLong(Math.Max(response.CurrentPrice, 1)), //force price >= 1
                        totalAmount: BzzValue.FromPlurLong(response.TotalAmount));
                }
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<EthAddress> GetChequebookAddressAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.ChequebookAddressAsync(cancellationToken).ConfigureAwait(false))
                        .ChequebookAddress;
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<ChequebookBalance> GetChequebookBalanceAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.ChequebookBalanceAsync(cancellationToken).ConfigureAwait(false);
                    return new ChequebookBalance(
                        totalBalance: BzzValue.FromPlurString(response.TotalBalance),
                        availableBalance: BzzValue.FromPlurString(response.AvailableBalance));
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<ChequebookCashout> GetChequebookCashoutForPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.ChequebookCashoutGetAsync(
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
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<ChequebookCheque> GetChequebookChequeForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.ChequebookChequeGetAsync(peerId, cancellationToken).ConfigureAwait(false);
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
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<SwarmChunk> GetChunkAsync(
            SwarmHash hash,
            SwarmChunkBmt swarmChunkBmt,
            bool? swarmCache = null,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default)
        {
            using var memoryStream = new MemoryStream();
            var stream = await GetChunkStreamAsync(
                hash,
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

            return SwarmChunk.BuildChunkFromHashAndData(hash, chunkData, swarmChunkBmt);
        }

        public async Task<Stream> GetChunkStreamAsync(
            SwarmHash hash,
            bool? swarmCache = null,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.ChunksGetAsync(
                        hash.ToString(),
                        swarmCache,
                        swarmActTimestamp,
                        swarmActPublisher,
                        swarmActHistoryAddress,
                        cancellationToken).ConfigureAwait(false)).Stream;
                case SwarmClients.Beehive:
                    return (await beehiveGeneratedClient.ChunksGetAsync(
                        hash.ToString(),
                        cancellationToken).ConfigureAwait(false)).Stream;
                default:
                    throw new InvalidOperationException();
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

        public async Task<BzzValue> GetConsumedBalanceWithPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return BzzValue.FromPlurString(
                        (await beeGeneratedClient.ConsumedGetAsync(peerAddress, cancellationToken).ConfigureAwait(false))
                        .Balance);
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

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
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    if (!address.HasPath)
                    {
                        var response = await beeGeneratedClient.BzzGetAsync(
                            reference: address.Reference.ToString(),
                            swarm_cache: swarmCache,
                            swarm_redundancy_level: (Clients.Bee.SwarmRedundancyLevel3?)swarmRedundancyLevel,
                            swarm_redundancy_strategy: (Clients.Bee.SwarmRedundancyStrategy2?)swarmRedundancyStrategy,
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
                        var response = await beeGeneratedClient.BzzGetAsync(
                            reference: address.Reference.ToString(),
                            path: address.Path,
                            swarm_redundancy_level: (Clients.Bee.SwarmRedundancyLevel5?)swarmRedundancyLevel,
                            swarm_redundancy_strategy: (Clients.Bee.SwarmRedundancyStrategy4?)swarmRedundancyStrategy,
                            swarm_redundancy_fallback_mode: swarmRedundancyFallbackMode,
                            swarm_chunk_retrieval_timeout: swarmChunkRetrievalTimeout,
                            cancellationToken: cancellationToken).ConfigureAwait(false);
                        return new FileResponse(
                            response.ContentHeaders,
                            response.Headers,
                            response.Stream);
                    }
                }
                case SwarmClients.Beehive:
                {
                    var response = await beehiveGeneratedClient.BzzGetAsync(
                        address: address.ToString(),
                        swarm_Redundancy_Level: (Clients.Beehive.SwarmRedundancyLevel5?)swarmRedundancyLevel,
                        swarm_Redundancy_Strategy: (Clients.Beehive.SwarmRedundancyStrategy3?)swarmRedundancyStrategy,
                        swarm_Redundancy_Fallback_Mode: swarmRedundancyFallbackMode,
                        cancellationToken: cancellationToken).ConfigureAwait(false);
                    return new FileResponse(
                        response.ContentHeaders,
                        response.Headers,
                        response.Stream);
                }
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<(PostageBatch PostageBatch, EthAddress Owner)[]> GetGlobalValidPostageBatchesAsync(
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.BatchesAsync(cancellationToken).ConfigureAwait(false);
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
                case SwarmClients.Beehive:
                {
                    var response = await beehiveGeneratedClient.BatchesAsync(cancellationToken).ConfigureAwait(false);
                    return response.Batches.Select(batch => (
                            new PostageBatch(
                                id: batch.BatchID,
                                amount: BzzValue.FromPlurLong(batch.Value!.Value),
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
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<Health> GetHealthAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.HealthAsync(cancellationToken).ConfigureAwait(false);
                    return new(
                        isStatusOk: response.Status switch
                        {
                            Clients.Bee.Response20Status.Ok => true,
                            Clients.Bee.Response20Status.Nok => false,
                            _ => throw new InvalidOperationException()
                        },
                        version: response.Version,
                        apiVersion: response.ApiVersion);
                }
                case SwarmClients.Beehive:
                {
                    var response = await beehiveGeneratedClient.HealthAsync(cancellationToken).ConfigureAwait(false);
                    return new(
                        isStatusOk: response.Status switch
                        {
                            "ok" => true,
                            "nok" => false,
                            _ => throw new InvalidOperationException()
                        },
                        version: response.Version,
                        apiVersion: response.ApiVersion);
                }
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<NeighborhoodStatus[]> GetNeighborhoodsStatus(
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.StatusNeighborhoodsAsync(cancellationToken).ConfigureAwait(false);
                    return response.Neighborhoods.Select(s => new NeighborhoodStatus(
                        s.Neighborhood,
                        s.Proximity,
                        s.ReserveSizeWithinRadius)).ToArray();
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<SwarmNodeAddresses> GetNodeAddressesAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.AddressesAsync(cancellationToken).ConfigureAwait(false);
                    return new SwarmNodeAddresses(
                        underlay: response.Underlay.Where(i => !string.IsNullOrWhiteSpace(i)),
                        overlay: response.Overlay,
                        ethereum: response.Ethereum,
                        chainAddress: response.Chain_address,
                        publicKey: response.PublicKey,
                        pssPublicKey: response.PssPublicKey);
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<NodeInfo> GetNodeInfoAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.NodeAsync(cancellationToken).ConfigureAwait(false);
                    return new(
                        beeMode: response.BeeMode switch
                        {
                            Clients.Bee.Response31BeeMode.Dev => InfoBeeMode.Dev,
                            Clients.Bee.Response31BeeMode.Full => InfoBeeMode.Full,
                            Clients.Bee.Response31BeeMode.Light => InfoBeeMode.Light,
                            _ => throw new InvalidOperationException()
                        },
                        chequebookEnabled: response.ChequebookEnabled,
                        swapEnabled: response.SwapEnabled);
                }
                case SwarmClients.Beehive:
                {
                    var response = await beehiveGeneratedClient.NodeAsync(cancellationToken).ConfigureAwait(false);
                    return new(
                        beeMode: response.BeeMode switch
                        {
                            "dev" => InfoBeeMode.Dev,
                            "full" => InfoBeeMode.Full,
                            "light" => InfoBeeMode.Light,
                            _ => throw new InvalidOperationException()
                        },
                        chequebookEnabled: response.ChequebookEnabled,
                        swapEnabled: response.SwapEnabled);
                }
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<PostageBatch[]> GetOwnedPostageBatchesAsync(
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.StampsGetAsync(cancellationToken).ConfigureAwait(false);
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
                case SwarmClients.Beehive:
                {
                    var response = await beehiveGeneratedClient.StampsGetAsync(cancellationToken).ConfigureAwait(false);
                    return response.Stamps.Select(b =>
                            new PostageBatch(
                                amount: b.Amount != null ? BzzValue.FromPlurLong(b.Amount.Value) : null,
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
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<EthTx[]> GetPendingTransactionsAsync(
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.TransactionsGetAsync(cancellationToken).ConfigureAwait(false);
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
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<bool> GetPinStatusAsync(
            SwarmReference reference,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    try
                    {
                        return (await beeGeneratedClient.PinsGetAsync(reference.ToString(), cancellationToken).ConfigureAwait(false))
                            .Reference == reference;
                    }
                    catch (BeeNetApiException e) when (e.StatusCode == 404)
                    {
                        return false;
                    }
                }
                case SwarmClients.Beehive:
                {
                    try
                    {
                        return (await beehiveGeneratedClient.PinsGetAsync(reference.ToString(), cancellationToken).ConfigureAwait(false))
                            .Reference == reference;
                    }
                    catch (BeeNetApiException e) when (e.StatusCode == 404)
                    {
                        return false;
                    }
                }
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<PostageBatch> GetPostageBatchAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.StampsGetAsync(
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
                case SwarmClients.Beehive:
                {
                    var response = await beehiveGeneratedClient.StampsGetAsync(
                        batchId.ToString(),
                        cancellationToken).ConfigureAwait(false);
                    return new PostageBatch(
                        amount: response.Amount != null ? BzzValue.FromPlurLong(response.Amount.Value) : null,
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
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<PostageBucketsStatus> GetPostageBatchBucketsAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.StampsBucketsAsync(
                        batchId.ToString(),
                        cancellationToken).ConfigureAwait(false);
                    return new(
                        bucketDepth: response.BucketDepth,
                        bucketUpperBound: (uint)response.BucketUpperBound,
                        collisions: response.Buckets.Select(b => (uint)b.Collisions),
                        depth: response.Depth);
                }
                case SwarmClients.Beehive:
                {
                    var response = await beehiveGeneratedClient.StampsBucketsAsync(
                        batchId.ToString(),
                        cancellationToken).ConfigureAwait(false);
                    return new(
                        bucketDepth: response.BucketDepth,
                        bucketUpperBound: (uint)response.BucketUpperBound,
                        collisions: response.Buckets.Select(b => (uint)b.Collisions),
                        depth: response.Depth);
                }
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<bool> GetReadinessAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    try
                    {
                        await beeGeneratedClient.ReadinessAsync(cancellationToken).ConfigureAwait(false);
                        return true;
                    }
                    catch (BeeNetApiException e) when (e.StatusCode == 400)
                    {
                        return false;
                    }
                }
                case SwarmClients.Beehive:
                {
                    try
                    {
                        await beehiveGeneratedClient.ReadinessAsync(cancellationToken).ConfigureAwait(false);
                        return true;
                    }
                    catch (BeeNetApiException e) when (e.StatusCode == 400)
                    {
                        return false;
                    }
                }
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<ReserveCommitment> GetReserveCommitmentAsync(int depth, string anchor1, string anchor2,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.RchashAsync(depth, anchor1, anchor2, cancellationToken)
                        .ConfigureAwait(false);
                    return new(
                        duration: TimeSpan.FromSeconds(Math.Min(response.DurationSeconds, TimeSpanMaxSeconds)),
                        hash: response.Hash,
                        proof1: new ReserveCommitmentProof(
                            chunkSpan: response.Proofs.Proof1.ChunkSpan,
                            postageProof: new PostageProof(
                                index: response.Proofs.Proof1.PostageProof.Index,
                                postageId: response.Proofs.Proof1.PostageProof.PostageId,
                                signature: response.Proofs.Proof1.PostageProof.Signature,
                                timeStamp: DateTimeOffset.FromUnixTimeSeconds(
                                    long.Parse(response.Proofs.Proof1.PostageProof.TimeStamp,
                                        CultureInfo.InvariantCulture))),
                            proofSegments: response.Proofs.Proof1.ProofSegments ?? Array.Empty<string>(),
                            proofSegments2: response.Proofs.Proof1.ProofSegments2 ?? Array.Empty<string>(),
                            proofSegments3: response.Proofs.Proof1.ProofSegments3 ?? Array.Empty<string>(),
                            proveSegment: response.Proofs.Proof1.ProveSegment,
                            proveSegment2: response.Proofs.Proof1.ProveSegment2,
                            socProof: (response.Proofs.Proof1.SocProof ?? Array.Empty<Clients.Bee.SocProof>())
                            .Select(p => new Models.SocProof(
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
                                timeStamp: DateTimeOffset.FromUnixTimeSeconds(
                                    long.Parse(response.Proofs.Proof2.PostageProof.TimeStamp,
                                        CultureInfo.InvariantCulture))),
                            proofSegments: response.Proofs.Proof2.ProofSegments ?? Array.Empty<string>(),
                            proofSegments2: response.Proofs.Proof2.ProofSegments2 ?? Array.Empty<string>(),
                            proofSegments3: response.Proofs.Proof2.ProofSegments3 ?? Array.Empty<string>(),
                            proveSegment: response.Proofs.Proof2.ProveSegment,
                            proveSegment2: response.Proofs.Proof2.ProveSegment2,
                            socProof: (response.Proofs.Proof2.SocProof ?? Array.Empty<Clients.Bee.SocProof2>())
                            .Select(p => new Models.SocProof(
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
                                timeStamp: DateTimeOffset.FromUnixTimeSeconds(
                                    long.Parse(response.Proofs.ProofLast.PostageProof.TimeStamp,
                                        CultureInfo.InvariantCulture))),
                            proofSegments: response.Proofs.ProofLast.ProofSegments ?? Array.Empty<string>(),
                            proofSegments2: response.Proofs.ProofLast.ProofSegments2 ?? Array.Empty<string>(),
                            proofSegments3: response.Proofs.ProofLast.ProofSegments3 ?? Array.Empty<string>(),
                            proveSegment: response.Proofs.ProofLast.ProveSegment,
                            proveSegment2: response.Proofs.ProofLast.ProveSegment2,
                            socProof: (response.Proofs.ProofLast.SocProof ?? Array.Empty<Clients.Bee.SocProof3>())
                            .Select(p => new Models.SocProof(
                                chunkHash: p.ChunkAddr,
                                identifier: p.Identifier,
                                signature: p.Signature,
                                signer: p.Signer))));
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<ReserveState> GetReserveStateAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.ReservestateAsync(cancellationToken).ConfigureAwait(false);
                    return new(
                        commitment: response.Commitment,
                        radius: response.Radius,
                        storageRadius: response.StorageRadius);
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<SettlementData> GetSettlementsWithPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.SettlementsGetAsync(peerAddress, cancellationToken).ConfigureAwait(false);
                    return new SettlementData(
                        peer: response.Peer,
                        received: BzzValue.FromPlurString(response.Received),
                        sent: BzzValue.FromPlurString(response.Sent));
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
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
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.SocGetAsync(
                        owner: owner.ToString(),
                        id: id,
                        swarm_only_root_chunk: swarmOnlyRootChunk,
                        swarm_cache: swarmCache,
                        swarm_redundancy_strategy: (Clients.Bee.SwarmRedundancyStrategy6?)swarmRedundancyStrategy,
                        swarm_redundancy_fallback_mode: swarmRedundancyFallbackMode,
                        swarm_chunk_retrieval_timeout: swarmChunkRetrievalTimeout,
                        cancellationToken: cancellationToken).ConfigureAwait(false);
                    return new FileResponse(
                        response.ContentHeaders,
                        response.Headers,
                        response.Stream);
                }
                case SwarmClients.Beehive:
                {
                    var response = await beehiveGeneratedClient.SocGetAsync(
                        owner: owner.ToString(),
                        id: id,
                        swarm_Only_Root_Chunk: swarmOnlyRootChunk,
                        swarm_Redundancy_Strategy: (Clients.Beehive.SwarmRedundancyStrategy9?)swarmRedundancyStrategy,
                        swarm_Redundancy_Fallback_Mode: swarmRedundancyFallbackMode,
                        cancellationToken: cancellationToken).ConfigureAwait(false);
                    return new FileResponse(
                        response.ContentHeaders,
                        response.Headers,
                        response.Stream);
                }
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<Topology> GetSwarmTopologyAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.TopologyAsync(cancellationToken).ConfigureAwait(false);
                    return new(
                        baseAddr: response.BaseAddr,
                        bins: response.Bins.ToDictionary(
                            i => i.Key,
                            i => new PeersAggregate(
                                population: i.Value.Population,
                                connected: i.Value.Connected,
                                disconnectedPeers: i.Value.DisconnectedPeers.Select(peer => new Peer(
                                    address: peer.Address,
                                    lastSeenTimestamp: peer.Metrics.LastSeenTimestamp,
                                    sessionConnectionRetry: peer.Metrics.SessionConnectionRetry,
                                    connectionTotalDuration: peer.Metrics.ConnectionTotalDuration,
                                    sessionConnectionDuration: peer.Metrics.SessionConnectionDuration,
                                    sessionConnectionDirection: peer.Metrics.SessionConnectionDirection,
                                    latencyEwma: peer.Metrics.LatencyEWMA)),
                                connectedPeers: i.Value.ConnectedPeers.Select(peer => new Peer(
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
                            Clients.Bee.Response38NetworkAvailability.Unknown => NetworkAvailability.Unknown,
                            Clients.Bee.Response38NetworkAvailability.Available => NetworkAvailability.Available,
                            Clients.Bee.Response38NetworkAvailability.Unavailable => NetworkAvailability.Unavailable,
                            _ => throw new InvalidOperationException(),
                        },
                        nnLowWatermark: response.NnLowWatermark,
                        population: response.Population,
                        reachability: response.Reachability switch
                        {
                            Clients.Bee.Response38Reachability.Unknown => Reachability.Unknown,
                            Clients.Bee.Response38Reachability.Public => Reachability.Public,
                            Clients.Bee.Response38Reachability.Private => Reachability.Private,
                            _ => throw new InvalidOperationException(),
                        },
                        timestamp: DateTimeOffset.FromUnixTimeSeconds(
                            long.Parse(response.Timestamp, CultureInfo.InvariantCulture)));
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<TagInfo> GetTagInfoAsync(
            TagId id,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.TagsGetAsync(id.Value, cancellationToken).ConfigureAwait(false);
                    return new TagInfo(
                        id: new TagId(response.Uid),
                        startedAt: response.StartedAt,
                        split: response.Split,
                        seen: response.Seen,
                        stored: response.Stored,
                        sent: response.Sent,
                        synced: response.Synced);
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<TagInfo[]> GetTagsListAsync(
            int? offset = null,
            int? limit = null,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var tags =
                        (await beeGeneratedClient.TagsGetAsync(offset, limit, cancellationToken).ConfigureAwait(false)).Tags ??
                        Array.Empty<Clients.Bee.Tags>();
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
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<EthTx> GetTransactionInfoAsync(
            EthTxHash txHash,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.TransactionsGetAsync(txHash.ToString(), cancellationToken).ConfigureAwait(false);
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
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<WalletBalances> GetWalletBalance(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.WalletAsync(cancellationToken).ConfigureAwait(false);
                    return new(
                        bzzBalance: BzzValue.FromPlurString(response.BzzBalance),
                        xDaiBalance: XDaiValue.FromWeiString(response.NativeTokenBalance));
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.WelcomeMessageGetAsync(cancellationToken).ConfigureAwait(false))
                        .WelcomeMessage;
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public Task<ICollection<string>> GranteeGetAsync(
            string reference,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return beeGeneratedClient.GranteeGetAsync(reference, cancellationToken);
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

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
            if (IsDryMode)
                return new GranteeResponse(SwarmHash.Zero.ToString(), "");
            
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.GranteePatchAsync(
                        reference,
                        swarmActHistoryAddress,
                        batchId.ToString(),
                        new Clients.Bee.Body2 { Add = addList, Revoke = revokeList },
                        tagId?.ToString(),
                        swarmPin,
                        swarmDeferredUpload,
                        cancellationToken).ConfigureAwait(false);
                    return new GranteeResponse(response.Ref, response.Historyref);
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
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
            if (IsDryMode)
                return new GranteeResponse(SwarmHash.Zero.ToString(), "");

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.GranteePostAsync(
                        batchId.ToString(),
                        new Clients.Bee.Body { Grantees = grantees },
                        tagId?.ToString(),
                        swarmPin,
                        swarmDeferredUpload,
                        swarmActHistoryAddress,
                        cancellationToken).ConfigureAwait(false);
                    return new GranteeResponse(response.Ref, response.Historyref);
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<bool> IsChunkExistingAsync(
            SwarmHash hash,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    try
                    {
                        await beeGeneratedClient.ChunksHeadAsync(
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
                case SwarmClients.Beehive:
                {
                    try
                    {
                        await beehiveGeneratedClient.ChunksHeadAsync(
                            hash.ToString(),
                            cancellationToken).ConfigureAwait(false);
                        return true;
                    }
                    catch (BeeNetApiException)
                    {
                        return false;
                    }
                }
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<bool> IsContentRetrievableAsync(
            SwarmReference reference,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    return (await beeGeneratedClient.StewardshipGetAsync(reference.ToString(), cancellationToken)
                            .ConfigureAwait(false))
                        .IsRetrievable;
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<LogData> LoggersGetAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.LoggersGetAsync(cancellationToken).ConfigureAwait(false);
                    return new(
                        loggers: response.Loggers.Select(
                            i => new Loggers(
                                id: i.Id,
                                logger: i.Logger,
                                subsystem: i.Subsystem,
                                verbosity: i.Verbosity)).ToList(),
                        tree: response.Tree.ToDictionary(i => i.Key, i => i.Value?.Plus.ToList() ?? new List<string>()));
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<LogData> LoggersGetAsync(string exp, CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.LoggersGetAsync(exp, cancellationToken).ConfigureAwait(false);
                    return new(
                        loggers: response.Loggers.Select(
                            i => new Loggers(
                                id: i.Id,
                                logger: i.Logger,
                                subsystem: i.Subsystem,
                                verbosity: i.Verbosity)).ToList(),
                        tree: response.Tree.ToDictionary(i => i.Key, i => i.Value?.Plus.ToList() ?? new List<string>()));
                }
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task LoggersPutAsync(string exp, CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return;
            
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    await beeGeneratedClient.LoggersPutAsync(exp, cancellationToken).ConfigureAwait(false);
                    break;
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

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
            urlBuilder.Append(NodeUrl);
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
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return "";

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.TransactionsPostAsync(txHash.ToString(), cancellationToken)
                        .ConfigureAwait(false)).TransactionHash;
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<RedistributionState> RedistributionStateAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.RedistributionstateAsync(cancellationToken).ConfigureAwait(false);
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
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public Task ReuploadContentAsync(
            SwarmReference reference,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return Task.CompletedTask;

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return beeGeneratedClient.StewardshipPutAsync(reference.ToString(), cancellationToken: cancellationToken);
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public Task SendPssAsync(
            string topic,
            string targets,
            PostageBatchId batchId,
            string? recipient = null,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return Task.CompletedTask;

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return beeGeneratedClient.PssSendAsync(topic, targets, batchId.ToString(), recipient, cancellationToken);
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public Task SetWelcomeMessageAsync(
            string welcomeMessage,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return Task.CompletedTask;

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return beeGeneratedClient.WelcomeMessagePostAsync(
                        new Clients.Bee.Body5 { WelcomeMessage = welcomeMessage },
                        cancellationToken);
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task StakeDeleteAsync(
            XDaiValue? gasPrice = null,
            ulong? gasLimit = null,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return;

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    await beeGeneratedClient.StakeDeleteAsync(
                        gasPrice?.ToWeiLong(),
                        gasLimit,
                        cancellationToken).ConfigureAwait(false);
                    break;
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task StakeGetAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    await beeGeneratedClient.StakeGetAsync(cancellationToken).ConfigureAwait(false);
                    break;
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task StakePostAsync(
            BzzValue amount,
            XDaiValue? gasPrice = null,
            ulong? gasLimit = null,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return;

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    await beeGeneratedClient.StakePostAsync(
                        amount.ToPlurString(),
                        gasPrice?.ToWeiLong(),
                        gasLimit,
                        cancellationToken).ConfigureAwait(false);
                    break;
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public Task StakeWithdrawableDeleteAsync(
            XDaiValue? gasPrice = null,
            ulong? gasLimit = null,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return Task.CompletedTask;

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return beeGeneratedClient.StakeWithdrawableDeleteAsync(gasPrice?.ToWeiLong(), gasLimit, cancellationToken);
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public Task StakeWithdrawableGetAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return beeGeneratedClient.StakeWithdrawableGetAsync(cancellationToken);
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<StatusNode> StatusNodeAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.StatusAsync(cancellationToken).ConfigureAwait(false);
                    return new(
                        overlay: response.Overlay,
                        beeMode: response.BeeMode switch
                        {
                            Clients.Bee.Response70BeeMode.Light => StatusBeeMode.Light,
                            Clients.Bee.Response70BeeMode.Full => StatusBeeMode.Full,
                            Clients.Bee.Response70BeeMode.UltraLight => StatusBeeMode.UltraLight,
                            Clients.Bee.Response70BeeMode.Unknown => StatusBeeMode.Unknown,
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
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<StatusNode[]> StatusPeersAsync(CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                {
                    var response = await beeGeneratedClient.StatusPeersAsync(cancellationToken).ConfigureAwait(false);
                    return response.Snapshots.Select(
                            s => new StatusNode(
                                beeMode: s.BeeMode switch
                                {
                                    Clients.Bee.SnapshotsBeeMode.Light => StatusBeeMode.Light,
                                    Clients.Bee.SnapshotsBeeMode.Full => StatusBeeMode.Full,
                                    Clients.Bee.SnapshotsBeeMode.Dev => StatusBeeMode.Dev,
                                    Clients.Bee.SnapshotsBeeMode.UltraLight => StatusBeeMode.UltraLight,
                                    Clients.Bee.SnapshotsBeeMode.Unknown => StatusBeeMode.Unknown,
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
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public Task SubscribeToGsocAsync(
            string reference,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return Task.CompletedTask;

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return beeGeneratedClient.GsocSubscribeAsync(reference, cancellationToken);
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public Task SubscribeToPssAsync(
            string topic,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return Task.CompletedTask;

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return beeGeneratedClient.PssSubscribeAsync(topic, cancellationToken);
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<EthTxHash> TopUpPostageBatchAsync(
            PostageBatchId batchId,
            BzzValue amount,
            XDaiValue? gasPrice = null,
            ulong? gasLimit = null,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return EthTxHash.Zero;

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.StampsTopupAsync(
                        batch_id: batchId.ToString(),
                        amount: amount.ToPlurLong(),
                        gas_limit: gasLimit,
                        gas_price: gasPrice?.ToWeiLong(),
                        cancellationToken: cancellationToken).ConfigureAwait(false)).TxHash;
                case SwarmClients.Beehive:
                    return (await beehiveGeneratedClient.StampsTopupAsync(
                        batchId: batchId.ToString(),
                        amount: amount.ToPlurLong(),
                        gas_Limit: gasLimit,
                        gas_Price: gasPrice?.ToWeiLong(),
                        cancellationToken: cancellationToken).ConfigureAwait(false)).TxHash;
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<string> TryConnectToPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return "";

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.PingpongAsync(peerId, cancellationToken).ConfigureAwait(false)).Rtt;
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<FileResponse?> TryGetFeedAsync(
            EthAddress owner,
            SwarmFeedTopic topic,
            long? at = null,
            ulong? after = null,
            int? afterLevel = null,
            SwarmFeedType type = SwarmFeedType.Sequence,
            bool? swarmOnlyRootChunk = null,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    try
                    {
                        var response = await beeGeneratedClient.FeedsGetAsync(
                            owner: owner.ToString(),
                            topic: topic.ToString(),
                            at: at,
                            after: after,
                            type: type.ToString(),
                            swarm_only_root_chunk: swarmOnlyRootChunk,
                            swarm_cache: swarmCache,
                            swarm_redundancy_strategy: (Clients.Bee.SwarmRedundancyStrategy7?)swarmRedundancyStrategy,
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
                case SwarmClients.Beehive:
                    try
                    {
                        var response = await beehiveGeneratedClient.FeedsGetAsync(
                            owner: owner.ToString(),
                            topic: topic.ToString(),
                            at: at,
                            after: after,
                            afterLevel: afterLevel,
                            swarm_Only_Root_Chunk: swarmOnlyRootChunk,
                            swarm_Redundancy_Strategy: (Clients.Beehive.SwarmRedundancyStrategy7?)swarmRedundancyStrategy,
                            swarm_Redundancy_Fallback_Mode: swarmRedundancyFallbackMode,
                            type: (Etherna.BeeNet.Clients.Beehive.Type?)type,
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
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<HttpContentHeaders?> TryGetFileHeadersAsync(
            SwarmAddress address,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null, 
            RedundancyLevel? redundancyLevel = null,
            RedundancyStrategy? redundancyStrategy = null, 
            bool? redundancyStrategyFallback = null,
            CancellationToken cancellationToken = default)
        {
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return address.HasPath ?
                        await beeGeneratedClient.BzzHeadAsync(
                            reference: address.Reference.ToString(),
                            path: address.Path,
                            swarm_redundancy_level: (Clients.Bee.SwarmRedundancyLevel6?)redundancyLevel,
                            swarm_redundancy_strategy: (Clients.Bee.SwarmRedundancyStrategy5?)redundancyStrategy,
                            swarm_redundancy_fallback_mode: redundancyStrategyFallback,
                            cancellationToken: cancellationToken).ConfigureAwait(false) :
                
                        await beeGeneratedClient.BzzHeadAsync(
                            reference: address.Reference.ToString(),
                            swarm_act_timestamp: swarmActTimestamp,
                            swarm_act_publisher: swarmActPublisher,
                            swarm_act_history_address: swarmActHistoryAddress,
                            swarm_redundancy_level: (Clients.Bee.SwarmRedundancyLevel4?)redundancyLevel,
                            swarm_redundancy_strategy: (Clients.Bee.SwarmRedundancyStrategy3?)redundancyStrategy,
                            swarm_redundancy_fallback_mode: redundancyStrategyFallback,
                            cancellationToken: cancellationToken).ConfigureAwait(false);
                case SwarmClients.Beehive:
                    return await beehiveGeneratedClient.BzzHeadAsync(
                        address.ToString(),
                        swarm_Redundancy_Level: (SwarmRedundancyLevel6?)redundancyLevel,
                        swarm_Redundancy_Strategy: (SwarmRedundancyStrategy4?)redundancyStrategy,
                        swarm_Redundancy_Fallback_Mode: redundancyStrategyFallback,
                        cancellationToken: cancellationToken).ConfigureAwait(false);
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<long?> TryGetFileSizeAsync(
            SwarmAddress address,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null, 
            RedundancyLevel? redundancyLevel = null,
            RedundancyStrategy? redundancyStrategy = null, 
            bool? redundancyStrategyFallback = null,
            CancellationToken cancellationToken = default)
        {
            var headers = await TryGetFileHeadersAsync(
                address: address,
                swarmActTimestamp: swarmActTimestamp,
                swarmActPublisher: swarmActPublisher,
                swarmActHistoryAddress: swarmActHistoryAddress,
                redundancyLevel: redundancyLevel,
                redundancyStrategy: redundancyStrategy,
                redundancyStrategyFallback: redundancyStrategyFallback,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            return headers?.ContentLength;
        }

        public Task UpdateTagAsync(
            TagId id,
            SwarmHash? hash = null,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return Task.CompletedTask;

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return beeGeneratedClient.TagsPatchAsync(
                        uid: id.Value,
                        body: hash.HasValue ? new Clients.Bee.Body4 { Address = hash.Value.ToString() } : null,
                        cancellationToken: cancellationToken);
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<SwarmReference> UploadBytesAsync(
            Stream body,
            PostageBatchId batchId,
            ushort? compactLevel = 0,
            TagId? tagId = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            bool? swarmDeferredUpload = null,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return swarmEncrypt == true ? SwarmReference.EncryptedZero : SwarmReference.PlainZero;

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.BytesPostAsync(
                        swarm_postage_batch_id: batchId.ToString(),
                        swarm_tag: tagId?.Value,
                        swarm_pin: swarmPin,
                        swarm_deferred_upload: swarmDeferredUpload,
                        swarm_encrypt: swarmEncrypt,
                        swarm_redundancy_level: (int)swarmRedundancyLevel,
                        body: body,
                        cancellationToken).ConfigureAwait(false)).Reference;
                case SwarmClients.Beehive:
                    return (await beehiveGeneratedClient.BytesPostAsync(
                        swarm_Postage_Batch_Id: batchId.ToString(),
                        body: body,
                        swarm_Compact_Level: compactLevel,
                        swarm_Encrypt: swarmEncrypt,
                        swarm_Pin: swarmPin,
                        swarm_Redundancy_Level: (Clients.Beehive.SwarmRedundancyLevel3?)swarmRedundancyLevel,
                        cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
                default:
                    throw new InvalidOperationException();
            }
        }

        public Task<SwarmHash> UploadChunkAsync(
            SwarmCac chunk,
            PostageBatchId? batchId,
            TagId? tagId = null,
            PostageStamp? presignedPostageStamp = null,
            bool? swarmAct = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(chunk);
            
#pragma warning disable CA2025
            using var memoryStream = new MemoryStream(chunk.GetFullPayloadToByteArray());
            return UploadChunkAsync(
                memoryStream,
                batchId,
                tagId,
                presignedPostageStamp,
                swarmAct,
                swarmActHistoryAddress,
                cancellationToken);
#pragma warning restore CA2025
        }

        public async Task<SwarmHash> UploadChunkAsync(
            Stream chunkData,
            PostageBatchId? batchId,
            TagId? tagId = null,
            PostageStamp? presignedPostageStamp = null,
            bool? swarmAct = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return SwarmHash.Zero;

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.ChunksPostAsync(
                        swarm_tag: tagId?.Value,
                        swarm_postage_batch_id: batchId?.ToString(),
                        swarm_postage_stamp: presignedPostageStamp?.ToString(),
                        swarm_act: swarmAct,
                        swarm_act_history_address: swarmActHistoryAddress,
                        body: chunkData,
                        cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
                case SwarmClients.Beehive:
                    return (await beehiveGeneratedClient.ChunksPostAsync(
                        body: chunkData,
                        swarm_Postage_Batch_Id: batchId?.ToString(),
                        swarm_Postage_Stamp: presignedPostageStamp?.ToString(),
                        cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<SwarmReference> UploadDirectoryAsync(
            string directoryPath,
            PostageBatchId batchId,
            ushort? compactLevel = 0,
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
            if (IsDryMode)
                return swarmEncrypt == true ? SwarmReference.EncryptedZero : SwarmReference.PlainZero;

            // Create tar file.
            using var memoryStream = new MemoryStream();
            await TarFile.CreateFromDirectoryAsync(directoryPath, memoryStream, false, cancellationToken).ConfigureAwait(false);
            memoryStream.Position = 0;
            
            // Try set index document.
            if (swarmIndexDocument is null &&
                File.Exists(Path.Combine(directoryPath, "index.html")))
                swarmIndexDocument = "index.html";

            // Upload directory.
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.BzzPostAsync(
                        new Clients.Bee.FileParameter(memoryStream, null, "application/x-tar"),
                        swarm_tag: tagId?.Value,
                        swarm_pin: swarmPin,
                        swarm_encrypt: swarmEncrypt,
                        swarm_collection: true,
                        swarm_index_document: swarmIndexDocument,
                        swarm_error_document: swarmErrorDocument,
                        swarm_postage_batch_id: batchId.ToString(),
                        swarm_deferred_upload: swarmDeferredUpload,
                        swarm_redundancy_level: (Clients.Bee.SwarmRedundancyLevel2)swarmRedundancyLevel,
                        swarm_act: swarmAct,
                        swarm_act_history_address: swarmActHistoryAddress,
                        cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
                case SwarmClients.Beehive:
                    return (await beehiveGeneratedClient.BzzPostAsync(
                        swarm_Postage_Batch_Id: batchId.ToString(),
                        body: new Clients.Beehive.FileParameter(memoryStream, null, "application/x-tar"),
                        name: null,
                        swarm_Compact_Level: compactLevel,
                        swarm_Encrypt: swarmEncrypt,
                        swarm_Pin: swarmPin,
                        swarm_Redundancy_Level: (Clients.Beehive.SwarmRedundancyLevel9)swarmRedundancyLevel,
                        swarm_Collection: true,
                        swarm_Index_Document: swarmIndexDocument,
                        swarm_Error_Document: swarmErrorDocument,
                        cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<SwarmHash> UploadFeedManifestAsync(
            SwarmFeedBase feed,
            PostageBatchId batchId,
            ushort? compactLevel = 0,
            bool swarmPin = false,
            bool? swarmAct = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(feed);
            
            if (IsDryMode)
                return SwarmHash.Zero;
            
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.FeedsPostAsync(
                        owner: feed.Owner.ToString(),
                        topic: feed.Topic.ToString(),
                        type: feed.Type.ToString(),
                        swarm_pin: swarmPin,
                        swarm_postage_batch_id: batchId.ToString(),
                        swarm_act: swarmAct,
                        swarm_act_history_address: swarmActHistoryAddress,
                        cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
                case SwarmClients.Beehive:
                    return (await beehiveGeneratedClient.FeedsPostAsync(
                        owner: feed.Owner.ToString(),
                        topic: feed.Topic.ToString(),
                        swarm_Postage_Batch_Id: batchId.ToString(),
                        swarm_Compact_Level: compactLevel,
                        swarm_Pin: swarmPin,
                        type: (Clients.Beehive.Type2?)feed.Type,
                        cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<SwarmReference> UploadFileAsync(
            Stream content,
            PostageBatchId batchId,
            ushort? compactLevel = 0,
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
            if (IsDryMode)
                return swarmEncrypt == true ? SwarmReference.EncryptedZero : SwarmReference.PlainZero;

            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.BzzPostAsync(
                        new Clients.Bee.FileParameter(content, name, contentType),
                        swarm_tag: tagId?.Value,
                        swarm_pin: swarmPin,
                        swarm_encrypt: swarmEncrypt,
                        swarm_collection: isFileCollection,
                        swarm_index_document: swarmIndexDocument,
                        swarm_error_document: swarmErrorDocument,
                        swarm_postage_batch_id: batchId.ToString(),
                        swarm_deferred_upload: swarmDeferredUpload,
                        swarm_redundancy_level: (Clients.Bee.SwarmRedundancyLevel2)swarmRedundancyLevel,
                        cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
                case SwarmClients.Beehive:
                    return (await beehiveGeneratedClient.BzzPostAsync(
                        swarm_Postage_Batch_Id: batchId.ToString(),
                        body: new Clients.Beehive.FileParameter(content, name, contentType),
                        name: name,
                        swarm_Compact_Level: compactLevel,
                        swarm_Encrypt: swarmEncrypt,
                        swarm_Pin: swarmPin,
                        swarm_Redundancy_Level: (Clients.Beehive.SwarmRedundancyLevel9)swarmRedundancyLevel,
                        swarm_Collection: isFileCollection,
                        swarm_Index_Document: swarmIndexDocument,
                        swarm_Error_Document: swarmErrorDocument,
                        cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<SwarmHash> UploadSocAsync(
            SwarmSoc soc,
            PostageBatchId? batchId,
            PostageStamp? presignedPostageStamp = null,
            bool? swarmAct = null,
            string? swarmActHistoryAddress = null,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(soc);
            if (!soc.Signature.HasValue)
                throw new InvalidOperationException("SOC is not signed");
            
            if (IsDryMode)
                return SwarmHash.Zero;

            using var bodyMemoryStream = new MemoryStream(soc.InnerChunk.SpanData.ToArray());
            
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.SocPostAsync(
                        owner: soc.Owner.ToString(false),
                        id: soc.Identifier.ToString(),
                        sig: soc.Signature.Value.ToString(),
                        swarm_postage_batch_id: batchId?.ToString(),
                        body: bodyMemoryStream,
                        swarm_postage_stamp: presignedPostageStamp?.ToString(),
                        swarm_act: swarmAct,
                        swarm_act_history_address: swarmActHistoryAddress,
                        cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
                case SwarmClients.Beehive:
                    return (await beehiveGeneratedClient.SocPostAsync(
                        owner: soc.Owner.ToString(false),
                        id: soc.Identifier.ToString(),
                        sig: soc.Signature.Value.ToString(),
                        body: bodyMemoryStream,
                        swarm_Postage_Batch_Id: batchId?.ToString(),
                        swarm_Postage_Stamp: presignedPostageStamp?.ToString(),
                        swarm_Pin: swarmPin,
                        cancellationToken: cancellationToken).ConfigureAwait(false)).Reference;
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<EthTxHash> WalletBzzWithdrawAsync(
            BzzValue amount,
            EthAddress address,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return EthTxHash.Zero;
            
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.WalletWithdrawAsync(
                        amount.ToPlurString(),
                        address.ToString(),
                        Clients.Bee.Coin.Bzz,
                        cancellationToken).ConfigureAwait(false)).TransactionHash;
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<EthTxHash> WalletNativeCoinWithdrawAsync(
            XDaiValue amount,
            EthAddress address,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return EthTxHash.Zero;
            
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.WalletWithdrawAsync(
                        amount.ToWeiString(),
                        address.ToString(),
                        Clients.Bee.Coin.Nativetoken,
                        cancellationToken).ConfigureAwait(false)).TransactionHash;
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }

        public async Task<EthTxHash> WithdrawFromChequebookAsync(
            BzzValue amount,
            XDaiValue? gasPrice = null,
            CancellationToken cancellationToken = default)
        {
            if (IsDryMode)
                return EthTxHash.Zero;
            
            switch (ApiCompatibility)
            {
                case SwarmClients.Bee:
                    return (await beeGeneratedClient.ChequebookWithdrawAsync(
                        amount.ToPlurLong(),
                        gasPrice?.ToWeiLong(),
                        cancellationToken).ConfigureAwait(false)).TransactionHash;
                case SwarmClients.Beehive:
                    throw new NotSupportedException();
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}