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

using Etherna.BeeNet.Models;
using Etherna.BeeNet.Services.Putter.Models;

namespace Etherna.BeeNet.Services.Putter
{
    /// <summary>
    /// Implements the integration of dispersed replicas in chunk upload
    /// </summary>
    public class ReplicaPutter : IPutter
    {
        private readonly IPutter nextPutter;
        private readonly RedundancyLevel redundancyLevel;

        public ReplicaPutter(
            IPutter nextPutter,
            RedundancyLevel redundancyLevel)
        {
            this.nextPutter = nextPutter;
            this.redundancyLevel = redundancyLevel;
        }

        public void Put(SwarmChunk swarmChunk)
        {
            nextPutter.Put(swarmChunk);
            var rr = new Replicator(swarmChunk.Address, redundancyLevel);
            

            errc := make(chan error, redundancyLevel.GetReplicaCount())
            wg := sync.WaitGroup{}
            for r := range rr.c {
                r := r
                wg.Add(1)
                go func() {
                    defer wg.Done()
                    sch, err := soc.New(r.id, ch).Sign(signer)
                    if err == nil {
                        err = p.putter.Put(ctx, sch)
                    }
                    errc <- err
                }()
            }

            wg.Wait()
            close(errc)
            for err := range errc {
                errs = append(errs, err)
            }
            return errors.Join(errs...)
        }
    }
}