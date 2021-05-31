using Etherna.BeeNet.Clients.GatewayApi;
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.SampleClient.Operations
{
    class GetInfo
    {
        internal static async Task RunAsync(BeeNodeClient beeClient)
        {
            if (beeClient.DebugClient is null)
                throw new InvalidOperationException();

            // Get some addresses as sample info.
            try
            {
                var addressesResponse = await beeClient.DebugClient.AddressesAsync();

                Console.WriteLine($"Ethereum address:\t{addressesResponse.Ethereum}");
                Console.WriteLine($"Overlay address:\t{addressesResponse.Overlay}");
                Console.WriteLine($"Public key:\t\t{addressesResponse.PublicKey}");
                Console.WriteLine($"Pss public key:\t\t{addressesResponse.PssPublicKey}");
            }
            catch (BeeNetGatewayApiException)
            {
                Console.WriteLine("Invalid response");
            }
        }
    }
}
