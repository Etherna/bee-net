using Etherna.BeeNet.Clients;
using Etherna.BeeNet.DtoModel;
using System.Threading.Tasks;
using Xunit;

namespace Etherna.BeeNet.IntegrationTest.BeeDebugClient.v1_4
{
    public class EndPointApiTest 
    {
        private readonly IBeeDebugClient _client;
        private const string _beeSwarmNodeUrl = "http://localhost";
        private const int _beeSwarmNodeDebugPort = 1635;
        private const int _beeSwarmNodeGatewayPort = 1633;

        public EndPointApiTest()
        {
            _client = new BeeNodeClient(_beeSwarmNodeUrl, _beeSwarmNodeGatewayPort, _beeSwarmNodeDebugPort, ClientVersions.v1_4).DebugClient;
        }
        
        [Fact]
        public async Task AddressesAsync()
        {
            var result = await _client.AddressesAsync();


            Assert.Equal("", result.Overlay);
            Assert.Equal(0, result.Underlay.Count);
            Assert.Equal("0xa61a68a1d460d01899400b50818090aa26554c1d", result.Ethereum);
            Assert.Equal("023777f04933dc35ce3981a2db52ec142e84bd5870d48ca38eabae295312168ec2", result.PublicKey);
            Assert.Equal("023777f04933dc35ce3981a2db52ec142e84bd5870d48ca38eabae295312168ec2", result.PssPublicKey);
        }


        [Fact]
        public async Task GetBalancesAsync()
        {
            var result = await _client.GetBalancesAsync();
            //TODO empty response need data for test
        }


        [Fact]
        public async Task GetBalanceAsync()
        {
            var result = await _client.GetBalanceAsync("address");
            //TODO empty response need data for test
        }


        [Fact]
        public async Task BlocklistAsync()
        {
            var result = await _client.BlocklistAsync();
            //TODO empty response need data for test
        }


        [Fact]
        public async Task ConsumedGetAsync()
        {
            var result = await _client.ConsumedGetAsync();
            //TODO empty response need data for test
        }


        [Fact]
        public async Task ConsumedGetWithAddressAsync()
        {
            var result = await _client.ConsumedGetAsync("address");
            //TODO empty response need data for test
        }


        [Fact]
        public async Task ChequebookAddressAsync()
        {
            var result = await _client.ChequebookAddressAsync();
            

            Assert.Equal("0x0000000000000000000000000000000000000000", result.ChequebookAddress);
        }


        [Fact]
        public async Task ChequebookBalanceAsync()
        {
            var result = await _client.ChequebookBalanceAsync();


            Assert.Equal("0", result.AvailableBalance);
            Assert.Equal("0", result.TotalBalance);
        }


        [Fact]
        public async Task ChunksGetAsync()
        {
            var result = await _client.ChunksGetAsync("address");
            //TODO empty response need data for test
        }


        [Fact]
        public async Task ChunksDeleteAsync()
        {
            var result = await _client.ChunksDeleteAsync("address");
            //TODO empty response need data for test
        }


        [Fact]
        public async Task ConnectAsync()
        {
            var result = await _client.ConnectAsync("multiAddress");
            //TODO what is the correct input?
        }


        [Fact]
        public async Task ReservestateAsync()
        {
            var result = await _client.ReservestateAsync();


            Assert.Equal(12, result.Radius);
            Assert.Equal("0", result.Inner);
            Assert.Equal("0", result.Outer);
            Assert.Equal(4194304, result.Available);
            //Assert.Equal(, result.StorageRadius); //TODO MISSING THAT
        }


        [Fact]
        public async Task ChainstateAsync()
        {
            var result = await _client.ChainstateAsync();

            Assert.Equal(0, result.Block);
            Assert.Equal(0, result.TotalAmount);//TODO totalAmount response from Endpoint was string instead of int
            Assert.Equal(0, result.CurrentPrice);//TODO currentPrice response from Endpoint was string instead of int
        }


        [Fact]
        public async Task HealthAsync()
        {
            var result = await _client.HealthAsync();


            Assert.Equal("ok", result.Status);
            Assert.Equal("1.4.1-238867f1", result.Version);
            Assert.Equal("0.0.0", result.ApiVersion);
            Assert.Equal("0.0.0", result.DebugApiVersion);
            //TODO empty response need data for test
        }

        
        [Fact]
        public async Task PeersGetAsync()
        {
            var result = await _client.PeersGetAsync();


            //TODO empty response need data for test
        }


        [Fact]
        public async Task PeersDeleteAsync()
        {
            var result = await _client.PeersDeleteAsync("address");
            //TODO empty response need data for test
        }


        [Fact]
        public async Task PingpongAsync()
        {
            var result = await _client.PingpongAsync("peerId");
            //TODO empty response need data for test
            //TODO Response:400   "invalid peer address"
        }


        [Fact]
        public async Task ReadinessAsync()
        {
            var result = await _client.ReadinessAsync();



            Assert.Equal("ok", result.Status);
            Assert.Equal("1.4.1-238867f1", result.Version);
            Assert.Equal("0.0.0", result.ApiVersion);
            Assert.Equal("0.0.0", result.DebugApiVersion);
        }


