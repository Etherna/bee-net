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

using Etherna.BeeNet.Models;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;

namespace Etherna.BeeNet.Hashing.Signer
{
    public class PrivateKeySigner(EthECKey privateKey) : ISigner
    {
        // Fields.
        private readonly EthereumMessageSigner messageSigner = new();
        
        // Properties.
        public EthAddress PublicAddress => privateKey.GetPublicAddress();
        
        // Methods.
        public byte[] GetPublicKey() => privateKey.GetPubKeyNoPrefix();

        /// <summary>
        /// Signs data with ethereum prefix (eip191 type 0x45)
        /// </summary>
        /// <param name="toSign">Data to sign</param>
        /// <returns>Signature</returns>
        public byte[] Sign(byte[] toSign) =>
            messageSigner.Sign(toSign, privateKey).HexToByteArray();
    }
}