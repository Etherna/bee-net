using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.DebugApi.v2_0_0
{
    public class TagTest
    {
        private readonly BeeNodeClient beeNodeClient;

        public TagTest()
        {
            beeNodeClient = new BeeNodeClient(
                "http://localhost/",
                1633,
                1635,
                Etherna.BeeNet.Clients.GatewayApi.GatewayApiVersion.v3_0_0,
                Etherna.BeeNet.Clients.DebugApi.DebugApiVersion.v2_0_0);
        }

        [Fact]
        public async Task CreateTagAsync()
        {
            // Arrange 
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("430b505bb0361b7a508559c10a6a9ea2b68a7320dabbddad585d0db78ba96a63");


            // Act 
            var tagInfo = await beeNodeClient.DebugClient.GetTagInfoAsync(tag.Uid);


            // Assert 
            Assert.True(tagInfo.Uid > 0);
        }

    }
}
