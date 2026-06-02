// Copyright 2021-present Etherna SA
// This file is part of SwarmSDK.
// 
// SwarmSDK is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// SwarmSDK is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with SwarmSDK.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.SwarmSdk.Hashing.Signer;
using Etherna.SwarmSdk.Models;
using Etherna.SwarmSdk.Stores;
using System;

namespace Etherna.SwarmSdk.Hashing.Postage
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