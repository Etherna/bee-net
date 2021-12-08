using Etherna.BeeNet.DtoModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace Etherna.BeeNet.Clients.v_1_4.GatewayApi
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    public class AdapterGatewayClient14 : IBeeGatewayClient
    {
        readonly IBeeGatewayClient_1_4 _beeGatewayApiClient;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:URI-like parameters should not be strings", Justification = "<Pending>")]
        public AdapterGatewayClient14(HttpClient httpClient, Uri? baseUrl)
        {
            if (baseUrl is null)
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }
            _beeGatewayApiClient = new BeeGatewayClient_1_4(httpClient) { BaseUrl = baseUrl.ToString() };
        }


        public async Task<AuthDto> AuthAsync(string role, int expiry)
        {
            var response = await _beeGatewayApiClient.AuthAsync(new Body { Role = role, Expiry = expiry }).ConfigureAwait(false);

            return new AuthDto(response.Key);
        }

        public async Task<RefreshDto> RefreshAsync(string role, int expiry)
        {
            var response = await _beeGatewayApiClient.RefreshAsync(new Body2 { Role = role, Expiry = expiry }).ConfigureAwait(false);

            return new RefreshDto(response.Key);
        }

        public async Task<ReferenceDto> BytesPostAsync(string swarmPostageBatchId, int? swarmTag = null, bool? swarmPin = null,
            bool? swarmEncrypt = null, bool? swarmDeferredUpload = null, Stream? body = null)
        {
            var response = await _beeGatewayApiClient.BytesPostAsync(
                swarmPostageBatchId, 
                swarmTag, 
                swarmPin, 
                swarmEncrypt,
                swarmDeferredUpload, 
                body).ConfigureAwait(false);

            return new ReferenceDto(response.Reference);
        }

        public async Task<Stream> BytesGetAsync(string reference)
        {
            var response = await _beeGatewayApiClient.BytesGetAsync(reference).ConfigureAwait(false);

            return response.Stream;
        }

        public async Task<Stream> ChunksGetAsync(string reference, string? targets = null)
        {
            var response = await _beeGatewayApiClient.ChunksGetAsync(reference, targets).ConfigureAwait(false);

            return response.Stream;
        }

        public async Task<VersionDto> ChunksPostAsync(string swarmPostageBatchId, int? swarmTag = null, bool? swarmPin = null,
            bool? swarmDeferredUpload = null, Stream? body = null)
        {
            var response = await _beeGatewayApiClient.ChunksPostAsync(
                swarmPostageBatchId, 
                swarmTag, 
                swarmPin, 
                swarmDeferredUpload, 
                body).ConfigureAwait(false);

            return new VersionDto(response.Status, response.Version, response.ApiVersion, response.DebugApiVersion);
        }

        public async Task ChunksStreamAsync(string swarmPostageBatchId, int? swarmTag = null, bool? swarmPin = null)
        {
            await _beeGatewayApiClient.ChunksStreamAsync(swarmPostageBatchId, swarmTag, swarmPin).ConfigureAwait(false);
        }

        public async Task<ReferenceDto> BzzPostAsync(
            string swarmPostageBatchId, 
            string? name, 
            int? swarmTag, 
            bool? swarmPin, 
            bool? swarmEncrypt, 
            string? contentType, 
            bool? swarmCollection, 
            string? swarmIndexDocument, 
            string? swarmErrorDocument, 
            bool? swarmDeferredUpload, IEnumerable<FileParameter>? file)
        {
            var response = await _beeGatewayApiClient.BzzPostAsync(
                swarmPostageBatchId, 
                name, 
                swarmTag, 
                swarmPin, 
                swarmEncrypt, 
                contentType, 
                swarmCollection, 
                swarmIndexDocument, 
                swarmErrorDocument, 
                swarmDeferredUpload).ConfigureAwait(false);

            return new ReferenceDto(response.Reference);
        }

        public async Task<Stream> BzzGetAsync(string reference, string? targets = null)
        {
            var response = await _beeGatewayApiClient.BzzGetAsync(reference, targets, CancellationToken.None).ConfigureAwait(false);

            return response.Stream;
        }

        public async Task<Stream> BzzGetAsync(string reference, string path, string? targets = null)
        {
            var response = await _beeGatewayApiClient.BzzGetAsync(reference, path, targets).ConfigureAwait(false);

            return response.Stream;
        }

        public async Task<List<TagInfoDto>> TagsGetAsync(int? offset = null, int? limit = null)
        {
            var response = await _beeGatewayApiClient.TagsGetAsync(offset, limit).ConfigureAwait(false);

            return response.Tags
                .Select(i => new TagInfoDto(i.Uid, i.StartedAt, i.Total, i.Processed, i.Synced))
                .ToList();
        }

        public async Task<TagInfoDto> CreateTagAsync(string address)
        {
            var response = await _beeGatewayApiClient.TagsPostAsync(new Body3 { Address = address }).ConfigureAwait(false);

            return new TagInfoDto(response.Uid, response.StartedAt, response.Total, response.Processed, response.Synced);
        }

        public async Task<TagInfoDto> TagsGetAsync(int uid)
        {
            var response = await _beeGatewayApiClient.TagsGetAsync(uid).ConfigureAwait(false);

            return new TagInfoDto(response.Uid, response.StartedAt, response.Total, response.Processed, response.Synced);
        }

        public async Task DeleteTagAsync(int uid)
        {
            await _beeGatewayApiClient.TagsDeleteAsync(uid).ConfigureAwait(false);
        }

        public async Task<VersionDto> UpdateTag(int uid, string? address = null)
        { //TODO check for data input (maybe missing some input)
            if (address is null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            var response = await _beeGatewayApiClient.TagsPatchAsync(uid, new Body4 { Address = address }).ConfigureAwait(false);

            return new VersionDto(response.Status, response.Version, response.ApiVersion, response.DebugApiVersion);
        }

        public async Task<MessageResponseDto> PinsPostAsync(string reference)
        {
            var response = await _beeGatewayApiClient.PinsPostAsync(reference).ConfigureAwait(false);

            return new MessageResponseDto(response.Message, response.Code);
        }

        public async Task<MessageResponseDto> PinsDeleteAsync(string reference)
        {
            var response = await _beeGatewayApiClient.PinsDeleteAsync(reference).ConfigureAwait(false);

            return new MessageResponseDto(response.Message, response.Code);
        }

        public async Task<string> PinsGetAsync(string reference)
        {
            return await _beeGatewayApiClient.PinsGetAsync(reference).ConfigureAwait(false);
        }

        public async Task<List<AddressDto>> PinsGetAsync()
        {
            var response = await _beeGatewayApiClient.PinsGetAsync().ConfigureAwait(false);

            return response.Addresses
                .Select(i => new AddressDto(i))
                .ToList();
        }

        public async Task PssSendAsync(string topic, string targets, string swarmPostageBatchId, string? recipient = null)
        {
            await _beeGatewayApiClient.PssSendAsync(topic, targets, swarmPostageBatchId, recipient).ConfigureAwait(false);
        }

        public async Task PssSubscribeAsync(string topic)
        {
            await _beeGatewayApiClient.PssSubscribeAsync(topic).ConfigureAwait(false);
        }

        public async Task<ReferenceDto> SocAsync(string owner, string id, string sig, bool? swarmPin = null)
        {
            var response = await _beeGatewayApiClient.SocAsync(owner, id, sig, swarmPin).ConfigureAwait(false);

            return new ReferenceDto(response.Reference);
        }

        public async Task<ReferenceDto> FeedsPostAsync(string owner, string topic, string swarmPostageBatchId, string? type = null,
            bool? swarmPin = null)
        {
            var response = await _beeGatewayApiClient.FeedsPostAsync(owner, topic, swarmPostageBatchId, type, swarmPin).ConfigureAwait(false);

            return new ReferenceDto(response.Reference);
        }

        public async Task<ReferenceDto> FeedsGetAsync(string owner, string topic, int? at = null, string? type = null)
        {
            var response = await _beeGatewayApiClient.FeedsGetAsync(owner, topic, at, type).ConfigureAwait(false);

            return new ReferenceDto(response.Reference);
        }

        public async Task<StewardshipGetDto> StewardshipGetAsync(string reference)
        {
            var response = await _beeGatewayApiClient.StewardshipGetAsync(reference).ConfigureAwait(false);

            return new StewardshipGetDto(response.IsRetrievable);
        }

        public async Task StewardshipPutAsync(string reference)
        {
            await _beeGatewayApiClient.StewardshipPutAsync(reference).ConfigureAwait(false);
        }
    }
}
#pragma warning restore CA1707 // Identifiers should not contain underscores