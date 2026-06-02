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

using Etherna.SwarmSdk.Models;
using System;

namespace Etherna.SwarmSdk.Hashing.Signer
{
    public class PrivateKeySigner(EthPrivateKey privateKey) : ISigner
    {
        // Properties.
        public EthAddress PublicAddress => privateKey.Address;

        // Methods.
        public byte[] GetPublicKey() => privateKey.PublicKey.ToByteArray();

        /// <summary>
        /// Signs data with ethereum prefix (eip191 type 0x45)
        /// </summary>
        /// <param name="toSign">Data to sign</param>
        /// <returns>Signature</returns>
        public byte[] Sign(byte[] toSign)
        {
            ArgumentNullException.ThrowIfNull(toSign);
            return privateKey.Sign(toSign);
        }
    }
}