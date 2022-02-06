using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Etherna.BeeNet.Clients.v1_4_1.GatewayApi;
using Etherna.BeeNet.DtoModel;

namespace Etherna.BeeNet.Clients
{
    public interface IBeeGatewayClient
    {
        /// <summary>Authenticate - This endpoint is experimental</summary>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<AuthDto> AuthenticateAsync(string role, int expiry);

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
        /// <returns>Created</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ReferenceDto> CreateFeedAsync(
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
        Task<IEnumerable<AddressDto>> GetAllPinsAsync();

        /// <summary>Get Chunk</summary>
        /// <param name="reference">Swarm address of chunk</param>
        /// <param name="targets">Global pinning targets prefix</param>
        /// <returns>Retrieved chunk content</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<Stream> GetChunkStreamAsync(
            string reference,
            string? targets = null);

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
        Task<ReferenceDto> GetFeedAsync(
            string owner,
            string topic,
            int? at = null,
            string? type = null);

        /// <summary>Get referenced file from a collection of files</summary>
        /// <param name="reference">Swarm address of content</param>
        /// <param name="path">Path to the file in the collection.</param>
        /// <param name="targets">Global pinning targets prefix</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<Stream> GetFileAsync(
            string reference,
            string path,
            string? targets = null);

        /// <summary>Get file or index document from a collection of files</summary>
        /// <param name="reference">Swarm address of content</param>
        /// <param name="targets">Global pinning targets prefix</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<Stream> GetFileAsync(string reference, string? targets = null);

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
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<RefreshDto> RefreshAuthAsync(string role, int expiry);

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
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ReferenceDto> UploadDataAsync(
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
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ReferenceDto> UploadFileAsync(
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
            IEnumerable<FileParameter>? file = null);

        /// <summary>Upload single owner chunk</summary>
        /// <param name="owner">Owner</param>
        /// <param name="id">Id</param>
        /// <param name="sig">Signature</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <returns>Created</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<ReferenceDto> UploadSocAsync(
            string owner,
            string id,
            string sig,
            bool? swarmPin = null);
    }
}
