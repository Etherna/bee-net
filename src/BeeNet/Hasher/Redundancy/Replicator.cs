// Copyright 2021-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Epoche;
using Etherna.BeeNet.Extensions;
using Etherna.BeeNet.Models;
using Nethereum.Hex.HexConvertors.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Etherna.BeeNet.Hasher.Redundancy
{
    internal class Replicator
    {
        // Consts.
        private readonly byte[] ReplicasOwner = "dc5b20847f43d67928f49cd4f85d696b5a7617b5".HexToByteArray();

        /// <summary>
        /// Number of distinct neighnourhoods redcorded for each depth.
        /// The actual number of replicas needed to keep the error rate below 1/10^6
        /// for the five levels of redundancy are 0, 2, 4, 5, 19
        /// we use an approximation as the successive powers of 2
        /// </summary>
        private readonly Dictionary<RedundancyLevel, int> Sizes = new()
        {
            [RedundancyLevel.None] = 0,
            [RedundancyLevel.Medium] = 2,
            [RedundancyLevel.Strong] = 4,
            [RedundancyLevel.Insane] = 8,
            [RedundancyLevel.Paranoid] = 16
        };

        // Fields.
        private byte[] addr; // chunk address
        private Replica?[] queue = new Replica[16]; // to sort addresses according to di
        private bool[] exist = new bool[30]; //  maps the 16 distinct nibbles on all levels

        private RedundancyLevel rLevel;

        // Constructor.
        public Replicator(SwarmAddress address, RedundancyLevel redundancyLevel)
        {
            addr = address.Hash.ToByteArray();
            rLevel = redundancyLevel;
            Replicas();
        }

        // Properties.
        public Collection<Replica> C { get; } = new();

        // Methods.
        /// <summary>
        /// Inserts the soc replica into a replicator so that addresses are balanced
        /// </summary>
        /// <param name="r"></param>
        /// <param name="rLevel"></param>
        /// <returns></returns>
        public (int depth, int rank) Add(Replica r, RedundancyLevel rLevel)
        {
            ArgumentNullException.ThrowIfNull(r, nameof(r));
            
            if (rLevel == RedundancyLevel.None)
                return (0, 0);
            var nh = Nh(rLevel, r.Addr);

            if (exist[nh])
                return (0, 0);
            exist[nh] = true;

            var d = (int)rLevel - 1;
            var (l, o) = Add(r, (RedundancyLevel)d);
            if (l == 0)
            {
                o = Sizes[(RedundancyLevel)d];
                Sizes[(RedundancyLevel)d]++;
                queue[o] = r;
                l = rLevel.GetReplicaCount();
            }

            return (l, o);
        }

        /// <summary>
        /// Returns a replica params strucure seeded with a byte of entropy as argument
        /// </summary>
        /// <param name="i">Byte of entropy</param>
        /// <returns></returns>
        public Replica Replicate(byte i)
        {
            // change the last byte of the address to create SOC ID
            var id = new byte[32];
            Array.Copy(addr, id, addr.Length);
            id[0] = i;

            // calculate SOC address for potential replica
            var address = Keccak256.ComputeHash(id.Concat(ReplicasOwner).ToArray());
            return new Replica(address, id);
        }

        /// <summary>
        /// Replicas enumerates replica parameters (SOC ID) pushing it in a channel given as argument
        /// the order of replicas is so that addresses are always maximally dispersed
        /// in successive sets of addresses.
        /// I.e., the binary tree representing the new addresses prefix bits up to depth is balanced
        /// </summary>
        public void Replicas()
        {
            var n = 0;
            for (byte i = 0; n < Sizes[rLevel] && i < byte.MaxValue; i++)
            {
                // create soc replica (ID and address using constant owner)
                // the soc is added to neighbourhoods of depths in the closed interval [from...to]
                var r = Replicate(i);
                var (d, m) = Add(r, rLevel);
                if (d == 0)
                    continue;

                foreach (var r2 in queue)
                {
                    if (r2 == null)
                        break;
                    C.Add(r2);
                }

                n += m;
            }
        }
        
        // Helpers.
        /// <summary>
        /// Returns the lookup key based on the redundancy level
        /// to be used as index to the replicators exist array
        /// </summary>
        /// <param name="rLevel"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
        private static int Nh(RedundancyLevel rLevel, byte[] addr)
        {
            var replicaIndexBases = new[] { 0, 2, 6, 14 };
            var d = (byte)rLevel;
            return replicaIndexBases[d - 1] + (addr[0] >> (8 - d));
        }
    }
}