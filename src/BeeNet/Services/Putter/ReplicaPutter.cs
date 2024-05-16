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
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

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

        [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        public void Put(SwarmChunk swarmChunk)
        {
            ArgumentNullException.ThrowIfNull(swarmChunk, nameof(swarmChunk));
            
            var errs = new ConcurrentBag<Exception>();
            try
            {
                nextPutter.Put(swarmChunk);
            }
            catch (Exception e)
            {
                errs.Add(e);
            }

            var rr = new Replicator(swarmChunk.Address, redundancyLevel);

            var tasks = rr.C.Select(r =>
            {
                return Task.Run(new Func<Task>(() =>
                {
                    throw new NotImplementedException();
                    //do things with SOC...
                    // try
                    // {
                    //     
                    // }
                    // catch (Exception err)
                    // {
                    //     errs.Add(err);
                    // }
                }));
            });

            Task.WhenAll(tasks).Wait();
            
            if(!errs.IsEmpty)
                throw new AggregateException(errs);
        }
    }
}