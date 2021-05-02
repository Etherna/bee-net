using Etherna.BeeNet;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        // Methods.
        [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings", Justification = "A string is required by Nswag generated client")]
        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Registering Dependency Injection here")]
        public static void AddBeeNet(this IServiceCollection services,
            string baseUrl = "http://localhost",
            int gatewayApiPort = 1633,
            int debugApiPort = 1635)
        {
            if (baseUrl is null)
                throw new ArgumentNullException(nameof(baseUrl));

            services.AddSingleton<IBeeNetClient>(new BeeNetClient(baseUrl, gatewayApiPort, debugApiPort));
        }
    }
}
