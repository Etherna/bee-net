using System;
using Etherna.BeeNet.DtoModel.GatewayApi;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Etherna.BeeNet.DtoInput.GatewayApi;

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace Etherna.BeeNet.Clients.v_1_4.GatewayApi
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    public class AdapterGatewayApiClient_1_4 : IBeeGatewayApiClient
    {
        readonly IBeeGatewayClient_1_4 _beeGatewayApiClient;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:URI-like parameters should not be strings", Justification = "<Pending>")]
        public AdapterGatewayApiClient_1_4(HttpClient httpClient, Uri baseUrl)
        {
            _beeGatewayApiClient = new BeeGatewayClient_1_4(httpClient) { BaseUrl = baseUrl.ToString() };
        }

        public async Task<AuthDto> AuthAsync(BodyDto body, CancellationToken? cancellationToken)
        {
            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            var bodyRequest = new Body
            {
                AdditionalProperties = body.AdditionalProperties,
                Expiry = body.Expiry,
                Role = body.Role
            };
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.AuthAsync(bodyRequest, cancellationToken.Value) : await _beeGatewayApiClient.AuthAsync(bodyRequest);

            return new AuthDto(response.Key, response.AdditionalProperties);
        }

        public async Task<RefreshDto> RefreshAsync(BodyDto body, CancellationToken? cancellationToken)
        {
            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            var bodyRequest = new Body2
            {
                AdditionalProperties = body.AdditionalProperties,
                Expiry = body.Expiry,
                Role = body.Role
            };
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.RefreshAsync(bodyRequest, cancellationToken.Value) : await _beeGatewayApiClient.RefreshAsync(bodyRequest);

            return new RefreshDto(response.Key, response.AdditionalProperties);
        }

        public async Task<BytesPostDto> BytesPOSTAsync(int? swarmTag, bool? swarmPin, bool? swarmEncrypt, string swarmPostageBatchId, Stream body, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.BytesPOSTAsync(swarmTag, swarmPin, swarmEncrypt, swarmPostageBatchId, body, cancellationToken.Value) : await _beeGatewayApiClient.BytesPOSTAsync(swarmTag, swarmPin, swarmEncrypt, swarmPostageBatchId, body);

            return new BytesPostDto(response.Reference, response.AdditionalProperties);
        }

        public async Task<DtoModel.GatewayApi.FileResponse> BytesGETAsync(string reference, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.BytesGETAsync(reference, cancellationToken.Value) : await _beeGatewayApiClient.BytesGETAsync(reference);

            return new DtoModel.GatewayApi.FileResponse(response.StatusCode, response.Headers, response.Stream);
        }

        public async Task<DtoModel.GatewayApi.FileResponse> ChunksGETAsync(string reference, string targets, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.ChunksGETAsync(reference, targets, cancellationToken.Value) : await _beeGatewayApiClient.ChunksGETAsync(reference, targets);

            return new DtoModel.GatewayApi.FileResponse(response.StatusCode, response.Headers, response.Stream);
        }

        public async Task<ChunksPostDto> ChunksPOSTAsync(int? swarmTag, bool? swarmPin, string swarmPostageBatchId, Stream body, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.ChunksPOSTAsync(swarmTag, swarmPin, swarmPostageBatchId, body, cancellationToken.Value) : await _beeGatewayApiClient.ChunksPOSTAsync(swarmTag, swarmPin, swarmPostageBatchId, body);

            return new ChunksPostDto(response.Status, response.Version, response.ApiVersion, response.DebugApiVersion, response.AdditionalProperties);
        }

        public async Task StreamAsync(int? swarmTag, bool? swarmPin, string swarmPostageBatchId, CancellationToken? cancellationToken)
        {
            if (cancellationToken.HasValue)
            {
                await _beeGatewayApiClient.StreamAsync(swarmTag, swarmPin, swarmPostageBatchId, cancellationToken.Value);
            }
            else
            {
                await _beeGatewayApiClient.StreamAsync(swarmTag, swarmPin, swarmPostageBatchId);
            }
        }

        public async Task<BzzPostDto> BzzPOSTAsync(string name, int? swarmTag, bool? swarmPin, bool? swarmEncrypt, string contentType, bool? swarmCollection, string swarmIndexDocument, string swarmErrorDocument, string swarmPostageBatchId, IEnumerable<FileParameterDto> file, CancellationToken? cancellationToken)
        {
            var fileParameter = file.Select(i => new FileParameter(i.Data)).ToList();

            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.BzzPOSTAsync(name, swarmTag, swarmPin, swarmEncrypt, contentType, swarmCollection, swarmIndexDocument, swarmErrorDocument, swarmPostageBatchId, fileParameter, cancellationToken.Value) : await _beeGatewayApiClient.BzzPOSTAsync(name, swarmTag, swarmPin, swarmEncrypt, contentType, swarmCollection, swarmIndexDocument, swarmErrorDocument, swarmPostageBatchId, fileParameter);

            return new BzzPostDto(response.Reference, response.AdditionalProperties);
        }

        [System.Obsolete("Reupload a root hash to the network; deprecated: use /stewardship/{reference} instead")]
        public async Task BzzPATCHAsync(string reference, CancellationToken? cancellationToken)
        {
            if (cancellationToken.HasValue)
            {
                await _beeGatewayApiClient.BzzPATCHAsync(reference, cancellationToken.Value);
            }
            else
            {
                await _beeGatewayApiClient.BzzPATCHAsync(reference);
            }

        }

        public async Task<DtoModel.GatewayApi.FileResponse> BzzGETAsync(string reference, string targets, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.BzzGETAsync(reference, targets, cancellationToken.Value) : await _beeGatewayApiClient.BzzGETAsync(reference, targets);

            return new DtoModel.GatewayApi.FileResponse(response.StatusCode, response.Headers, response.Stream);
        }

        public async Task<DtoModel.GatewayApi.FileResponse> BzzGET2Async(string reference, string path, string targets, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.BzzGET2Async(reference, path, targets, cancellationToken.Value) : await _beeGatewayApiClient.BzzGET2Async(reference, path, targets);

            return new DtoModel.GatewayApi.FileResponse(response.StatusCode, response.Headers, response.Stream);
        }

        public async Task<TagsGetDto> TagsGETAsync(int? offset, int? limit, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.TagsGETAsync(offset, limit, cancellationToken.Value) : await _beeGatewayApiClient.TagsGETAsync(offset, limit);

            var tags = response.Tags
                .Select(i => new TagsDto(i.Uid, i.StartedAt, i.Total, i.Processed, i.Synced, i.AdditionalProperties))
                .ToList();
            return new TagsGetDto(tags, response.AdditionalProperties);
        }

        public async Task<TagsPostDto> TagsPOSTAsync(Body3Dto body, CancellationToken? cancellationToken)
        {
            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            var bodyRequest = new Body3
            {
                AdditionalProperties = body.AdditionalProperties,
                Address = body.Address
            };
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.TagsPOSTAsync(bodyRequest, cancellationToken.Value) : await _beeGatewayApiClient.TagsPOSTAsync(bodyRequest);

            return new TagsPostDto(response.Uid, response.StartedAt, response.Total, response.Processed, response.Synced, response.AdditionalProperties);
        }

        public async Task<TagsGet2Dto> TagsGET2Async(int uid, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.TagsGET2Async(uid, cancellationToken.Value) : await _beeGatewayApiClient.TagsGET2Async(uid);

            return new TagsGet2Dto(response.Uid, response.StartedAt, response.Total, response.Processed, response.Synced, response.AdditionalProperties);
        }

        public async Task TagsDELETEAsync(int uid, CancellationToken? cancellationToken)
        {
            if (cancellationToken.HasValue)
            {
                await _beeGatewayApiClient.TagsDELETEAsync(uid, cancellationToken.Value);
            }
            else
            {
                await _beeGatewayApiClient.TagsDELETEAsync(uid);
            }
        }

        public async Task<TagsPatchDto> TagsPATCHAsync(int uid, Body3Dto body, CancellationToken? cancellationToken)
        {
            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            var bodyRequest = new Body4
            {
                AdditionalProperties = body.AdditionalProperties,
                Address = body.Address
            };

            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.TagsPATCHAsync(uid, bodyRequest, cancellationToken.Value) : await _beeGatewayApiClient.TagsPATCHAsync(uid, bodyRequest);

            return new TagsPatchDto(response.Status, response.Version, response.ApiVersion, response.DebugApiVersion, response.AdditionalProperties);
        }

        public async Task<PinsPostDto> PinsPOSTAsync(string reference, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.PinsPOSTAsync(reference, cancellationToken.Value) : await _beeGatewayApiClient.PinsPOSTAsync(reference);

            return new PinsPostDto(response.Message, response.Code, response.AdditionalProperties);
        }

        public async Task<PinsDeleteDto> PinsDELETEAsync(string reference, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.PinsDELETEAsync(reference, cancellationToken.Value) : await _beeGatewayApiClient.PinsDELETEAsync(reference);

            return new PinsDeleteDto(response.Message, response.Code, response.AdditionalProperties);
        }

        public async Task<string> PinsGETAsync(string reference, CancellationToken? cancellationToken)
        {
            return cancellationToken.HasValue ? await _beeGatewayApiClient.PinsGETAsync(reference, cancellationToken.Value) : await _beeGatewayApiClient.PinsGETAsync(reference);
        }

        public async Task<PinsGet2Dto> PinsGET2Async(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.PinsGET2Async(cancellationToken.Value) : await _beeGatewayApiClient.PinsGET2Async();

            return new PinsGet2Dto(response.Addresses, response.AdditionalProperties);
        }

        public async Task SendAsync(string topic, string targets, string recipient, string swarmPostageBatchId, CancellationToken? cancellationToken)
        {
            if (cancellationToken.HasValue)
            {
                await _beeGatewayApiClient.SendAsync(topic, targets, recipient, swarmPostageBatchId, cancellationToken.Value);
            }
            else
            {
                await _beeGatewayApiClient.SendAsync(topic, targets, recipient, swarmPostageBatchId);
            }
        }

        public async Task SubscribeAsync(string topic, CancellationToken? cancellationToken)
        {
            if (cancellationToken.HasValue)
            {
                await _beeGatewayApiClient.SubscribeAsync(topic, cancellationToken.Value);
            }
            else
            {
                await _beeGatewayApiClient.SubscribeAsync(topic);
            }
        }

        public async Task<SocDto> SocAsync(string owner, string id, string sig, bool? swarmPin, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.SocAsync(owner, id, sig, swarmPin, cancellationToken.Value) : await _beeGatewayApiClient.SocAsync(owner, id, sig, swarmPin);

            return new SocDto(response.Reference, response.AdditionalProperties);
        }

        public async Task<FeedsPostDto> FeedsPOSTAsync(string owner, string topic, string type, bool? swarmPin, string swarmPostageBatchId, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.FeedsPOSTAsync(owner, topic, type, swarmPin, swarmPostageBatchId, cancellationToken.Value) : await _beeGatewayApiClient.FeedsPOSTAsync(owner, topic, type, swarmPin, swarmPostageBatchId);

            return new FeedsPostDto(response.Reference, response.AdditionalProperties);
        }

        public async Task<FeedsGetDto> FeedsGETAsync(string owner, string topic, int? at, string type, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.FeedsGETAsync(owner, topic, at, type, cancellationToken.Value) : await _beeGatewayApiClient.FeedsGETAsync(owner, topic, at, type);

            return new FeedsGetDto(response.Reference, response.AdditionalProperties);
        }

        public async Task<StewardshipGetDto> StewardshipGETAsync(string reference, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.StewardshipGETAsync(reference, cancellationToken.Value) : await _beeGatewayApiClient.StewardshipGETAsync(reference);

            return new StewardshipGetDto(response.IsRetrievable, response.AdditionalProperties);
        }

        public async Task StewardshipPUTAsync(string reference, CancellationToken? cancellationToken)
        {
            if (cancellationToken.HasValue)
            {
                await _beeGatewayApiClient.StewardshipPUTAsync(reference, cancellationToken.Value);
            }
            else
            {
                await _beeGatewayApiClient.StewardshipPUTAsync(reference);
            }
        }

    }
}
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
#pragma warning restore CA1707 // Identifiers should not contain underscores