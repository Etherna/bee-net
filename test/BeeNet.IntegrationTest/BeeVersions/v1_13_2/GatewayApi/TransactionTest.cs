//   Copyright 2021-present Etherna SA
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Threading.Tasks;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions.v1_13_2.GatewayApi
{
    public class TransactionTest : BaseTest_Gateway_v5_0_0
    {
        [Fact]
        public async Task GetPendingTransactionsAsync()
        {
            // Arrange 

            // Act 
            await beeNodeClient.GetPendingTransactionsAsync(); //TODO How to get transaction

            // Assert
        }

        /*
        [Fact]
        public async Task GetTransactionInfoAsync()
        {
            // Arrange


            // Act
            var pendingTransactions = await beeNodeClient.GetTransactionInfoAsync(""); //TODO How to get transaction


            // Assert
        }

        [Fact]
        public async Task RebroadcastTransactionAsync()
        {
            // Arrange


            // Act
            var pendingTransactions = await beeNodeClient.RebroadcastTransactionAsync(""); //TODO How to get transaction


            // Assert
        }

        [Fact]
        public async Task DeleteTransactionAsync()
        {
            // Arrange


            // Act
            var pendingTransactions = await beeNodeClient.DeleteTransactionAsync(""); //TODO How to get transaction


            // Assert
        }
        */
    }
}
