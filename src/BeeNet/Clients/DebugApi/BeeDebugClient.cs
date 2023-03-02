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

using Etherna.BeeNet.Clients.DebugApi.V4_0_0;
using Etherna.BeeNet.DtoModels;
using Etherna.BeeNet.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Clients.DebugApi
{
    public class BeeDebugClient : IBeeDebugClient
    {
        // Fields.
        private readonly IBeeDebugClient_4_0_0 beeDebugClient_4_0_0;

        // Constructors.
        public BeeDebugClient(HttpClient httpClient, Uri baseUrl, DebugApiVersion apiVersion)
        {
            if (baseUrl is null)
                throw new ArgumentNullException(nameof(baseUrl));

            beeDebugClient_4_0_0 = new BeeDebugClient_4_0_0(httpClient) { BaseUrl = baseUrl.ToString() };
            CurrentApiVersion = apiVersion;
        }

        // Properties.
        public DebugApiVersion CurrentApiVersion { get; set; }

        // Methods.
        public async Task<Dictionary<string, AccountDto>> AccountingAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.AccountingAsync(cancellationToken).ConfigureAwait(false)).PeerData.ToDictionary(i => i.Key, i => new AccountDto(i.Value)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> BuyPostageBatchAsync(
            long amount,
            int depth,
            string? label = null,
            bool? immutable = null,
            long? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.StampsPostAsync(amount.ToString(CultureInfo.InvariantCulture), depth, label, immutable, gasPrice, cancellationToken).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> CashoutChequeForPeerAsync(
            string peerId,
            long? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.ChequebookCashoutPostAsync(peerId, gasPrice, gasLimit, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> ConnectToPeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.ConnectAsync(address, cancellationToken).ConfigureAwait(false)).Address,
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeleteChunkAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new MessageResponseDto(await beeDebugClient_4_0_0.ChunksDeleteAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> DeletePeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new MessageResponseDto(await beeDebugClient_4_0_0.PeersDeleteAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DeleteTransactionAsync(
            string txHash,
            long? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.TransactionsDeleteAsync(txHash, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DepositIntoChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.ChequebookDepositAsync(amount, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> DilutePostageBatchAsync(
            string id,
            int depth,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.StampsDiluteAsync(id, depth, cancellationToken).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<AddressDetailDto> GetAddressesAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new AddressDetailDto(await beeDebugClient_4_0_0.AddressesAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetAllBalancesAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.BalancesGetAsync(cancellationToken).ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<ChequeBookChequeGetDto>> GetAllChequeBookChequesAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.ChequebookChequeGetAsync(cancellationToken).ConfigureAwait(false)).Lastcheques.Select(i => new ChequeBookChequeGetDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<BalanceDto>> GetAllConsumedBalancesAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.ConsumedGetAsync(cancellationToken).ConfigureAwait(false)).Balances.Select(i => new BalanceDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetAllPeerAddressesAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.PeersGetAsync(cancellationToken).ConfigureAwait(false)).Peers.Select(i => i.Address),
                _ => throw new InvalidOperationException()
            };

        public async Task<SettlementDto> GetAllSettlementsAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new SettlementDto(await beeDebugClient_4_0_0.SettlementsGetAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TimeSettlementsDto> GetAllTimeSettlementsAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new TimeSettlementsDto(await beeDebugClient_4_0_0.TimesettlementsAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PostageBatchShortDto>> GetAllValidPostageBatchesFromAllNodesAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.BatchesAsync(cancellationToken).ConfigureAwait(false)).Batches.Select(i => new PostageBatchShortDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<BalanceDto> GetBalanceWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new BalanceDto(await beeDebugClient_4_0_0.BalancesGetAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<string>> GetBlocklistedPeerAddressesAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.BlocklistAsync(cancellationToken).ConfigureAwait(false)).Peers.Select(i => i.Address),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChainStateDto> GetChainStateAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new ChainStateDto(await beeDebugClient_4_0_0.ChainstateAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetChequeBookAddressAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.ChequebookAddressAsync(cancellationToken).ConfigureAwait(false)).ChequebookAddress,
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookBalanceDto> GetChequeBookBalanceAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new ChequeBookBalanceDto(await beeDebugClient_4_0_0.ChequebookBalanceAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookCashoutGetDto> GetChequeBookCashoutForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new ChequeBookCashoutGetDto(await beeDebugClient_4_0_0.ChequebookCashoutGetAsync(peerId, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<ChequeBookChequeGetDto> GetChequeBookChequeForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new ChequeBookChequeGetDto(await beeDebugClient_4_0_0.ChequebookChequeGetAsync(peerId, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<MessageResponseDto> GetChunkAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new MessageResponseDto(await beeDebugClient_4_0_0.ChunksGetAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<BalanceDto> GetConsumedBalanceWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new BalanceDto(await beeDebugClient_4_0_0.ConsumedGetAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<VersionDto> GetHealthAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new VersionDto(await beeDebugClient_4_0_0.HealthAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<NodeInfoDto> GetNodeInfoAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new NodeInfoDto(await beeDebugClient_4_0_0.NodeAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PostageBatchDto>> GetOwnedPostageBatchesByNodeAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.StampsGetAllAsync(null, cancellationToken).ConfigureAwait(false)).Stamps.Select(i => new PostageBatchDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<IEnumerable<PendingTransactionDto>> GetPendingTransactionsAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.TransactionsGetAsync(cancellationToken).ConfigureAwait(false)).PendingTransactions.Select(i => new PendingTransactionDto(i)),
                _ => throw new InvalidOperationException()
            };

        public async Task<PostageBatchDto> GetPostageBatchAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new PostageBatchDto(await beeDebugClient_4_0_0.StampsGetAsync(id, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<bool> GetReadinessAsync(CancellationToken cancellationToken = default)
        {
            switch (CurrentApiVersion)
            {
                case DebugApiVersion.v4_0_0:
                    try
                    {
                        await beeDebugClient_4_0_0.ReadinessAsync(cancellationToken).ConfigureAwait(false);
                        return true;
                    }
                    catch (BeeNetDebugApiException e) when (e.StatusCode == 400)
                    {
                        return false;
                    }
                default: throw new InvalidOperationException();
            }
        }

        public async Task<ReserveStateDto> GetReserveStateAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new ReserveStateDto(await beeDebugClient_4_0_0.ReservestateAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<SettlementDataDto> GetSettlementsWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new SettlementDataDto(await beeDebugClient_4_0_0.SettlementsGetAsync(address, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<StampsBucketsDto> GetStampsBucketsForBatchAsync(
            string batchId,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new StampsBucketsDto(await beeDebugClient_4_0_0.StampsBucketsAsync(batchId, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TopologyDto> GetSwarmTopologyAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new TopologyDto(await beeDebugClient_4_0_0.TopologyAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TagDto> GetTagInfoAsync(
            long uid,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new TagDto(await beeDebugClient_4_0_0.TagsAsync(uid, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<TransactionsDto> GetTransactionInfoAsync(
            string txHash,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new TransactionsDto(await beeDebugClient_4_0_0.TransactionsGetAsync(txHash, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<WalletDto> GetWalletBalance(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new WalletDto(await beeDebugClient_4_0_0.WalletAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.WelcomeMessageGetAsync(cancellationToken).ConfigureAwait(false)).WelcomeMessage,
                _ => throw new InvalidOperationException()
            };

        public async Task<LogDataDto> LoggersGetAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new LogDataDto(await beeDebugClient_4_0_0.LoggersGetAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task<LogDataDto> LoggersGetAsync(string exp, CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new LogDataDto(await beeDebugClient_4_0_0.LoggersGetAsync(exp, cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task LoggersPutAsync(string exp, CancellationToken cancellationToken = default)
        {
            if (CurrentApiVersion == DebugApiVersion.v4_0_0)
                await beeDebugClient_4_0_0.LoggersPutAsync(exp, cancellationToken).ConfigureAwait(false);
            else
                throw new InvalidOperationException();
        }

        public async Task<string> RebroadcastTransactionAsync(
            string txHash,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.TransactionsPostAsync(txHash, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };

        public async Task<RedistributionStateDto> RedistributionStateAsync(CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new RedistributionStateDto(await beeDebugClient_4_0_0.RedistributionstateAsync(cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };
        
        public async Task<VersionDto> SetWelcomeMessageAsync(
            string welcomeMessage,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => new VersionDto(await beeDebugClient_4_0_0.WelcomeMessagePostAsync(
                    new V4_0_0.Body
                    {
                        WelcomeMessage = welcomeMessage
                    },
                    cancellationToken).ConfigureAwait(false)),
                _ => throw new InvalidOperationException()
            };

        public async Task StakeDeleteAsync(long? gas_price = null, long? gas_limit = null, CancellationToken cancellationToken = default)
        {
            if (CurrentApiVersion == DebugApiVersion.v4_0_0)
                await beeDebugClient_4_0_0.StakeDeleteAsync(gas_price, gas_limit, cancellationToken).ConfigureAwait(false);
            else
                throw new InvalidOperationException();
        }

        public async Task StakeGetAsync(CancellationToken cancellationToken = default)
        {
            if (CurrentApiVersion == DebugApiVersion.v4_0_0)
                await beeDebugClient_4_0_0.StakeGetAsync(cancellationToken).ConfigureAwait(false);
            else
                throw new InvalidOperationException();
        }

        public async Task StakePostAsync(string? amount = null, long? gas_price = null, long? gas_limit = null, CancellationToken cancellationToken = default)
        {
            if (CurrentApiVersion == DebugApiVersion.v4_0_0)
                await beeDebugClient_4_0_0.StakePostAsync(amount, gas_price, gas_limit, cancellationToken).ConfigureAwait(false);
            else
                throw new InvalidOperationException();
        }

        public async Task<string> TopUpPostageBatchAsync(
            string id,
            long amount,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.StampsTopupAsync(id, amount, cancellationToken).ConfigureAwait(false)).BatchID,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> TryConnectToPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.PingpongAsync(peerId, cancellationToken).ConfigureAwait(false)).Rtt,
                _ => throw new InvalidOperationException()
            };

        public async Task<string> WithdrawFromChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            CurrentApiVersion switch
            {
                DebugApiVersion.v4_0_0 => (await beeDebugClient_4_0_0.ChequebookWithdrawAsync(amount, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash,
                _ => throw new InvalidOperationException()
            };
    }
}
