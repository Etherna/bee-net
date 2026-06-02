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

namespace Etherna.SwarmSdk.Hashing.Postage
{
    public interface IPostageStamper
    {
        // Properties.
        ISigner Signer { get; }
        IPostageStampIssuer StampIssuer { get; }
        IStampStore StampStore { get; }

        // Methods.
        PostageStamp Stamp(SwarmHash hash);
    }
}