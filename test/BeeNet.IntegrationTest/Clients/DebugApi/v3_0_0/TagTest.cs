using Etherna.BeeNet;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.Clients.DebugApi.v3_0_0
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

            /*
                        var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Get, "/tags");
                        client.BaseAddress = new Uri("http://localhost:1633");
                        var response = await client.SendAsync(request);


                        response.EnsureSuccessStatusCode();

                        var responseString = await response.Content.ReadAsStringAsync();
            */




            //var postRequest = new HttpRequestMessage(HttpMethod.Get, "/topology");
            /*var formModel = new Dictionary<string, string>
            {
                { "Name", "New Employee" },
                { "Age", "25" }
            };
            postRequest.Content = new FormUrlEncodedContent(formModel);*/
        }
    }
}
