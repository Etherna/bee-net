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

using Etherna.BeeNet.DtoModels;
using Etherna.BeeNet.InputModels;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Clients.GatewayApi
{
    public interface IBeeGatewayClient
    {
        // Properties.
        void SetAuthToken(string token);

        // Methods.
        /// <summary>Authenticate - This endpoint is experimental</summary>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<AuthDto> AuthenticateAsync(string role, int expiry);

        /// <summary>Buy a new postage batch.</summary>
        /// <param name="amount">Amount of BZZ added that the postage batch will have.</param>
        /// <param name="depth">Batch depth which specifies how many chunks can be signed with the batch. It is a logarithm. Must be higher than default bucket depth (16)</param>
        /// <param name="label">An optional label for this batch</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Returns the newly created postage batch ID</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> BuyPostageBatchAsync(
            long amount,
            int depth,
            string? label = null,
            bool? immutable = null,
            long? gasPrice = null,
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
            long? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Check if chunk at address exists locally
        /// </summary>
        /// <param name="address">Swarm address of chunk</param>
        /// <returns>Chunk exists</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<bool> CheckChunkExistsAsync(
            string address,
            CancellationToken cancellationToken = default);

        /// <summary>Check if content is available</summary>
        /// <param name="reference">Root hash of content (can be of any type: collection, file, chunk)</param>
        /// <returns>Returns if the content is retrievable</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<StewardShipGetDto> CheckIsContentAvailableAsync(
            string reference,
            CancellationToken cancellationToken = default);

        /// <summary>Connect to address</summary>
        /// <param name="address">Underlay address of peer</param>
        /// <returns>Returns overlay address of connected peer</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> ConnectToPeerAsync(
            string address,
            CancellationToken cancellationToken = default);

        /// <summary>Create an initial feed root manifest</summary>
        /// <param name="owner">Owner</param>
        /// <param name="topic">Topic</param>
        /// <param name="swarmPostageBatchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="type">Feed indexing scheme (default: sequence)</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <returns>Reference hash</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> CreateFeedAsync(
            string owner,
            string topic,
            string swarmPostageBatchId,
            string? type = null,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default);

        /// <summary>Pin the root hash with the given reference</summary>
        /// <param name="reference">Swarm reference of the root hash</param>
        /// <returns>Pin already exists, so no operation</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<MessageResponseDto> CreatePinAsync(
            string reference,
            CancellationToken cancellationToken = default);

        /// <summary>Create Tag</summary>
        /// <returns>New Tag Info</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<TagInfoDto> CreateTagAsync(
            string address,
            CancellationToken cancellationToken = default);

        /// <summary>Remove peer</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Disconnected peer</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<MessageResponseDto> DeletePeerAsync(
            string address,
            CancellationToken cancellationToken = default);

        /// <summary>Unpin the root hash with the given reference</summary>
        /// <param name="reference">Swarm reference of the root hash</param>
        /// <returns>Unpinning root hash with reference</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<MessageResponseDto> DeletePinAsync(
            string reference,
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
            long? gasPrice = null,
            CancellationToken cancellationToken = default);

        /// <summary>Deposit tokens from overlay address into chequebook</summary>
        /// <param name="amount">amount of tokens to deposit</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Transaction hash of the deposit transaction</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> DepositIntoChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default);

        /// <summary>Dilute an existing postage batch.</summary>
        /// <param name="id">Batch ID to dilute</param>
        /// <param name="depth">New batch depth. Must be higher than the previous depth.</param>
        /// <returns>Returns the postage batch ID that was diluted.</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> DilutePostageBatchAsync(
            string id, 
            int depth,
            long? gasPrice = null,
            long? gasLimit = null,
            CancellationToken cancellationToken = default);

        /// <summary>Get overlay and underlay addresses of the node</summary>
        /// <returns>Own node underlay and overlay addresses</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<AddressDetailDto> GetAddressesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get the balances with all known peers including prepaid services</summary>
        /// <returns>Own balances with all known peers</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<BalanceDto>> GetAllBalancesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get last cheques for all peers</summary>
        /// <returns>Last cheques</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<ChequeBookChequeGetDto>> GetAllChequeBookChequesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get the past due consumption balances with all known peers</summary>
        /// <returns>Own past due consumption balances with all known peers</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<BalanceDto>> GetAllConsumedBalancesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get a list of peers</summary>
        /// <returns>Returns overlay addresses of connected peers</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<string>> GetAllPeerAddressesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get the list of pinned root hash references</summary>
        /// <returns>List of pinned root hash references</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<string>> GetAllPinsAsync(CancellationToken cancellationToken = default);

        /// <summary>Get settlements with all known peers and total amount sent or received</summary>
        /// <returns>Settlements with all known peers and total amount sent or received</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SettlementDto> GetAllSettlementsAsync(CancellationToken cancellationToken = default);

        /// <summary>Get time based settlements with all known peers and total amount sent or received</summary>
        /// <returns>Time based settlements with all known peers and total amount sent or received</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<TimeSettlementsDto> GetAllTimeSettlementsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all globally available batches that were purchased by all nodes.
        /// </summary>
        /// <returns></returns>
        /// <returns>Returns an array of all available and currently valid postage batches.</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<PostageBatchShortDto>> GetAllValidPostageBatchesFromAllNodesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get the balances with a specific peer including prepaid services</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Balance with the specific peer</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<BalanceDto> GetBalanceWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default);

        /// <summary>Get referenced data</summary>
        /// <param name="reference">Swarm address reference to content</param>
        /// <param name="swarmCache">Determines if the download data should be cached on the node. By default the download will be cached</param>
        /// <param name="swarmRedundancyStrategy">Specify the retrieve strategy on redundant data. The numbers stand for NONE, DATA, PROX and RACE, respectively. Strategy NONE means no prefetching takes place. Strategy DATA means only data chunks are prefetched. Strategy PROX means only chunks that are close to the node are prefetched. Strategy RACE means all chunks are prefetched: n data chunks and k parity chunks. The first n chunks to arrive are used to reconstruct the file. Multiple strategies can be used in a fallback cascade if the swarm redundancy fallback mode is set to true. The default strategy is NONE, DATA, falling back to PROX, falling back to RACE</param>
        /// <param name="swarmRedundancyFallbackMode">Specify if the retrieve strategies (chunk prefetching on redundant data) are used in a fallback cascade. The default is true.</param>
        /// <param name="swarmChunkRetrievalTimeout">Specify the timeout for chunk retrieval. The default is 30 seconds.</param>
        /// <returns>Retrieved content specified by reference</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<Stream> GetBytesAsync(
            string reference,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Requests the headers containing the content type and length for the reference
        /// </summary>
        /// <param name="address">Swarm address of chunk</param>
        /// <returns>Chunk exists</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task GetBytesHeadAsync(string address, CancellationToken cancellationToken = default);

        /// <summary>Get a list of blocklisted peers</summary>
        /// <returns>Returns overlay addresses of blocklisted peers</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<string>> GetBlocklistedPeerAddressesAsync(CancellationToken cancellationToken = default);

        /// <summary>Get chain state</summary>
        /// <returns>Chain State</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ChainStateDto> GetChainStateAsync(CancellationToken cancellationToken = default);

        /// <summary>Get the address of the chequebook contract used</summary>
        /// <returns>Ethereum address of chequebook contract</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> GetChequeBookAddressAsync(CancellationToken cancellationToken = default);

        /// <summary>Get the balance of the chequebook</summary>
        /// <returns>Balance of the chequebook</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ChequeBookBalanceDto> GetChequeBookBalanceAsync(CancellationToken cancellationToken = default);

        /// <summary>Get last cashout action for the peer</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <returns>Cashout status</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ChequeBookCashoutGetDto> GetChequeBookCashoutForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default);

        /// <summary>Get last cheques for the peer</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <returns>Last cheques</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ChequeBookChequeGetDto> GetChequeBookChequeForPeerAsync(
            string peerId,
            CancellationToken cancellationToken = default);

        /// <summary>Get Chunk</summary>
        /// <param name="reference">Swarm address of chunk</param>
        /// <returns>Retrieved chunk content</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<Stream> GetChunkAsync(
            string reference,
            bool? swarmCache = null,
            CancellationToken cancellationToken = default);

        /// <summary>Get the past due consumption balance with a specific peer</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Past-due consumption balance with the specific peer</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<BalanceDto> GetConsumedBalanceWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default);

        /// <summary>Find feed update</summary>
        /// <param name="owner">Owner</param>
        /// <param name="topic">Topic</param>
        /// <param name="at">Timestamp of the update (default: now)</param>
        /// <param name="after">Start index (default: 0)</param>
        /// <param name="type">Feed indexing scheme (default: sequence)</param>
        /// <returns>Latest feed update</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> GetFeedAsync(
            string owner,
            string topic,
            int? at = null,
            int? after = null,
            string? type = null,
            CancellationToken cancellationToken = default);

        /// <summary>Get file or index document from a collection of files</summary>
        /// <param name="reference">Swarm address of content</param>
        /// <param name="swarmCache">Determines if the download data should be cached on the node. By default the download will be cached</param>
        /// <param name="swarmRedundancyStrategy">Specify the retrieve strategy on redundant data. The numbers stand for NONE, DATA, PROX and RACE, respectively. Strategy NONE means no prefetching takes place. Strategy DATA means only data chunks are prefetched. Strategy PROX means only chunks that are close to the node are prefetched. Strategy RACE means all chunks are prefetched: n data chunks and k parity chunks. The first n chunks to arrive are used to reconstruct the file. Multiple strategies can be used in a fallback cascade if the swarm redundancy fallback mode is set to true. The default strategy is NONE, DATA, falling back to PROX, falling back to RACE</param>
        /// <param name="swarmRedundancyFallbackMode">Specify if the retrieve strategies (chunk prefetching on redundant data) are used in a fallback cascade. The default is true.</param>
        /// <param name="swarmChunkRetrievalTimeout">Specify the timeout for chunk retrieval. The default is 30 seconds.</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<FileResponseDto> GetFileAsync(
            string reference,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Get the headers containing the content type and length for the reference
        /// </summary>
        /// <param name="address">Swarm address of chunk</param>
        /// <returns>Chunk exists</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task GetFileHeadAsync(string address, CancellationToken cancellationToken = default);

        /// <summary>Get referenced file from a collection of files</summary>
        /// <param name="reference">Swarm address of content</param>
        /// <param name="path">Path to the file in the collection.</param>
        /// <param name="swarmRedundancyStrategy">Specify the retrieve strategy on redundant data. The numbers stand for NONE, DATA, PROX and RACE, respectively. Strategy NONE means no prefetching takes place. Strategy DATA means only data chunks are prefetched. Strategy PROX means only chunks that are close to the node are prefetched. Strategy RACE means all chunks are prefetched: n data chunks and k parity chunks. The first n chunks to arrive are used to reconstruct the file. Multiple strategies can be used in a fallback cascade if the swarm redundancy fallback mode is set to true. The default strategy is NONE, DATA, falling back to PROX, falling back to RACE</param>
        /// <param name="swarmRedundancyFallbackMode">Specify if the retrieve strategies (chunk prefetching on redundant data) are used in a fallback cascade. The default is true.</param>
        /// <param name="swarmChunkRetrievalTimeout">Specify the timeout for chunk retrieval. The default is 30 seconds.</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<FileResponseDto> GetFileWithPathAsync(
            string reference,
            string path,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default);

        /// <summary>Get health of node</summary>
        /// <returns>Health State of node</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<VersionDto> GetHealthAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get information about the node
        /// </summary>
        /// <returns>Information about the node</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<NodeInfoDto> GetNodeInfoAsync(CancellationToken cancellationToken = default);

        /// <summary>Get all owned postage batches by this node</summary>
        /// <returns>List of all owned postage batches</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred</exception>
        Task<IEnumerable<PostageBatchDto>> GetOwnedPostageBatchesByNodeAsync(CancellationToken cancellationToken = default);

        /// <summary>Get list of pending transactions</summary>
        /// <returns>List of pending transactions</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<PendingTransactionDto>> GetPendingTransactionsAsync(CancellationToken cancellationToken = default);

        /// <summary>Get pinning status of the root hash with the given reference</summary>
        /// <param name="reference">Swarm reference of the root hash</param>
        /// <returns>Reference of the pinned root hash</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> GetPinStatusAsync(
            string reference,
            CancellationToken cancellationToken = default);

        /// <summary>Get an individual postage batch status</summary>
        /// <param name="id">Swarm address of the stamp</param>
        /// <returns>Returns an individual postage batch state</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<PostageBatchDto> GetPostageBatchAsync(
            string id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get reserve commitment hash with sample proofs
        /// </summary>
        /// <param name="depth">The storage depth.</param>
        /// <param name="anchor1">The first anchor.</param>
        /// <param name="anchor2">The second anchor.</param>
        /// <returns>Reserve sample response</returns>
        /// <returns></returns>
        Task<ReserveCommitmentDto> GetReserveCommitmentAsync(
            int depth,
            string anchor1,
            string anchor2,
            CancellationToken cancellationToken = default);

        /// <summary>Get reserve state</summary>
        /// <returns>Reserve State</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ReserveStateDto> GetReserveStateAsync(CancellationToken cancellationToken = default);

        /// <summary>Get amount of sent and received from settlements with a peer</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Amount of sent or received from settlements with a peer</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SettlementDataDto> GetSettlementsWithPeerAsync(
            string address,
            CancellationToken cancellationToken = default);

        /// <summary>Get extended bucket data of a batch</summary>
        /// <param name="batchId">Swarm address of the stamp</param>
        /// <returns>Returns extended bucket data of the provided batch ID</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<StampsBucketsDto> GetStampsBucketsForBatchAsync(
            string batchId,
            CancellationToken cancellationToken = default);

        /// <returns>Swarm topology of the bee node</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<TopologyDto> GetSwarmTopologyAsync(CancellationToken cancellationToken = default);

        /// <summary>Get Tag information using Uid</summary>
        /// <param name="uid">Uid</param>
        /// <returns>Tag info</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<TagInfoDto> GetTagInfoAsync(
            long uid,
            CancellationToken cancellationToken = default);

        /// <summary>Get list of tags</summary>
        /// <param name="offset">The number of items to skip before starting to collect the result set.</param>
        /// <param name="limit">The numbers of items to return.</param>
        /// <returns>List of tags</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<TagInfoDto>> GetTagsListAsync(
            int? offset = null,
            int? limit = null,
            CancellationToken cancellationToken = default);

        /// <summary>Get information about a sent transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <returns>Get info about transaction</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<TransactionsDto> GetTransactionInfoAsync(
            string txHash,
            CancellationToken cancellationToken = default);

        /// <summary>Get configured P2P welcome message</summary>
        /// <returns>Welcome message</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> GetWelcomeMessageAsync(CancellationToken cancellationToken = default);

        /// <summary>Rebroadcast existing transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <returns>Hash of the transaction</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> RebroadcastTransactionAsync(
            string txHash,
            CancellationToken cancellationToken = default);

        /// <summary>Refresh the auth token - This endpoint is experimental</summary>
        /// <returns>Key</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> RefreshAuthAsync(
            string role, 
            int expiry,
            CancellationToken cancellationToken = default);

        /// <summary>Reupload a root hash to the network</summary>
        /// <param name="reference">Root hash of content (can be of any type: collection, file, chunk)</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task ReuploadContentAsync(
            string reference,
            CancellationToken cancellationToken = default);

        /// <summary>Send to recipient or target with Postal Service for Swarm</summary>
        /// <param name="topic">Topic name</param>
        /// <param name="targets">Target message address prefix. If multiple targets are specified, only one would be matched.</param>
        /// <param name="swarmPostageBatchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="recipient">Recipient publickey</param>
        /// <returns>Subscribed to topic</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task SendPssAsync(
            string topic,
            string targets,
            string swarmPostageBatchId,
            string? recipient = null,
            CancellationToken cancellationToken = default);

        /// <summary>Set P2P welcome message</summary>
        /// <returns>OK</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<VersionDto> SetWelcomeMessageAsync(
            string welcomeMessage,
            CancellationToken cancellationToken = default);

        /// <summary>Subscribe for messages on the given topic.</summary>
        /// <param name="topic">Topic name</param>
        /// <returns>Returns a WebSocket with a subscription for incoming message data on the requested topic.</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task SubscribeToPssAsync(
            string topic,
            CancellationToken cancellationToken = default);

        /// <summary>Top up an existing postage batch.</summary>
        /// <param name="id">Batch ID to top up</param>
        /// <param name="amount">Amount of BZZ per chunk to top up to an existing postage batch.</param>
        /// <returns>Returns the postage batch ID that was topped up</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> TopUpPostageBatchAsync(
            string id, 
            long amount,
            long? gasPrice,
            long? gasLimit,
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
        /// <param name="address">Can contain swarm hash to use for the tag</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<VersionDto> UpdateTagAsync(
            long uid,
            string? address = null,
            CancellationToken cancellationToken = default);

        /// <summary>Upload Chunk</summary>
        /// <param name="swarmPostageBatchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="swarmTag">Associate upload with an existing Tag UID</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarmDeferredUpload">Determines if the uploaded data should be sent to the network immediately or in a deferred fashion. By default the upload will be deferred.</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> UploadChunkAsync(
            string swarmPostageBatchId,
            long? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmDeferredUpload = null,
            Stream? body = null,
            CancellationToken cancellationToken = default);

        /// <summary>Upload stream of chunks</summary>
        /// <param name="swarmPostageBatchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="swarmTag">Associate upload with an existing Tag UID</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <returns>Returns a Websocket connection on which stream of chunks can be uploaded. Each chunk sent is acknowledged using a binary response `0` which serves as confirmation of upload of single chunk. Chunks should be packaged as binary messages for uploading.</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task UploadChunksStreamAsync(
            string swarmPostageBatchId,
            int? swarmTag = null,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default);

        /// <summary>Upload data</summary>
        /// <param name="swarmPostageBatchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="swarmTag">Associate upload with an existing Tag UID</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarmEncrypt">Represents the encrypting state of the file
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarmDeferredUpload">Determines if the uploaded data should be sent to the network immediately or in a deferred fashion. By default the upload will be deferred.</param>
        /// <returns>Reference hash</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> UploadBytesAsync(
            string swarmPostageBatchId,
            Stream body,
            int? swarmTag = null, 
            bool? swarmPin = null, 
            bool? swarmEncrypt = null, 
            bool? swarmDeferredUpload = null,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None0,
            CancellationToken cancellationToken = default);

        /// <summary>Upload file or a collection of files</summary>
        /// <param name="swarmPostageBatchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="content">Input file content</param>
        /// <param name="name">Filename when uploading single file</param>
        /// <param name="contentType">The specified content-type is preserved for download of the asset</param>
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
        Task<string> UploadFileAsync(string swarmPostageBatchId,
            Stream content,
            string? name = null,
            string? contentType = null,
            int? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            bool? swarmCollection = null,
            string? swarmIndexDocument = null,
            string? swarmErrorDocument = null,
            bool? swarmDeferredUpload = null,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None0,
            CancellationToken cancellationToken = default);

        /// <summary>Upload single owner chunk</summary>
        /// <param name="owner">Owner</param>
        /// <param name="id">Id</param>
        /// <param name="sig">Signature</param>
        /// <param name="body">The SOC binary data is composed of the span (8 bytes) and the at most 4KB payload.</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <returns>Reference hash</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> UploadSocAsync(
            string owner,
            string id,
            string sig,
            string swarmPostageBatchId,
            Stream body,
            bool? swarmPin = null,
            CancellationToken cancellationToken = default);

        /// <summary>Withdraw tokens from the chequebook to the overlay address</summary>
        /// <param name="amount">amount of tokens to withdraw</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Transaction hash of the withdraw transaction</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> WithdrawFromChequeBookAsync(
            long amount,
            long? gasPrice = null,
            CancellationToken cancellationToken = default);
    }
}
