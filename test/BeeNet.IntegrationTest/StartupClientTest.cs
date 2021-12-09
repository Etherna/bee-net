using System;
using Etherna.BeeNet;
using Etherna.BeeNet.Clients;
using Xunit;

namespace BeeNet.IntegrationTest
{
    public class StartupClientTest : BaseTest
    {
        [Fact]
        public void ShouldSupport_Version_1_4()
        {
            var client = new BeeNodeClient(BaseUrl, 1633, 1635, ClientVersionEnum.v_1_4);

            Assert.Equal(ClientVersionEnum.v_1_4, client.ClientVersion);
        }

        [Fact]
        public void ShouldSupport_DefaultVersion_1_4()
        {
            var client = new BeeNodeClient(BaseUrl, 1633, 1635);

            Assert.Equal(ClientVersionEnum.v_1_4, client.ClientVersion);
        }
    }
}
