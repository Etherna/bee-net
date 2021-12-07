using Etherna.BeeNet.Clients.v_1_4.DebugApi;
using Etherna.BeeNet.Clients.v_1_4.GatewayApi;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Etherna.BeeNet.Clients;

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
            string? version = "default")
        {
            httpClient = new HttpClient();

            if (debugApiPort is not null)
            {
                DebugApiUrl = new Uri(BuildBaseUrl(baseUrl, debugApiPort.Value));
                DebugClient = version switch
                {
                    "1.4" => new AdapterBeeDebugVersion_1_4(httpClient, DebugApiUrl),
                    _ => new AdapterBeeDebugVersion_1_4(httpClient, DebugApiUrl),
                };
            }

            if (gatewayApiPort is not null)
            {
                GatewayApiUrl = new Uri(BuildBaseUrl(baseUrl, gatewayApiPort.Value));
                GatewayClient = version switch
                {
                    "1.4" => new AdapterGatewayClient14(httpClient, GatewayApiUrl),
                    _ => new AdapterGatewayClient14(httpClient, GatewayApiUrl),
                };
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