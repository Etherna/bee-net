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

using Etherna.BeeNet.Clients.DebugApi.V3_0_2;
using Etherna.BeeNet.DtoModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Clients.DebugApi
{
    public class BeeDebugClient : IBeeDebugClient
    {
        // Fields.
        private readonly IBeeDebugClient_3_0_2 beeDebugClient_3_0_2;

        // Constructors.
        public BeeDebugClient(HttpClient httpClient, Uri baseUrl, DebugApiVersion apiVersion)
        {
            if (baseUrl is null)
                throw new ArgumentNullException(nameof(baseUrl));

            beeDebugClient_3_0_2 = new BeeDebugClient_3_0_2(httpClient) { BaseUrl = baseUrl.ToString() };
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
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.StampsPostAsync(amount.ToString(CultureInfo.InvariantCulture), depth, label, immutable, gasPrice).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> CashoutChequeForPeerAsync(
            string peerId,
            long? gasPrice = null,
            long? gasLimit = null) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.ChequebookCashoutPostAsync(peerId, gasPrice, gasLimit).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> ConnectToPeerAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.ConnectAsync(address).ConfigureAwait(false)).Address,
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeleteChunkAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new MessageResponseDto(await beeDebugClient_3_0_2.ChunksDeleteAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeletePeerAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new MessageResponseDto(await beeDebugClient_3_0_2.PeersDeleteAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DeleteTransactionAsync(
            string txHash,
            long? gasPrice = null) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.TransactionsDeleteAsync(txHash, gasPrice).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DepositIntoChequeBookAsync(
            long amount,
            long? gasPrice = null) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DilutePostageBatchAsync(
            string id,
            int depth) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.StampsDiluteAsync(id, depth).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<AddressDetailDto> GetAddressesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new AddressDetailDto(await beeDebugClient_3_0_2.AddressesAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetAllBalancesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.BalancesGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<ChequeBookChequeGetDto>> GetAllChequeBookChequesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.ChequebookChequeGetAsync().ConfigureAwait(false)).Lastcheques.Select(i => new ChequeBookChequeGetDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetAllConsumedBalancesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.ConsumedGetAsync().ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetAllPeerAddressesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.PeersGetAsync().ConfigureAwait(false)).Peers.Select(i => i.Address),
                _ => throw new InvalidOperationException()
            };

        public async Task<SettlementDto> GetAllSettlementsAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new SettlementDto(await beeDebugClient_3_0_2.SettlementsGetAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TimeSettlementsDto> GetAllTimeSettlementsAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new TimeSettlementsDto(await beeDebugClient_3_0_2.TimesettlementsAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PostageBatchShortDto>> GetAllValidPostageBatchesFromAllNodesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.BatchesAsync().ConfigureAwait(false)).Batches.Select(i => new PostageBatchShortDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<BalanceDto> GetBalanceWithPeerAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new BalanceDto(await beeDebugClient_3_0_2.BalancesGetAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetBlocklistedPeerAddressesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.BlocklistAsync().ConfigureAwait(false)).Peers.Select(i => i.Address),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChainStateDto> GetChainStateAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new ChainStateDto(await beeDebugClient_3_0_2.ChainstateAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetChequeBookAddressAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.ChequebookAddressAsync().ConfigureAwait(false)).ChequebookAddress,
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookBalanceDto> GetChequeBookBalanceAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new ChequeBookBalanceDto(await beeDebugClient_3_0_2.ChequebookBalanceAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookCashoutGetDto> GetChequeBookCashoutForPeerAsync(string peerId) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new ChequeBookCashoutGetDto(await beeDebugClient_3_0_2.ChequebookCashoutGetAsync(peerId).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookChequeGetDto> GetChequeBookChequeForPeerAsync(string peerId) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new ChequeBookChequeGetDto(await beeDebugClient_3_0_2.ChequebookChequeGetAsync(peerId).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> GetChunkAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new MessageResponseDto(await beeDebugClient_3_0_2.ChunksGetAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<BalanceDto> GetConsumedBalanceWithPeerAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new BalanceDto(await beeDebugClient_3_0_2.ConsumedGetAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> GetHealthAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new VersionDto(await beeDebugClient_3_0_2.HealthAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<NodeInfoDto> GetNodeInfoAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new NodeInfoDto(await beeDebugClient_3_0_2.NodeAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PostageBatchDto>> GetPostageBatchesAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.StampsGetAsync().ConfigureAwait(false)).Stamps.Select(i => new PostageBatchDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PendingTransactionDto>> GetPendingTransactionsAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.TransactionsGetAsync().ConfigureAwait(false)).PendingTransactions.Select(i => new PendingTransactionDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<PostageBatchDto> GetPostageBatchAsync(string id) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new PostageBatchDto(await beeDebugClient_3_0_2.StampsGetAsync(id).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> GetReadinessAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new VersionDto(await beeDebugClient_3_0_2.ReadinessAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ReserveStateDto> GetReserveStateAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new ReserveStateDto(await beeDebugClient_3_0_2.ReservestateAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<SettlementDataDto> GetSettlementsWithPeerAsync(string address) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new SettlementDataDto(await beeDebugClient_3_0_2.SettlementsGetAsync(address).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<StampsBucketsDto> GetStampsBucketsForBatchAsync(string batchId) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new StampsBucketsDto(await beeDebugClient_3_0_2.StampsBucketsAsync(batchId).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TopologyDto> GetSwarmTopologyAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new TopologyDto(await beeDebugClient_3_0_2.TopologyAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TagDto> GetTagInfoAsync(long uid) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new TagDto(await beeDebugClient_3_0_2.TagsAsync(uid).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TransactionsDto> GetTransactionInfoAsync(string txHash) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new TransactionsDto(await beeDebugClient_3_0_2.TransactionsGetAsync(txHash).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<WalletDto> GetWalletBalance() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new WalletDto(await beeDebugClient_3_0_2.WalletAsync().ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetWelcomeMessageAsync() =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.WelcomeMessageGetAsync().ConfigureAwait(false)).WelcomeMessage,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> RebroadcastTransactionAsync(string txHash) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.TransactionsPostAsync(txHash).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> SetWelcomeMessageAsync(string welcomeMessage) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new VersionDto(await beeDebugClient_3_0_2.WelcomeMessagePostAsync(
                    new V3_0_2.Body
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
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.StampsTopupAsync(id, amount).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> TryConnectToPeerAsync(string peerId) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.PingpongAsync(peerId).ConfigureAwait(false)).Rtt,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> WithdrawFromChequeBookAsync(
            long amount,
            long? gasPrice = null) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.ChequebookWithdrawAsync(amount, gasPrice).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };
    }
}
