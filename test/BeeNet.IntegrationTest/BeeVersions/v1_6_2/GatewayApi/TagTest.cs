using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_6_2.GatewayApi
{
    public class TagTest : BaseTest_Gateway_v3_0_2
    {
        [Fact]
        public async Task GetTagsListAsync()
        {
            // Arrange 
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("430b505bb0361b7a508559c10a6a9ea2b68a7320dabbddad585d0db78ba96a63");
            var tag2 = await beeNodeClient.GatewayClient.CreateTagAsync("430b505bb0361b7a508559c10a6a9ea2b68a7320dabbddad585d0db78ba96a63");


            // Act 
            var tagInfo = await beeNodeClient.GatewayClient.GetTagsListAsync();


            // Assert 
            Assert.Contains(tagInfo, i =>i.Uid == tag.Uid);
            Assert.Contains(tagInfo, i => i.Uid == tag2.Uid);
        }

        [Fact]
        public async Task CreateTagAsync()
        {
            // Arrange 
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("430b505bb0361b7a508559c10a6a9ea2b68a7320dabbddad585d0db78ba96a63");


            // Act 
            var tagInfo = await beeNodeClient.GatewayClient.GetTagInfoAsync(tag.Uid);


            // Assert 
            Assert.True(tagInfo.Uid > 0);
        }

        [Fact]
        public async Task GetTagInfoAsync()
        {
            // Arrange 
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("430b505bb0361b7a508559c10a6a9ea2b68a7320dabbddad585d0db78ba96a63");


            // Act 
            var tagInfo = await beeNodeClient.GatewayClient.GetTagInfoAsync(tag.Uid);


            // Assert 
            Assert.True(tagInfo.Uid > 0);
        }

        [Fact]
        public async Task UpdateTagAsync()
        {
            // Arrange 
            var tag = await beeNodeClient.GatewayClient.CreateTagAsync("430b505bb0361b7a508559c10a6a9ea2b68a7320dabbddad585d0db78ba96a63");


            // Act 
            var tagInfo = await beeNodeClient.GatewayClient.UpdateTagAsync(tag.Uid);


            // Assert 
        }

    }
}
