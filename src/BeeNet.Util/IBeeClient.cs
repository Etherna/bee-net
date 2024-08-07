﻿// Copyright 2021-present Etherna SA
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

using Etherna.BeeNet.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet
{
    public interface IBeeClient
    {
        // Properties.
        Uri BaseUrl { get; }
        
        // Methods.
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Get all accounting associated values with all known peers
        /// </summary>
        /// <returns>Own accounting associated values with all known peers</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<Dictionary<string, Account>> AccountingAsync(CancellationToken cancellationToken = default);

        /// <summary>Authenticate - This endpoint is experimental</summary>
        /// <returns>Auth key</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> AuthenticateAsync(string role, int expiry);

        /// <summary>Buy a new postage batch.</summary>
        /// <param name="amount">Amount of BZZ added that the postage batch will have.</param>
        /// <param name="depth">Batch depth which specifies how many chunks can be signed with the batch. It is a logarithm. Must be higher than default bucket depth (16)</param>
        /// <param name="label">An optional label for this batch</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Returns the newly created postage batch ID</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<PostageBatchId> BuyPostageBatchAsync(
            BzzBalance amount,
            int depth,
            string? label = null,
            bool? immutable = null,
            XDaiBalance? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default);

        /// <summary>Cashout the last cheque for the peer</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <param name="gasLimit">Gas limit for transaction</param>
        /// <returns>Hash of the transaction</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> CashoutChequeForPeerAsync(
            string peerId,
            XDaiBalance? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Check if chunk at address exists locally
        /// </summary>
        /// <param name="hash">Swarm hash of chunk</param>
        /// <returns>Chunk exists</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<bool> CheckChunkExistsAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Validate pinned chunks integerity
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        Task<CheckPinsResult> CheckPinsAsync(
            SwarmHash? hash,
            CancellationToken cancellationToken = default);

        /// <summary>Connect to address</summary>
        /// <param name="peerAddress">Underlay address of peer</param>
        /// <returns>Returns overlay address of connected peer</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> ConnectToPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default);

        /// <summary>Create an initial feed root manifest</summary>
        /// <param name="owner">Owner</param>
        /// <param name="topic">Topic</param>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="type">Feed indexing scheme (default: sequence)</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <returns>Reference hash</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SwarmHash> CreateFeedAsync(
            string owner,
            string topic,
            PostageBatchId batchId,
            string? type = null,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default);

        /// <summary>Pin the root hash with the given reference</summary>
        /// <param name="hash">Swarm reference of the root hash</param>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task CreatePinAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default);

        /// <summary>Create Tag</summary>
        /// <returns>New Tag Info</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<TagInfo> CreateTagAsync(
            CancellationToken cancellationToken = default);

        /// <summary>Remove peer</summary>
        /// <param name="peerAddress">Swarm address of peer</param>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task DeletePeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default);

        /// <summary>Unpin the root hash with the given reference</summary>
        /// <param name="hash">Swarm reference of the root hash</param>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task DeletePinAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default);

        /// <summary>Delete Tag information using Uid</summary>
        /// <param name="uid">Uid</param>
        /// <returns>The resource was deleted successfully.</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task DeleteTagAsync(
            long uid,
            CancellationToken cancellationToken = default);

        /// <summary>Cancel existing transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Hash of the transaction</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> DeleteTransactionAsync(
            string txHash,
            XDaiBalance? gasPrice = null,
            CancellationToken cancellationToken = default);

        /// <summary>Deposit tokens from overlay address into chequebook</summary>
        /// <param name="amount">amount of tokens to deposit</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Transaction hash of the deposit transaction</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> DepositIntoChequebookAsync(
            BzzBalance amount,
            XDaiBalance? gasPrice = null,
            CancellationToken cancellationToken = default);

        /// <summary>Dilute an existing postage batch.</summary>
        /// <param name="batchId">Batch ID to dilute</param>
        /// <param name="depth">New batch depth. Must be higher than the previous depth.</param>
        /// <returns>Returns the postage batch ID that was diluted.</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<PostageBatchId> DilutePostageBatchAsync(
            PostageBatchId batchId,
            int depth,
            XDaiBalance? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default);

        /// <summary>Get overlay and underlay addresses of the node</summary>
        /// <returns>Own node underlay and overlay addresses</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<AddressDetail> GetAddressesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get the balances with all known peers including prepaid services</summary>
        /// <returns>Own balances with all known peers</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IDictionary<string, BzzBalance>> GetAllBalancesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get last cheques for all peers</summary>
        /// <returns>Last cheques</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<ChequebookCheque>> GetAllChequebookChequesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get the past due consumption balances with all known peers</summary>
        /// <returns>Own past due consumption balances with all known peers</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IDictionary<string, BzzBalance>> GetAllConsumedBalancesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get a list of peers</summary>
        /// <returns>Returns overlay addresses of connected peers</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<string>> GetAllPeerAddressesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get the list of pinned root hash references</summary>
        /// <returns>List of pinned root hash references</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<SwarmHash>> GetAllPinsAsync(CancellationToken cancellationToken = default);

        /// <summary>Get settlements with all known peers and total amount sent or received</summary>
        /// <returns>Settlements with all known peers and total amount sent or received</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<Settlement> GetAllSettlementsAsync(CancellationToken cancellationToken = default);

        /// <summary>Get time based settlements with all known peers and total amount sent or received</summary>
        /// <returns>Time based settlements with all known peers and total amount sent or received</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<Settlement> GetAllTimeSettlementsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all globally available batches that were purchased by all nodes.
        /// </summary>
        /// <returns></returns>
        /// <returns>Returns a dictionary with owner as keys, and enumerable of currently valid owned postage batches as values.</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IDictionary<string, IEnumerable<PostageBatch>>> GetAllValidPostageBatchesFromAllNodesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get the balances with a specific peer including prepaid services</summary>
        /// <param name="peerAddress">Swarm address of peer</param>
        /// <returns>Balance with the specific peer</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<BzzBalance> GetBalanceWithPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default);

        /// <summary>Get referenced data</summary>
        /// <param name="hash">Swarm hash reference to content</param>
        /// <param name="swarmCache">Determines if the download data should be cached on the node. By default the download will be cached</param>
        /// <param name="swarmRedundancyStrategy">Specify the retrieve strategy on redundant data. The numbers stand for NONE, DATA, PROX and RACE, respectively. Strategy NONE means no prefetching takes place. Strategy DATA means only data chunks are prefetched. Strategy PROX means only chunks that are close to the node are prefetched. Strategy RACE means all chunks are prefetched: n data chunks and k parity chunks. The first n chunks to arrive are used to reconstruct the file. Multiple strategies can be used in a fallback cascade if the swarm redundancy fallback mode is set to true. The default strategy is NONE, DATA, falling back to PROX, falling back to RACE</param>
        /// <param name="swarmRedundancyFallbackMode">Specify if the retrieve strategies (chunk prefetching on redundant data) are used in a fallback cascade. The default is true.</param>
        /// <param name="swarmChunkRetrievalTimeout">Specify the timeout for chunk retrieval. The default is 30 seconds.</param>
        /// <returns>Retrieved content specified by reference</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<Stream> GetBytesAsync(
            SwarmHash hash,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Requests the headers containing the content type and length for the reference
        /// </summary>
        /// <param name="hash">Swarm hash of chunk</param>
        /// <returns>Chunk exists</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task GetBytesHeadAsync(SwarmHash hash, CancellationToken cancellationToken = default);

        /// <summary>Get a list of blocklisted peers</summary>
        /// <returns>Returns overlay addresses of blocklisted peers</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<string>> GetBlocklistedPeerAddressesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get chain state</summary>
        /// <returns>Chain State</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ChainState> GetChainStateAsync(CancellationToken cancellationToken = default);

        /// <summary>Get the address of the chequebook contract used</summary>
        /// <returns>Ethereum address of chequebook contract</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> GetChequebookAddressAsync(CancellationToken cancellationToken = default);

        /// <summary>Get the balance of the chequebook</summary>
        /// <returns>Balance of the chequebook</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ChequebookBalance> GetChequebookBalanceAsync(CancellationToken cancellationToken = default);

        /// <summary>Get last cashout action for the peer</summary>
        /// <param name="peerAddress">Swarm address of peer</param>
        /// <returns>Cashout status</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ChequebookCashout> GetChequebookCashoutForPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default);

        /// <summary>Get last cheques for the peer</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <returns>Last cheques</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ChequebookCheque> GetChequebookChequeForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default);

        /// <summary>Get Chunk</summary>
        /// <param name="hash">Swarm address of chunk</param>
        /// <returns>Retrieved chunk content</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SwarmChunk> GetChunkAsync(
            SwarmHash hash,
            bool? swarmCache = null,
            CancellationToken cancellationToken = default);

        /// <summary>Get Chunk</summary>
        /// <param name="hash">Swarm address of chunk</param>
        /// <returns>Retrieved chunk content</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<Stream> GetChunkStreamAsync(
            SwarmHash hash,
            bool? swarmCache = null,
            CancellationToken cancellationToken = default);

        /// <summary>Get the past due consumption balance with a specific peer</summary>
        /// <param name="peerAddress">Swarm address of peer</param>
        /// <returns>Past-due consumption balance with the specific peer</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<BzzBalance> GetConsumedBalanceWithPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default);

        /// <summary>Find feed update</summary>
        /// <param name="owner">Owner</param>
        /// <param name="topic">Topic</param>
        /// <param name="at">Timestamp of the update (default: now)</param>
        /// <param name="after">Start index (default: 0)</param>
        /// <param name="type">Feed indexing scheme (default: sequence)</param>
        /// <returns>Latest feed update</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SwarmHash> GetFeedAsync(
            string owner,
            string topic,
            int? at = null,
            int? after = null,
            string? type = null,
            CancellationToken cancellationToken = default);

        /// <summary>Get file or index document from a collection of files</summary>
        /// <param name="address">Swarm address of content</param>
        /// <param name="swarmCache">Determines if the download data should be cached on the node. By default the download will be cached</param>
        /// <param name="swarmRedundancyStrategy">Specify the retrieve strategy on redundant data. The numbers stand for NONE, DATA, PROX and RACE, respectively. Strategy NONE means no prefetching takes place. Strategy DATA means only data chunks are prefetched. Strategy PROX means only chunks that are close to the node are prefetched. Strategy RACE means all chunks are prefetched: n data chunks and k parity chunks. The first n chunks to arrive are used to reconstruct the file. Multiple strategies can be used in a fallback cascade if the swarm redundancy fallback mode is set to true. The default strategy is NONE, DATA, falling back to PROX, falling back to RACE</param>
        /// <param name="swarmRedundancyFallbackMode">Specify if the retrieve strategies (chunk prefetching on redundant data) are used in a fallback cascade. The default is true.</param>
        /// <param name="swarmChunkRetrievalTimeout">Specify the timeout for chunk retrieval. The default is 30 seconds.</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<FileResponse> GetFileAsync(
            SwarmAddress address,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Get the headers containing the content type and length for the reference
        /// </summary>
        /// <param name="hash">Swarm address of chunk</param>
        /// <returns>Chunk exists</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task GetFileHeadAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default);

        /// <summary>Get health of node</summary>
        /// <returns>Health State of node</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<Health> GetHealthAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get information about the node
        /// </summary>
        /// <returns>Information about the node</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<NodeInfo> GetNodeInfoAsync(CancellationToken cancellationToken = default);

        /// <summary>Get all owned postage batches by this node</summary>
        /// <returns>List of all owned postage batches</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred</exception>
        Task<IEnumerable<PostageBatch>> GetOwnedPostageBatchesByNodeAsync(CancellationToken cancellationToken = default);

        /// <summary>Get list of pending transactions</summary>
        /// <returns>List of pending transactions</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<TxInfo>> GetPendingTransactionsAsync(CancellationToken cancellationToken = default);

        /// <summary>Get pinning status of the root hash with the given reference</summary>
        /// <param name="hash">Swarm reference of the root hash</param>
        /// <returns>Reference of the pinned root hash</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> GetPinStatusAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default);

        /// <summary>Get an individual postage batch status</summary>
        /// <param name="batchId">Swarm address of the stamp</param>
        /// <returns>Returns an individual postage batch state</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<PostageBatch> GetPostageBatchAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get reserve commitment hash with sample proofs
        /// </summary>
        /// <param name="depth">The storage depth.</param>
        /// <param name="anchor1">The first anchor.</param>
        /// <param name="anchor2">The second anchor.</param>
        /// <returns>Reserve sample response</returns>
        /// <returns></returns>
        Task<ReserveCommitment> GetReserveCommitmentAsync(
            int depth,
            string anchor1,
            string anchor2,
            CancellationToken cancellationToken = default);

        /// <summary>Get reserve state</summary>
        /// <returns>Reserve State</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ReserveState> GetReserveStateAsync(CancellationToken cancellationToken = default);

        /// <summary>Get amount of sent and received from settlements with a peer</summary>
        /// <param name="peerAddress">Swarm address of peer</param>
        /// <returns>Amount of sent or received from settlements with a peer</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SettlementData> GetSettlementsWithPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default);

        /// <summary>Get extended bucket data of a batch</summary>
        /// <param name="batchId">Swarm address of the stamp</param>
        /// <returns>Returns extended bucket data of the provided batch ID</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<StampsBuckets> GetStampsBucketsForBatchAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default);

        /// <returns>Swarm topology of the bee node</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<Topology> GetSwarmTopologyAsync(CancellationToken cancellationToken = default);

        /// <summary>Get Tag information using Uid</summary>
        /// <param name="uid">Uid</param>
        /// <returns>Tag info</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<TagInfo> GetTagInfoAsync(
            long uid,
            CancellationToken cancellationToken = default);

        /// <summary>Get list of tags</summary>
        /// <param name="offset">The number of items to skip before starting to collect the result set.</param>
        /// <param name="limit">The numbers of items to return.</param>
        /// <returns>List of tags</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<TagInfo>> GetTagsListAsync(
            int? offset = null,
            int? limit = null,
            CancellationToken cancellationToken = default);

        /// <summary>Get information about a sent transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <returns>Get info about transaction</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<TxInfo> GetTransactionInfoAsync(
            string txHash,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get wallet balance for BZZ and xDai
        /// </summary>
        /// <returns>Wallet balance info</returns>
        Task<WalletBalances> GetWalletBalance(CancellationToken cancellationToken = default);

        /// <summary>Get configured P2P welcome message</summary>
        /// <returns>Welcome message</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default);

        /// <summary>Check if content is retrievable</summary>
        /// <param name="hash">Root hash of content (can be of any type: collection, file, chunk)</param>
        /// <returns>Returns if the content is retrievable</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<bool> IsContentRetrievableAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Get all available loggers.
        /// </summary>
        /// <returns>Returns an array of all available loggers, also represented in short form in a tree.</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<LogData> LoggersGetAsync(CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Get all available loggers that match the specified expression.
        /// </summary>
        /// <param name="exp">Regular expression or a subsystem that matches the logger(s).</param>
        /// <returns>Returns an array of all available loggers that matches given expression, also represented in short form in a tree.</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<LogData> LoggersGetAsync(string exp, CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Set logger(s) verbosity level.
        /// </summary>
        /// <param name="exp">Regular expression or a subsystem that matches the logger(s).</param>
        /// <returns>The verbosity was changed successfully.</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task LoggersPutAsync(string exp, CancellationToken cancellationToken = default);

        /// <summary>Rebroadcast existing transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <returns>Hash of the transaction</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> RebroadcastTransactionAsync(
            string txHash,
            CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Get current status of node in redistribution game
        /// </summary>
        /// <returns>Redistribution status info</returns>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task<RedistributionState> RedistributionStateAsync(CancellationToken cancellationToken = default);

        /// <summary>Refresh the auth token - This endpoint is experimental</summary>
        /// <returns>Key</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> RefreshAuthAsync(
            string role, 
            int expiry,
            CancellationToken cancellationToken = default);

        Task<SwarmChunkReference> ResolveAddressToChunkReferenceAsync(SwarmAddress address);

        /// <summary>Reupload a root hash to the network</summary>
        /// <param name="hash">Root hash of content (can be of any type: collection, file, chunk)</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task ReuploadContentAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default);

        /// <summary>Send to recipient or target with Postal Service for Swarm</summary>
        /// <param name="topic">Topic name</param>
        /// <param name="targets">Target message address prefix. If multiple targets are specified, only one would be matched.</param>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="recipient">Recipient publickey</param>
        /// <returns>Subscribed to topic</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task SendPssAsync(
            string topic,
            string targets,
            PostageBatchId batchId,
            string? recipient = null,
            CancellationToken cancellationToken = default);

        /// <summary>Set P2P welcome message</summary>
        /// <returns>OK</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task SetWelcomeMessageAsync(
            string welcomeMessage,
            CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Withdraw all staked amount.
        /// </summary>
        /// <remarks>
        /// Be aware, this endpoint creates an on-chain transactions and transfers BZZ from the node's Ethereum account and hence directly manipulates the wallet balance.
        /// </remarks>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <param name="gasLimit">Gas limit for transaction</param>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task StakeDeleteAsync(
            XDaiBalance? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Get the staked amount.
        /// </summary>
        /// <remarks>
        /// This endpoint fetches the staked amount from the blockchain.
        /// </remarks>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task StakeGetAsync(CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Deposit some amount for staking.
        /// </summary>
        /// <remarks>
        /// Be aware, this endpoint creates an on-chain transactions and transfers BZZ from the node's Ethereum account and hence directly manipulates the wallet balance.
        /// </remarks>
        /// <param name="amount">Amount of BZZ added that will be deposited for staking.</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <param name="gasLimit">Gas limit for transaction</param>
        /// <exception cref="BeeNetDebugApiException">A server side error occurred.</exception>
        Task StakePostAsync(
            BzzBalance amount,
            XDaiBalance? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the current status snapshot of this node.
        /// </summary>
        /// <returns>Returns the current node status snapshot.</returns>
        Task<StatusNode> StatusNodeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the current status snapshot of this node connected peers.
        /// </summary>
        /// <returns>Returns the status snapshot of this node connected peers</returns>
        Task<IEnumerable<StatusNode>> StatusPeersAsync(CancellationToken cancellationToken = default);

        /// <summary>Subscribe for messages on the given topic.</summary>
        /// <param name="topic">Topic name</param>
        /// <returns>Returns a WebSocket with a subscription for incoming message data on the requested topic.</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task SubscribeToPssAsync(
            string topic,
            CancellationToken cancellationToken = default);

        /// <summary>Top up an existing postage batch.</summary>
        /// <param name="batchId">Batch ID to top up</param>
        /// <param name="amount">Amount of BZZ per chunk to top up to an existing postage batch.</param>
        /// <returns>Returns the postage batch ID that was topped up</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<PostageBatchId> TopUpPostageBatchAsync(
            PostageBatchId batchId, 
            BzzBalance amount,
            XDaiBalance? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default);

        /// <summary>Try connection to node</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <returns>Returns round trip time for given peer</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> TryConnectToPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default);

        /// <summary>Update Total Count and swarm hash for a tag of an input stream of unknown size using Uid</summary>
        /// <param name="uid">Uid</param>
        /// <param name="hash">Can contain swarm hash to use for the tag</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task UpdateTagAsync(
            long uid,
            SwarmHash? hash = null,
            CancellationToken cancellationToken = default);

        /// <summary>Upload Chunk</summary>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="chunkData"></param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        ///     <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarmDeferredUpload">Determines if the uploaded data should be sent to the network immediately or in a deferred fashion. By default the upload will be deferred.</param>
        /// <param name="swarmTag">Associate upload with an existing Tag UID</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SwarmHash> UploadChunkAsync(PostageBatchId batchId,
            Stream chunkData,
            bool swarmPin = false,
            bool swarmDeferredUpload = true,
            long? swarmTag = null,
            CancellationToken cancellationToken = default);

        /// <summary>Upload stream of chunks</summary>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="swarmTag">Associate upload with an existing Tag UID</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <returns>Returns a Websocket connection on which stream of chunks can be uploaded. Each chunk sent is acknowledged using a binary response `0` which serves as confirmation of upload of single chunk. Chunks should be packaged as binary messages for uploading.</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task UploadChunksStreamAsync(
            PostageBatchId batchId,
            int? swarmTag = null,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default);

        /// <summary>Upload data</summary>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="swarmTag">Associate upload with an existing Tag UID</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarmEncrypt">Represents the encrypting state of the file
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarmDeferredUpload">Determines if the uploaded data should be sent to the network immediately or in a deferred fashion. By default the upload will be deferred.</param>
        /// <returns>Reference hash</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SwarmHash> UploadBytesAsync(
            PostageBatchId batchId,
            Stream body,
            int? swarmTag = null, 
            bool? swarmPin = null, 
            bool? swarmEncrypt = null, 
            bool? swarmDeferredUpload = null,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            CancellationToken cancellationToken = default);
        
