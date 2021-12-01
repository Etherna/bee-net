using System;
using System.Collections.Generic;
using System.Text;
using TestAdapter.Dtos.GatewayApi;

namespace TestAdapter.Clients
{
    public interface IFacadeBeeGatewayApiClient
    {

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Authenticate - This endpoint is experimental</summary>
        /// <returns>Ok</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<AuthResponse> AuthAsync(BodyDto body, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Refresh the auth token - This endpoint is experimental</summary>
        /// <returns>Ok</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<RefreshResponse> RefreshAsync(BodyDto body, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Upload data</summary>
        /// <param name="swarm_tag">Associate upload with an existing Tag UID</param>
        /// <param name="swarm_pin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarm_encrypt">Represents the encrypting state of the file
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarm_postage_batch_id">ID of Postage Batch that is used to upload data with</param>
        /// <returns>Ok</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<BytesPOSTResponse> BytesPOSTAsync(int? swarm_tag, bool? swarm_pin, bool? swarm_encrypt, string swarm_postage_batch_id, System.IO.Stream body, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get referenced data</summary>
        /// <param name="reference">Swarm address reference to content</param>
        /// <returns>Retrieved content specified by reference</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<FileResponse> BytesGETAsync(string reference, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get Chunk</summary>
        /// <param name="reference">Swarm address of chunk</param>
        /// <param name="targets">Global pinning targets prefix</param>
        /// <returns>Retrieved chunk content</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<FileResponse> ChunksGETAsync(string reference, string targets, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Upload Chunk</summary>
        /// <param name="swarm_tag">Associate upload with an existing Tag UID</param>
        /// <param name="swarm_pin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarm_postage_batch_id">ID of Postage Batch that is used to upload data with</param>
        /// <returns>Ok</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<ChunksPOSTResponse> ChunksPOSTAsync(int? swarm_tag, bool? swarm_pin, string swarm_postage_batch_id, System.IO.Stream body, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Upload stream of chunks</summary>
        /// <param name="swarm_tag">Associate upload with an existing Tag UID</param>
        /// <param name="swarm_pin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarm_postage_batch_id">ID of Postage Batch that is used to upload data with</param>
        /// <returns>Returns a Websocket connection on which stream of chunks can be uploaded. Each chunk sent is acknowledged using a binary response `0` which serves as confirmation of upload of single chunk. Chunks should be packaged as binary messages for uploading.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task StreamAsync(int? swarm_tag, bool? swarm_pin, string swarm_postage_batch_id, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Upload file or a collection of files</summary>
        /// <param name="name">Filename when uploading single file</param>
        /// <param name="swarm_tag">Associate upload with an existing Tag UID</param>
        /// <param name="swarm_pin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarm_encrypt">Represents the encrypting state of the file
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="content_Type">The specified content-type is preserved for download of the asset</param>
        /// <param name="swarm_collection">Upload file/files as a collection</param>
        /// <param name="swarm_index_document">Default file to be referenced on path, if exists under that path</param>
        /// <param name="swarm_error_document">Configure custom error document to be returned when a specified path can not be found in collection</param>
        /// <param name="swarm_postage_batch_id">ID of Postage Batch that is used to upload data with</param>
        /// <returns>Ok</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<BzzPOSTAResponse> BzzPOSTAsync(string name, int? swarm_tag, bool? swarm_pin, bool? swarm_encrypt, string content_Type, bool? swarm_collection, string swarm_index_document, string swarm_error_document, string swarm_postage_batch_id, IEnumerable<FileParameterDto> file, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Reupload a root hash to the network; deprecated: use /stewardship/{reference} instead</summary>
        /// <param name="reference">Root hash of content (can be of any type: collection, file, chunk)</param>
        /// <returns>Ok</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        [System.Obsolete]
        System.Threading.Tasks.Task BzzPATCHAsync(string reference, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get file or index document from a collection of files</summary>
        /// <param name="reference">Swarm address of content</param>
        /// <param name="targets">Global pinning targets prefix</param>
        /// <returns>Ok</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<FileResponse> BzzGETAsync(string reference, string targets, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get referenced file from a collection of files</summary>
        /// <param name="reference">Swarm address of content</param>
        /// <param name="path">Path to the file in the collection.</param>
        /// <param name="targets">Global pinning targets prefix</param>
        /// <returns>Ok</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<FileResponse> BzzGET2Async(string reference, string path, string targets, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get list of tags</summary>
        /// <param name="offset">The number of items to skip before starting to collect the result set.</param>
        /// <param name="limit">The numbers of items to return.</param>
        /// <returns>List of tags</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TagsGETResponse> TagsGETAsync(int? offset, int? limit, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Create Tag</summary>
        /// <returns>New Tag Info</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TagsPOSTResponse> TagsPOSTAsync(Body3Dto body, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get Tag information using Uid</summary>
        /// <param name="uid">Uid</param>
        /// <returns>Tag info</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TagsGET2Response> TagsGET2Async(int uid, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Delete Tag information using Uid</summary>
        /// <param name="uid">Uid</param>
        /// <returns>The resource was deleted successfully.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task TagsDELETEAsync(int uid, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Update Total Count and swarm hash for a tag of an input stream of unknown size using Uid</summary>
        /// <param name="uid">Uid</param>
        /// <param name="body">Can contain swarm hash to use for the tag</param>
        /// <returns>Ok</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<TagsPATCHResponse> TagsPATCHAsync(int uid, Body3Dto body, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Pin the root hash with the given reference</summary>
        /// <param name="reference">Swarm reference of the root hash</param>
        /// <returns>Pin already exists, so no operation</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<PinsPOSTResponse> PinsPOSTAsync(string reference, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Unpin the root hash with the given reference</summary>
        /// <param name="reference">Swarm reference of the root hash</param>
        /// <returns>Unpinning root hash with reference</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<PinsDELETEResponse> PinsDELETEAsync(string reference, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get pinning status of the root hash with the given reference</summary>
        /// <param name="reference">Swarm reference of the root hash</param>
        /// <returns>Reference of the pinned root hash</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<string> PinsGETAsync(string reference, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Get the list of pinned root hash references</summary>
        /// <returns>List of pinned root hash references</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<PinsGET2Response> PinsGET2Async(System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Send to recipient or target with Postal Service for Swarm</summary>
        /// <param name="topic">Topic name</param>
        /// <param name="targets">Target message address prefix. If multiple targets are specified, only one would be matched.</param>
        /// <param name="recipient">Recipient publickey</param>
        /// <param name="swarm_postage_batch_id">ID of Postage Batch that is used to upload data with</param>
        /// <returns>Subscribed to topic</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task SendAsync(string topic, string targets, string recipient, string swarm_postage_batch_id, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Subscribe for messages on the given topic.</summary>
        /// <param name="topic">Topic name</param>
        /// <returns>Returns a WebSocket with a subscription for incoming message data on the requested topic.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task SubscribeAsync(string topic, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Upload single owner chunk</summary>
        /// <param name="owner">Owner</param>
        /// <param name="id">Id</param>
        /// <param name="sig">Signature</param>
        /// <param name="swarm_pin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <returns>Created</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<SocResponse> SocAsync(string owner, string id, string sig, bool? swarm_pin, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Create an initial feed root manifest</summary>
        /// <param name="owner">Owner</param>
        /// <param name="topic">Topic</param>
        /// <param name="type">Feed indexing scheme (default: sequence)</param>
        /// <param name="swarm_pin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarm_postage_batch_id">ID of Postage Batch that is used to upload data with</param>
        /// <returns>Created</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<FeedsPOSTResponse> FeedsPOSTAsync(string owner, string topic, string type, bool? swarm_pin, string swarm_postage_batch_id, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Find feed update</summary>
        /// <param name="owner">Owner</param>
        /// <param name="topic">Topic</param>
        /// <param name="at">Timestamp of the update (default: now)</param>
        /// <param name="type">Feed indexing scheme (default: sequence)</param>
        /// <returns>Latest feed update</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<FeedsGETResponse> FeedsGETAsync(string owner, string topic, int? at, string type, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Check if content is available</summary>
        /// <param name="reference">Root hash of content (can be of any type: collection, file, chunk)</param>
        /// <returns>Returns if the content is retrievable</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<StewardshipGETResponse> StewardshipGETAsync(string reference, System.Threading.CancellationToken? cancellationToken);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Reupload a root hash to the network</summary>
        /// <param name="reference">Root hash of content (can be of any type: collection, file, chunk)</param>
        /// <returns>Ok</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task StewardshipPUTAsync(string reference, System.Threading.CancellationToken? cancellationToken);


    }
}
