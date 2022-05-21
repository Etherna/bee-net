using Etherna.BeeNet;

namespace BeeNet.IntegrationTest.Clients
{
    public abstract class BaseTest_Gateway_v3_0_0
    {
        protected readonly BeeNodeClient beeNodeClient;
        protected string pathTestFileForUpload = "Data\\TestFileForUpload_Gateway.txt";

        public BaseTest_Gateway_v3_0_0()
        {
            beeNodeClient = new BeeNodeClient(
                "http://192.168.1.103/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_0_0,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v2_0_0);
        }
    }
}
