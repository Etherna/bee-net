using Etherna.BeeNet.DtoModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Clients.v1_4_1.GatewayApi
{
    [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Version number should containt underscores")]
    public class AdapterGatewayClient_1_4_1 : IBeeGatewayClient
    {
        // Fields.
        private readonly IBeeGatewayClient_1_4_1 beeGatewayApiClient;


        // Constructors.
        public AdapterGatewayClient_1_4_1(HttpClient httpClient, Uri baseUrl)
        {
            if (baseUrl is null)
                throw new ArgumentNullException(nameof(baseUrl));

            beeGatewayApiClient = new BeeGatewayClient_1_4_1(httpClient) { BaseUrl = baseUrl.ToString() };
        }


        public async Task<AuthDto> AuthAsync(string role, int expiry)
        {
            var response = await beeGatewayApiClient.AuthAsync(new Body
            {
                Role = role,
                Expiry = expiry
            }).ConfigureAwait(false);

            return new AuthDto(response);
        }

        public async Task<RefreshDto> RefreshAsync(string role, int expiry)
        {
            var response = await beeGatewayApiClient.RefreshAsync(new Body2
            {
                Role = role,
                Expiry = expiry
            }).ConfigureAwait(false);

            return new RefreshDto(response);
        }

        public async Task<ReferenceDto> BytesPostAsync(
            string swarmPostageBatchId,
            int? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmEncrypt = null,
            bool? swarmDeferredUpload = null,
            Stream? body = null)
        {
            var response = await beeGatewayApiClient.BytesPostAsync(
                swarmPostageBatchId,
                swarmTag,
                swarmPin,
                swarmEncrypt,
                swarmDeferredUpload,
                body).ConfigureAwait(false);

            return new ReferenceDto(response);
        }

        public async Task<Stream> BytesGetAsync(string reference)
        {
            var response = await beeGatewayApiClient.BytesGetAsync(reference).ConfigureAwait(false);

            return response.Stream;
        }

        public async Task<Stream> ChunksGetAsync(string reference, string? targets = null)
        {
            var response = await beeGatewayApiClient.ChunksGetAsync(reference, targets).ConfigureAwait(false);

            return response.Stream;
        }

        public async Task<VersionDto> ChunksPostAsync(
            string swarmPostageBatchId,
            int? swarmTag = null,
            bool? swarmPin = null,
            bool? swarmDeferredUpload = null,
            Stream? body = null)
        {
            var response = await beeGatewayApiClient.ChunksPostAsync(
                swarmPostageBatchId,
                swarmTag,
                swarmPin,
                swarmDeferredUpload,
                body).ConfigureAwait(false);

            return new VersionDto(response);
        }

        public async Task ChunksStreamAsync(
            string swarmPostageBatchId,
            int? swarmTag = null,
            bool? swarmPin = null)
        {
            await beeGatewayApiClient.ChunksStreamAsync(swarmPostageBatchId, swarmTag, swarmPin).ConfigureAwait(false);
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
            var response = await beeGatewayApiClient.BzzPostAsync(
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

            return new ReferenceDto(response);
        }

        public async Task<Stream> BzzGetAsync(string reference, string? targets = null)
        {
            var response = await beeGatewayApiClient.BzzGetAsync(reference, targets, CancellationToken.None).ConfigureAwait(false);

            return response.Stream;
        }

        public async Task<Stream> BzzGetAsync(string reference, string path, string? targets = null)
        {
            var response = await beeGatewayApiClient.BzzGetAsync(reference, path, targets).ConfigureAwait(false);

            return response.Stream;
        }

        public async Task<IEnumerable<TagInfoDto>> TagsGetAsync(int? offset = null, int? limit = null)
        {
            var response = await beeGatewayApiClient.TagsGetAsync(offset, limit).ConfigureAwait(false);

            return response.Tags
                .Select(i => new TagInfoDto(i));
        }

        public async Task<TagInfoDto> CreateTagAsync(string address)
        {
            var response = await beeGatewayApiClient.TagsPostAsync(new Body3
            {
                Address = address
            }).ConfigureAwait(false);

            return new TagInfoDto(response);
        }

        public async Task<TagInfoDto> TagsGetAsync(int uid)
        {
            var response = await beeGatewayApiClient.TagsGetAsync(uid).ConfigureAwait(false);

            return new TagInfoDto(response);
        }

        public async Task DeleteTagAsync(int uid)
        {
            await beeGatewayApiClient.TagsDeleteAsync(uid).ConfigureAwait(false);
        }

        public async Task<VersionDto> UpdateTag(int uid, string? address = null)
        {
            var body = address is null ?
                null :
                new Body4 { Address = address };

            var response = await beeGatewayApiClient.TagsPatchAsync(uid, body).ConfigureAwait(false);

            return new VersionDto(response);
        }

        public async Task<MessageResponseDto> PinsPostAsync(string reference)
        {
            var response = await beeGatewayApiClient.PinsPostAsync(reference).ConfigureAwait(false);

            return new MessageResponseDto(response);
        }

        public async Task<MessageResponseDto> PinsDeleteAsync(string reference)
        {
            var response = await beeGatewayApiClient.PinsDeleteAsync(reference).ConfigureAwait(false);

            return new MessageResponseDto(response);
        }

        public async Task<string> PinsGetAsync(string reference)
        {
            return await beeGatewayApiClient.PinsGetAsync(reference).ConfigureAwait(false);
        }

        public async Task<IEnumerable<AddressDto>> PinsGetAsync()
        {
            var response = await beeGatewayApiClient.PinsGetAsync().ConfigureAwait(false);

            return response.Addresses
                .Select(i => new AddressDto(i));
        }

        public async Task PssSendAsync(
            string topic,
            string targets,
            string swarmPostageBatchId,
            string? recipient = null)
        {
            await beeGatewayApiClient.PssSendAsync(topic, targets, swarmPostageBatchId, recipient).ConfigureAwait(false);
        }

        public async Task PssSubscribeAsync(string topic)
        {
            await beeGatewayApiClient.PssSubscribeAsync(topic).ConfigureAwait(false);
        }

        public async Task<ReferenceDto> SocAsync(
            string owner,
            string id,
            string sig,
            bool? swarmPin = null)
        {
            var response = await beeGatewayApiClient.SocAsync(owner, id, sig, swarmPin).ConfigureAwait(false);

            return new ReferenceDto(response);
        }

        public async Task<ReferenceDto> FeedsPostAsync(
            string owner,
            string topic,
            string swarmPostageBatchId,
            string? type = null,
            bool? swarmPin = null)
        {
            var response = await beeGatewayApiClient.FeedsPostAsync(owner, topic, swarmPostageBatchId, type, swarmPin).ConfigureAwait(false);

            return new ReferenceDto(response);
        }

        public async Task<ReferenceDto> FeedsGetAsync(
            string owner,
            string topic,
            int? at = null,
            string? type = null)
        {
            var response = await beeGatewayApiClient.FeedsGetAsync(owner, topic, at, type).ConfigureAwait(false);

            return new ReferenceDto(response);
        }

        public async Task<StewardShipGetDto> StewardShipGetAsync(string reference)
        {
            var response = await beeGatewayApiClient.StewardshipGetAsync(reference).ConfigureAwait(false);

            return new StewardShipGetDto(response);
        }

        public async Task StewardShipPutAsync(string reference)
        {
            await beeGatewayApiClient.StewardshipPutAsync(reference).ConfigureAwait(false);
        }
    }
}