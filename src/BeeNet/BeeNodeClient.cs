using Etherna.BeeNet.Clients;
using Etherna.BeeNet.Clients.v_1_4.DebugApi;
using Etherna.BeeNet.Clients.v_1_4.GatewayApi;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

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
            int gatewayApiPort = 1633,
            int debugApiPort = 1635,
            string version = "default")
        {
            httpClient = new HttpClient();

            // Generate api clients.
            switch (version)
            {
                case "1.5":
                    //_beeDebugClient = new AdapterBeeVersion_1_5(httpClient, baseUrl);
                    break;
                case "1.4":
                default:
                    DebugApiUrl = new Uri(BuildBaseUrl(baseUrl, debugApiPort));
                    DebugClient = new AdapterBeeDebugVersion_1_4(httpClient, DebugApiUrl.ToString());
                    GatewayApiUrl = new Uri(BuildBaseUrl(baseUrl, gatewayApiPort));
                    GatewayClient = new AdapterGatewayApiClient_1_4(httpClient, GatewayApiUrl.ToString());
                    break;
            }
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
        public Uri DebugApiUrl { get; }
        public IBeeNodeDebugClient DebugClient { get; }
        public Uri GatewayApiUrl { get; }
        public IBeeGatewayApiClient GatewayClient { get; }

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