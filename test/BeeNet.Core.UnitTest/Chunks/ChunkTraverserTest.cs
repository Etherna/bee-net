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

using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Hashing.Pipeline;
using Etherna.BeeNet.Hashing.Postage;
using Etherna.BeeNet.Manifest;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Etherna.BeeNet.Chunks
{
    public class ChunkTraverserTest
    {
        // Internal classes.
        public class TraverseFromDataChunkTestElement(
            IChunkStore chunkStore,
            SwarmHash rootHash,
            IEnumerable<SwarmHash> expectedFoundHashes,
            IEnumerable<SwarmHash> expectedNotFoundHashes)
        {
            public IChunkStore ChunkStore { get; } = chunkStore;
            public SwarmHash RootHash { get; } = rootHash;
            public IEnumerable<SwarmHash> ExpectedFoundHashes { get; } = expectedFoundHashes;
            public IEnumerable<SwarmHash> ExpectedNotFoundHashes { get; } = expectedNotFoundHashes;
        }
        
        public class TraverseFromMantarayManifestRootTestElement(
            IChunkStore chunkStore,
            SwarmHash rootHash,
            IEnumerable<SwarmHash> expectedFoundHashes,
            IEnumerable<SwarmHash> expectedNotFoundHashes)
        {
            public IChunkStore ChunkStore { get; } = chunkStore;
            public SwarmHash RootHash { get; } = rootHash;
            public IEnumerable<SwarmHash> ExpectedFoundHashes { get; } = expectedFoundHashes;
            public IEnumerable<SwarmHash> ExpectedNotFoundHashes { get; } = expectedNotFoundHashes;
        }
        
        public class TraverseFromMantarayNodeChunkTestElement(
            IChunkStore chunkStore,
            SwarmHash rootHash,
            IEnumerable<SwarmHash> expectedFoundHashes,
            IEnumerable<SwarmHash> expectedNotFoundHashes)
        {
            public IChunkStore ChunkStore { get; } = chunkStore;
            public SwarmHash RootHash { get; } = rootHash;
            public IEnumerable<SwarmHash> ExpectedFoundHashes { get; } = expectedFoundHashes;
            public IEnumerable<SwarmHash> ExpectedNotFoundHashes { get; } = expectedNotFoundHashes;
        }
        
        // Data.
        public static IEnumerable<object[]> TraverseFromDataChunkTests
        {
            get
            {
                var tests = new List<TraverseFromDataChunkTestElement>();
                
                //find all chunks
                {
                    var chunkStoreInitTask = BuildChunkStoreAsync();
                    chunkStoreInitTask.Wait();
                    var chunkStore = chunkStoreInitTask.Result;
                    
                    tests.Add(new TraverseFromDataChunkTestElement(
                        chunkStore,
                        "3abeb3b5c9690c1d453c20a1d492664a1f5336c2cb359cce04a37df996f45466",
                        [
                            "3abeb3b5c9690c1d453c20a1d492664a1f5336c2cb359cce04a37df996f45466",
                            "185ca0c92f3a1760c64206da9ce63f66c81094b467713701c5150c8115eb4651",
                            "4a36ba2c1185612926e7f73e1eca905839263b3c4c0f21e6c6c8e731110ecdcb",
                            "405b276d2bdd8257a4864947a5d2bf8b3908bba72250a0d92a9127eb417f106c"
                        ],
                        []));
                }
                
                //missing traversing root
                {
                    var chunkStoreInitTask = BuildChunkStoreAsync();
                    chunkStoreInitTask.Wait();
                    var chunkStore = chunkStoreInitTask.Result;
                    
                    //remove root
                    var deleteTask = chunkStore.DeleteAsync("3abeb3b5c9690c1d453c20a1d492664a1f5336c2cb359cce04a37df996f45466");
                    deleteTask.Wait();
                    
                    tests.Add(new TraverseFromDataChunkTestElement(
                        chunkStore,
                        "3abeb3b5c9690c1d453c20a1d492664a1f5336c2cb359cce04a37df996f45466",
                        [],
                        ["3abeb3b5c9690c1d453c20a1d492664a1f5336c2cb359cce04a37df996f45466"]));
                }
                
                //missing leaf data
                {
                    var chunkStoreInitTask = BuildChunkStoreAsync();
                    chunkStoreInitTask.Wait();
                    var chunkStore = chunkStoreInitTask.Result;
                    
                    //remove "fileB" leaf data
                    var deleteTask = chunkStore.DeleteAsync("4a36ba2c1185612926e7f73e1eca905839263b3c4c0f21e6c6c8e731110ecdcb");
                    deleteTask.Wait();
                    
                    tests.Add(new TraverseFromDataChunkTestElement(
                        chunkStore,
                        "3abeb3b5c9690c1d453c20a1d492664a1f5336c2cb359cce04a37df996f45466",
                        [
                            "3abeb3b5c9690c1d453c20a1d492664a1f5336c2cb359cce04a37df996f45466",
                            "185ca0c92f3a1760c64206da9ce63f66c81094b467713701c5150c8115eb4651",
                            "405b276d2bdd8257a4864947a5d2bf8b3908bba72250a0d92a9127eb417f106c"
                        ],
                        ["4a36ba2c1185612926e7f73e1eca905839263b3c4c0f21e6c6c8e731110ecdcb"]));
                }

                return tests.Select(t => new object[] { t });
            }
        }
        
        public static IEnumerable<object[]> TraverseFromMantarayManifestRootTests
        {
            get
            {
                var tests = new List<TraverseFromMantarayManifestRootTestElement>();
                
                //find all chunks
                {
                    var chunkStoreInitTask = BuildChunkStoreAsync();
                    chunkStoreInitTask.Wait();
                    var chunkStore = chunkStoreInitTask.Result;
                    
                    tests.Add(new TraverseFromMantarayManifestRootTestElement(
                        chunkStore,
                        "46be4f99d106518c74fe02262aa4d738117eee47f9bb6bceea523346762372cc",
                        [
                            "46be4f99d106518c74fe02262aa4d738117eee47f9bb6bceea523346762372cc",
                            "df08922becf846a73648dd793d2b4e8c7d8e14d812ae953b169c3948cc7208db",
                            "b4dbbcabd06b975c982b1165526ca86d645957bdb4ad185c7762349bc3d05db1",
                            "72e71b431b78633506acd4198d023ac535f0612dbd9a0aaf6d8b1ba496f4637e",
                            "ddf10d58bc29ff8aa4596d0d6f1c7ad4dc96b422c1f8879f22fbd5cb62c63fac",
                            "45a2fb3301637e7cd235a7f4a26ae78da6a115d87d5f8eef840e35ec1d9833bf",
                            "fd85afac03d0fce2ecd169c0f40d2f4afddd265f03a5f9938d1a7fedd6e670c1",
                            "3abeb3b5c9690c1d453c20a1d492664a1f5336c2cb359cce04a37df996f45466",
                            "185ca0c92f3a1760c64206da9ce63f66c81094b467713701c5150c8115eb4651",
                            "4a36ba2c1185612926e7f73e1eca905839263b3c4c0f21e6c6c8e731110ecdcb",
                            "405b276d2bdd8257a4864947a5d2bf8b3908bba72250a0d92a9127eb417f106c",
                            "1396c4610b0a3417e54bdb8ef646c2e089a9949bd82814c27b0420c8148c3d43",
                            "28634e6ed6ff1f72936052f2ef6b806606d00ec04a97c97b84d13200fd6ddd00",
                            "b3a110c515642b16ecf50c024a80b42a7dacc9b99be8880334ac01160686c27c",
                            "4890e9a8a9656096fdfc57ba7423778efc44c374f0a5812a7ef7cad395fe7185",
                            "670371ff59c155195ac366e8f398b226fc97ee092edf1b57e0eea8fe7beafd20",
                            "caf0f6a6ba3b3cd0c242f046e6c41c59ac3e31ef7a2c4e74427b6c8a6929c420",
                            "b3881d42550e27238c5bb2f2ea63aa96a3a287a9f4cabd065df5f7bc432effb3"
                        ],
                        []));
                }
                
                //missing manifest root
                {
                    var chunkStoreInitTask = BuildChunkStoreAsync();
                    chunkStoreInitTask.Wait();
                    var chunkStore = chunkStoreInitTask.Result;
                    
                    //remove root
                    var deleteTask = chunkStore.DeleteAsync("46be4f99d106518c74fe02262aa4d738117eee47f9bb6bceea523346762372cc");
                    deleteTask.Wait();
                
                    tests.Add(new TraverseFromMantarayManifestRootTestElement(
                        chunkStore,
                        "46be4f99d106518c74fe02262aa4d738117eee47f9bb6bceea523346762372cc",
                        [],
                        ["46be4f99d106518c74fe02262aa4d738117eee47f9bb6bceea523346762372cc"]));
                }
                
                //missing manifest fork node
                {
                    var chunkStoreInitTask = BuildChunkStoreAsync();
                    chunkStoreInitTask.Wait();
                    var chunkStore = chunkStoreInitTask.Result;
                    
                    //remove "A" fork node
                    var deleteTask = chunkStore.DeleteAsync("b4dbbcabd06b975c982b1165526ca86d645957bdb4ad185c7762349bc3d05db1");
                    deleteTask.Wait();
                
                    tests.Add(new TraverseFromMantarayManifestRootTestElement(
                        chunkStore,
                        "46be4f99d106518c74fe02262aa4d738117eee47f9bb6bceea523346762372cc",
                        [
                            "46be4f99d106518c74fe02262aa4d738117eee47f9bb6bceea523346762372cc",
                            "df08922becf846a73648dd793d2b4e8c7d8e14d812ae953b169c3948cc7208db",
                            "fd85afac03d0fce2ecd169c0f40d2f4afddd265f03a5f9938d1a7fedd6e670c1",
                            "3abeb3b5c9690c1d453c20a1d492664a1f5336c2cb359cce04a37df996f45466",
                            "185ca0c92f3a1760c64206da9ce63f66c81094b467713701c5150c8115eb4651",
                            "4a36ba2c1185612926e7f73e1eca905839263b3c4c0f21e6c6c8e731110ecdcb",
                            "405b276d2bdd8257a4864947a5d2bf8b3908bba72250a0d92a9127eb417f106c",
                            "1396c4610b0a3417e54bdb8ef646c2e089a9949bd82814c27b0420c8148c3d43",
                            "28634e6ed6ff1f72936052f2ef6b806606d00ec04a97c97b84d13200fd6ddd00",
                            "b3a110c515642b16ecf50c024a80b42a7dacc9b99be8880334ac01160686c27c",
                            "4890e9a8a9656096fdfc57ba7423778efc44c374f0a5812a7ef7cad395fe7185",
                            "670371ff59c155195ac366e8f398b226fc97ee092edf1b57e0eea8fe7beafd20",
                            "caf0f6a6ba3b3cd0c242f046e6c41c59ac3e31ef7a2c4e74427b6c8a6929c420",
                            "b3881d42550e27238c5bb2f2ea63aa96a3a287a9f4cabd065df5f7bc432effb3"
                        ],
                        [ "b4dbbcabd06b975c982b1165526ca86d645957bdb4ad185c7762349bc3d05db1" ]));
                }
                
                //missing data intermediate
                {
                    var chunkStoreInitTask = BuildChunkStoreAsync();
                    chunkStoreInitTask.Wait();
                    var chunkStore = chunkStoreInitTask.Result;
                    
                    //remove "fileA" intermediate data
                    var deleteTask = chunkStore.DeleteAsync("72e71b431b78633506acd4198d023ac535f0612dbd9a0aaf6d8b1ba496f4637e");
                    deleteTask.Wait();

                    tests.Add(new TraverseFromMantarayManifestRootTestElement(
                        chunkStore,
                        "46be4f99d106518c74fe02262aa4d738117eee47f9bb6bceea523346762372cc",
                        [
                            "46be4f99d106518c74fe02262aa4d738117eee47f9bb6bceea523346762372cc",
                            "df08922becf846a73648dd793d2b4e8c7d8e14d812ae953b169c3948cc7208db",
                            "b4dbbcabd06b975c982b1165526ca86d645957bdb4ad185c7762349bc3d05db1",
                            "fd85afac03d0fce2ecd169c0f40d2f4afddd265f03a5f9938d1a7fedd6e670c1",
                            "3abeb3b5c9690c1d453c20a1d492664a1f5336c2cb359cce04a37df996f45466",
                            "185ca0c92f3a1760c64206da9ce63f66c81094b467713701c5150c8115eb4651",
                            "4a36ba2c1185612926e7f73e1eca905839263b3c4c0f21e6c6c8e731110ecdcb",
                            "405b276d2bdd8257a4864947a5d2bf8b3908bba72250a0d92a9127eb417f106c",
                            "1396c4610b0a3417e54bdb8ef646c2e089a9949bd82814c27b0420c8148c3d43",
                            "28634e6ed6ff1f72936052f2ef6b806606d00ec04a97c97b84d13200fd6ddd00",
                            "b3a110c515642b16ecf50c024a80b42a7dacc9b99be8880334ac01160686c27c",
                            "4890e9a8a9656096fdfc57ba7423778efc44c374f0a5812a7ef7cad395fe7185",
                            "670371ff59c155195ac366e8f398b226fc97ee092edf1b57e0eea8fe7beafd20",
                            "caf0f6a6ba3b3cd0c242f046e6c41c59ac3e31ef7a2c4e74427b6c8a6929c420",
                            "b3881d42550e27238c5bb2f2ea63aa96a3a287a9f4cabd065df5f7bc432effb3"
                        ],
                        ["72e71b431b78633506acd4198d023ac535f0612dbd9a0aaf6d8b1ba496f4637e"]));
                }
                
                //missing data leaf
                {
                    var chunkStoreInitTask = BuildChunkStoreAsync();
                    chunkStoreInitTask.Wait();
                    var chunkStore = chunkStoreInitTask.Result;
                    
                    //remove "fileA" leaf data
                    var deleteTask = chunkStore.DeleteAsync("45a2fb3301637e7cd235a7f4a26ae78da6a115d87d5f8eef840e35ec1d9833bf");
                    deleteTask.Wait();
                    
                    tests.Add(new TraverseFromMantarayManifestRootTestElement(
                        chunkStore,
                        "46be4f99d106518c74fe02262aa4d738117eee47f9bb6bceea523346762372cc",
                        [
                            "46be4f99d106518c74fe02262aa4d738117eee47f9bb6bceea523346762372cc",
                            "df08922becf846a73648dd793d2b4e8c7d8e14d812ae953b169c3948cc7208db",
                            "b4dbbcabd06b975c982b1165526ca86d645957bdb4ad185c7762349bc3d05db1",
                            "72e71b431b78633506acd4198d023ac535f0612dbd9a0aaf6d8b1ba496f4637e",
                            "ddf10d58bc29ff8aa4596d0d6f1c7ad4dc96b422c1f8879f22fbd5cb62c63fac",
                            "fd85afac03d0fce2ecd169c0f40d2f4afddd265f03a5f9938d1a7fedd6e670c1",
                            "3abeb3b5c9690c1d453c20a1d492664a1f5336c2cb359cce04a37df996f45466",
                            "185ca0c92f3a1760c64206da9ce63f66c81094b467713701c5150c8115eb4651",
                            "4a36ba2c1185612926e7f73e1eca905839263b3c4c0f21e6c6c8e731110ecdcb",
                            "405b276d2bdd8257a4864947a5d2bf8b3908bba72250a0d92a9127eb417f106c",
                            "1396c4610b0a3417e54bdb8ef646c2e089a9949bd82814c27b0420c8148c3d43",
                            "28634e6ed6ff1f72936052f2ef6b806606d00ec04a97c97b84d13200fd6ddd00",
                            "b3a110c515642b16ecf50c024a80b42a7dacc9b99be8880334ac01160686c27c",
                            "4890e9a8a9656096fdfc57ba7423778efc44c374f0a5812a7ef7cad395fe7185",
                            "670371ff59c155195ac366e8f398b226fc97ee092edf1b57e0eea8fe7beafd20",
                            "caf0f6a6ba3b3cd0c242f046e6c41c59ac3e31ef7a2c4e74427b6c8a6929c420",
                            "b3881d42550e27238c5bb2f2ea63aa96a3a287a9f4cabd065df5f7bc432effb3"
                        ],
                        ["45a2fb3301637e7cd235a7f4a26ae78da6a115d87d5f8eef840e35ec1d9833bf"]));
                }

                return tests.Select(t => new object[] { t });
            }
        }
        
        public static IEnumerable<object[]> TraverseFromMantarayNodeChunkTests
        {
            get
            {
                var tests = new List<TraverseFromMantarayNodeChunkTestElement>();
                
                //find all chunks
                {
                    var chunkStoreInitTask = BuildChunkStoreAsync();
                    chunkStoreInitTask.Wait();
                    var chunkStore = chunkStoreInitTask.Result;
                    
                    tests.Add(new TraverseFromMantarayNodeChunkTestElement(
                        chunkStore,
                        "df08922becf846a73648dd793d2b4e8c7d8e14d812ae953b169c3948cc7208db",
                        [
                            "df08922becf846a73648dd793d2b4e8c7d8e14d812ae953b169c3948cc7208db",
                            "b4dbbcabd06b975c982b1165526ca86d645957bdb4ad185c7762349bc3d05db1",
                            "72e71b431b78633506acd4198d023ac535f0612dbd9a0aaf6d8b1ba496f4637e",
                            "ddf10d58bc29ff8aa4596d0d6f1c7ad4dc96b422c1f8879f22fbd5cb62c63fac",
                            "45a2fb3301637e7cd235a7f4a26ae78da6a115d87d5f8eef840e35ec1d9833bf",
                            "fd85afac03d0fce2ecd169c0f40d2f4afddd265f03a5f9938d1a7fedd6e670c1",
                            "3abeb3b5c9690c1d453c20a1d492664a1f5336c2cb359cce04a37df996f45466",
                            "185ca0c92f3a1760c64206da9ce63f66c81094b467713701c5150c8115eb4651",
                            "4a36ba2c1185612926e7f73e1eca905839263b3c4c0f21e6c6c8e731110ecdcb",
                            "405b276d2bdd8257a4864947a5d2bf8b3908bba72250a0d92a9127eb417f106c"
                        ],
                        []));
                }
                
                //missing traversing root
                {
                    var chunkStoreInitTask = BuildChunkStoreAsync();
                    chunkStoreInitTask.Wait();
                    var chunkStore = chunkStoreInitTask.Result;
                    
                    //remove root
                    var deleteTask = chunkStore.DeleteAsync("df08922becf846a73648dd793d2b4e8c7d8e14d812ae953b169c3948cc7208db");
                    deleteTask.Wait();
                    
                    tests.Add(new TraverseFromMantarayNodeChunkTestElement(
                        chunkStore,
                        "df08922becf846a73648dd793d2b4e8c7d8e14d812ae953b169c3948cc7208db",
                        [],
                        ["df08922becf846a73648dd793d2b4e8c7d8e14d812ae953b169c3948cc7208db"]));
                }
                
                //missing manifest internal node
                {
                    var chunkStoreInitTask = BuildChunkStoreAsync();
                    chunkStoreInitTask.Wait();
                    var chunkStore = chunkStoreInitTask.Result;
                    
                    //remove "A" fork node
                    var deleteTask = chunkStore.DeleteAsync("b4dbbcabd06b975c982b1165526ca86d645957bdb4ad185c7762349bc3d05db1");
                    deleteTask.Wait();
                    
                    tests.Add(new TraverseFromMantarayNodeChunkTestElement(
                        chunkStore,
                        "df08922becf846a73648dd793d2b4e8c7d8e14d812ae953b169c3948cc7208db",
                        [
                            "df08922becf846a73648dd793d2b4e8c7d8e14d812ae953b169c3948cc7208db",
                            "fd85afac03d0fce2ecd169c0f40d2f4afddd265f03a5f9938d1a7fedd6e670c1",
                            "3abeb3b5c9690c1d453c20a1d492664a1f5336c2cb359cce04a37df996f45466",
                            "185ca0c92f3a1760c64206da9ce63f66c81094b467713701c5150c8115eb4651",
                            "4a36ba2c1185612926e7f73e1eca905839263b3c4c0f21e6c6c8e731110ecdcb",
                            "405b276d2bdd8257a4864947a5d2bf8b3908bba72250a0d92a9127eb417f106c"
                        ],
                        ["b4dbbcabd06b975c982b1165526ca86d645957bdb4ad185c7762349bc3d05db1"]));
                }
                
                //missing data intermediate
                {
                    var chunkStoreInitTask = BuildChunkStoreAsync();
                    chunkStoreInitTask.Wait();
                    var chunkStore = chunkStoreInitTask.Result;
                    
                    //remove "fileA" intermediate data
                    var deleteTask = chunkStore.DeleteAsync("72e71b431b78633506acd4198d023ac535f0612dbd9a0aaf6d8b1ba496f4637e");
                    deleteTask.Wait();
                    
                    tests.Add(new TraverseFromMantarayNodeChunkTestElement(
                        chunkStore,
                        "df08922becf846a73648dd793d2b4e8c7d8e14d812ae953b169c3948cc7208db",
                        [
                            "df08922becf846a73648dd793d2b4e8c7d8e14d812ae953b169c3948cc7208db",
                            "b4dbbcabd06b975c982b1165526ca86d645957bdb4ad185c7762349bc3d05db1",
                            "fd85afac03d0fce2ecd169c0f40d2f4afddd265f03a5f9938d1a7fedd6e670c1",
                            "3abeb3b5c9690c1d453c20a1d492664a1f5336c2cb359cce04a37df996f45466",
                            "185ca0c92f3a1760c64206da9ce63f66c81094b467713701c5150c8115eb4651",
                            "4a36ba2c1185612926e7f73e1eca905839263b3c4c0f21e6c6c8e731110ecdcb",
                            "405b276d2bdd8257a4864947a5d2bf8b3908bba72250a0d92a9127eb417f106c"
                        ],
                        ["72e71b431b78633506acd4198d023ac535f0612dbd9a0aaf6d8b1ba496f4637e"]));
                }
                
                //missing data leaf
                {
                    var chunkStoreInitTask = BuildChunkStoreAsync();
                    chunkStoreInitTask.Wait();
                    var chunkStore = chunkStoreInitTask.Result;
                    
                    //remove "fileA" leaf data
                    var deleteTask = chunkStore.DeleteAsync("45a2fb3301637e7cd235a7f4a26ae78da6a115d87d5f8eef840e35ec1d9833bf");
                    deleteTask.Wait();
                    
                    tests.Add(new TraverseFromMantarayNodeChunkTestElement(
                        chunkStore,
                        "df08922becf846a73648dd793d2b4e8c7d8e14d812ae953b169c3948cc7208db",
                        [
                            "df08922becf846a73648dd793d2b4e8c7d8e14d812ae953b169c3948cc7208db",
                            "b4dbbcabd06b975c982b1165526ca86d645957bdb4ad185c7762349bc3d05db1",
                            "72e71b431b78633506acd4198d023ac535f0612dbd9a0aaf6d8b1ba496f4637e",
                            "ddf10d58bc29ff8aa4596d0d6f1c7ad4dc96b422c1f8879f22fbd5cb62c63fac",
                            "fd85afac03d0fce2ecd169c0f40d2f4afddd265f03a5f9938d1a7fedd6e670c1",
                            "3abeb3b5c9690c1d453c20a1d492664a1f5336c2cb359cce04a37df996f45466",
                            "185ca0c92f3a1760c64206da9ce63f66c81094b467713701c5150c8115eb4651",
                            "4a36ba2c1185612926e7f73e1eca905839263b3c4c0f21e6c6c8e731110ecdcb",
                            "405b276d2bdd8257a4864947a5d2bf8b3908bba72250a0d92a9127eb417f106c"
                        ],
                        ["45a2fb3301637e7cd235a7f4a26ae78da6a115d87d5f8eef840e35ec1d9833bf"]));
                }

                return tests.Select(t => new object[] { t });
            }
        }
        
        // Tests.
        [Theory, MemberData(nameof(TraverseFromDataChunkTests))]
        public async Task TraverseFromDataChunk(TraverseFromDataChunkTestElement test)
        {
            var chunkTraverser = new ChunkTraverser(test.ChunkStore);
            List<SwarmHash> foundChunkHashes = [];
            List<SwarmHash> notFoundChunkHashes = [];
        
            await chunkTraverser.TraverseFromDataChunkAsync(
                new SwarmChunkReference(test.RootHash, null, false),
                c => { foundChunkHashes.Add(c.Hash); return Task.CompletedTask; },
                h => { notFoundChunkHashes.Add(h); return Task.CompletedTask; });
        
            Assert.Equal(test.ExpectedFoundHashes, foundChunkHashes);
            Assert.Equal(test.ExpectedNotFoundHashes, notFoundChunkHashes);
        }
        
        [Theory, MemberData(nameof(TraverseFromMantarayManifestRootTests))]
        public async Task TraverseFromMantarayManifestRoot(TraverseFromMantarayManifestRootTestElement test)
        {
            var chunkTraverser = new ChunkTraverser(test.ChunkStore);
            List<SwarmHash> foundChunkHashes = [];
            List<SwarmHash> notFoundChunkHashes = [];

            await chunkTraverser.TraverseFromMantarayManifestRootAsync(
                test.RootHash,
                c => { foundChunkHashes.Add(c.Hash); return Task.CompletedTask; },
                h => { notFoundChunkHashes.Add(h); return Task.CompletedTask; });

            Assert.Equal(test.ExpectedFoundHashes, foundChunkHashes);
            Assert.Equal(test.ExpectedNotFoundHashes, notFoundChunkHashes);
        }
        
        [Theory, MemberData(nameof(TraverseFromMantarayNodeChunkTests))]
        public async Task TraverseFromMantarayNodeChunk(TraverseFromMantarayNodeChunkTestElement test)
        {
            var chunkTraverser = new ChunkTraverser(test.ChunkStore);
            List<SwarmHash> foundChunkHashes = [];
            List<SwarmHash> notFoundChunkHashes = [];
        
            await chunkTraverser.TraverseFromMantarayNodeChunkAsync(
                test.RootHash,
                null,
                null,
                NodeType.Edge,
                c => { foundChunkHashes.Add(c.Hash); return Task.CompletedTask; },
                h => { notFoundChunkHashes.Add(h); return Task.CompletedTask; });
        
            Assert.Equal(test.ExpectedFoundHashes, foundChunkHashes);
            Assert.Equal(test.ExpectedNotFoundHashes, notFoundChunkHashes);
        }
        
        // Helpers.
        private static async Task<IChunkStore> BuildChunkStoreAsync()
        {
            var chunkStore = new MemoryChunkStore();
            var postageStamper = new FakePostageStamper();
            
            /* Build dir manifest structure as:
             * /
             * |-- fileA (8kB)
             * |-- fileB (12kB)
             * |-- myDir
             *     |-- fileA (8kB)
             *     |-- fileC (16kB)
             *
             * Chunks:
             * 46be4f99d106518c74fe02262aa4d738117eee47f9bb6bceea523346762372cc (root)
             * |-- df08922becf846a73648dd793d2b4e8c7d8e14d812ae953b169c3948cc7208db ("file" fork)
             *     |-- b4dbbcabd06b975c982b1165526ca86d645957bdb4ad185c7762349bc3d05db1 ("A" fork)
             *         |-- 72e71b431b78633506acd4198d023ac535f0612dbd9a0aaf6d8b1ba496f4637e (data intermediate)
             *             |-- ddf10d58bc29ff8aa4596d0d6f1c7ad4dc96b422c1f8879f22fbd5cb62c63fac (data leaf)
             *             |-- 45a2fb3301637e7cd235a7f4a26ae78da6a115d87d5f8eef840e35ec1d9833bf (data leaf)
             *     |-- fd85afac03d0fce2ecd169c0f40d2f4afddd265f03a5f9938d1a7fedd6e670c1 ("B" fork)
             *         |-- 3abeb3b5c9690c1d453c20a1d492664a1f5336c2cb359cce04a37df996f45466 (data intermediate)
             *             |-- 185ca0c92f3a1760c64206da9ce63f66c81094b467713701c5150c8115eb4651 (data leaf)
             *             |-- 4a36ba2c1185612926e7f73e1eca905839263b3c4c0f21e6c6c8e731110ecdcb (data leaf)
             *             |-- 405b276d2bdd8257a4864947a5d2bf8b3908bba72250a0d92a9127eb417f106c (data leaf)
             * |-- 1396c4610b0a3417e54bdb8ef646c2e089a9949bd82814c27b0420c8148c3d43 ("myDir/file" fork)
             *     |-- b4dbbcabd06b975c982b1165526ca86d645957bdb4ad185c7762349bc3d05db1 ("A" fork)
             *         |-- 72e71b431b78633506acd4198d023ac535f0612dbd9a0aaf6d8b1ba496f4637e (data intermediate)
             *             |-- ddf10d58bc29ff8aa4596d0d6f1c7ad4dc96b422c1f8879f22fbd5cb62c63fac (data leaf)
             *             |-- 45a2fb3301637e7cd235a7f4a26ae78da6a115d87d5f8eef840e35ec1d9833bf (data leaf)
             *     |-- 28634e6ed6ff1f72936052f2ef6b806606d00ec04a97c97b84d13200fd6ddd00 ("C" fork)
             *         |-- b3a110c515642b16ecf50c024a80b42a7dacc9b99be8880334ac01160686c27c (data intermediate)
             *             |-- 4890e9a8a9656096fdfc57ba7423778efc44c374f0a5812a7ef7cad395fe7185 (data leaf)
             *             |-- 670371ff59c155195ac366e8f398b226fc97ee092edf1b57e0eea8fe7beafd20 (data leaf)
             *             |-- caf0f6a6ba3b3cd0c242f046e6c41c59ac3e31ef7a2c4e74427b6c8a6929c420 (data leaf)
             *             |-- b3881d42550e27238c5bb2f2ea63aa96a3a287a9f4cabd065df5f7bc432effb3 (data leaf)
             */
            
            // Create files.
            var fileRand = new Random(0);

            var fileAData = new byte[8 * 1024];
            var fileBData = new byte[12 * 1024];
            var fileCData = new byte[16 * 1024];
            
            fileRand.NextBytes(fileAData);
            fileRand.NextBytes(fileBData);
            fileRand.NextBytes(fileCData);
            
            // Build manifest.
            var dirManifest = new MantarayManifest(
                _ => HasherPipelineBuilder.BuildNewHasherPipeline(
                    chunkStore,
                    postageStamper,
                    RedundancyLevel.None,
                    false,
                    0,
                    null),
                0);

            var fileAHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                chunkStore,
                postageStamper,
                RedundancyLevel.None,
                false,
                0,
                null);
            var fileAEntryMetadata = new Dictionary<string, string>
            {
                [ManifestEntry.ContentTypeKey] = "application/octet-stream",
                [ManifestEntry.FilenameKey] = "fileA"
            };
            var fileAHashingResult = await fileAHasherPipeline.HashDataAsync(fileAData).ConfigureAwait(false);
            dirManifest.Add(
                "fileA",
                ManifestEntry.NewFile(
                    fileAHashingResult.Hash,
                    fileAEntryMetadata));
            dirManifest.Add(
                "myDir/fileA",
                ManifestEntry.NewFile(
                    fileAHashingResult.Hash,
                    fileAEntryMetadata));

            var fileBHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                chunkStore,
                postageStamper,
                RedundancyLevel.None,
                false,
                0,
                null);
            var fileBEntryMetadata = new Dictionary<string, string>
            {
                [ManifestEntry.ContentTypeKey] = "application/octet-stream",
                [ManifestEntry.FilenameKey] = "fileB"
            };
            var fileBHashingResult = await fileBHasherPipeline.HashDataAsync(fileBData).ConfigureAwait(false);
            dirManifest.Add(
                "fileB",
                ManifestEntry.NewFile(
                    fileBHashingResult.Hash,
                    fileBEntryMetadata));

            var fileCHasherPipeline = HasherPipelineBuilder.BuildNewHasherPipeline(
                chunkStore,
                postageStamper,
                RedundancyLevel.None,
                false,
                0,
                null);
            var fileCEntryMetadata = new Dictionary<string, string>
            {
                [ManifestEntry.ContentTypeKey] = "application/octet-stream",
                [ManifestEntry.FilenameKey] = "fileC"
            };
            var fileCHashingResult = await fileCHasherPipeline.HashDataAsync(fileCData).ConfigureAwait(false);
            dirManifest.Add(
                "myDir/fileC",
                ManifestEntry.NewFile(
                    fileCHashingResult.Hash,
                    fileCEntryMetadata));

            // Build manifest chunks.
            await dirManifest.GetHashAsync(new Hasher()).ConfigureAwait(false);

            return chunkStore;
        }
    }
}