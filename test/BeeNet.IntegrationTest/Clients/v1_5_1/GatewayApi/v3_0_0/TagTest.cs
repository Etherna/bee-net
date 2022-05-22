using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.v1_5_1.GatewayApi.v3_0_0
{
    public class TagTest : BaseTest_Gateway_v3_0_0
    {
        [Fact]
        public async Task CreateTagAsync()
        {
            // Arrange 


            // Act 
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("430b505bb0361b7a508559c10a6a9ea2b68a7320dabbddad585d0db78ba96a63");


            // Assert 
            Assert.True(tag.Uid > 0);
        }

        [Fact]
        public async Task DeleteTagAsync()
        {
            // Arrange 
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("430b505bb0361b7a508559c10a6a9ea2b68a7320dabbddad585d0db78ba96a63");


            // Act 
            await beeNodeClient.GatewayClient.DeleteTagAsync(tag.Uid);


            // Assert 
            var tags = await beeNodeClient.GatewayClient.GetTagsListAsync();
            Assert.DoesNotContain(tags, t => t.Uid == tag.Uid);
        }

        [Fact]
        public async Task GetTagInfoAsync()
        {
            // Arrange 
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("430b505bb0361b7a508559c10a6a9ea2b68a7320dabbddad585d0db78ba96a63");


            // Act 
            var tagInfo = await beeNodeClient.GatewayClient.GetTagInfoAsync(tag.Uid);


            // Assert 
            Assert.Equal(tagInfo.Uid, tag.Uid);
            Assert.Equal(tagInfo.StartedAt, tag.StartedAt);
            Assert.Equal(tagInfo.Total, tag.Total);
            Assert.Equal(tagInfo.Processed, tag.Processed);
            Assert.Equal(tagInfo.Synced, tag.Synced);
        }

        [Fact]
        public async Task GetTagsListAsync()
        {
            // Arrange 
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("430b505bb0361b7a508559c10a6a9ea2b68a7320dabbddad585d0db78ba96a63");


            // Act 
            var tags = await beeNodeClient.GatewayClient.GetTagsListAsync();


            // Assert 
            Assert.Contains(tags, t =>t.Uid == tag.Uid);
        }

        [Fact]
        public async Task UpdateTagAsync()
        {
            // Arrange 
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("430b505bb0361b7a508559c10a6a9ea2b68a7320dabbddad585d0db78ba96a63");


            // Act 
            await beeNodeClient.GatewayClient.UpdateTagAsync(tag.Uid);


            // Assert 
        }
    }
}