#if NET7_0_OR_GREATER
        /// <summary>Upload a directory</summary>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="directoryPath">The directory path</param>
        /// <param name="swarmTag">Associate upload with an existing Tag UID</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarmEncrypt">Represents the encrypting state of the file
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarmCollection">Upload file/files as a collection</param>
        /// <param name="swarmIndexDocument">Default file to be referenced on path, if exists under that path</param>
        /// <param name="swarmErrorDocument">Configure custom error document to be returned when a specified path can not be found in collection</param>
        /// <param name="swarmDeferredUpload">Determines if the uploaded data should be sent to the network immediately or in a deferred fashion. By default the upload will be deferred.</param>
        /// <param name="swarmRedundancyLevel">Add redundancy to the data being uploaded so that downloaders can download it with better UX. 0 value is default and does not add any redundancy to the file.</param>
        /// <returns>Reference hash</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SwarmHash> UploadDirectoryAsync(
            PostageBatchId batchId,
            string directoryPath,
            int? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            string? swarmIndexDocument = null,
            string? swarmErrorDocument = null,
            bool? swarmDeferredUpload = null,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            CancellationToken cancellationToken = default);
#endif

        /// <summary>Upload a file</summary>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="content">Input file content</param>
        /// <param name="name">Filename when uploading single file</param>
        /// <param name="contentType">The specified content-type is preserved for download of the asset</param>
        /// <param name="isFileCollection">Upload file/files as a collection</param>
        /// <param name="swarmTag">Associate upload with an existing Tag UID</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarmEncrypt">Represents the encrypting state of the file
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarmIndexDocument">Default file to be referenced on path, if exists under that path</param>
        /// <param name="swarmErrorDocument">Configure custom error document to be returned when a specified path can not be found in collection</param>
        /// <param name="swarmDeferredUpload">Determines if the uploaded data should be sent to the network immediately or in a deferred fashion. By default the upload will be deferred.</param>
        /// <param name="swarmRedundancyLevel">Add redundancy to the data being uploaded so that downloaders can download it with better UX. 0 value is default and does not add any redundancy to the file.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Reference hash</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SwarmHash> UploadFileAsync(
            PostageBatchId batchId,
            Stream content,
            string? name = null,
            string? contentType = null,
            bool isFileCollection = false,
            int? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            string? swarmIndexDocument = null,
            string? swarmErrorDocument = null,
            bool? swarmDeferredUpload = null,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            CancellationToken cancellationToken = default);

        /// <summary>Upload single owner chunk</summary>
        /// <param name="owner">Owner</param>
        /// <param name="id">Id</param>
        /// <param name="sig">Signature</param>
        /// <param name="batchId"></param>
        /// <param name="body">The SOC binary data is composed of the span (8 bytes) and the at most 4KB payload.</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <returns>Reference hash</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SwarmHash> UploadSocAsync(
            string owner,
            string id,
            string sig,
            PostageBatchId batchId,
            Stream body,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Allows withdrawals of BZZ or xDAI to provided (whitelisted) address
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="address"></param>
        /// <param name="coin"></param>
        /// <returns>Tx hash</returns>
        Task<string> WalletWithdrawAsync(
            BzzBalance amount,
            string address,
            XDaiBalance coin,
            CancellationToken cancellationToken = default);

        /// <summary>Withdraw tokens from the chequebook to the overlay address</summary>
        /// <param name="amount">amount of tokens to withdraw</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Transaction hash of the withdraw transaction</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> WithdrawFromChequebookAsync(
            BzzBalance amount,
            XDaiBalance? gasPrice = null,
            CancellationToken cancellationToken = default);
    }
}