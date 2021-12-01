using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using TestAdapter.Clients;

namespace TestAdapter
{
    public class BeeNodeClient : IBeeNodeClient, IDisposable
    {
        private readonly HttpClient httpClient;
        private bool disposed;
        public Uri DebugApiUrl { get; }
        public IFacadeBeeDebugClient DebugClient { get; }
        public Uri GatewayApiUrl { get; }
        public IFacadeBeeGatewayApiClient GatewayClient { get; }

        public BeeNodeClient(
            string baseUrl = "http://localhost/",
            int? gatewayApiPort = 1633,
            int? debugApiPort = 1635,
            string version = "default")
        {
            httpClient = new HttpClient();

            // Generate api clients.
            if (debugApiPort != null)
            {
                DebugApiUrl = new Uri(BuildBaseUrl(baseUrl, debugApiPort.Value));
                DebugClient = new FacadeBeeDebugClient(version, httpClient, DebugApiUrl.ToString());
            }

            if (gatewayApiPort != null)
            {
                GatewayApiUrl = new Uri(BuildBaseUrl(baseUrl, gatewayApiPort.Value));
                GatewayClient = new FacadeBeeGatewayApiClient(version, httpClient, GatewayApiUrl.ToString());
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
