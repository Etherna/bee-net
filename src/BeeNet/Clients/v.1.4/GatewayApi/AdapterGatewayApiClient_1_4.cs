using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TestAdapter.Clients;
using TestAdapter.Clients.v_1_4.GatewayApi;
using TestAdapter.Dtos.GatewayApi;

namespace TestAdapter
{
    public class AdapterGatewayApiClient_1_4 : IFacadeBeeGatewayApiClient
    {
        readonly IBeeGatewayClient_1_4 _beeGatewayApiClient;

        public AdapterGatewayApiClient_1_4(HttpClient httpClient, string baseUrl)
        {
            _beeGatewayApiClient = new BeeGatewayClient_1_4(httpClient) { BaseUrl = baseUrl };
        }

        public async Task<AuthResponse> AuthAsync(Dtos.GatewayApi.BodyDto body, CancellationToken? cancellationToken)
        {
            var bodyRequest = new Body
            {
                AdditionalProperties = body.AdditionalProperties,
                Expiry = body.Expiry,
                Role = body.Role
            };
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.AuthAsync(bodyRequest, cancellationToken.Value) : await _beeGatewayApiClient.AuthAsync(bodyRequest);

            return new AuthResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Key = response.Key
            };
        }

        public async Task<RefreshResponse> RefreshAsync(Dtos.GatewayApi.BodyDto body, CancellationToken? cancellationToken)
        {
            var bodyRequest = new Body2
            {
                AdditionalProperties = body.AdditionalProperties,
                Expiry = body.Expiry,
                Role = body.Role
            };
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.RefreshAsync(bodyRequest, cancellationToken.Value) : await _beeGatewayApiClient.RefreshAsync(bodyRequest);

            return new RefreshResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Key = response.Key
            };
        }

        public async Task<BytesPOSTResponse> BytesPOSTAsync(int? swarm_tag, bool? swarm_pin, bool? swarm_encrypt, string swarm_postage_batch_id, Stream body, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.BytesPOSTAsync(swarm_tag, swarm_pin, swarm_encrypt, swarm_postage_batch_id, body, cancellationToken.Value) : await _beeGatewayApiClient.BytesPOSTAsync(swarm_tag, swarm_pin, swarm_encrypt, swarm_postage_batch_id, body);

            return new BytesPOSTResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Reference = response.Reference
            };
        }

        public async Task<Dtos.GatewayApi.FileResponse> BytesGETAsync(string reference, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.BytesGETAsync(reference, cancellationToken.Value) : await _beeGatewayApiClient.BytesGETAsync(reference);

            return new Dtos.GatewayApi.FileResponse(response.StatusCode, response.Headers, response.Stream);
        }

        public async Task<Dtos.GatewayApi.FileResponse> ChunksGETAsync(string reference, string targets, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.ChunksGETAsync(reference, targets, cancellationToken.Value) : await _beeGatewayApiClient.ChunksGETAsync(reference, targets);

            return new Dtos.GatewayApi.FileResponse(response.StatusCode, response.Headers, response.Stream);
        }

        public async Task<ChunksPOSTResponse> ChunksPOSTAsync(int? swarm_tag, bool? swarm_pin, string swarm_postage_batch_id, Stream body, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.ChunksPOSTAsync(swarm_tag, swarm_pin, swarm_postage_batch_id, body, cancellationToken.Value) : await _beeGatewayApiClient.ChunksPOSTAsync(swarm_tag, swarm_pin, swarm_postage_batch_id, body);

            return new ChunksPOSTResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                ApiVersion = response.ApiVersion,
                DebugApiVersion = response.DebugApiVersion,
                Status = response.Status,
                Version = response.Version
            };
        }

        public async Task StreamAsync(int? swarm_tag, bool? swarm_pin, string swarm_postage_batch_id, CancellationToken? cancellationToken)
        {
            if (cancellationToken.HasValue)
            {
                await _beeGatewayApiClient.StreamAsync(swarm_tag, swarm_pin, swarm_postage_batch_id, cancellationToken.Value);
            }
            else
            {
                await _beeGatewayApiClient.StreamAsync(swarm_tag, swarm_pin, swarm_postage_batch_id);
            }
        }

        public async Task<BzzPOSTAResponse> BzzPOSTAsync(string name, int? swarm_tag, bool? swarm_pin, bool? swarm_encrypt, string content_Type, bool? swarm_collection, string swarm_index_document, string swarm_error_document, string swarm_postage_batch_id, IEnumerable<FileParameterDto> file, CancellationToken? cancellationToken)
        {
            var fileParameter = file.Select(i => new FileParameter(i.Data)).ToList();

            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.BzzPOSTAsync(name, swarm_tag, swarm_pin, swarm_encrypt, content_Type, swarm_collection, swarm_index_document, swarm_error_document, swarm_postage_batch_id, fileParameter, cancellationToken.Value) : await _beeGatewayApiClient.BzzPOSTAsync(name, swarm_tag, swarm_pin, swarm_encrypt, content_Type, swarm_collection, swarm_index_document, swarm_error_document, swarm_postage_batch_id, fileParameter);

            return new BzzPOSTAResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Reference = response.Reference
            };
        }

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

        public async Task<Dtos.GatewayApi.FileResponse> BzzGETAsync(string reference, string targets, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.BzzGETAsync(reference, targets, cancellationToken.Value) : await _beeGatewayApiClient.BzzGETAsync(reference, targets);

            return new Dtos.GatewayApi.FileResponse(response.StatusCode, response.Headers, response.Stream);
        }

        public async Task<Dtos.GatewayApi.FileResponse> BzzGET2Async(string reference, string path, string targets, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.BzzGET2Async(reference, path, targets, cancellationToken.Value) : await _beeGatewayApiClient.BzzGET2Async(reference, path, targets);

            return new Dtos.GatewayApi.FileResponse(response.StatusCode, response.Headers, response.Stream);
        }

        public async Task<TagsGETResponse> TagsGETAsync(int? offset, int? limit, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.TagsGETAsync(offset, limit, cancellationToken.Value) : await _beeGatewayApiClient.TagsGETAsync(offset, limit);

            return new TagsGETResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Tags = response.Tags.Select(i => new TagsDto
                {
                    AdditionalProperties = i.AdditionalProperties,
                    Processed = i.Processed,
                    StartedAt = i.StartedAt,
                    Synced = i.Synced,
                    Total = i.Total,
                    Uid = i.Uid
                }).ToList()
            };
        }

        public async Task<TagsPOSTResponse> TagsPOSTAsync(Body3Dto body, CancellationToken? cancellationToken)
        {
            var bodyRequest = new Body3
            {
                AdditionalProperties = body.AdditionalProperties,
                Address = body.Address
            };
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.TagsPOSTAsync(bodyRequest, cancellationToken.Value) : await _beeGatewayApiClient.TagsPOSTAsync(bodyRequest);

            return new TagsPOSTResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Processed = response.Processed,
                StartedAt = response.StartedAt,
                Synced = response.Synced,
                Total = response.Total,
                Uid = response.Uid
            };
        }

        public async Task<TagsGET2Response> TagsGET2Async(int uid, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.TagsGET2Async(uid, cancellationToken.Value) : await _beeGatewayApiClient.TagsGET2Async(uid);

            return new TagsGET2Response
            {
                AdditionalProperties = response.AdditionalProperties,
                Processed = response.Processed,
                StartedAt = response.StartedAt,
                Synced = response.Synced,
                Total = response.Total,
                Uid = response.Uid
            };
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

        public async Task<TagsPATCHResponse> TagsPATCHAsync(int uid, Body3Dto body, CancellationToken? cancellationToken)
        {
            var bodyRequest = new Body4
            {
                AdditionalProperties = body.AdditionalProperties,
                Address = body.Address
            };

            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.TagsPATCHAsync(uid, bodyRequest, cancellationToken.Value) : await _beeGatewayApiClient.TagsPATCHAsync(uid, bodyRequest);

            return new TagsPATCHResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                ApiVersion = response.ApiVersion,
                DebugApiVersion = response.DebugApiVersion,
                Status = response.Status,
                Version = response.Version
            };
        }

        public async Task<PinsPOSTResponse> PinsPOSTAsync(string reference, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.PinsPOSTAsync(reference, cancellationToken.Value) : await _beeGatewayApiClient.PinsPOSTAsync(reference);

            return new PinsPOSTResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Message = response.Message,
                Code = response.Code
            };
        }

        public async Task<PinsDELETEResponse> PinsDELETEAsync(string reference, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.PinsDELETEAsync(reference, cancellationToken.Value) : await _beeGatewayApiClient.PinsDELETEAsync(reference);

            return new PinsDELETEResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Message = response.Message,
                Code = response.Code
            };
        }

        public async Task<string> PinsGETAsync(string reference, CancellationToken? cancellationToken)
        {
            return cancellationToken.HasValue ? await _beeGatewayApiClient.PinsGETAsync(reference, cancellationToken.Value) : await _beeGatewayApiClient.PinsGETAsync(reference);
        }

        public async Task<PinsGET2Response> PinsGET2Async(CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.PinsGET2Async(cancellationToken.Value) : await _beeGatewayApiClient.PinsGET2Async();

            return new PinsGET2Response
            {
                AdditionalProperties = response.AdditionalProperties,
                Addresses = response.Addresses
            };
        }

        public async Task SendAsync(string topic, string targets, string recipient, string swarm_postage_batch_id, CancellationToken? cancellationToken)
        {
            if (cancellationToken.HasValue)
            {
                await _beeGatewayApiClient.SendAsync(topic, targets, recipient, swarm_postage_batch_id, cancellationToken.Value);
            }
            else
            {
                await _beeGatewayApiClient.SendAsync(topic, targets, recipient, swarm_postage_batch_id);
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

        public async Task<SocResponse> SocAsync(string owner, string id, string sig, bool? swarm_pin, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.SocAsync(owner, id, sig, swarm_pin, cancellationToken.Value) : await _beeGatewayApiClient.SocAsync(owner, id, sig, swarm_pin);

            return new SocResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Reference = response.Reference
            };
        }

        public async Task<FeedsPOSTResponse> FeedsPOSTAsync(string owner, string topic, string type, bool? swarm_pin, string swarm_postage_batch_id, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.FeedsPOSTAsync(owner, topic, type, swarm_pin, swarm_postage_batch_id, cancellationToken.Value) : await _beeGatewayApiClient.FeedsPOSTAsync(owner, topic, type, swarm_pin, swarm_postage_batch_id);

            return new FeedsPOSTResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Reference = response.Reference
            };
        }

        public async Task<FeedsGETResponse> FeedsGETAsync(string owner, string topic, int? at, string type, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.FeedsGETAsync(owner, topic, at, type, cancellationToken.Value) : await _beeGatewayApiClient.FeedsGETAsync(owner, topic, at, type);

            return new FeedsGETResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                Reference = response.Reference
            };
        }

        public async Task<StewardshipGETResponse> StewardshipGETAsync(string reference, CancellationToken? cancellationToken)
        {
            var response = cancellationToken.HasValue ? await _beeGatewayApiClient.StewardshipGETAsync(reference, cancellationToken.Value) : await _beeGatewayApiClient.StewardshipGETAsync(reference);

            return new StewardshipGETResponse
            {
                AdditionalProperties = response.AdditionalProperties,
                IsRetrievable = response.IsRetrievable
            };
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
