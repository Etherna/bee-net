using Etherna.BeeNet.Clients.DebugApi;
using Etherna.BeeNet.Clients.GatewayApi;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace Etherna.BeeNet
{
    public class BeeNetClient : IBeeNetClient, IDisposable
    {
        // Fields.
        private readonly HttpClient httpClient;
        private bool disposed;

        // Constructors.
        [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings",
            Justification = "A string is required by Nswag generated client")]
        public BeeNetClient(
            string baseUrl = "http://localhost",
            int gatewayApiPort = 1633,
            int debugApiPort = 1635)
        {
            httpClient = new HttpClient();

            // Generate api clients.
            BeeDebugClient = new BeeDebugClient(httpClient)
            { BaseUrl = string.Concat(baseUrl, ':', debugApiPort) };

            BeeGatewayClient = new BeeGatewayClient(httpClient)
            { BaseUrl = string.Concat(baseUrl, ':', gatewayApiPort) };
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
        public IBeeDebugClient BeeDebugClient { get; }
        public IBeeGatewayClient BeeGatewayClient { get; }
    }
}
