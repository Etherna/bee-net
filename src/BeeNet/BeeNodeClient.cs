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
using Etherna.BeeNet.Clients.GatewayApi;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Etherna.BeeNet
{
    public class BeeNodeClient : IBeeNodeClient, IDisposable
    {
        // Fields.
        private readonly HttpClient httpClient;
        private bool disposed;

        // Constructors.
        [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings",
            Justification = "A string is required by Nswag generated client")]
        public BeeNodeClient(
            string baseUrl = "http://localhost/",
            int? gatewayApiPort = 1633,
            int? debugApiPort = 1635,
            GatewayApiVersion gatewayApiVersion = GatewayApiVersion.v3_0_2,
            DebugApiVersion debugApiVersion = DebugApiVersion.v2_0_1)
        {
            httpClient = new HttpClient();

            if (debugApiPort is not null)
            {
                DebugApiUrl = new Uri(BuildBaseUrl(baseUrl, debugApiPort.Value));
                DebugClient = new BeeDebugClient(httpClient, DebugApiUrl, debugApiVersion);
            }

            if (gatewayApiPort is not null)
            {
                GatewayApiUrl = new Uri(BuildBaseUrl(baseUrl, gatewayApiPort.Value));
                GatewayClient = new BeeGatewayClient(httpClient, GatewayApiUrl, gatewayApiVersion);
            }
        }

        [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings",
            Justification = "A string is required by Nswag generated client")]
        static public async Task<BeeNodeClient?> AuthenticatedBeeNodeClientAsync(
            BeeAuthicationData beeAuthicationData,
            string baseUrl = "http://localhost/",
            int? gatewayApiPort = 1633,
            int? debugApiPort = 1635,
            GatewayApiVersion gatewayApiVersion = GatewayApiVersion.v3_0_2,
            DebugApiVersion debugApiVersion = DebugApiVersion.v2_0_1)
        {
#pragma warning disable CA2000 // Dispose objects must be done by client
            var nodeClient = new BeeNodeClient(baseUrl, gatewayApiPort, debugApiPort, gatewayApiVersion, debugApiVersion);
#pragma warning restore CA2000 

            if (nodeClient.GatewayClient is null)
                return null;

            var authDto = await nodeClient.GatewayClient.AuthenticateAsync(beeAuthicationData, "", 0).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(authDto.Key))
                return null;

            nodeClient.GatewayClient.SetAuthToken(authDto.Key);

            return nodeClient;
        }

        // Dispose.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            // Dispose managed resources.
            if (disposing)
                httpClient.Dispose();

            disposed = true;
        }


        // Properties.
        public Uri? DebugApiUrl { get; }
        public IBeeDebugClient? DebugClient { get; }
        public Uri? GatewayApiUrl { get; }
        public IBeeGatewayClient? GatewayClient { get; }

        // Helpers.
        private static string BuildBaseUrl(string url, int port)
        {
            var normalizedUrl = url;
            if (normalizedUrl.Last() != '/')
                normalizedUrl += '/';

            var baseUrl = "";

            var urlRegex = new Regex(@"^((?<proto>\w+)://)?(?<host>[^/:]+)",
                RegexOptions.None, TimeSpan.FromMilliseconds(150));
            var urlMatch = urlRegex.Match(normalizedUrl);

            if (!urlMatch.Success)
                throw new ArgumentException("Url is not valid", nameof(url));

            if (!string.IsNullOrEmpty(urlMatch.Groups["proto"].Value))
                baseUrl += urlMatch.Groups["proto"].Value + "://";

            baseUrl += $"{urlMatch.Groups["host"].Value}:{port}";

            return baseUrl;
        }
    }
}