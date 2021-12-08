using System.Collections.Generic;
using Etherna.BeeNet.Clients.v_1_4.DebugApi;
using Etherna.BeeNet.DtoModel;

namespace Etherna.BeeNet.Clients
{
    public interface IBeeDebugClient
    {
        
        /// <summary>Get overlay and underlay addresses of the node</summary>
        /// <returns>Own node underlay and overlay addresses</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<AddressDetailDto> AddressesAsync();

        
        /// <summary>Get the balances with all known peers including prepaid services</summary>
        /// <returns>Own balances with all known peers</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<List<BalanceDto>?> BalancesGetAsync();

        
        /// <summary>Get the balances with a specific peer including prepaid services</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Balance with the specific peer</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<List<BalanceDto>?> BalancesGetAsync(string address);

        
        /// <summary>Get a list of blocklisted peers</summary>
        /// <returns>Returns overlay addresses of blocklisted peers</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<List<AddressDto>?> BlocklistAsync();

        
        /// <summary>Get the past due consumption balances with all known peers</summary>
        /// <returns>Own past due consumption balances with all known peers</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<List<BalanceDto>?> ConsumedGetAsync();

        
        /// <summary>Get the past due consumption balance with a specific peer</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Past-due consumption balance with the specific peer</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<List<BalanceDto>?> ConsumedGetAsync(string address);

        
        /// <summary>Get the address of the chequebook contract used</summary>
        /// <returns>Ethereum address of chequebook contract</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ChequebookAddressDto> ChequebookAddressAsync();

        
        /// <summary>Get the balance of the chequebook</summary>
        /// <returns>Balance of the chequebook</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ChequebookBalanceDto> ChequebookBalanceAsync();

        
        /// <summary>Check if chunk at address exists locally</summary>
        /// <param name="address">Swarm address of chunk</param>
        /// <returns>Chunk exists</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<MessageResponseDto> ChunksGetAsync(string address);

        
        /// <summary>Delete a chunk from local storage</summary>
        /// <param name="address">Swarm address of chunk</param>
        /// <returns>Chunk exists</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<MessageResponseDto> ChunksDeleteAsync(string address);

        
        /// <summary>Connect to address</summary>
        /// <param name="multiAddress">Underlay address of peer</param>
        /// <returns>Returns overlay address of connected peer</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ConnectDto> ConnectAsync(string multiAddress);

        
        /// <summary>Get reserve state</summary>
        /// <returns>Reserve State</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ReservestateDto> ReservestateAsync();

        
        /// <summary>Get chain state</summary>
        /// <returns>Chain State</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ChainstateDto> ChainstateAsync();

        
        /// <summary>Get health of node</summary>
        /// <returns>Health State of node</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<VersionDto> HealthAsync();

        
        /// <summary>Get a list of peers</summary>
        /// <returns>Returns overlay addresses of connected peers</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<List<AddressDto>?> PeersGetAsync();

        
        /// <summary>Remove peer</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Disconnected peer</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<MessageResponseDto> PeersDeleteAsync(string address);

        
        /// <summary>Try connection to node</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <returns>Returns round trip time for given peer</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<PingpongDto> PingpongAsync(string peerId);

        
        /// <summary>Get readiness state of node</summary>
        /// <returns>Health State of node</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<VersionDto> ReadinessAsync();

        
        /// <summary>Get amount of sent and received from settlements with a peer</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Amount of sent or received from settlements with a peer</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<List<SettlementDataDto>?> SettlementsGetAsync(string address);

        
        /// <summary>Get settlements with all known peers and total amount sent or received</summary>
        /// <returns>Settlements with all known peers and total amount sent or received</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<SettlementDto> SettlementsGetAsync();

        
        /// <summary>Get time based settlements with all known peers and total amount sent or received</summary>
        /// <returns>Time based settlements with all known peers and total amount sent or received</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TimesettlementsDto> TimesettlementsAsync();

        
        /// <returns>Swarm topology of the bee node</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TopologyDto> TopologyAsync();

        
        /// <summary>Get configured P2P welcome message</summary>
        /// <returns>Welcome message</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<string> WelcomeMessageGetAsync();

        
        /// <summary>Set P2P welcome message</summary>
        /// <returns>OK</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<VersionDto> WelcomeMessagePostAsync(string welcomeMessage);

        
        /// <summary>Get last cashout action for the peer</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <returns>Cashout status</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ChequebookCashoutGetDto> ChequebookCashoutGetAsync(string peerId);

        
        /// <summary>Cashout the last cheque for the peer</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <param name="gasLimit">Gas limit for transaction</param>
        /// <returns>OK</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TransactionHashDto> ChequebookCashoutPostAsync(string peerId, int? gasPrice = null, long? gasLimit = null);

        
        /// <summary>Get last cheques for the peer</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <returns>Last cheques</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ChequebookChequeGetDto> ChequebookChequeGetAsync(string peerId);

        
        /// <summary>Get last cheques for all peers</summary>
        /// <returns>Last cheques</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<List<ChequebookChequeGetDto>> ChequebookChequeGetAsync();

        
        /// <summary>Deposit tokens from overlay address into chequebook</summary>
        /// <param name="amount">amount of tokens to deposit</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Transaction hash of the deposit transaction</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TransactionHashDto> ChequebookDepositAsync(int amount, int? gasPrice = null);

        
        /// <summary>Withdraw tokens from the chequebook to the overlay address</summary>
        /// <param name="amount">amount of tokens to withdraw</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Transaction hash of the withdraw transaction</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TransactionHashDto> ChequebookWithdrawAsync(int amount, int? gasPrice = null);

        
        /// <summary>Get Tag information using Uid</summary>
        /// <param name="uid">Uid</param>
        /// <returns>Tag info</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TagDto> TagAsync(int uid);

        
        /// <summary>Get list of pending transactions</summary>
        /// <returns>List of pending transactions</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<List<PendingTransactionDto>> TransactionsGetAsync();

        
        /// <summary>Get information about a sent transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <returns>Get info about transaction</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TransactionsDto> TransactionsGetAsync(string txHash);

        
        /// <summary>Rebroadcast existing transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <returns>Hash of the transaction</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TransactionHashDto> TransactionsPostAsync(string txHash);

        
        /// <summary>Cancel existing transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Hash of the transaction</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TransactionHashDto> TransactionsDeleteAsync(string txHash, int? gasPrice = null);

        
        /// <summary>Get all available stamps for this node</summary>
        /// <returns>Returns an array of all available postage batches.</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<List<StampsGetDto>> StampsGetAsync();

        
        /// <summary>Get an individual postage batch status</summary>
        /// <param name="id">Swarm address of the stamp</param>
        /// <returns>Returns an individual postage batch state</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<StampsGetDto> StampsGetAsync(object id);

        
        /// <summary>Get extended bucket data of a batch</summary>
        /// <param name="id">Swarm address of the stamp</param>
        /// <returns>Returns extended bucket data of the provided batch ID</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<StampsBucketsDto> StampsBucketsAsync(object id);

        
        /// <summary>Buy a new postage batch.</summary>
        /// <param name="amount">Amount of BZZ added that the postage batch will have.</param>
        /// <param name="depth">Batch depth which specifies how many chunks can be signed with the batch. It is a logarithm. Must be higher than default bucket depth (16)</param>
        /// <param name="label">An optional label for this batch</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Returns the newly created postage batch ID</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<BatchDto> StampsPostAsync(int amount, int depth, string? label = null, bool? immutable = null, int? gasPrice = null);

        
        /// <summary>Top up an existing postage batch.</summary>
        /// <param name="id">Batch ID to top up</param>
        /// <param name="amount">Amount of BZZ per chunk to top up to an existing postage batch.</param>
        /// <returns>Returns the postage batch ID that was topped up</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<BatchDto> StampsTopupAsync(object id, int amount);

        
        /// <summary>Dilute an existing postage batch.</summary>
        /// <param name="id">Batch ID to dilute</param>
        /// <param name="depth">New batch depth. Must be higher than the previous depth.</param>
        /// <returns>Returns the postage batch ID that was diluted.</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<BatchDto> StampsDiluteAsync(object id, int depth);

    }
}
