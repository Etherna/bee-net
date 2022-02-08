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

using Etherna.BeeNet.DtoModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Clients.GatewayApi.v2_0_0
{
    public class AdapterGatewayClient_2_0_0 : IBeeGatewayClient
    {
        // Fields.
        private readonly IBeeGatewayClient_2_0_0 beeGatewayApiClient;


        // Constructors.
        public AdapterGatewayClient_2_0_0(HttpClient httpClient, Uri baseUrl)
        {
            if (baseUrl is null)
                throw new ArgumentNullException(nameof(baseUrl));

            beeGatewayApiClient = new BeeGatewayClient_2_0_0(httpClient) { BaseUrl = baseUrl.ToString() };
        }

        // Methods.
        public async Task<AuthDto> AuthenticateAsync(string role, int expiry)
        {
            var response = await beeGatewayApiClient.AuthAsync(new Body
            {
                Role = role,
                Expiry = expiry
            }).ConfigureAwait(false);

            return new AuthDto(response);
        }

        public async Task<StewardShipGetDto> CheckIsContentAvailableAsync(string reference)
        {
            var response = await beeGatewayApiClient.StewardshipGetAsync(reference).ConfigureAwait(false);

            return new StewardShipGetDto(response);
        }

        public async Task<ReferenceDto> CreateFeedAsync(
            string owner,
            string topic,
            string swarmPostageBatchId,
            string? type = null,
            bool? swarmPin = null)
        {
            var response = await beeGatewayApiClient.FeedsPostAsync(owner, topic, swarmPostageBatchId, type, swarmPin).ConfigureAwait(false);

            return new ReferenceDto(response);
        }

        public async Task<MessageResponseDto> CreatePinAsync(string reference)
        {
            var response = await beeGatewayApiClient.PinsPostAsync(reference).ConfigureAwait(false);

            return new MessageResponseDto(response);
        }

        public async Task<TagInfoDto> CreateTagAsync(string address)
        {
            var response = await beeGatewayApiClient.TagsPostAsync(new Body3
            {
                Address = address
            }).ConfigureAwait(false);

            return new TagInfoDto(response);
        }

        public async Task<MessageResponseDto> DeletePinAsync(string reference)
        {
            var response = await beeGatewayApiClient.PinsDeleteAsync(reference).ConfigureAwait(false);

            return new MessageResponseDto(response);
        }

        public async Task DeleteTagAsync(int uid)
        {
            await beeGatewayApiClient.TagsDeleteAsync(uid).ConfigureAwait(false);
        }

        public async Task<IEnumerable<AddressDto>> GetAllPinsAsync()
        {
            var response = await beeGatewayApiClient.PinsGetAsync().ConfigureAwait(false);

            return response.Addresses
                .Select(i => new AddressDto(i));
        }

        public async Task<Stream> GetChunkStreamAsync(string reference, string? targets = null)
        {
            var response = await beeGatewayApiClient.ChunksGetAsync(reference, targets).ConfigureAwait(false);

            return response.Stream;
        }

        public async Task<Stream> GetDataAsync(string reference)
        {
            var response = await beeGatewayApiClient.BytesGetAsync(reference).ConfigureAwait(false);

            return response.Stream;
        }

        public async Task<ReferenceDto> GetFeedAsync(
            string owner,
            string topic,
            int? at = null,
            string? type = null)
        {
            var response = await beeGatewayApiClient.FeedsGetAsync(owner, topic, at, type).ConfigureAwait(false);

            return new ReferenceDto(response);
        }

        public async Task<Stream> GetFileAsync(string reference, string path, string? targets = null)
        {
            var response = await beeGatewayApiClient.BzzGetAsync(reference, path, targets).ConfigureAwait(false);

            return response.Stream;
        }

        public async Task<Stream> GetFileAsync(string reference, string? targets = null)
        {
            var response = await beeGatewayApiClient.BzzGetAsync(reference, targets, CancellationToken.None).ConfigureAwait(false);

            return response.Stream;
        }

        public async Task<string> GetPinStatusAsync(string reference)
        {
            return await beeGatewayApiClient.PinsGetAsync(reference).ConfigureAwait(false);
        }

        public async Task<TagInfoDto> GetTagInfoAsync(int uid)
        {
            var response = await beeGatewayApiClient.TagsGetAsync(uid).ConfigureAwait(false);

            return new TagInfoDto(response);
        }

        public async Task<IEnumerable<TagInfoDto>> GetTagsListAsync(int? offset = null, int? limit = null)
        {
            var response = await beeGatewayApiClient.TagsGetAsync(offset, limit).ConfigureAwait(false);

            return response.Tags
                .Select(i => new TagInfoDto(i));
        }

        public async Task<RefreshDto> RefreshAuthAsync(string role, int expiry)
        {
            var response = await beeGatewayApiClient.RefreshAsync(new Body2
            {
                Role = role,
                Expiry = expiry
            }).ConfigureAwait(false);

            return new RefreshDto(response);
        }

        public async Task ReuploadContentAsync(string reference)
        {
            await beeGatewayApiClient.StewardshipPutAsync(reference).ConfigureAwait(false);
        }

        public async Task SendPssAsync(
            string topic,
            string targets,
            string swarmPostageBatchId,
            string? recipient = null)
        {
            await beeGatewayApiClient.PssSendAsync(topic, targets, swarmPostageBatchId, recipient).ConfigureAwait(false);
        }

        public async Task SubscribeToPssAsync(string topic)
        {
            await beeGatewayApiClient.PssSubscribeAsync(topic).ConfigureAwait(false);
        }

        public async Task<VersionDto> UpdateTagAsync(int uid, string? address = null)
        {
            var body = address is null ?
                null :
                new Body4 { Address = address };

            var response = await beeGatewayApiClient.TagsPatchAsync(uid, body).ConfigureAwait(false);

            return new VersionDto(response);
        }

        public async Task<VersionDto> UploadChunkAsync(
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

        public async Task UploadChunksStreamAsync(
            string swarmPostageBatchId,
            int? swarmTag = null,
            bool? swarmPin = null)
        {
            await beeGatewayApiClient.ChunksStreamAsync(swarmPostageBatchId, swarmTag, swarmPin).ConfigureAwait(false);
        }

        public async Task<ReferenceDto> UploadDataAsync(
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

        public async Task<ReferenceDto> UploadFileAsync(
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

        public async Task<ReferenceDto> UploadSocAsync(
            string owner,
            string id,
            string sig,
            bool? swarmPin = null)
        {
            var response = await beeGatewayApiClient.SocAsync(owner, id, sig, swarmPin).ConfigureAwait(false);

            return new ReferenceDto(response);
        }
    }
}