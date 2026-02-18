// Copyright 2021-present Etherna SA
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

using Etherna.BeeNet.Exceptions;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet
{
    public interface ISwarmClient
    {
        // Properties.
        public SwarmClients ApiCompatibility { get; }
        public bool IsDryMode { get; }
        public Uri NodeUrl { get; }
        
        // Methods.
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Get all accounting associated values with all known peers
        /// </summary>
        /// <returns>Own accounting associated values with all known peers</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<Dictionary<string, Account>> AccountingAsync(CancellationToken cancellationToken = default);

        /// <summary>Buy a new postage batch.</summary>
        /// <param name="amount">Amount of BZZ added that the postage batch will have.</param>
        /// <param name="depth">Batch depth which specifies how many chunks can be signed with the batch. It is a logarithm. Must be higher than default bucket depth (16)</param>
        /// <param name="label">An optional label for this batch</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Returns the newly created postage batch ID</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<(PostageBatchId BatchId, EthTxHash TxHash)> BuyPostageBatchAsync(
            BzzValue amount,
            int depth,
            string? label = null,
            bool? immutable = null,
            ulong? gasLimit = null,
            XDaiValue? gasPrice = null,
            CancellationToken cancellationToken = default);

        /// <summary>Cashout the last cheque for the peer</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <param name="gasLimit">Gas limit for transaction</param>
        /// <returns>Hash of the transaction</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<EthTxHash> CashoutChequeForPeerAsync(
            string peerId,
            XDaiValue? gasPrice = null,
            ulong? gasLimit = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Validate pinned chunks integerity
        /// </summary>
        /// <param name="reference">Swarm content reference</param>
        /// <returns></returns>
        Task<CheckPinsResult> CheckPinsAsync(
            SwarmReference? reference,
            CancellationToken cancellationToken = default);

        Task ChunksBulkUploadAsync(
            SwarmChunk[] chunks,
            PostageBatchId batchId,
            CancellationToken cancellationToken = default);

        /// <summary>Connect to address</summary>
        /// <param name="peerAddress">Underlay address of peer</param>
        /// <returns>Returns overlay address of connected peer</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<SwarmOverlayAddress> ConnectToPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default);

        /// <summary>Pin the root hash with the given reference</summary>
        /// <param name="reference">Swarm content reference</param>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<bool> CreatePinAsync(
            SwarmReference reference,
            CancellationToken cancellationToken = default);

        /// <summary>Create Tag</summary>
        /// <returns>New Tag Info</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<TagInfo> CreateTagAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default);

        /// <summary>Remove peer</summary>
        /// <param name="peerAddress">Swarm address of peer</param>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task DeletePeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default);

        /// <summary>Unpin the root hash with the given reference</summary>
        /// <param name="reference">Swarm content reference</param>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task DeletePinAsync(
            SwarmReference reference,
            CancellationToken cancellationToken = default);

        /// <summary>Delete Tag information using Uid</summary>
        /// <param name="uid">Uid</param>
        /// <returns>The resource was deleted successfully.</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task DeleteTagAsync(
            TagId id,
            CancellationToken cancellationToken = default);

        /// <summary>Cancel existing transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Hash of the transaction</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<EthTxHash> DeleteTransactionAsync(
            EthTxHash txHash,
            XDaiValue? gasPrice = null,
            CancellationToken cancellationToken = default);

        /// <summary>Deposit tokens from overlay address into chequebook</summary>
        /// <param name="amount">amount of tokens to deposit</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Transaction hash of the deposit transaction</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<EthTxHash> DepositIntoChequebookAsync(
            BzzValue amount,
            XDaiValue? gasPrice = null,
            CancellationToken cancellationToken = default);

        /// <summary>Dilute an existing postage batch.</summary>
        /// <param name="batchId">Batch ID to dilute</param>
        /// <param name="depth">New batch depth. Must be higher than the previous depth.</param>
        /// <returns>Returns the tx hash updating the postage batch</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<EthTxHash> DilutePostageBatchAsync(
            PostageBatchId batchId,
            int depth,
            XDaiValue? gasPrice = null,
            ulong? gasLimit = null,
            CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Create postage stamp signature against given chunk address
        /// </summary>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<EnvelopeResponse> EnvelopeAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default);

        /// <summary>Get the balances with all known peers including prepaid services</summary>
        /// <returns>Own balances with all known peers</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<IDictionary<string, BzzValue>> GetAllBalancesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get last cheques for all peers</summary>
        /// <returns>Last cheques</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<ChequebookCheque[]> GetAllChequebookChequesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get the past due consumption balances with all known peers</summary>
        /// <returns>Own past due consumption balances with all known peers</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<IDictionary<string, BzzValue>> GetAllConsumedBalancesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get a list of peers</summary>
        /// <returns>Returns overlay addresses of connected peers</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<string[]> GetAllPeerAddressesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get the list of pinned root hash references</summary>
        /// <returns>List of pinned references</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<SwarmReference[]> GetAllPinsAsync(CancellationToken cancellationToken = default);

        /// <summary>Get settlements with all known peers and total amount sent or received</summary>
        /// <returns>Settlements with all known peers and total amount sent or received</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<Settlement> GetAllSettlementsAsync(CancellationToken cancellationToken = default);

        /// <summary>Get time based settlements with all known peers and total amount sent or received</summary>
        /// <returns>Time based settlements with all known peers and total amount sent or received</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<Settlement> GetAllTimeSettlementsAsync(CancellationToken cancellationToken = default);

        /// <summary>Get the balances with a specific peer including prepaid services</summary>
        /// <param name="peerAddress">Swarm address of peer</param>
        /// <returns>Balance with the specific peer</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<BzzValue> GetBalanceWithPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default);

        /// <summary>Get referenced data</summary>
        /// <param name="reference">Swarm content reference</param>
        /// <param name="swarmCache">Determines if the download data should be cached on the node. By default the download will be cached</param>
        /// <param name="swarmRedundancyLevel"></param>
        /// <param name="swarmRedundancyStrategy">Specify the retrieve strategy on redundant data. The numbers stand for NONE, DATA, PROX and RACE, respectively. Strategy NONE means no prefetching takes place. Strategy DATA means only data chunks are prefetched. Strategy PROX means only chunks that are close to the node are prefetched. Strategy RACE means all chunks are prefetched: n data chunks and k parity chunks. The first n chunks to arrive are used to reconstruct the file. Multiple strategies can be used in a fallback cascade if the swarm redundancy fallback mode is set to true. The default strategy is NONE, DATA, falling back to PROX, falling back to RACE</param>
        /// <param name="swarmRedundancyFallbackMode">Specify if the retrieve strategies (chunk prefetching on redundant data) are used in a fallback cascade. The default is true.</param>
        /// <param name="swarmChunkRetrievalTimeout">Specify the timeout for chunk retrieval. The default is 30 seconds.</param>
        /// <returns>Retrieved content specified by reference</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<Stream> GetBytesAsync(
            SwarmReference reference,
            bool? swarmCache = null,
            RedundancyLevel? swarmRedundancyLevel = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null, 
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Requests the headers containing the content type and length for the reference
        /// </summary>
        /// <param name="reference">Swarm content reference</param>
        /// <returns>Chunk exists</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<HttpContentHeaders?> GetBytesHeadersAsync(
            SwarmReference reference,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default);

        /// <summary>Get a list of blocklisted peers</summary>
        /// <returns>Returns overlay addresses of blocklisted peers</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<string[]> GetBlocklistedPeerAddressesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get chain state</summary>
        /// <returns>Chain State</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<ChainState> GetChainStateAsync(CancellationToken cancellationToken = default);

        /// <summary>Get the address of the chequebook contract used</summary>
        /// <returns>Ethereum address of chequebook contract</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<EthAddress> GetChequebookAddressAsync(CancellationToken cancellationToken = default);

        /// <summary>Get the balance of the chequebook</summary>
        /// <returns>Balance of the chequebook</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<ChequebookBalance> GetChequebookBalanceAsync(CancellationToken cancellationToken = default);

        /// <summary>Get last cashout action for the peer</summary>
        /// <param name="peerAddress">Swarm address of peer</param>
        /// <returns>Cashout status</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<ChequebookCashout> GetChequebookCashoutForPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default);

        /// <summary>Get last cheques for the peer</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <returns>Last cheques</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<ChequebookCheque> GetChequebookChequeForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default);

        /// <summary>Get Chunk</summary>
        /// <returns>Retrieved chunk content</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<SwarmChunk> GetChunkAsync(
            SwarmHash hash,
            SwarmChunkBmt swarmChunkBmt,
            bool? swarmCache = null,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default);

        /// <summary>Get Chunk</summary>
        /// <returns>Retrieved chunk content</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<Stream> GetChunkStreamAsync(
            SwarmHash hash,
            bool? swarmCache = null,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default);

        /// <summary>Upload stream of chunks</summary>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="swarmTag">Associate upload with an existing Tag UID</param>
        /// <returns>Returns a Websocket connection on which stream of chunks can be uploaded. Each chunk sent is acknowledged using a binary response `0` which serves as confirmation of upload of single chunk. Chunks should be packaged as binary messages for uploading.</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<IChunkWebSocketUploader> GetChunkUploaderWebSocketAsync(
            PostageBatchId batchId,
            TagId? tagId = null,
            CancellationToken cancellationToken = default);

        /// <summary>Get the past due consumption balance with a specific peer</summary>
        /// <param name="peerAddress">Swarm address of peer</param>
        /// <returns>Past-due consumption balance with the specific peer</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<BzzValue> GetConsumedBalanceWithPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default);

        /// <summary>Get file or index document from a collection of files</summary>
        /// <param name="address">Swarm address of content</param>
        /// <param name="swarmCache">Determines if the download data should be cached on the node. By default the download will be cached</param>
        /// <param name="swarmRedundancyStrategy">Specify the retrieve strategy on redundant data. The numbers stand for NONE, DATA, PROX and RACE, respectively. Strategy NONE means no prefetching takes place. Strategy DATA means only data chunks are prefetched. Strategy PROX means only chunks that are close to the node are prefetched. Strategy RACE means all chunks are prefetched: n data chunks and k parity chunks. The first n chunks to arrive are used to reconstruct the file. Multiple strategies can be used in a fallback cascade if the swarm redundancy fallback mode is set to true. The default strategy is NONE, DATA, falling back to PROX, falling back to RACE</param>
        /// <param name="swarmRedundancyFallbackMode">Specify if the retrieve strategies (chunk prefetching on redundant data) are used in a fallback cascade. The default is true.</param>
        /// <param name="swarmChunkRetrievalTimeout">Specify the timeout for chunk retrieval. The default is 30 seconds.</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<FileResponse> GetFileAsync(
            SwarmAddress address,
            bool? swarmCache = null,
            RedundancyLevel? swarmRedundancyLevel = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all globally available batches that were purchased by all nodes.
        /// </summary>
        /// <returns></returns>
        /// <returns>Returns a dictionary with owner as keys, and enumerable of currently valid owned postage batches as values.</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<(PostageBatch PostageBatch, EthAddress Owner)[]> GetGlobalValidPostageBatchesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get health of node</summary>
        /// <returns>Health State of node</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<Health> GetHealthAsync(CancellationToken cancellationToken = default);

        Task<NeighborhoodStatus[]> GetNeighborhoodsStatus(
            CancellationToken cancellationToken = default);

        /// <summary>Get overlay and underlay addresses of the node</summary>
        /// <returns>Own node underlay and overlay addresses</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<SwarmNodeAddresses> GetNodeAddressesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get information about the node
        /// </summary>
        /// <returns>Information about the node</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<NodeInfo> GetNodeInfoAsync(CancellationToken cancellationToken = default);

        /// <summary>Get all owned postage batches by this node</summary>
        /// <returns>List of all owned postage batches</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred</exception>
        Task<PostageBatch[]> GetOwnedPostageBatchesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get list of pending transactions</summary>
        /// <returns>List of pending transactions</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<EthTx[]> GetPendingTransactionsAsync(CancellationToken cancellationToken = default);

        /// <summary>Get pinning status of the root hash with the given reference</summary>
        /// <param name="reference">Swarm content reference</param>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<PinStatus> GetPinStatusAsync(
            SwarmReference reference,
            CancellationToken cancellationToken = default);

        /// <summary>Get an individual postage batch status</summary>
        /// <param name="batchId">Swarm address of the stamp</param>
        /// <returns>Returns an individual postage batch state</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<PostageBatch> GetPostageBatchAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default);

        /// <summary>Get extended bucket data of a batch</summary>
        /// <param name="batchId">Swarm address of the stamp</param>
        /// <returns>Returns extended bucket data of the provided batch ID</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<PostageBucketsStatus> GetPostageBatchBucketsAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default);

        Task<bool> GetReadinessAsync(CancellationToken cancellationToken = default);

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
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<ReserveState> GetReserveStateAsync(CancellationToken cancellationToken = default);

        /// <summary>Get amount of sent and received from settlements with a peer</summary>
        /// <param name="peerAddress">Swarm address of peer</param>
        /// <returns>Amount of sent or received from settlements with a peer</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<SettlementData> GetSettlementsWithPeerAsync(
            string peerAddress,
            CancellationToken cancellationToken = default);

        Task<FileResponse> GetSocDataAsync(
            EthAddress owner,
            string id,
            bool? swarmOnlyRootChunk = null,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default);

        /// <returns>Swarm topology of the bee node</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<Topology> GetSwarmTopologyAsync(CancellationToken cancellationToken = default);

        /// <summary>Get Tag information using Uid</summary>
        /// <param name="uid">Uid</param>
        /// <returns>Tag info</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<TagInfo> GetTagInfoAsync(
            TagId id,
            CancellationToken cancellationToken = default);

        /// <summary>Get list of tags</summary>
        /// <param name="offset">The number of items to skip before starting to collect the result set.</param>
        /// <param name="limit">The numbers of items to return.</param>
        /// <returns>List of tags</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<TagInfo[]> GetTagsListAsync(
            int? offset = null,
            int? limit = null,
            CancellationToken cancellationToken = default);

        /// <summary>Get information about a sent transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <returns>Get info about transaction</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<EthTx> GetTransactionInfoAsync(
            EthTxHash txHash,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get wallet balance for BZZ and xDai
        /// </summary>
        /// <returns>Wallet balance info</returns>
        Task<WalletBalances> GetWalletBalance(CancellationToken cancellationToken = default);

        /// <summary>Get configured P2P welcome message</summary>
        /// <returns>Welcome message</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Get grantee list
        /// </summary>
        /// <param name="reference">Grantee list reference</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<ICollection<string>> GranteeGetAsync(string reference, CancellationToken cancellationToken = default);
        
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Update grantee list
        /// </summary>
        /// <remarks>
        /// Add or remove grantees from an existing grantee list
        /// </remarks>
        /// <param name="reference">Grantee list reference</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<GranteeResponse> GranteePatchAsync(string reference, string swarmActHistoryAddress, PostageBatchId batchId, string[] addList, string[] revokeList, TagId? tagId = null, bool? swarmPin = null, bool? swarmDeferredUpload = null, CancellationToken cancellationToken = default);
        
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Create grantee list
        /// </summary>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<GranteeResponse> GranteePostAsync(PostageBatchId batchId, string[] grantees, TagId? tagId = null, bool? swarmPin = null, bool? swarmDeferredUpload = null, string? swarmActHistoryAddress = null, CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Check if chunk at address exists locally
        /// </summary>
        /// <param name="hash">Swarm hash of chunk</param>
        /// <returns>Chunk exists</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<bool> IsChunkExistingAsync(
            SwarmHash hash,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default);

        /// <summary>Check if content is retrievable</summary>
        /// <param name="reference">Swarm content reference</param>
        /// <returns>Returns if the content is retrievable</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<bool> IsContentRetrievableAsync(
            SwarmReference reference,
            CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Get all available loggers.
        /// </summary>
        /// <returns>Returns an array of all available loggers, also represented in short form in a tree.</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<LogData> LoggersGetAsync(CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Get all available loggers that match the specified expression.
        /// </summary>
        /// <param name="exp">Regular expression or a subsystem that matches the logger(s).</param>
        /// <returns>Returns an array of all available loggers that matches given expression, also represented in short form in a tree.</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<LogData> LoggersGetAsync(string exp, CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Set logger(s) verbosity level.
        /// </summary>
        /// <param name="exp">Regular expression or a subsystem that matches the logger(s).</param>
        /// <returns>The verbosity was changed successfully.</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task LoggersPutAsync(string exp, CancellationToken cancellationToken = default);

        Task<WebSocket> OpenChunkUploadWebSocketConnectionAsync(
            string endpointPath,
            PostageBatchId batchId,
            TagId? tagId,
            int internalBufferSize,
            int receiveBufferSize,
            int sendBufferSize,
            CancellationToken cancellationToken);

        /// <summary>Rebroadcast existing transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <returns>Hash of the transaction</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<string> RebroadcastTransactionAsync(
            EthTxHash txHash,
            CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Get current status of node in redistribution game
        /// </summary>
        /// <returns>Redistribution status info</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<RedistributionState> RedistributionStateAsync(CancellationToken cancellationToken = default);

        /// <summary>Reupload a root hash to the network</summary>
        /// <param name="reference">Swarm content reference</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task ReuploadContentAsync(
            SwarmReference reference,
            CancellationToken cancellationToken = default);

        /// <summary>Send to recipient or target with Postal Service for Swarm</summary>
        /// <param name="topic">Topic name</param>
        /// <param name="targets">Target message address prefix. If multiple targets are specified, only one would be matched.</param>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="recipient">Recipient publickey</param>
        /// <returns>Subscribed to topic</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task SendPssAsync(
            string topic,
            string targets,
            PostageBatchId batchId,
            string? recipient = null,
            CancellationToken cancellationToken = default);

        /// <summary>Set P2P welcome message</summary>
        /// <returns>OK</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
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
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task StakeDeleteAsync(
            XDaiValue? gasPrice = null,
            ulong? gasLimit = null,
            CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Get the staked amount.
        /// </summary>
        /// <remarks>
        /// This endpoint fetches the staked amount from the blockchain.
        /// </remarks>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
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
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task StakePostAsync(
            BzzValue amount,
            XDaiValue? gasPrice = null,
            ulong? gasLimit = null,
            CancellationToken cancellationToken = default);
        
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Withdraw the extra withdrawable staked amount.
        /// </summary>
        /// <remarks>
        /// This endpoint withdraws any amount that is possible to withdraw as surplus.
        /// </remarks>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <param name="gasLimit">Gas limit for transaction</param>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task StakeWithdrawableDeleteAsync(XDaiValue? gasPrice = null, ulong? gasLimit = null, CancellationToken cancellationToken = default);
        
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Get the withdrawable staked amount.
        /// </summary>
        /// <remarks>
        /// This endpoint fetches any amount that is possible to withdraw as surplus.
        /// </remarks>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task StakeWithdrawableGetAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the current status snapshot of this node.
        /// </summary>
        /// <returns>Returns the current node status snapshot.</returns>
        Task<StatusNode> StatusNodeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the current status snapshot of this node connected peers.
        /// </summary>
        /// <returns>Returns the status snapshot of this node connected peers</returns>
        Task<StatusNode[]> StatusPeersAsync(CancellationToken cancellationToken = default);

        Task SubscribeToGsocAsync(
            string reference,
            CancellationToken cancellationToken = default);

        /// <summary>Subscribe for messages on the given topic.</summary>
        /// <param name="topic">Topic name</param>
        /// <returns>Returns a WebSocket with a subscription for incoming message data on the requested topic.</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task SubscribeToPssAsync(
            string topic,
            CancellationToken cancellationToken = default);

        /// <summary>Top up an existing postage batch.</summary>
        /// <param name="batchId">Batch ID to top up</param>
        /// <param name="amount">Amount of BZZ per chunk to top up to an existing postage batch.</param>
        /// <returns>Returns the tx hash updating the postage batch</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<EthTxHash> TopUpPostageBatchAsync(
            PostageBatchId batchId, 
            BzzValue amount,
            XDaiValue? gasPrice = null,
            ulong? gasLimit = null,
            CancellationToken cancellationToken = default);

        /// <summary>Try connection to node</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <returns>Returns round trip time for given peer</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<string> TryConnectToPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default);

        /// <summary>Find feed update</summary>
        /// <param name="owner">Owner</param>
        /// <param name="topic">Topic</param>
        /// <param name="at">Timestamp of the update (default: now)</param>
        /// <param name="after">Start index (default: 0)</param>
        /// <param name="type">Feed indexing scheme (default: sequence)</param>
        /// <returns>Latest feed update</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<FileResponse?> TryGetFeedAsync(
            EthAddress owner,
            SwarmFeedTopic topic,
            long? at = null,
            ulong? after = null,
            int? afterLevel = null,
            SwarmFeedType type = SwarmFeedType.Sequence,
            bool? swarmOnlyRootChunk = null,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Try to get file headers
        /// </summary>
        /// <param name="address">Swarm address of chunk</param>
        /// <returns>Headers with values</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<HttpContentHeaders?> TryGetFileHeadersAsync(
            SwarmAddress address,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,  
            RedundancyLevel? redundancyLevel = null,
            RedundancyStrategy? redundancyStrategy = null, 
            bool? redundancyStrategyFallback = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Try to get the byte length of a file
        /// </summary>
        /// <param name="address">Swarm address of chunk</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Byte size of file</returns>
        Task<long?> TryGetFileSizeAsync(
            SwarmAddress address,
            long? swarmActTimestamp = null,
            string? swarmActPublisher = null,
            string? swarmActHistoryAddress = null,
            RedundancyLevel? redundancyLevel = null,
            RedundancyStrategy? redundancyStrategy = null,
            bool? redundancyStrategyFallback = null,
            CancellationToken cancellationToken = default);

        /// <summary>Update Total Count and swarm hash for a tag of an input stream of unknown size using Uid</summary>
        /// <param name="uid">Uid</param>
        /// <param name="hash">Can contain swarm hash to use for the tag</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task UpdateTagAsync(
            TagId id,
            SwarmHash? hash = null,
            CancellationToken cancellationToken = default);

        /// <summary>Upload data</summary>
        /// <param name="body"></param>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="tagId">Associate upload with an existing Tag UID</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.</param>
        /// <param name="swarmEncrypt">Represents the encrypting state of the file</param>
        /// <param name="swarmDeferredUpload">Determines if the uploaded data should be sent to the network immediately or in a deferred fashion. By default the upload will be deferred.</param>
        /// <param name="swarmRedundancyLevel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Content reference</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<SwarmReference> UploadBytesAsync(
            Stream body,
            PostageBatchId batchId,
            ushort? compactLevel = 0,
            TagId? tagId = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            bool? swarmDeferredUpload = null,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            CancellationToken cancellationToken = default);

        /// <summary>Upload Chunk</summary>
        /// <param name="chunkData"></param>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="tagId">Associate upload with an existing Tag UID</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Content reference</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<SwarmHash> UploadChunkAsync(
            Stream chunkData,
            PostageBatchId? batchId,
            TagId? tagId = null,
            PostageStamp? presignedPostageStamp = null,
            bool? swarmAct = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default);

        /// <summary>Upload Chunk</summary>
        /// <param name="chunk"></param>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="tagId">Associate upload with an existing Tag UID</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Content reference</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<SwarmHash> UploadChunkAsync(
            SwarmCac chunk,
            PostageBatchId? batchId,
            TagId? tagId = null,
            PostageStamp? presignedPostageStamp = null,
            bool? swarmAct = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default);

        /// <summary>Upload a directory</summary>
        /// <param name="directoryPath">The directory path</param>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="tagId">Associate upload with an existing Tag UID</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.</param>
        /// <param name="swarmEncrypt">Represents the encrypting state of the file</param>
        /// <param name="swarmIndexDocument">Default file to be referenced on path, if exists under that path</param>
        /// <param name="swarmErrorDocument">Configure custom error document to be returned when a specified path can not be found in collection</param>
        /// <param name="swarmDeferredUpload">Determines if the uploaded data should be sent to the network immediately or in a deferred fashion. By default the upload will be deferred.</param>
        /// <param name="swarmRedundancyLevel">Add redundancy to the data being uploaded so that downloaders can download it with better UX. 0 value is default and does not add any redundancy to the file.</param>
        /// <param name="swarmAct"></param>
        /// <param name="swarmActHistoryAddress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Content reference</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<SwarmReference> UploadDirectoryAsync(
            string directoryPath,
            PostageBatchId batchId,
            ushort? compactLevel = 0,
            TagId? tagId = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            string? swarmIndexDocument = null,
            string? swarmErrorDocument = null,
            bool? swarmDeferredUpload = null,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            bool? swarmAct = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default);

        /// <summary>Upload feed root manifest</summary>
        /// <param name="feed">Feed</param>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.</param>
        /// <returns>Reference hash</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<SwarmHash> UploadFeedManifestAsync(
            SwarmFeedBase feed,
            PostageBatchId batchId,
            ushort? compactLevel = 0,
            bool swarmPin = false,
            bool? swarmAct = null,
            string? swarmActHistoryAddress = null,
            CancellationToken cancellationToken = default);

        /// <summary>Upload a file</summary>
        /// <param name="content">Input file content</param>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="name">Filename when uploading single file</param>
        /// <param name="contentType">The specified content-type is preserved for download of the asset</param>
        /// <param name="isFileCollection">Upload file/files as a collection</param>
        /// <param name="tagId">Associate upload with an existing Tag UID</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.</param>
        /// <param name="swarmEncrypt">Represents the encrypting state of the file</param>
        /// <param name="swarmIndexDocument">Default file to be referenced on path, if exists under that path</param>
        /// <param name="swarmErrorDocument">Configure custom error document to be returned when a specified path can not be found in collection</param>
        /// <param name="swarmDeferredUpload">Determines if the uploaded data should be sent to the network immediately or in a deferred fashion. By default the upload will be deferred.</param>
        /// <param name="swarmRedundancyLevel">Add redundancy to the data being uploaded so that downloaders can download it with better UX. 0 value is default and does not add any redundancy to the file.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Content reference</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<SwarmReference> UploadFileAsync(
            Stream content,
            PostageBatchId batchId,
            ushort? compactLevel = 0,
            string? name = null,
            string? contentType = null,
            bool isFileCollection = false,
            TagId? tagId = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            string? swarmIndexDocument = null,
            string? swarmErrorDocument = null,
            bool? swarmDeferredUpload = null,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            CancellationToken cancellationToken = default);

        /// <summary>Upload single owner chunk</summary>
        /// <param name="soc">Single owner chunk</param>
        /// <param name="batchId"></param>
        /// <returns>Reference hash</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<SwarmHash> UploadSocAsync(
            SwarmSoc soc,
            PostageBatchId? batchId,
            PostageStamp? presignedPostageStamp = null,
            bool? swarmAct = null,
            string? swarmActHistoryAddress = null,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Allows withdrawals of BZZ to provided (whitelisted) address
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="address"></param>
        /// <param name="coin"></param>
        /// <returns>Tx hash</returns>
        Task<EthTxHash> WalletBzzWithdrawAsync(
            BzzValue amount,
            EthAddress address,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Allows withdrawals of xDAI to provided (whitelisted) address
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="address"></param>
        /// <param name="coin"></param>
        /// <returns>Tx hash</returns>
        Task<EthTxHash> WalletNativeCoinWithdrawAsync(
            XDaiValue amount,
            EthAddress address,
            CancellationToken cancellationToken = default);

        /// <summary>Withdraw tokens from the chequebook to the overlay address</summary>
        /// <param name="amount">amount of tokens to withdraw</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Transaction hash of the withdraw transaction</returns>
        /// <exception cref="BeeNetApiException">A server side error occurred.</exception>
        Task<EthTxHash> WithdrawFromChequebookAsync(
            BzzValue amount,
            XDaiValue? gasPrice = null,
            CancellationToken cancellationToken = default);
    }
}