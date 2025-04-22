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

using Etherna.BeeNet.Hashing.Signer;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Stores;
using System;

namespace Etherna.BeeNet.Hashing.Postage
{
    public class FakePostageStamper : IPostageStamper
    {
        public ISigner Signer { get; } = new FakeSigner();
        public IPostageStampIssuer StampIssuer { get; } = new FakePostageStampIssuer();
        public IStampStore StampStore { get; } = new MemoryStampStore();

        public PostageStamp Stamp(SwarmHash hash) =>
            new(StampIssuer.PostageBatch.Id, new PostageBucketIndex(0, 0), DateTimeOffset.Now, Array.Empty<byte>());
    }
}