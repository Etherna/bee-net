using System;
using System.Collections.Generic;
using System.Text;
using TestAdapter.Dtos;
using TestAdapter.Dtos.Debug;

namespace TestAdapter
{
    public interface IFacadeBeeDebugClient
    {

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get overlay and underlay addresses of the node</summary>
        /// <returns>Own node underlay and overlay addresses</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<AddressesResponse> AddressesAsync(System.Threading.CancellationToken? cancellationToken = null);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get the balances with all known peers including prepaid services</summary>
        /// <returns>Own balances with all known peers</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<BalancesResponse> BalancesAsync(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get the balances with a specific peer including prepaid services</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Balance with the specific peer</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<Balances2Response> Balances2Async(string address, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get a list of blocklisted peers</summary>
        /// <returns>Returns overlay addresses of blocklisted peers</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<BlocklistReponse> BlocklistAsync(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get the past due consumption balances with all known peers</summary>
        /// <returns>Own past due consumption balances with all known peers</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ConsumedResponse> ConsumedAsync(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get the past due consumption balance with a specific peer</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Past-due consumption balance with the specific peer</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<Consumed2Response> Consumed2Async(string address, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get the address of the chequebook contract used</summary>
        /// <returns>Ethereum address of chequebook contract</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<AddressResponse> AddressAsync(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get the balance of the chequebook</summary>
        /// <returns>Balance of the chequebook</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<BalanceResponse> BalanceAsync(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Check if chunk at address exists locally</summary>
        /// <param name="address">Swarm address of chunk</param>
        /// <returns>Chunk exists</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ChunksGETResponse> ChunksGETAsync(string address, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Delete a chunk from local storage</summary>
        /// <param name="address">Swarm address of chunk</param>
        /// <returns>Chunk exists</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ChunksDELETEResponse> ChunksDELETEAsync(string address, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Connect to address</summary>
        /// <param name="multiAddress">Underlay address of peer</param>
        /// <returns>Returns overlay address of connected peer</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ConnectResponse> ConnectAsync(string multiAddress, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get reserve state</summary>
        /// <returns>Reserve State</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ReservestateResponse> ReservestateAsync(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get chain state</summary>
        /// <returns>Chain State</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ChainstateReponse> ChainstateAsync(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get health of node</summary>
        /// <returns>Health State of node</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<HealthResponse> HealthAsync(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get a list of peers</summary>
        /// <returns>Returns overlay addresses of connected peers</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<PeersGETResponse> PeersGETAsync(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Remove peer</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Disconnected peer</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<PeersDELETE> PeersDELETEAsync(string address, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Try connection to node</summary>
        /// <param name="peer_id">Swarm address of peer</param>
        /// <returns>Returns round trip time for given peer</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<PingpongResponse> PingpongAsync(string peer_id, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get readiness state of node</summary>
        /// <returns>Health State of node</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ReadinessResponse> ReadinessAsync(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get amount of sent and received from settlements with a peer</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Amount of sent or received from settlements with a peer</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<SettlementsResponse> SettlementsAsync(string address, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get settlements with all known peers and total amount sent or received</summary>
        /// <returns>Settlements with all known peers and total amount sent or received</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<Settlements2Response> Settlements2Async(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get time based settlements with all known peers and total amount sent or received</summary>
        /// <returns>Time based settlements with all known peers and total amount sent or received</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TimesettlementsResponse> TimesettlementsAsync(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Swarm topology of the bee node</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TopologyResponse> TopologyAsync(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get configured P2P welcome message</summary>
        /// <returns>Welcome message</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<WelcomeMessageGETResponse> WelcomeMessageGETAsync(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Set P2P welcome message</summary>
        /// <returns>OK</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<WelcomeMessagePOSTResponse> WelcomeMessagePOSTAsync(BodyDto body, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get last cashout action for the peer</summary>
        /// <param name="peer_id">Swarm address of peer</param>
        /// <returns>Cashout status</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<CashoutGETResponse> CashoutGETAsync(string peer_id, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Cashout the last cheque for the peer</summary>
        /// <param name="peer_id">Swarm address of peer</param>
        /// <param name="gas_price">Gas price for transaction</param>
        /// <param name="gas_limit">Gas limit for transaction</param>
        /// <returns>OK</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<CashoutPOSTResponse> CashoutPOSTAsync(string peer_id, int? gas_price, long? gas_limit, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get last cheques for the peer</summary>
        /// <param name="peer_id">Swarm address of peer</param>
        /// <returns>Last cheques</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ChequeResponse> ChequeAsync(string peer_id, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get last cheques for all peers</summary>
        /// <returns>Last cheques</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<Cheque2Response> Cheque2Async(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Deposit tokens from overlay address into chequebook</summary>
        /// <param name="amount">amount of tokens to deposit</param>
        /// <param name="gas_price">Gas price for transaction</param>
        /// <returns>Transaction hash of the deposit transaction</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<DepositResponse> DepositAsync(int amount, int? gas_price, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Withdraw tokens from the chequebook to the overlay address</summary>
        /// <param name="amount">amount of tokens to withdraw</param>
        /// <param name="gas_price">Gas price for transaction</param>
        /// <returns>Transaction hash of the withdraw transaction</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<WithdrawResponse> WithdrawAsync(int amount, int? gas_price, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get Tag information using Uid</summary>
        /// <param name="uid">Uid</param>
        /// <returns>Tag info</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TagsResponse> TagsAsync(int uid, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get list of pending transactions</summary>
        /// <returns>List of pending transactions</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TransactionsResponse> TransactionsGETAsync(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get information about a sent transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <returns>Get info about transaction</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TransactionsGET2Response> TransactionsGET2Async(string txHash, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Rebroadcast existing transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <returns>Hash of the transaction</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TransactionsPOSTResponse> TransactionsPOSTAsync(string txHash, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Cancel existing transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <param name="gas_price">Gas price for transaction</param>
        /// <returns>Hash of the transaction</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TransactionsDELETEResponse> TransactionsDELETEAsync(string txHash, int? gas_price, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get all available stamps for this node</summary>
        /// <returns>Returns an array of all available postage batches.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<StampsGETResponse> StampsGETAsync(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get an individual postage batch status</summary>
        /// <param name="id">Swarm address of the stamp</param>
        /// <returns>Returns an individual postage batch state</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<StampsGET2Response> StampsGET2Async(object id, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get extended bucket data of a batch</summary>
        /// <param name="id">Swarm address of the stamp</param>
        /// <returns>Returns extended bucket data of the provided batch ID</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<BucketsResponse> BucketsAsync(object id, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Buy a new postage batch.</summary>
        /// <param name="amount">Amount of BZZ added that the postage batch will have.</param>
        /// <param name="depth">Batch depth which specifies how many chunks can be signed with the batch. It is a logarithm. Must be higher than default bucket depth (16)</param>
        /// <param name="label">An optional label for this batch</param>
        /// <param name="gas_price">Gas price for transaction</param>
        /// <returns>Returns the newly created postage batch ID</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<StampsPOSTResponse> StampsPOSTAsync(int amount, int depth, string label, bool? immutable, int? gas_price, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Top up an existing postage batch.</summary>
        /// <param name="id">Batch ID to top up</param>
        /// <param name="amount">Amount of BZZ per chunk to top up to an existing postage batch.</param>
        /// <returns>Returns the postage batch ID that was topped up</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TopupResponse> TopupAsync(object id, int amount, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Dilute an existing postage batch.</summary>
        /// <param name="id">Batch ID to dilute</param>
        /// <param name="depth">New batch depth. Must be higher than the previous depth.</param>
        /// <returns>Returns the postage batch ID that was diluted.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<DiluteResponse> DiluteAsync(object id, int depth, System.Threading.CancellationToken? cancellationToken);

    }
}
