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

using Etherna.BeeNet.DtoModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Clients.v1_4_1.DebugApi
{
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Version number should containt underscores")]
    public class AdapterBeeDebugVersion_1_4_1 : IBeeDebugClient
    {
        // Fields.
        private readonly IBeeDebugClient_1_4_1 beeDebugClient;

        // Constructors.
        public AdapterBeeDebugVersion_1_4_1(HttpClient httpClient, Uri baseUrl)
        {
            if (baseUrl is null)
                throw new ArgumentNullException(nameof(baseUrl));

            beeDebugClient = new BeeDebugClient_1_4_1(httpClient) { BaseUrl = baseUrl.ToString() };
        }

        // Methods.
        public async Task<BatchDto> BuyPostageBatchAsync(
            int amount,
            int depth,
            string? label = null,
            bool? immutable = null,
            int? gasPrice = null)
        {
            var response = await beeDebugClient.StampsPostAsync(amount, depth, label, immutable, gasPrice).ConfigureAwait(false);

            return new BatchDto(response);
        }

        public async Task<TransactionHashDto> CashoutChequeForPeerAsync(
            string peerId,
            int? gasPrice = null,
            long? gasLimit = null)
        {
            var response = await beeDebugClient.ChequebookCashoutPostAsync(peerId, gasPrice, gasLimit).ConfigureAwait(false);

            return new TransactionHashDto(response);
        }

        public async Task<ConnectDto> ConnectAsync(string address)
        {
            var response = await beeDebugClient.ConnectAsync(address).ConfigureAwait(false);

            return new ConnectDto(response);
        }

        public async Task<MessageResponseDto> DeleteChunkAsync(string address)
        {
            var response = await beeDebugClient.ChunksDeleteAsync(address).ConfigureAwait(false);

            return new MessageResponseDto(response);
        }

        public async Task<MessageResponseDto> DeletePeerAsync(string address)
        {
            var response = await beeDebugClient.PeersDeleteAsync(address).ConfigureAwait(false);

            return new MessageResponseDto(response);
        }

        public async Task<TransactionHashDto> DeleteTransactionAsync(
            string txHash,
            int? gasPrice = null)
        {
            var response = await beeDebugClient.TransactionsDeleteAsync(txHash).ConfigureAwait(false);

            return new TransactionHashDto(response);
        }

        public async Task<TransactionHashDto> DepositIntoChequeBookAsync(
            int amount,
            int? gasPrice = null)
        {
            var response = await beeDebugClient.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false);

            return new TransactionHashDto(response);
        }

        public async Task<BatchDto> DilutePostageBatchAsync(
            object id,
            int depth)
        {
            var response = await beeDebugClient.StampsDiluteAsync(id, depth).ConfigureAwait(false);

            return new BatchDto(response);
        }

        public async Task<AddressDetailDto> GetAddressesAsync()
        {
            var response = await beeDebugClient.AddressesAsync().ConfigureAwait(false);

            return new AddressDetailDto(response);
        }

        public async Task<IEnumerable<BalanceDto>> GetAllBalancesAsync()
        {
            var response = await beeDebugClient.BalancesGetAsync().ConfigureAwait(false);

            return response.Balances
                .Select(i => new BalanceDto(i));
        }

        public async Task<IEnumerable<ChequeBookChequeGetDto>> GetAllChequeBookChequesAsync()
        {
            var response = await beeDebugClient.ChequebookChequeGetAsync().ConfigureAwait(false);

            return response.Lastcheques
                .Select(i => new ChequeBookChequeGetDto(i));
        }

        public async Task<IEnumerable<BalanceDto>> GetAllConsumedBalancesAsync()
        {
            var response = await beeDebugClient.ConsumedGetAsync().ConfigureAwait(false);

            return response.Balances
                .Select(i => new BalanceDto(i));
        }

        public async Task<IEnumerable<AddressDto>> GetAllPeersAsync()
        {
            var response = await beeDebugClient.PeersGetAsync().ConfigureAwait(false);

            return response.Peers
                .Select(i => new AddressDto(i));
        }

        public async Task<SettlementDto> GetAllSettlementsAsync()
        {
            var response = await beeDebugClient.SettlementsGetAsync().ConfigureAwait(false);

            return new SettlementDto(response);
        }

        public async Task<IEnumerable<StampsGetDto>> GetAllStampsAsync()
        {
            var response = await beeDebugClient.StampsGetAsync().ConfigureAwait(false);

            return response.Stamps
                .Select(i => new StampsGetDto(i));
        }

        public async Task<TimeSettlementsDto> GetAllTimeSettlementsAsync()
        {
            var response = await beeDebugClient.TimesettlementsAsync().ConfigureAwait(false);

            return new TimeSettlementsDto(response);
        }

        public async Task<IEnumerable<BalanceDto>> GetBalanceWithPeerAsync(string address)
        {
            var response = await beeDebugClient.BalancesGetAsync().ConfigureAwait(false);

            return response.Balances
                .Select(i => new BalanceDto(i));
        }

        public async Task<IEnumerable<AddressDto>> GetBlocklistedPeersAsync()
        {
            var response = await beeDebugClient.BlocklistAsync().ConfigureAwait(false);

            return response.Peers
                .Select(i => new AddressDto(i));
        }

        public async Task<ChainStateDto> GetChainStateAsync()
        {
            var response = await beeDebugClient.ChainstateAsync().ConfigureAwait(false);

            return new ChainStateDto(response);
        }

        public async Task<ChequeBookAddressDto> GetChequeBookAddressAsync()
        {
            var response = await beeDebugClient.ChequebookAddressAsync().ConfigureAwait(false);

            return new ChequeBookAddressDto(response);
        }

        public async Task<ChequeBookBalanceDto> GetChequeBookBalanceAsync()
        {
            var response = await beeDebugClient.ChequebookBalanceAsync().ConfigureAwait(false);

            return new ChequeBookBalanceDto(response);
        }

        public async Task<ChequeBookCashoutGetDto> GetChequeBookCashoutForPeerAsync(string peerId)
        {
            var response = await beeDebugClient.ChequebookCashoutGetAsync(peerId).ConfigureAwait(false);

            return new ChequeBookCashoutGetDto(response);
        }

        public async Task<ChequeBookChequeGetDto> GetChequeBookChequeForPeerAsync(string peerId)
        {
            var response = await beeDebugClient.ChequebookChequeGetAsync(peerId).ConfigureAwait(false);

            return new ChequeBookChequeGetDto(response);
        }

        public async Task<MessageResponseDto> GetChunkAsync(string address)
        {
            var response = await beeDebugClient.ChunksGetAsync(address).ConfigureAwait(false);

            return new MessageResponseDto(response);
        }

        public async Task<IEnumerable<BalanceDto>> GetConsumedBalanceWithPeerAsync(string address)
        {
            var response = await beeDebugClient.ConsumedGetAsync().ConfigureAwait(false);

            return response.Balances
                .Select(i => new BalanceDto(i));
        }

        public async Task<VersionDto> GetHealthAsync()
        {
            var response = await beeDebugClient.HealthAsync().ConfigureAwait(false);

            return new VersionDto(response);
        }

        public async Task<IEnumerable<PendingTransactionDto>> GetPendingTransactionsAsync()
        {
            var response = await beeDebugClient.TransactionsGetAsync().ConfigureAwait(false);

            return response.PendingTransactions
                .Select(i => new PendingTransactionDto(i));
        }

        public async Task<StampsGetDto> GetPostageBatchStatusAsync(object id)
        {
            var response = await beeDebugClient.StampsGetAsync(id).ConfigureAwait(false);

            return new StampsGetDto(response);
        }

        public async Task<VersionDto> GetReadinessAsync()
        {
            var response = await beeDebugClient.ReadinessAsync().ConfigureAwait(false);

            return new VersionDto(response);
        }

        public async Task<ReserveStateDto> GetReserveStateAsync()
        {
            var response = await beeDebugClient.ReservestateAsync().ConfigureAwait(false);

            return new ReserveStateDto(response);
        }

        public async Task<IEnumerable<SettlementDataDto>> GetSettlementsWithPeerAsync(string address)
        {
            var response = await beeDebugClient.SettlementsGetAsync().ConfigureAwait(false);

            return response.Settlements
                .Select(i => new SettlementDataDto(i));
        }

        public async Task<StampsBucketsDto> GetStampsBucketsForBatchAsync(object batchId)
        {
            var response = await beeDebugClient.StampsBucketsAsync(batchId).ConfigureAwait(false);

            return new StampsBucketsDto(response);
        }

        public async Task<TopologyDto> GetSwarmTopologyAsync()
        {
            var response = await beeDebugClient.TopologyAsync().ConfigureAwait(false);

            return new TopologyDto(response);
        }

        public async Task<TagDto> GetTagInfoAsync(int uid)
        {
            var response = await beeDebugClient.TagsAsync(uid).ConfigureAwait(false);

            return new TagDto(response);
        }

        public async Task<TransactionsDto> GetTransactionInfoAsync(string txHash)
        {
            var response = await beeDebugClient.TransactionsGetAsync(txHash).ConfigureAwait(false);

            return new TransactionsDto(response);
        }

        public async Task<string> GetWelcomeMessageAsync()
        {
            var response = await beeDebugClient.WelcomeMessageGetAsync().ConfigureAwait(false);

            return response.WelcomeMessage;
        }

        public async Task<TransactionHashDto> RebroadcastTransactionAsync(string txHash)
        {
            var response = await beeDebugClient.TransactionsPostAsync(txHash).ConfigureAwait(false);

            return new TransactionHashDto(response);
        }

        public async Task<VersionDto> SetWelcomeMessageAsync(string welcomeMessage)
        {
            var response = await beeDebugClient.WelcomeMessagePostAsync(
                new Body
                {
                    WelcomeMessage = welcomeMessage
                }).ConfigureAwait(false);

            return new VersionDto(response);
        }

        public async Task<BatchDto> TopUpPostageBatchAsync(
            object id,
            int amount)
        {
            var response = await beeDebugClient.StampsTopupAsync(id, amount).ConfigureAwait(false);

            return new BatchDto(response);
        }

        public async Task<PingPongDto> TryConnectToPeerAsync(string peerId)
        {
            var response = await beeDebugClient.PingpongAsync(peerId).ConfigureAwait(false);

            return new PingPongDto(response);
        }

        public async Task<TransactionHashDto> WithdrawFromChequeBookAsync(
            int amount,
            int? gasPrice = null)
        {
            var response = await beeDebugClient.ChequebookDepositAsync(amount, gasPrice).ConfigureAwait(false);

            return new TransactionHashDto(response);
        }
    }
}