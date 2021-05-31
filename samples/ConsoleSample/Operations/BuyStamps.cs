using Etherna.BeeNet.Clients.GatewayApi;
using System;
using System.Threading.Tasks;

namespace Etherna.BeeNet.SampleClient.Operations
{
    class BuyStamps
    {
        internal static async Task RunAsync(BeeNodeClient beeClient, long ammount, int depth)
        {
            if (beeClient.GatewayClient is null)
                throw new InvalidOperationException();

            // Try to buy stamps.
            try
            {
                var batchResponse = await beeClient.GatewayClient.StampsPostAsync(ammount, depth, gas_price: 30);
                Console.WriteLine("Stamps batch id: " + batchResponse.BatchID);
            }
            catch (BeeNetGatewayApiException)
            {
                Console.WriteLine("Invalid request");
            }
        }
    }
}
