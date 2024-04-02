//   Copyright 2021-present Etherna SA
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

using Etherna.BeeNet.Exceptions;
using Etherna.BeeNet.Models;
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
        private readonly BeeDebugGeneratedClient generatedClient;

        // Constructors.
        public BeeDebugClient(Uri baseUrl, HttpClient httpClient)
        {
            ArgumentNullException.ThrowIfNull(baseUrl, nameof(baseUrl));

            generatedClient = new BeeDebugGeneratedClient(httpClient) { BaseUrl = baseUrl.ToString() };
        }

        // Methods.
        public async Task<Dictionary<string, Account>> AccountingAsync(
            CancellationToken cancellationToken = default) =>
            (await generatedClient.AccountingAsync(cancellationToken).ConfigureAwait(false)).PeerData.ToDictionary(i => i.Key, i => new Account(i.Value));

        public async Task<string> BuyPostageBatchAsync(
            long amount,
            int depth,
            string? label = null,
            bool? immutable = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.StampsPostAsync(amount.ToString(CultureInfo.InvariantCulture), depth, label, immutable, cancellationToken).ConfigureAwait(false)).BatchID;

        public async Task<string> CashoutChequeForPeerAsync(
            string peerId,
            long? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookCashoutPostAsync(peerId, gasPrice, gasLimit, cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<string> ConnectToPeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ConnectAsync(address, cancellationToken).ConfigureAwait(false)).Address;

        public async Task<MessageResponse> DeletePeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.PeersDeleteAsync(address, cancellationToken).ConfigureAwait(false));

        public async Task<string> DeleteTransactionAsync(
            string txHash,
            long? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.TransactionsDeleteAsync(txHash, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<string> DepositIntoChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookDepositAsync(amount, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<string> DilutePostageBatchAsync(
            string id,
            int depth,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.StampsDiluteAsync(id, depth, cancellationToken).ConfigureAwait(false)).BatchID;

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

        public async Task<Settlement> GetAllSettlementsAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.SettlementsGetAsync(cancellationToken).ConfigureAwait(false));

        public async Task<Settlement> GetAllTimeSettlementsAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.TimesettlementsAsync(cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<PostageBatchShort>> GetAllValidPostageBatchesFromAllNodesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.BatchesAsync(cancellationToken).ConfigureAwait(false)).Batches.Select(i => new PostageBatchShort(i));

        public async Task<PeerBalance> GetBalanceWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.BalancesGetAsync(address, cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<string>> GetBlocklistedPeerAddressesAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.BlocklistAsync(cancellationToken).ConfigureAwait(false)).Peers.Select(i => i.Address);

        public async Task<ChainState> GetChainStateAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.ChainstateAsync(cancellationToken).ConfigureAwait(false));

        public async Task<string> GetChequeBookAddressAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookAddressAsync(cancellationToken).ConfigureAwait(false)).ChequebookAddress;

        public async Task<ChequebookBalance> GetChequeBookBalanceAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.ChequebookBalanceAsync(cancellationToken).ConfigureAwait(false));

        public async Task<ChequebookCashoutGet> GetChequeBookCashoutForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.ChequebookCashoutGetAsync(peerId, cancellationToken).ConfigureAwait(false));

        public async Task<ChequebookChequeGet> GetChequeBookChequeForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.ChequebookChequeGetAsync(peerId, cancellationToken).ConfigureAwait(false));

        public async Task<MessageResponse> GetChunkAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.ChunksAsync(address, cancellationToken).ConfigureAwait(false));

        public async Task<PeerBalance> GetConsumedBalanceWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.ConsumedGetAsync(address, cancellationToken).ConfigureAwait(false));

        public async Task<Health> GetHealthAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.HealthAsync(cancellationToken).ConfigureAwait(false));

        public async Task<NodeInfo> GetNodeInfoAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.NodeAsync(cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<PostageBatch>> GetOwnedPostageBatchesByNodeAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.StampsGetAsync(cancellationToken).ConfigureAwait(false)).Stamps.Select(i => new PostageBatch(i));

        public async Task<IEnumerable<PendingTransaction>> GetPendingTransactionsAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.TransactionsGetAsync(cancellationToken).ConfigureAwait(false)).PendingTransactions.Select(i => new PendingTransaction(i));

        public async Task<PostageBatch> GetPostageBatchAsync(
            string id,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.StampsGetAsync(id, cancellationToken).ConfigureAwait(false));

        public async Task<bool> GetReadinessAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await generatedClient.ReadinessAsync(cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (BeeNetDebugApiException e) when (e.StatusCode == 400)
            {
                return false;
            }
        }

        public async Task<ReserveState> GetReserveStateAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.ReservestateAsync(cancellationToken).ConfigureAwait(false));

        public async Task<SettlementData> GetSettlementsWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.SettlementsGetAsync(address, cancellationToken).ConfigureAwait(false));

        public async Task<StampsBuckets> GetStampsBucketsForBatchAsync(
            string batchId,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.StampsBucketsAsync(batchId, cancellationToken).ConfigureAwait(false));

        public async Task<Topology> GetSwarmTopologyAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.TopologyAsync(cancellationToken).ConfigureAwait(false));

        public async Task<TransactionInfo> GetTransactionInfoAsync(
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
        
        public async Task<Health> SetWelcomeMessageAsync(
            string welcomeMessage,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.WelcomeMessagePostAsync(
                new Body
                {
                    WelcomeMessage = welcomeMessage
                },
                cancellationToken).ConfigureAwait(false));

        public async Task StakeDeleteAsync(long? gas_price = null, long? gas_limit = null, CancellationToken cancellationToken = default) =>
            await generatedClient.StakeDeleteAsync(gas_price, gas_limit, cancellationToken).ConfigureAwait(false);

        public async Task StakeGetAsync(CancellationToken cancellationToken = default) =>
            await generatedClient.StakeGetAsync(cancellationToken).ConfigureAwait(false);

        public async Task StakePostAsync(string? amount = null, long? gas_price = null, long? gas_limit = null, CancellationToken cancellationToken = default) =>
            await generatedClient.StakePostAsync(amount, gas_price, gas_limit, cancellationToken).ConfigureAwait(false);

        public async Task<StatusNode> StatusNodeAsync(CancellationToken cancellationToken = default) =>
            new(await generatedClient.StatusAsync(cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<StatusNode>> StatusPeersAsync(CancellationToken cancellationToken = default) =>
            (await generatedClient.StatusPeersAsync(cancellationToken).ConfigureAwait(false)).Stamps.Select(p => new StatusNode(p));

        public async Task<string> TopUpPostageBatchAsync(
            string id,
            long amount,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.StampsTopupAsync(id, amount, cancellationToken).ConfigureAwait(false)).BatchID;

        public async Task<string> TryConnectToPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.PingpongAsync(peerId, cancellationToken).ConfigureAwait(false)).Rtt;

        public async Task<string> WalletWithdrawAsync(
            long amount,
            string address,
            string coin,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.WalletWithdrawAsync(amount.ToString(CultureInfo.InvariantCulture), address, coin, cancellationToken).ConfigureAwait(false)).TransactionHash;

        public async Task<string> WithdrawFromChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.ChequebookWithdrawAsync(amount, gasPrice, cancellationToken).ConfigureAwait(false)).TransactionHash;
    }
}
