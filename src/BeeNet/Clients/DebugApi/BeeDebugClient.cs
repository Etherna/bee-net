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

using Etherna.BeeNet.Clients.DebugApi.V1_2_0;
using Etherna.BeeNet.Clients.DebugApi.V1_2_1;
using Etherna.BeeNet.Clients.DebugApi.V2_0_0;
using Etherna.BeeNet.DtoModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Clients.DebugApi
{
    public class BeeDebugClient : IBeeDebugClient
    {
        // Fields.
        private readonly IBeeDebugClient_1_2_0 beeDebugClient_1_2_0;
        private readonly IBeeDebugClient_1_2_1 beeDebugClient_1_2_1;
        private readonly IBeeDebugClient_2_0_0 beeDebugClient_2_0_0;

        // Constructors.
        public BeeDebugClient(HttpClient httpClient, Uri baseUrl, DebugApiVersion apiVersion)
        {
            if (baseUrl is null)
                throw new ArgumentNullException(nameof(baseUrl));

            beeDebugClient_1_2_0 = new BeeDebugClient_1_2_0(httpClient) { BaseUrl = baseUrl.ToString() };
            beeDebugClient_1_2_1 = new BeeDebugClient_1_2_1(httpClient) { BaseUrl = baseUrl.ToString() };
            beeDebugClient_2_0_0 = new BeeDebugClient_2_0_0(httpClient) { BaseUrl = baseUrl.ToString() };
            CurrentApiVersion = apiVersion;
        }

        // Properties.
        public DebugApiVersion CurrentApiVersion { get; set; }

        // Methods.
        public async Task<string> BuyPostageBatchAsync(
            long amount,
            int depth,
            string? label = null,
            bool? immutable = null,
            long? gasPrice = null) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => ((JsonElement)(await beeDebugClient_1_2_0.StampsPostAsync(amount, depth, label, immutable, gasPrice).ConfigureAwait(false)).BatchID).ToString(),
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.StampsPostAsync(amount, depth, label, immutable, gasPrice).ConfigureAwait(false)).BatchID,
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.StampsPostAsync(amount.ToString(CultureInfo.InvariantCulture), depth, label, immutable, gasPrice).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> CashoutChequeForPeerAsync(
            string peerId,
            long? gasPrice = null,
            long? gasLimit = null) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.ChequebookCashoutPostAsync(peerId, gasPrice, gasLimit).ConfigureAwait(false)).TransactionHash,
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.ChequebookCashoutPostAsync(peerId, gasPrice, gasLimit).ConfigureAwait(false)).TransactionHash,
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.ChequebookCashoutPostAsync(peerId, gasPrice, gasLimit).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> ConnectToPeerAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.ConnectAsync(address).ConfigureAwait(false)).Address,
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.ConnectAsync(address).ConfigureAwait(false)).Address,
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.ConnectAsync(address).ConfigureAwait(false)).Address,
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeleteChunkAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new MessageResponseDto(await beeDebugClient_1_2_0.ChunksDeleteAsync(address).ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new MessageResponseDto(await beeDebugClient_1_2_1.ChunksDeleteAsync(address).ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new MessageResponseDto(await beeDebugClient_2_0_0.ChunksDeleteAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeletePeerAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new MessageResponseDto(await beeDebugClient_1_2_0.PeersDeleteAsync(address).ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new MessageResponseDto(await beeDebugClient_1_2_1.PeersDeleteAsync(address).ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new MessageResponseDto(await beeDebugClient_2_0_0.PeersDeleteAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DeleteTransactionAsync(
            string txHash,
            long? gasPrice = null) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.TransactionsDeleteAsync(txHash, gasPrice).ConfigureAwait(false)).TransactionHash,
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.TransactionsDeleteAsync(txHash, gasPrice).ConfigureAwait(false)).TransactionHash,
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.TransactionsDeleteAsync(txHash, gasPrice).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DepositIntoChequeBookAsync(
            long amount,
            long? gasPrice = null) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false)).TransactionHash,
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false)).TransactionHash,
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DilutePostageBatchAsync(
            string id,
            int depth) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => ((JsonElement)(await beeDebugClient_1_2_0.StampsDiluteAsync(id, depth).ConfigureAwait(false)).BatchID).ToString(),
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.StampsDiluteAsync(id, depth).ConfigureAwait(false)).BatchID,
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.StampsDiluteAsync(id, depth).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<AddressDetailDto> GetAddressesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new AddressDetailDto(await beeDebugClient_1_2_0.AddressesAsync().ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new AddressDetailDto(await beeDebugClient_1_2_1.AddressesAsync().ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new AddressDetailDto(await beeDebugClient_2_0_0.AddressesAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetAllBalancesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.BalancesGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.BalancesGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.BalancesGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<ChequeBookChequeGetDto>> GetAllChequeBookChequesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.ChequebookChequeGetAsync().ConfigureAwait(false)).Lastcheques.Select(i => new ChequeBookChequeGetDto(i)),
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.ChequebookChequeGetAsync().ConfigureAwait(false)).Lastcheques.Select(i => new ChequeBookChequeGetDto(i)),
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.ChequebookChequeGetAsync().ConfigureAwait(false)).Lastcheques.Select(i => new ChequeBookChequeGetDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetAllConsumedBalancesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.ConsumedGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.ConsumedGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.ConsumedGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetAllPeerAddressesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.PeersGetAsync().ConfigureAwait(false)).Peers.Select(i => i.Address),
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.PeersGetAsync().ConfigureAwait(false)).Peers.Select(i => i.Address),
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.PeersGetAsync().ConfigureAwait(false)).Peers.Select(i => i.Address),
                _ => throw new InvalidOperationException()
            };

        public async Task<SettlementDto> GetAllSettlementsAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new SettlementDto(await beeDebugClient_1_2_0.SettlementsGetAsync().ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new SettlementDto(await beeDebugClient_1_2_1.SettlementsGetAsync().ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new SettlementDto(await beeDebugClient_2_0_0.SettlementsGetAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TimeSettlementsDto> GetAllTimeSettlementsAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new TimeSettlementsDto(await beeDebugClient_1_2_0.TimesettlementsAsync().ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new TimeSettlementsDto(await beeDebugClient_1_2_1.TimesettlementsAsync().ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new TimeSettlementsDto(await beeDebugClient_2_0_0.TimesettlementsAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BatchDto>> GetAllValidPostageBatchesFromAllNodesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => throw new InvalidOperationException($"Debug API {CurrentApiVersion} doesn't implement this function"),
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.BatchesAsync().ConfigureAwait(false)).Stamps.Select(i => new BatchDto(i)),
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.BatchesAsync().ConfigureAwait(false)).Batches.Select(i => new BatchDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetBalanceWithPeerAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.BalancesGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.BalancesGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.BalancesGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetBlocklistedPeerAddressesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.BlocklistAsync().ConfigureAwait(false)).Peers.Select(i => i.Address),
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.BlocklistAsync().ConfigureAwait(false)).Peers.Select(i => i.Address),
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.BlocklistAsync().ConfigureAwait(false)).Peers.Select(i => i.Address),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChainStateDto> GetChainStateAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new ChainStateDto(await beeDebugClient_1_2_0.ChainstateAsync().ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new ChainStateDto(await beeDebugClient_1_2_1.ChainstateAsync().ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new ChainStateDto(await beeDebugClient_2_0_0.ChainstateAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetChequeBookAddressAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.ChequebookAddressAsync().ConfigureAwait(false)).ChequebookAddress,
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.ChequebookAddressAsync().ConfigureAwait(false)).ChequebookAddress,
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.ChequebookAddressAsync().ConfigureAwait(false)).ChequebookAddress,
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookBalanceDto> GetChequeBookBalanceAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new ChequeBookBalanceDto(await beeDebugClient_1_2_0.ChequebookBalanceAsync().ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new ChequeBookBalanceDto(await beeDebugClient_1_2_1.ChequebookBalanceAsync().ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new ChequeBookBalanceDto(await beeDebugClient_2_0_0.ChequebookBalanceAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookCashoutGetDto> GetChequeBookCashoutForPeerAsync(string peerId) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new ChequeBookCashoutGetDto(await beeDebugClient_1_2_0.ChequebookCashoutGetAsync(peerId).ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new ChequeBookCashoutGetDto(await beeDebugClient_1_2_1.ChequebookCashoutGetAsync(peerId).ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new ChequeBookCashoutGetDto(await beeDebugClient_2_0_0.ChequebookCashoutGetAsync(peerId).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookChequeGetDto> GetChequeBookChequeForPeerAsync(string peerId) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new ChequeBookChequeGetDto(await beeDebugClient_1_2_0.ChequebookChequeGetAsync(peerId).ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new ChequeBookChequeGetDto(await beeDebugClient_1_2_1.ChequebookChequeGetAsync(peerId).ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new ChequeBookChequeGetDto(await beeDebugClient_2_0_0.ChequebookChequeGetAsync(peerId).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> GetChunkAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new MessageResponseDto(await beeDebugClient_1_2_0.ChunksGetAsync(address).ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new MessageResponseDto(await beeDebugClient_1_2_1.ChunksGetAsync(address).ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new MessageResponseDto(await beeDebugClient_2_0_0.ChunksGetAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetConsumedBalanceWithPeerAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.ConsumedGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.ConsumedGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.ConsumedGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> GetHealthAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new VersionDto(await beeDebugClient_1_2_0.HealthAsync().ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new VersionDto(await beeDebugClient_1_2_1.HealthAsync().ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new VersionDto(await beeDebugClient_2_0_0.HealthAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<NodeInfoDto> GetNodeInfoAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => throw new InvalidOperationException($"Debug API {CurrentApiVersion} doesn't implement this function"),
                DebugApiVersion.v1_2_1 => new NodeInfoDto(await beeDebugClient_1_2_1.NodeAsync().ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new NodeInfoDto(await beeDebugClient_2_0_0.NodeAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PostageBatchDto>> GetOwnedPostageBatchesByNodeAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.StampsGetAsync().ConfigureAwait(false)).Stamps.Select(i => new PostageBatchDto(i)),
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.StampsGetAsync().ConfigureAwait(false)).Stamps.Select(i => new PostageBatchDto(i)),
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.StampsGetAsync().ConfigureAwait(false)).Stamps.Select(i => new PostageBatchDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PendingTransactionDto>> GetPendingTransactionsAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.TransactionsGetAsync().ConfigureAwait(false)).PendingTransactions.Select(i => new PendingTransactionDto(i)),
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.TransactionsGetAsync().ConfigureAwait(false)).PendingTransactions.Select(i => new PendingTransactionDto(i)),
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.TransactionsGetAsync().ConfigureAwait(false)).PendingTransactions.Select(i => new PendingTransactionDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<PostageBatchDto> GetPostageBatchAsync(string id) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new PostageBatchDto(await beeDebugClient_1_2_0.StampsGetAsync(id).ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new PostageBatchDto(await beeDebugClient_1_2_1.StampsGetAsync(id).ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new PostageBatchDto(await beeDebugClient_2_0_0.StampsGetAsync(id).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> GetReadinessAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new VersionDto(await beeDebugClient_1_2_0.ReadinessAsync().ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new VersionDto(await beeDebugClient_1_2_1.ReadinessAsync().ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new VersionDto(await beeDebugClient_2_0_0.ReadinessAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ReserveStateDto> GetReserveStateAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new ReserveStateDto(await beeDebugClient_1_2_0.ReservestateAsync().ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new ReserveStateDto(await beeDebugClient_1_2_1.ReservestateAsync().ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new ReserveStateDto(await beeDebugClient_2_0_0.ReservestateAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<SettlementDataDto>> GetSettlementsWithPeerAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.SettlementsGetAsync().ConfigureAwait(false)).Settlements.Select(i => new SettlementDataDto(i)),
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.SettlementsGetAsync().ConfigureAwait(false)).Settlements.Select(i => new SettlementDataDto(i)),
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.SettlementsGetAsync().ConfigureAwait(false)).Settlements.Select(i => new SettlementDataDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<StampsBucketsDto> GetStampsBucketsForBatchAsync(string batchId) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new StampsBucketsDto(await beeDebugClient_1_2_0.StampsBucketsAsync(batchId).ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new StampsBucketsDto(await beeDebugClient_1_2_1.StampsBucketsAsync(batchId).ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new StampsBucketsDto(await beeDebugClient_2_0_0.StampsBucketsAsync(batchId).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TopologyDto> GetSwarmTopologyAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new TopologyDto(await beeDebugClient_1_2_0.TopologyAsync().ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new TopologyDto(await beeDebugClient_1_2_1.TopologyAsync().ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new TopologyDto(await beeDebugClient_2_0_0.TopologyAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TagDto> GetTagInfoAsync(int uid) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new TagDto(await beeDebugClient_1_2_0.TagsAsync(uid).ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new TagDto(await beeDebugClient_1_2_1.TagsAsync(uid).ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new TagDto(await beeDebugClient_2_0_0.TagsAsync(uid).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TransactionsDto> GetTransactionInfoAsync(string txHash) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new TransactionsDto(await beeDebugClient_1_2_0.TransactionsGetAsync(txHash).ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new TransactionsDto(await beeDebugClient_1_2_1.TransactionsGetAsync(txHash).ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new TransactionsDto(await beeDebugClient_2_0_0.TransactionsGetAsync(txHash).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetWelcomeMessageAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.WelcomeMessageGetAsync().ConfigureAwait(false)).WelcomeMessage,
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.WelcomeMessageGetAsync().ConfigureAwait(false)).WelcomeMessage,
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.WelcomeMessageGetAsync().ConfigureAwait(false)).WelcomeMessage,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> RebroadcastTransactionAsync(string txHash) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.TransactionsPostAsync(txHash).ConfigureAwait(false)).TransactionHash,
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.TransactionsPostAsync(txHash).ConfigureAwait(false)).TransactionHash,
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.TransactionsPostAsync(txHash).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> SetWelcomeMessageAsync(string welcomeMessage) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => new VersionDto(await beeDebugClient_1_2_0.WelcomeMessagePostAsync(
                    new DebugApi.V1_2_0.Body
                    {
                        WelcomeMessage = welcomeMessage
                    }).ConfigureAwait(false)),
                DebugApiVersion.v1_2_1 => new VersionDto(await beeDebugClient_1_2_1.WelcomeMessagePostAsync(
                    new DebugApi.V1_2_1.Body
                    {
                        WelcomeMessage = welcomeMessage
                    }).ConfigureAwait(false)),
                DebugApiVersion.v2_0_0 => new VersionDto(await beeDebugClient_2_0_0.WelcomeMessagePostAsync(
                    new DebugApi.V2_0_0.Body
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
                DebugApiVersion.v1_2_0 => ((JsonElement)(await beeDebugClient_1_2_0.StampsTopupAsync(id, amount).ConfigureAwait(false)).BatchID).ToString(),
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.StampsTopupAsync(id, amount).ConfigureAwait(false)).BatchID,
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.StampsTopupAsync(id, amount).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> TryConnectToPeerAsync(string peerId) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.PingpongAsync(peerId).ConfigureAwait(false)).Rtt,
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.PingpongAsync(peerId).ConfigureAwait(false)).Rtt,
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.PingpongAsync(peerId).ConfigureAwait(false)).Rtt,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> WithdrawFromChequeBookAsync(
            long amount,
            long? gasPrice = null) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v1_2_0 => (await beeDebugClient_1_2_0.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false)).TransactionHash,
                DebugApiVersion.v1_2_1 => (await beeDebugClient_1_2_1.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false)).TransactionHash,
                DebugApiVersion.v2_0_0 => (await beeDebugClient_2_0_0.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };
    }
}