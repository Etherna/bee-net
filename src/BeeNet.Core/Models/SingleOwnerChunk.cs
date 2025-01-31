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

using System;

namespace Etherna.BeeNet.Models
{
    public class SingleOwnerChunk(byte[] id, byte[] signature, byte[] owner, object chunk)
    {
        // Consts.
        public const int MinChunkSize = SwarmHash.HashSize + SocSignatureSize + SwarmChunk.SpanSize;
        public const int SocSignatureSize = 65;
        
        // Properties.
        public ReadOnlyMemory<byte> Id { get; } = id;
        public ReadOnlyMemory<byte> Signature { get; } = signature;
        public ReadOnlyMemory<byte> Owner { get; } = owner;
        public object Chunk { get; } = chunk;
    }
}