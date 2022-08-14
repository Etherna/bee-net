using Etherna.BeeNet.DtoModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Clients.DebugApi
{
    public interface IBeeDebugClient
    {
        // Properties.
        DebugApiVersion CurrentApiVersion { get; set; }

        // Methods.
        /// <summary>Buy a new postage batch.</summary>
        /// <param name="amount">Amount of BZZ added that the postage batch will have.</param>
        /// <param name="depth">Batch depth which specifies how many chunks can be signed with the batch. It is a logarithm. Must be higher than default bucket depth (16)</param>
        /// <param name="label">An optional label for this batch</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Returns the newly created postage batch ID</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<string> BuyPostageBatchAsync(
            long amount,
            int depth,
            string? label = null,
            bool? immutable = null,
            long? gasPrice = null,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Cashout the last cheque for the peer</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <param name="gasLimit">Gas limit for transaction</param>
        /// <returns>Hash of the transaction</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<string> CashoutChequeForPeerAsync(
            string peerId,
            long? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Connect to address</summary>
        /// <param name="address">Underlay address of peer</param>
        /// <returns>Returns overlay address of connected peer</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<string> ConnectToPeerAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Delete a chunk from local storage</summary>
        /// <param name="address">Swarm address of chunk</param>
        /// <returns>Chunk exists</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<MessageResponseDto> DeleteChunkAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Remove peer</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Disconnected peer</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<MessageResponseDto> DeletePeerAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Cancel existing transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Hash of the transaction</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<string> DeleteTransactionAsync(
            string txHash,
            long? gasPrice = null,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Deposit tokens from overlay address into chequebook</summary>
        /// <param name="amount">amount of tokens to deposit</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Transaction hash of the deposit transaction</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<string> DepositIntoChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Dilute an existing postage batch.</summary>
        /// <param name="id">Batch ID to dilute</param>
        /// <param name="depth">New batch depth. Must be higher than the previous depth.</param>
        /// <returns>Returns the postage batch ID that was diluted.</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<string> DilutePostageBatchAsync(
            string id, 
            int depth,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get overlay and underlay addresses of the node</summary>
        /// <returns>Own node underlay and overlay addresses</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<AddressDetailDto> GetAddressesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get the balances with all known peers including prepaid services</summary>
        /// <returns>Own balances with all known peers</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<IEnumerable<BalanceDto>> GetAllBalancesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get last cheques for all peers</summary>
        /// <returns>Last cheques</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<IEnumerable<ChequeBookChequeGetDto>> GetAllChequeBookChequesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get the past due consumption balances with all known peers</summary>
        /// <returns>Own past due consumption balances with all known peers</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<IEnumerable<BalanceDto>> GetAllConsumedBalancesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get a list of peers</summary>
        /// <returns>Returns overlay addresses of connected peers</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<IEnumerable<string>> GetAllPeerAddressesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get settlements with all known peers and total amount sent or received</summary>
        /// <returns>Settlements with all known peers and total amount sent or received</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<SettlementDto> GetAllSettlementsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get time based settlements with all known peers and total amount sent or received</summary>
        /// <returns>Time based settlements with all known peers and total amount sent or received</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<TimeSettlementsDto> GetAllTimeSettlementsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get all globally available batches that were purchased by all nodes.
        /// </summary>
        /// <returns></returns>
        /// <returns>Returns an array of all available and currently valid postage batches.</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<IEnumerable<PostageBatchShortDto>> GetAllValidPostageBatchesFromAllNodesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get the balances with a specific peer including prepaid services</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Balance with the specific peer</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<BalanceDto> GetBalanceWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get a list of blocklisted peers</summary>
        /// <returns>Returns overlay addresses of blocklisted peers</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<IEnumerable<string>> GetBlocklistedPeerAddressesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get chain state</summary>
        /// <returns>Chain State</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<ChainStateDto> GetChainStateAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get the address of the chequebook contract used</summary>
        /// <returns>Ethereum address of chequebook contract</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<string> GetChequeBookAddressAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get the balance of the chequebook</summary>
        /// <returns>Balance of the chequebook</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<ChequeBookBalanceDto> GetChequeBookBalanceAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get last cashout action for the peer</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <returns>Cashout status</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<ChequeBookCashoutGetDto> GetChequeBookCashoutForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get last cheques for the peer</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <returns>Last cheques</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<ChequeBookChequeGetDto> GetChequeBookChequeForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Check if chunk at address exists locally</summary>
        /// <param name="address">Swarm address of chunk</param>
        /// <returns>Chunk exists</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<MessageResponseDto> GetChunkAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get the past due consumption balance with a specific peer</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Past-due consumption balance with the specific peer</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<BalanceDto> GetConsumedBalanceWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get health of node</summary>
        /// <returns>Health State of node</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<VersionDto> GetHealthAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get information about the node
        /// </summary>
        /// <returns>Information about the node</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<NodeInfoDto> GetNodeInfoAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get all owned postage batches by this node</summary>
        /// <returns>List of all owned postage batches</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred</exception>
        Task<IEnumerable<PostageBatchDto>> GetOwnedPostageBatchesByNodeAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get list of pending transactions</summary>
        /// <returns>List of pending transactions</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<IEnumerable<PendingTransactionDto>> GetPendingTransactionsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get an individual postage batch status</summary>
        /// <param name="id">Swarm address of the stamp</param>
        /// <returns>Returns an individual postage batch state</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<PostageBatchDto> GetPostageBatchAsync(
            string id,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get readiness state of node</summary>
        /// <returns>Health State of node</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<VersionDto> GetReadinessAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get reserve state</summary>
        /// <returns>Reserve State</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<ReserveStateDto> GetReserveStateAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get amount of sent and received from settlements with a peer</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Amount of sent or received from settlements with a peer</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<IEnumerable<SettlementDataDto>> GetSettlementsWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get extended bucket data of a batch</summary>
        /// <param name="batchId">Swarm address of the stamp</param>
        /// <returns>Returns extended bucket data of the provided batch ID</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<StampsBucketsDto> GetStampsBucketsForBatchAsync(
            string batchId,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <returns>Swarm topology of the bee node</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<TopologyDto> GetSwarmTopologyAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get Tag information using Uid</summary>
        /// <param name="uid">Uid</param>
        /// <returns>Tag info</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<TagDto> GetTagInfoAsync(
            long uid,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get information about a sent transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <returns>Get info about transaction</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<TransactionsDto> GetTransactionInfoAsync(
            string txHash,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get wallet balance for BZZ and xDai
        /// </summary>
        /// <returns>Wallet balance info</returns>
        Task<WalletDto> GetWalletBalance(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Get configured P2P welcome message</summary>
        /// <returns>Welcome message</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Rebroadcast existing transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <returns>Hash of the transaction</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<string> RebroadcastTransactionAsync(
            string txHash,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Set P2P welcome message</summary>
        /// <returns>OK</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<VersionDto> SetWelcomeMessageAsync(string welcomeMessage,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Top up an existing postage batch.</summary>
        /// <param name="id">Batch ID to top up</param>
        /// <param name="amount">Amount of BZZ per chunk to top up to an existing postage batch.</param>
        /// <returns>Returns the postage batch ID that was topped up</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<string> TopUpPostageBatchAsync(
            string id, 
            long amount,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Try connection to node</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <returns>Returns round trip time for given peer</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<string> TryConnectToPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>Withdraw tokens from the chequebook to the overlay address</summary>
        /// <param name="amount">amount of tokens to withdraw</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Transaction hash of the withdraw transaction</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<string> WithdrawFromChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}