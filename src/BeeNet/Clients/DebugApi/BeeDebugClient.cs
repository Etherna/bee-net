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
using System.Text;
using System.Threading;
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
            long? gasPrice = null, 
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.StampsPostAsync(amount.ToString(CultureInfo.InvariantCulture), depth, label, immutable, gasPrice, cancellationToken).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> CashoutChequeForPeerAsync(
            string peerId,
            long? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.ChequebookCashoutPostAsync(peerId, gasPrice, gasLimit, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> ConnectToPeerAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.ConnectAsync(address, cancellationToken).ConfigureAwait(false)).Address,
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeleteChunkAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new MessageResponseDto(await beeDebugClient_3_0_2.ChunksDeleteAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeletePeerAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new MessageResponseDto(await beeDebugClient_3_0_2.PeersDeleteAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DeleteTransactionAsync(
            string txHash,
            long? gasPrice = null,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.TransactionsDeleteAsync(txHash, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DepositIntoChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.ChequebookDepositAsync(amount, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DilutePostageBatchAsync(
            string id,
            int depth,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.StampsDiluteAsync(id, depth, cancellationToken).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<AddressDetailDto> GetAddressesAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new AddressDetailDto(await beeDebugClient_3_0_2.AddressesAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetAllBalancesAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.BalancesGetAsync(cancellationToken).ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<ChequeBookChequeGetDto>> GetAllChequeBookChequesAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.ChequebookChequeGetAsync(cancellationToken).ConfigureAwait(false)).Lastcheques.Select(i => new ChequeBookChequeGetDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetAllConsumedBalancesAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.ConsumedGetAsync(cancellationToken).ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetAllPeerAddressesAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.PeersGetAsync(cancellationToken).ConfigureAwait(false)).Peers.Select(i => i.Address),
                _ => throw new InvalidOperationException()
            };

        public async Task<SettlementDto> GetAllSettlementsAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new SettlementDto(await beeDebugClient_3_0_2.SettlementsGetAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TimeSettlementsDto> GetAllTimeSettlementsAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new TimeSettlementsDto(await beeDebugClient_3_0_2.TimesettlementsAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PostageBatchShortDto>> GetAllValidPostageBatchesFromAllNodesAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.BatchesAsync(cancellationToken).ConfigureAwait(false)).Batches.Select(i => new PostageBatchShortDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<BalanceDto> GetBalanceWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new BalanceDto(await beeDebugClient_3_0_2.BalancesGetAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetBlocklistedPeerAddressesAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.BlocklistAsync(cancellationToken).ConfigureAwait(false)).Peers.Select(i => i.Address),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChainStateDto> GetChainStateAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new ChainStateDto(await beeDebugClient_3_0_2.ChainstateAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetChequeBookAddressAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.ChequebookAddressAsync(cancellationToken).ConfigureAwait(false)).ChequebookAddress,
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookBalanceDto> GetChequeBookBalanceAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new ChequeBookBalanceDto(await beeDebugClient_3_0_2.ChequebookBalanceAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookCashoutGetDto> GetChequeBookCashoutForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new ChequeBookCashoutGetDto(await beeDebugClient_3_0_2.ChequebookCashoutGetAsync(peerId, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookChequeGetDto> GetChequeBookChequeForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new ChequeBookChequeGetDto(await beeDebugClient_3_0_2.ChequebookChequeGetAsync(peerId, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> GetChunkAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new MessageResponseDto(await beeDebugClient_3_0_2.ChunksGetAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<BalanceDto> GetConsumedBalanceWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new BalanceDto(await beeDebugClient_3_0_2.ConsumedGetAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> GetHealthAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new VersionDto(await beeDebugClient_3_0_2.HealthAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<NodeInfoDto> GetNodeInfoAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new NodeInfoDto(await beeDebugClient_3_0_2.NodeAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PostageBatchDto>> GetOwnedPostageBatchesByNodeAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.StampsGetAsync(cancellationToken).ConfigureAwait(false)).Stamps.Select(i => new PostageBatchDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PendingTransactionDto>> GetPendingTransactionsAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.TransactionsGetAsync(cancellationToken).ConfigureAwait(false)).PendingTransactions.Select(i => new PendingTransactionDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<PostageBatchDto> GetPostageBatchAsync(
            string id,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new PostageBatchDto(await beeDebugClient_3_0_2.StampsGetAsync(id, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> GetReadinessAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new VersionDto(await beeDebugClient_3_0_2.ReadinessAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ReserveStateDto> GetReserveStateAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new ReserveStateDto(await beeDebugClient_3_0_2.ReservestateAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<SettlementDataDto> GetSettlementsWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new SettlementDataDto(await beeDebugClient_3_0_2.SettlementsGetAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<StampsBucketsDto> GetStampsBucketsForBatchAsync(
            string batchId,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new StampsBucketsDto(await beeDebugClient_3_0_2.StampsBucketsAsync(batchId, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TopologyDto> GetSwarmTopologyAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new TopologyDto(await beeDebugClient_3_0_2.TopologyAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TagDto> GetTagInfoAsync(
            long uid,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new TagDto(await beeDebugClient_3_0_2.TagsAsync(uid, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TransactionsDto> GetTransactionInfoAsync(
            string txHash,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new TransactionsDto(await beeDebugClient_3_0_2.TransactionsGetAsync(txHash, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<WalletDto> GetWalletBalance(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new WalletDto(await beeDebugClient_3_0_2.WalletAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.WelcomeMessageGetAsync(cancellationToken).ConfigureAwait(false)).WelcomeMessage,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> RebroadcastTransactionAsync(
            string txHash,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.TransactionsPostAsync(txHash, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> SetWelcomeMessageAsync(
            string welcomeMessage,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => new VersionDto(await beeDebugClient_3_0_2.WelcomeMessagePostAsync(
                    new V3_0_2.Body
                    {
                        WelcomeMessage = welcomeMessage
                    },
                    cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> TopUpPostageBatchAsync(
            string id,
            long amount,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.StampsTopupAsync(id, amount, cancellationToken).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> TryConnectToPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.PingpongAsync(peerId, cancellationToken).ConfigureAwait(false)).Rtt,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> WithdrawFromChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default(CancellationToken)) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v3_0_2 => (await beeDebugClient_3_0_2.ChequebookWithdrawAsync(amount, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };
    }
}
