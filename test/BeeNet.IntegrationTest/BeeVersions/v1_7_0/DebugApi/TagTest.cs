using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_7_0.DebugApi
{
    public class TagTest : BaseTest_Debug_v3_0_2
    {

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
