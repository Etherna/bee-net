﻿using Etherna.BeeNet.Clients.DebugApi;
using Etherna.BeeNet.Clients.GatewayApi;
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
            int? gatewayApiPort = 1633,
            int? debugApiPort = 1635)
        {
            httpClient = new HttpClient();

            // Generate api clients.
            if (debugApiPort is not null)
            {
                DebugApiUrl = new Uri(BuildBaseUrl(baseUrl, debugApiPort.Value));
                DebugClient = new BeeDebugClient(httpClient) { BaseUrl = DebugApiUrl.ToString() };
            }

            if (gatewayApiPort is not null)
            {
                GatewayApiUrl = new Uri(BuildBaseUrl(baseUrl, gatewayApiPort.Value));
                GatewayClient = new BeeGatewayClient(httpClient) { BaseUrl = GatewayApiUrl.ToString() };
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
