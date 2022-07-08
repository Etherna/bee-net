using Etherna.BeeNet;

namespace BeeNet.IntegrationTest.BeeVersions.v1_6_2
{
    public abstract class BaseTest_Debug_v2_0_1
    {
        protected readonly BeeNodeClient beeNodeClient;
        protected string pathTestFileForUpload = "Data\\TestFileForUpload_Debug.txt";
        protected readonly string ethAddress = "0x26234a2ad3ba8b398a762f279b792cfacd536a3f";
        protected readonly string peerId = "8379f8a710eb0f533661c06797a1c21e8ae03983dcbad97feffad2eff409d224";
        protected const string version = "2.0.1";

        public BaseTest_Debug_v2_0_1()
        {
            beeNodeClient = new BeeNodeClient(
                System.Environment.GetEnvironmentVariable("BeeNet_IT_NodeEndPoint") ?? "http://89.145.161.170/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_0_2,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v2_0_1);
        }
    }
}
