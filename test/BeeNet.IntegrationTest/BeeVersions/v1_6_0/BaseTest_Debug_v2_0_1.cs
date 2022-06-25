using Etherna.BeeNet;

namespace BeeNet.IntegrationTest.BeeVersions.v1_6_0
{
    public abstract class BaseTest_Debug_v2_0_1
    {
        protected readonly BeeNodeClient beeNodeClient;
        protected string pathTestFileForUpload = "Data\\TestFileForUpload_Debug.txt";
        protected readonly string ethAddress = "0x26234a2ad3ba8b398a762f279b792cfacd536a3f";
        protected readonly string peerId = "1ad59c25783b028275f5f542cfaed34d9bb4adf0818cd8de07582311857d78f4";
        protected const string version = "2.0.1";

        public BaseTest_Debug_v2_0_1()
        {
            beeNodeClient = new BeeNodeClient(
                System.Environment.GetEnvironmentVariable("BeeNet_IT_NodeEndPoint") ?? "http://89.145.161.170/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_0_1,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v2_0_1);
        }
    }
}
