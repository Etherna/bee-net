using Etherna.BeeNet;

namespace BeeNet.IntegrationTest.BeeVersions.v1_6_0
{
    public abstract class BaseTest_Debug_v2_0_1
    {
        protected readonly BeeNodeClient beeNodeClient;
        protected string pathTestFileForUpload = "Data\\TestFileForUpload_Debug.txt";
        protected readonly string ethAddress = "0x26234a2ad3ba8b398a762f279b792cfacd536a3f";

        public BaseTest_Debug_v2_0_1()
        {
            beeNodeClient = new BeeNodeClient(
                "http://192.168.1.103/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_0_1,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v2_0_1);
        }
    }
}
