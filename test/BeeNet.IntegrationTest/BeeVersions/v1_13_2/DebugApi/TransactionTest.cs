using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_13_2.DebugApi
{
    public class TransactionTest : BaseTest_Debug_v5_0_0
    {

        [Fact]
        public async Task GetPendingTransactionsAsync()
        {
            // Arrange 

            // Act 
            await beeNodeClient.DebugClient.GetPendingTransactionsAsync(); //TODO How to get transaction

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
