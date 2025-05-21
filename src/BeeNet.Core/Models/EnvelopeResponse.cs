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

namespace Etherna.BeeNet.Models
{
    public class EnvelopeResponse(
        string issuer,
        string index,
        string timestamp,
        string signature)
    {
        public string Issuer { get; set; } = issuer;

        /// <summary>
        /// Hexadecimal string representation of 8 bytes
        /// </summary>
        public string Index { get; set; } = index;

        /// <summary>
        /// Hexadecimal string representation of 8 bytes
        /// </summary>
        public string Timestamp { get; set; } = timestamp;

        /// <summary>
        /// Hexadecimal string representation of cryptographic signature
        /// </summary>
        public string Signature { get; set; } = signature;
    }
}