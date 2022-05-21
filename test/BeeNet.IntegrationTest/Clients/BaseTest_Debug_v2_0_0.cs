using Etherna.BeeNet;

namespace BeeNet.IntegrationTest.Clients
{
    public abstract class BaseTest_Debug_v2_0_0
    {
        protected readonly BeeNodeClient beeNodeClient;
        protected string pathTestFileForUpload = "Data\\TestFileForUpload_Debug.txt";

        public BaseTest_Debug_v2_0_0()
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
