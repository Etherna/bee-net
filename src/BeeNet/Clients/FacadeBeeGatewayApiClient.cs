using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestAdapter.Clients;
using TestAdapter.Dtos.GatewayApi;

namespace TestAdapter
{
    public class FacadeBeeGatewayApiClient : IFacadeBeeGatewayApiClient
    {
        readonly IFacadeBeeGatewayApiClient _beeDebugClient;
        readonly BeeVersionEnum _beeVersion;

        public FacadeBeeGatewayApiClient(string version, HttpClient httpClient, string baseUrl)
        {
            if (version == "1.4")
            {
                _beeVersion = BeeVersionEnum.v1_4;
                _beeDebugClient = new AdapterGatewayApiClient_1_4(httpClient, baseUrl);
            }
            else if (version == "1.5")
            {
                _beeVersion = BeeVersionEnum.v1_5;
                //_beeDebugClient = new AdapterGatewayApiClient_1_5(httpClient);
            }
            else if (version == "default")
            {
                _beeVersion = BeeVersionEnum.v1_4;
                _beeDebugClient = new AdapterGatewayApiClient_1_4(httpClient, baseUrl);
            }
        }

        public async Task<AuthResponse> AuthAsync(BodyDto body, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.AuthAsync(body, cancellationToken);
        }

        public async Task<FileResponse> BytesGETAsync(string reference, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.BytesGETAsync(reference, cancellationToken);
        }

        public async Task<BytesPOSTResponse> BytesPOSTAsync(int? swarm_tag, bool? swarm_pin, bool? swarm_encrypt, string swarm_postage_batch_id, Stream body, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.BytesPOSTAsync(swarm_tag, swarm_pin, swarm_encrypt, swarm_postage_batch_id, body, cancellationToken);
        }

        public async Task<FileResponse> BzzGET2Async(string reference, string path, string targets, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.BzzGET2Async(reference, path, targets, cancellationToken);
        }

        public async Task<FileResponse> BzzGETAsync(string reference, string targets, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.BzzGETAsync(reference, targets, cancellationToken);
        }

        public async Task BzzPATCHAsync(string reference, CancellationToken? cancellationToken)
        {
            await _beeDebugClient.BzzPATCHAsync(reference, cancellationToken);
        }

        public async Task<BzzPOSTAResponse> BzzPOSTAsync(string name, int? swarm_tag, bool? swarm_pin, bool? swarm_encrypt, string content_Type, bool? swarm_collection, string swarm_index_document, string swarm_error_document, string swarm_postage_batch_id, IEnumerable<FileParameterDto> file, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.BzzPOSTAsync(name, swarm_tag, swarm_pin, swarm_encrypt, content_Type, swarm_collection, swarm_index_document, swarm_error_document, swarm_postage_batch_id, file, cancellationToken);
        }

        public async Task<FileResponse> ChunksGETAsync(string reference, string targets, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.ChunksGETAsync(reference, targets, cancellationToken);
        }

        public async Task<ChunksPOSTResponse> ChunksPOSTAsync(int? swarm_tag, bool? swarm_pin, string swarm_postage_batch_id, Stream body, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.ChunksPOSTAsync(swarm_tag, swarm_pin, swarm_postage_batch_id, body, cancellationToken);
        }

        public async Task<FeedsGETResponse> FeedsGETAsync(string owner, string topic, int? at, string type, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.FeedsGETAsync(owner, topic, at, type, cancellationToken);
        }

        public async Task<FeedsPOSTResponse> FeedsPOSTAsync(string owner, string topic, string type, bool? swarm_pin, string swarm_postage_batch_id, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.FeedsPOSTAsync(owner, topic, type, swarm_pin, swarm_postage_batch_id, cancellationToken);
        }

        public async Task<PinsDELETEResponse> PinsDELETEAsync(string reference, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.PinsDELETEAsync(reference, cancellationToken);
        }

        public async Task<PinsGET2Response> PinsGET2Async(CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.PinsGET2Async(cancellationToken);
        }

        public async Task<string> PinsGETAsync(string reference, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.PinsGETAsync(reference, cancellationToken);
        }

        public async Task<PinsPOSTResponse> PinsPOSTAsync(string reference, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.PinsPOSTAsync(reference, cancellationToken);
        }

        public async Task<RefreshResponse> RefreshAsync(BodyDto body, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.RefreshAsync(body, cancellationToken);
        }

        public async Task SendAsync(string topic, string targets, string recipient, string swarm_postage_batch_id, CancellationToken? cancellationToken)
        {
            await _beeDebugClient.SendAsync(topic, targets, recipient, swarm_postage_batch_id, cancellationToken);
        }

        public async Task<SocResponse> SocAsync(string owner, string id, string sig, bool? swarm_pin, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.SocAsync(owner, id, sig, swarm_pin, cancellationToken);
        }

        public async Task<StewardshipGETResponse> StewardshipGETAsync(string reference, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.StewardshipGETAsync(reference, cancellationToken);
        }

        public async Task StewardshipPUTAsync(string reference, CancellationToken? cancellationToken)
        {
            await _beeDebugClient.StewardshipPUTAsync(reference, cancellationToken);
        }

        public async Task StreamAsync(int? swarm_tag, bool? swarm_pin, string swarm_postage_batch_id, CancellationToken? cancellationToken)
        {
            await _beeDebugClient.StreamAsync(swarm_tag, swarm_pin, swarm_postage_batch_id, cancellationToken);
        }

        public async Task SubscribeAsync(string topic, CancellationToken? cancellationToken)
        {
            await _beeDebugClient.SubscribeAsync(topic, cancellationToken);
        }

        public async Task TagsDELETEAsync(int uid, CancellationToken? cancellationToken)
        {
            await _beeDebugClient.TagsDELETEAsync(uid, cancellationToken);
        }

        public async Task<TagsGET2Response> TagsGET2Async(int uid, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.TagsGET2Async(uid, cancellationToken);
        }

        public async Task<TagsGETResponse> TagsGETAsync(int? offset, int? limit, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.TagsGETAsync(offset, limit, cancellationToken);
        }

        public async Task<TagsPATCHResponse> TagsPATCHAsync(int uid, Body3Dto body, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.TagsPATCHAsync(uid, body, cancellationToken);
        }

        public async Task<TagsPOSTResponse> TagsPOSTAsync(Body3Dto body, CancellationToken? cancellationToken)
        {
            return await _beeDebugClient.TagsPOSTAsync(body, cancellationToken);
        }
    }
}
