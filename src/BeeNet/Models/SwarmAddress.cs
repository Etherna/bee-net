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

using Nethereum.Hex.HexConvertors.Extensions;
using System;

namespace Etherna.BeeNet.Models
{
    public class SwarmAddress
    {
        // Consts.
        public const int HashSize = 32;
        
        // Fields.
        private readonly byte[] byteAddress;

        // Constructors.
        public SwarmAddress(byte[] byteAddress)
        {
            ArgumentNullException.ThrowIfNull(byteAddress, nameof(byteAddress));
            if (byteAddress.Length != HashSize / 2)
                throw new ArgumentOutOfRangeException(nameof(byteAddress));
            
            this.byteAddress = byteAddress;
        }

        public SwarmAddress(string strAddress)
        {
            ArgumentNullException.ThrowIfNull(strAddress, nameof(strAddress));
            if (strAddress.Length != HashSize)
                throw new ArgumentOutOfRangeException(nameof(strAddress));
            
            byteAddress = strAddress.HexToByteArray();
        }
        
        // Static properties.
        public static SwarmAddress Zero { get; } = new(new byte[16]);

        // Methods.
        public override string ToString() => byteAddress.ToHex();
    }
}