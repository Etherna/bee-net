using BeeNet.IntegrationTest.BeeVersions.v1_6_0;
using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_6_2.GatewayApi
{
    public class TransactionTest : BaseTest_Gateway_v3_0_2
    {

        [Fact]
        public async Task GetPendingTransactionsAsync()
        {
            // Arrange 


            // Act 
            var pendingTransactions = await beeNodeClient.GatewayClient.GetPendingTransactionsAsync(); //TODO How to get transaction


            // Assert
        }
        /*
        [Fact]
        public async Task GetTransactionInfoAsync()
        {
            // Arrange 


            // Act 
            var pendingTransactions = await beeNodeClient.DebugClient.GetTransactionInfoAsync(""); //TODO How to get transaction


            // Assert
        }
        
        [Fact]
        public async Task RebroadcastTransactionAsync()
        {
            // Arrange 


            // Act 
            var pendingTransactions = await beeNodeClient.DebugClient.RebroadcastTransactionAsync(""); //TODO How to get transaction


            // Assert
        }
        
        [Fact]
        public async Task DeleteTransactionAsync()
        {
            // Arrange 


            // Act 
            var pendingTransactions = await beeNodeClient.DebugClient.DeleteTransactionAsync(""); //TODO How to get transaction


            // Assert
        }
        */

    }
}
