using System;
using Etherna.BeeNet;
using Etherna.BeeNet.Clients;
using Etherna.BeeNet.DtoModel;
using Xunit;

namespace BeeNet.UnitTest
{
    public class StartupClientTest
    {
        [Fact]
        public void ShouldSupport_Version_1_4()
        {
            var client = new BeeNodeClient("localhost", 1633, 1635, ClientVersions.v1_4);

            Assert.Equal(ClientVersions.v1_4, client.ClientVersion);
        }

        [Fact]
        public void ShouldSupport_DefaultVersion_1_4()
        {
            var client = new BeeNodeClient("localhost", 1633, 1635);

            Assert.Equal(ClientVersions.v1_4, client.ClientVersion);
        }
    }
}