        [Fact]
        public async Task SettlementsGetAsync()
        {
            var result = await _client.SettlementsGetAsync();


            Assert.NotEmpty(result.Settlements);//TODO empty response need data for test
            Assert.Equal(0, result.TotalSent);//TODO totalSent response from Endpoint was string instead of int
            Assert.Equal(0, result.TotalReceived);//TODO totalReceived response from Endpoint was string instead of int
        }


        [Fact]
        public async Task SettlementsWithAddressGetAsync()
        {
            var result = await _client.SettlementsGetAsync("address");
            //TODO empty response need data for test
        }


        [Fact]
        public async Task TimesettlementsAsync()
        {
            var result = await _client.TimesettlementsAsync();


            Assert.NotEmpty(result.Settlements);//TODO empty response need data for test
            Assert.Equal(0, result.TotalSent);//TODO totalSent response from Endpoint was string instead of int
            Assert.Equal(0, result.TotalReceived);//TODO totalReceived response from Endpoint was string instead of int
        }


        [Fact]
        public async Task TopologyAsync()
        {
            var result = await _client.TopologyAsync();
            //TODO empty response need data for test
        }


        [Fact]
        public async Task WelcomeMessageGetAsync()
        {
            var result = await _client.WelcomeMessageGetAsync();
            //TODO empty response need data for test
        }


        [Fact]
        public async Task WelcomeMessagePostAsync()
        {
            var result = await _client.WelcomeMessagePostAsync("welcomeMessage");
            //TODO empty response need data for test
        }


        [Fact]
        public async Task ChequebookCashoutGetAsync()
        {
            var result = await _client.ChequebookCashoutGetAsync("peerId");
            //TODO empty response need data for test
        }

        
        [Fact]
        public async Task ChequebookCashoutPostAsync()
        {
            string peerId = "";//TODO insert real data
            int? gasPrice = null;//TODO insert real data
            long? gasLimit = null;//TODO insert real data


            var result = await _client.ChequebookCashoutPostAsync(peerId, gasPrice, gasLimit);
        }


        [Fact]
        public async Task ChequebookChequeGetAsync_WithId()
        {
            string peerId = "";//TODO insert real data


            var result = await _client.ChequebookChequeGetAsync(peerId);
        }


        [Fact]
        public async Task ChequebookChequeGetAsync()
        {
            var result = await _client.ChequebookChequeGetAsync();
            //TODO empty response need data for test
        }

        
        [Fact]
        public async Task ChequebookDepositAsync()
        {
            int amount = 0;//TODO insert real data
            int? gasPrice = null;//TODO insert real data


            var result = await _client.ChequebookDepositAsync(amount, gasPrice);
            //TODO empty response need data for test
        }


        [Fact]
        public async Task ChequebookWithdrawAsync()
        {
            int amount = 0;//TODO insert real data
            int? gasPrice = null;//TODO insert real data


            var result = await _client.ChequebookWithdrawAsync(amount, gasPrice);
        }


        [Fact]
        public async Task TagAsync()
        {
            int uid = 0;//TODO insert real data


            var result = await _client.TagAsync(uid);
        }


        [Fact]
        public async Task TransactionsGetAsync()
        {
            var result = await _client.TransactionsGetAsync();
            //TODO empty response need data for test
        }


        [Fact]
        public async Task TransactionsGetWithHashAsync()
        {
            string txHash = "";//TODO insert real data


            var result = await _client.TransactionsGetAsync(txHash);
        }


        [Fact]
        public async Task TransactionsPostAsync()
        {
            string txHash = "";//TODO insert real data


            var result = await _client.TransactionsPostAsync(txHash);
        }


        [Fact]
        public async Task TransactionsDeleteAsync()
        {
            string txHash = "";//TODO insert real data
            int? gasPrice = null;//TODO insert real data


            var result = await _client.TransactionsDeleteAsync(txHash, gasPrice);
            //TODO empty response need data for test
        }


        [Fact]
        public async Task StampsGetAsync()
        {
            var result = await _client.StampsGetAsync();
            //TODO empty response need data for test
        }


        [Fact]
        public async Task StampsGetAsync_WithId()
        {
            object id = 1;//TODO insert real data

            var result = await _client.StampsGetAsync(id);
        }


        [Fact]
        public async Task StampsBucketsAsync()
        {
            object id = 1;//TODO insert real data


            var result = await _client.StampsBucketsAsync(id);
        }


        [Fact]
        public async Task StampsPostAsync()
        {
            int amount = 0;//TODO insert real data
            int depth = 0;//TODO insert real data
            string label = "";//TODO insert real data
            bool? immutable = null;//TODO insert real data
            int? gasPrice = null;//TODO insert real data


            var result = await _client.StampsPostAsync(amount, depth, label, immutable, gasPrice);
        }


        [Fact]
        public async Task StampsTopupAsync()
        {
            object id = 1;//TODO insert real data
            int amount = 0;//TODO insert real data


            var result = await _client.StampsTopupAsync(id, amount);
        }


        [Fact]
        public async Task StampsDiluteAsync()
        {
            object id = 1;//TODO insert real data
            int depth = 0;//TODO insert real data


            var result = await _client.StampsDiluteAsync(id, depth);
        }
        
    }
}
