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

using Etherna.BeeNet.Clients.DebugApi;
using Etherna.BeeNet.DtoModels;
using Etherna.BeeNet.InputModels;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Clients.GatewayApi
{
    public interface IBeeGatewayClient
    {
        // Properties.
        GatewayApiVersion CurrentApiVersion { get; set; }

        void SetAuthToken(string token);

        // Methods.
        /// <summary>Authenticate - This endpoint is experimental</summary>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<AuthDto> AuthenticateAsync(BeeAuthicationData beeAuthicationData, string role, int expiry);

        /// <summary>Check if content is available</summary>
        /// <param name="reference">Root hash of content (can be of any type: collection, file, chunk)</param>
        /// <returns>Returns if the content is retrievable</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<StewardShipGetDto> CheckIsContentAvailableAsync(string reference);

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
            bool? swarmPin = null);

        /// <summary>Pin the root hash with the given reference</summary>
        /// <param name="reference">Swarm reference of the root hash</param>
        /// <returns>Pin already exists, so no operation</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<MessageResponseDto> CreatePinAsync(string reference);

        /// <summary>Create Tag</summary>
        /// <returns>New Tag Info</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<TagInfoDto> CreateTagAsync(string address);

        /// <summary>Unpin the root hash with the given reference</summary>
        /// <param name="reference">Swarm reference of the root hash</param>
        /// <returns>Unpinning root hash with reference</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<MessageResponseDto> DeletePinAsync(string reference);

        /// <summary>Delete Tag information using Uid</summary>
        /// <param name="uid">Uid</param>
        /// <returns>The resource was deleted successfully.</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task DeleteTagAsync(int uid);

        /// <summary>Get the list of pinned root hash references</summary>
        /// <returns>List of pinned root hash references</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<string>> GetAllPinsAsync();

        /// <summary>Get Chunk</summary>
        /// <param name="reference">Swarm address of chunk</param>
        /// <returns>Retrieved chunk content</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<Stream> GetChunkStreamAsync(string reference);

        /// <summary>Get referenced data</summary>
        /// <param name="reference">Swarm address reference to content</param>
        /// <returns>Retrieved content specified by reference</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<Stream> GetDataAsync(string reference);

        /// <summary>Find feed update</summary>
        /// <param name="owner">Owner</param>
        /// <param name="topic">Topic</param>
        /// <param name="at">Timestamp of the update (default: now)</param>
        /// <param name="type">Feed indexing scheme (default: sequence)</param>
        /// <returns>Latest feed update</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> GetFeedAsync(
            string owner,
            string topic,
            int? at = null,
            string? type = null);

        /// <summary>Get referenced file from a collection of files</summary>
        /// <param name="reference">Swarm address of content</param>
        /// <param name="path">Path to the file in the collection.</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<Stream> GetFileWithPathAsync(string reference, string path);

        /// <summary>Get file or index document from a collection of files</summary>
        /// <param name="reference">Swarm address of content</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<Stream> GetFileAsync(string reference);

        /// <summary>Get pinning status of the root hash with the given reference</summary>
        /// <param name="reference">Swarm reference of the root hash</param>
        /// <returns>Reference of the pinned root hash</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> GetPinStatusAsync(string reference);

        /// <summary>Get Tag information using Uid</summary>
        /// <param name="uid">Uid</param>
        /// <returns>Tag info</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<TagInfoDto> GetTagInfoAsync(int uid);

        /// <summary>Get list of tags</summary>
        /// <param name="offset">The number of items to skip before starting to collect the result set.</param>
        /// <param name="limit">The numbers of items to return.</param>
        /// <returns>List of tags</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<TagInfoDto>> GetTagsListAsync(
            int? offset = null,
            int? limit = null);

        /// <summary>Refresh the auth token - This endpoint is experimental</summary>
        /// <returns>Key</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> RefreshAuthAsync(string role, int expiry);

        /// <summary>Reupload a root hash to the network</summary>
        /// <param name="reference">Root hash of content (can be of any type: collection, file, chunk)</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task ReuploadContentAsync(string reference);

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
            string? recipient = null);

        /// <summary>Subscribe for messages on the given topic.</summary>
        /// <param name="topic">Topic name</param>
        /// <returns>Returns a WebSocket with a subscription for incoming message data on the requested topic.</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task SubscribeToPssAsync(string topic);

        /// <summary>Update Total Count and swarm hash for a tag of an input stream of unknown size using Uid</summary>
        /// <param name="uid">Uid</param>
        /// <param name="address">Can contain swarm hash to use for the tag</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<VersionDto> UpdateTagAsync(
            int uid,
            string? address = null);

        /// <summary>Upload Chunk</summary>
        /// <param name="swarmPostageBatchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="swarmTag">Associate upload with an existing Tag UID</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarmDeferredUpload">Determines if the uploaded data should be sent to the network immediately or in a deferred fashion. By default the upload will be deferred.</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<VersionDto> UploadChunkAsync(
            string swarmPostageBatchId,
            int? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmDeferredUpload = null,
            Stream? body = null);

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
            bool? swarmPin = null);

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
        Task<string> UploadDataAsync(
            string swarmPostageBatchId, 
            int? swarmTag = null, 
            bool? swarmPin = null, 
            bool? swarmEncrypt = null, 
            bool? swarmDeferredUpload = null, 
            Stream? body = null);

        /// <summary>Upload file or a collection of files</summary>
        /// <param name="swarmPostageBatchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="name">Filename when uploading single file</param>
        /// <param name="swarmTag">Associate upload with an existing Tag UID</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarmEncrypt">Represents the encrypting state of the file
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="contentType">The specified content-type is preserved for download of the asset</param>
        /// <param name="swarmCollection">Upload file/files as a collection</param>
        /// <param name="swarmIndexDocument">Default file to be referenced on path, if exists under that path</param>
        /// <param name="swarmErrorDocument">Configure custom error document to be returned when a specified path can not be found in collection</param>
        /// <param name="swarmDeferredUpload">Determines if the uploaded data should be sent to the network immediately or in a deferred fashion. By default the upload will be deferred.</param>
        /// <returns>Reference hash</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> UploadFileAsync(
            string swarmPostageBatchId,
            string? name = null,
            int? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            string? contentType = null,
            bool? swarmCollection = null,
            string? swarmIndexDocument = null,
            string? swarmErrorDocument = null,
            bool? swarmDeferredUpload = null,
            IEnumerable<FileParameterInput>? file = null);

        /// <summary>Upload single owner chunk</summary>
        /// <param name="owner">Owner</param>
        /// <param name="id">Id</param>
        /// <param name="sig">Signature</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <returns>Reference hash</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> UploadSocAsync(
            string owner,
            string id,
            string sig,
            bool? swarmPin = null);

        // Methods.
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
            long? gasPrice = null);

        /// <summary>Cashout the last cheque for the peer</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <param name="gasLimit">Gas limit for transaction</param>
        /// <returns>Hash of the transaction</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> CashoutChequeForPeerAsync(
            string peerId,
            long? gasPrice = null,
            long? gasLimit = null);

        /// <summary>Connect to address</summary>
        /// <param name="address">Underlay address of peer</param>
        /// <returns>Returns overlay address of connected peer</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> ConnectToPeerAsync(string address);

        /// <summary>Delete a chunk from local storage</summary>
        /// <param name="address">Swarm address of chunk</param>
        /// <returns>Chunk exists</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<MessageResponseDto> DeleteChunkAsync(string address);

        /// <summary>Remove peer</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Disconnected peer</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<MessageResponseDto> DeletePeerAsync(string address);

        /// <summary>Cancel existing transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Hash of the transaction</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> DeleteTransactionAsync(
            string txHash,
            long? gasPrice = null);

        /// <summary>Deposit tokens from overlay address into chequebook</summary>
        /// <param name="amount">amount of tokens to deposit</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Transaction hash of the deposit transaction</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> DepositIntoChequeBookAsync(
            long amount,
            long? gasPrice = null);

        /// <summary>Dilute an existing postage batch.</summary>
        /// <param name="id">Batch ID to dilute</param>
        /// <param name="depth">New batch depth. Must be higher than the previous depth.</param>
        /// <returns>Returns the postage batch ID that was diluted.</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> DilutePostageBatchAsync(string id, int depth);

        /// <summary>Get overlay and underlay addresses of the node</summary>
        /// <returns>Own node underlay and overlay addresses</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<AddressDetailDto> GetAddressesAsync();

        /// <summary>Get the balances with all known peers including prepaid services</summary>
        /// <returns>Own balances with all known peers</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<BalanceDto>> GetAllBalancesAsync();

        /// <summary>Get last cheques for all peers</summary>
        /// <returns>Last cheques</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<ChequeBookChequeGetDto>> GetAllChequeBookChequesAsync();

        /// <summary>Get the past due consumption balances with all known peers</summary>
        /// <returns>Own past due consumption balances with all known peers</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<BalanceDto>> GetAllConsumedBalancesAsync();

        /// <summary>Get a list of peers</summary>
        /// <returns>Returns overlay addresses of connected peers</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<string>> GetAllPeerAddressesAsync();

        /// <summary>Get settlements with all known peers and total amount sent or received</summary>
        /// <returns>Settlements with all known peers and total amount sent or received</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SettlementDto> GetAllSettlementsAsync();

        /// <summary>Get time based settlements with all known peers and total amount sent or received</summary>
        /// <returns>Time based settlements with all known peers and total amount sent or received</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<TimeSettlementsDto> GetAllTimeSettlementsAsync();

        /// <summary>
        /// Get all globally available batches that were purchased by all nodes.
        /// </summary>
        /// <returns></returns>
        /// <returns>Returns an array of all available and currently valid postage batches.</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<PostageBatchShortDto>> GetAllValidPostageBatchesFromAllNodesAsync();

        /// <summary>Get the balances with a specific peer including prepaid services</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Balance with the specific peer</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<BalanceDto> GetBalanceWithPeerAsync(string address);

        /// <summary>Get a list of blocklisted peers</summary>
        /// <returns>Returns overlay addresses of blocklisted peers</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<string>> GetBlocklistedPeerAddressesAsync();

        /// <summary>Get chain state</summary>
        /// <returns>Chain State</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ChainStateDto> GetChainStateAsync();

        /// <summary>Get the address of the chequebook contract used</summary>
        /// <returns>Ethereum address of chequebook contract</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> GetChequeBookAddressAsync();

        /// <summary>Get the balance of the chequebook</summary>
        /// <returns>Balance of the chequebook</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ChequeBookBalanceDto> GetChequeBookBalanceAsync();

        /// <summary>Get last cashout action for the peer</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <returns>Cashout status</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ChequeBookCashoutGetDto> GetChequeBookCashoutForPeerAsync(string peerId);

        /// <summary>Get last cheques for the peer</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <returns>Last cheques</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ChequeBookChequeGetDto> GetChequeBookChequeForPeerAsync(string peerId);

        /// <summary>Check if chunk at address exists locally</summary>
        /// <param name="address">Swarm address of chunk</param>
        /// <returns>Chunk exists</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<FileResponseDto> GetChunkAsync(string address);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>
        /// Check if chunk at address exists locally
        /// </summary>
        /// <param name="address">Swarm address of chunk</param>
        /// <returns>Chunk exists</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<MessageResponseDto> ChunksHeadAsync(string address);

        /// <summary>Get the past due consumption balance with a specific peer</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Past-due consumption balance with the specific peer</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<BalanceDto> GetConsumedBalanceWithPeerAsync(string address);

        /// <summary>Get health of node</summary>
        /// <returns>Health State of node</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<VersionDto> GetHealthAsync();

        /// <summary>
        /// Get information about the node
        /// </summary>
        /// <returns>Information about the node</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<NodeInfoDto> GetNodeInfoAsync();

        /// <summary>Get all owned postage batches by this node</summary>
        /// <returns>List of all owned postage batches</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred</exception>
        Task<IEnumerable<PostageBatchDto>> GetOwnedPostageBatchesByNodeAsync();

        /// <summary>Get list of pending transactions</summary>
        /// <returns>List of pending transactions</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<PendingTransactionDto>> GetPendingTransactionsAsync();

        /// <summary>Get an individual postage batch status</summary>
        /// <param name="id">Swarm address of the stamp</param>
        /// <returns>Returns an individual postage batch state</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<PostageBatchDto> GetPostageBatchAsync(string id);

        /// <summary>Get reserve state</summary>
        /// <returns>Reserve State</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ReserveStateDto> GetReserveStateAsync();

        /// <summary>Get amount of sent and received from settlements with a peer</summary>
        /// <param name="address">Swarm address of peer</param>
        /// <returns>Amount of sent or received from settlements with a peer</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SettlementDataDto> GetSettlementsWithPeerAsync(string address);

        /// <summary>Get extended bucket data of a batch</summary>
        /// <param name="batchId">Swarm address of the stamp</param>
        /// <returns>Returns extended bucket data of the provided batch ID</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<StampsBucketsDto> GetStampsBucketsForBatchAsync(string batchId);

        /// <returns>Swarm topology of the bee node</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<TopologyDto> GetSwarmTopologyAsync();

        /// <summary>Get information about a sent transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <returns>Get info about transaction</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<TransactionsDto> GetTransactionInfoAsync(string txHash);

        /// <summary>Get configured P2P welcome message</summary>
        /// <returns>Welcome message</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> GetWelcomeMessageAsync();

        /// <summary>Rebroadcast existing transaction</summary>
        /// <param name="txHash">Hash of the transaction</param>
        /// <returns>Hash of the transaction</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> RebroadcastTransactionAsync(string txHash);

        /// <summary>Set P2P welcome message</summary>
        /// <returns>OK</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<VersionDto> SetWelcomeMessageAsync(string welcomeMessage);

        /// <summary>Top up an existing postage batch.</summary>
        /// <param name="id">Batch ID to top up</param>
        /// <param name="amount">Amount of BZZ per chunk to top up to an existing postage batch.</param>
        /// <returns>Returns the postage batch ID that was topped up</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> TopUpPostageBatchAsync(string id, long amount);

        /// <summary>Try connection to node</summary>
        /// <param name="peerId">Swarm address of peer</param>
        /// <returns>Returns round trip time for given peer</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> TryConnectToPeerAsync(string peerId);

        /// <summary>Withdraw tokens from the chequebook to the overlay address</summary>
        /// <param name="amount">amount of tokens to withdraw</param>
        /// <param name="gasPrice">Gas price for transaction</param>
        /// <returns>Transaction hash of the withdraw transaction</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<string> WithdrawFromChequeBookAsync(
            long amount,
            long? gasPrice = null);
    }
}
