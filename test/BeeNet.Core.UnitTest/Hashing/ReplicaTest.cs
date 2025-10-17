// Copyright 2021-present Etherna SA
// This file is part of Bee.Net.
// 
// Bee.Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Bee.Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Bee.Net.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.BeeNet.Hashing.Replica;
using Etherna.BeeNet.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Etherna.BeeNet.Hashing
{
    public class ReplicaTest
    {
        // Internal classes.
        public record GenerateReplicasTestElement(
            SwarmHash ChunkHash,
            RedundancyLevel RedundancyLevel,
            IEnumerable<SwarmHash> ExpectedReplicaHashes);

        // Data.
        public static IEnumerable<object[]> GenerateReplicasTests
        {
            get
            {
                var tests = new List<GenerateReplicasTestElement>
                {
                    new("7a41dd7f156bc47de7dadd3b0bac0ac07dcb47f45d3528192e9e25574c8744b9",
                        RedundancyLevel.None, []),

                    new("7a41dd7f156bc47de7dadd3b0bac0ac07dcb47f45d3528192e9e25574c8744b9",
                        RedundancyLevel.Medium,
                        [
                            "e078fc174277e0f2379093d2612e051993b23b586406c276d8ae53b1887e6f93",
                            "73cc50c9458425b3aba63725fbfad1a26a19b528cc5256a54ee95a3a816bc837"
                        ]),

                    new("7a41dd7f156bc47de7dadd3b0bac0ac07dcb47f45d3528192e9e25574c8744b9",
                        RedundancyLevel.Strong,
                        [
                            "e078fc174277e0f2379093d2612e051993b23b586406c276d8ae53b1887e6f93",
                            "73cc50c9458425b3aba63725fbfad1a26a19b528cc5256a54ee95a3a816bc837",
                            "0316ff191df02467350a26d86f01181727ae128669acaa331dd54c33353a0784",
                            "a12b2550263afb6aa5fe0f69a5dc7fbf6d4879381dcf7679dae93a71477520a8"
                        ]),
                    
                    new("7a41dd7f156bc47de7dadd3b0bac0ac07dcb47f45d3528192e9e25574c8744b9",
                        RedundancyLevel.Insane,
                        [
                            "e078fc174277e0f2379093d2612e051993b23b586406c276d8ae53b1887e6f93",
                            "73cc50c9458425b3aba63725fbfad1a26a19b528cc5256a54ee95a3a816bc837",
                            "0316ff191df02467350a26d86f01181727ae128669acaa331dd54c33353a0784",
                            "a12b2550263afb6aa5fe0f69a5dc7fbf6d4879381dcf7679dae93a71477520a8",
                            "30a9029f724ee698eff31a5b6058bcc742ec99bfe9d8de594598f416798602b9",
                            "c82e6d25e027119f56ed57caaffc0407d2ac6cebed1130ac9e9d0d63bcf285cc",
                            "5144ce7850400aeaed9eb7e8c4810e7325ce7d5af65858f551101e2ee2302013",
                            "811789e0aa5e5041a4cbce18a27bd4816d9b026c1277d7245f469430b590a7d0"
                        ]),
                    
                    new("7a41dd7f156bc47de7dadd3b0bac0ac07dcb47f45d3528192e9e25574c8744b9",
                        RedundancyLevel.Paranoid,
                        [
                            "e078fc174277e0f2379093d2612e051993b23b586406c276d8ae53b1887e6f93",
                            "73cc50c9458425b3aba63725fbfad1a26a19b528cc5256a54ee95a3a816bc837",
                            "0316ff191df02467350a26d86f01181727ae128669acaa331dd54c33353a0784",
                            "a12b2550263afb6aa5fe0f69a5dc7fbf6d4879381dcf7679dae93a71477520a8",
                            "30a9029f724ee698eff31a5b6058bcc742ec99bfe9d8de594598f416798602b9",
                            "c82e6d25e027119f56ed57caaffc0407d2ac6cebed1130ac9e9d0d63bcf285cc",
                            "5144ce7850400aeaed9eb7e8c4810e7325ce7d5af65858f551101e2ee2302013",
                            "811789e0aa5e5041a4cbce18a27bd4816d9b026c1277d7245f469430b590a7d0",
                            "fa5818425b772270326058915615a82427a4f6a19d33cf580908f732c2404b93",
                            "bbabbb73418f03ee00c1a501a346a55d7ade1f2d26ed68c805fa4c56dea07f1a",
                            "2f03a55bc76e2d5298f3d689d2b677493f934839261620aafc1cdb5219bc9e39",
                            "4d247475b4eb961ab139bb9bc123f13863c613cb1ab03627fd138a9b8b0633d6",
                            "6a7e5e08ccfaab0e2e7d31878659689a47683889b55faa759b5c0df2af5ca054",
                            "dd4d8d3549a73719d0e1eda7de91f41f31a82b82788d32397788e2bec95b8ce2",
                            "1b92cc8c598ed1a1ddf63a28c98a29b23e1e381d850f90b9acd431fd73c095b2",
                            "9b8fdbcebe875858dfc11c9cbfd81b78bc492717b977ef15c83f57b03bbd9086"
                        ])
                };

                return tests.Select(t => new object[] { t });
            }
        }

        // Tests.
        [Theory, MemberData(nameof(GenerateReplicasTests))]
        public void GenerateReplicas(GenerateReplicasTestElement test)
        {
            var result = ReplicasGenerator.GenerateReplicaIds(test.ChunkHash, test.RedundancyLevel, new Hasher());

            Assert.Equal(test.ExpectedReplicaHashes, result.Select(c => c.Hash));
        }
    }
}