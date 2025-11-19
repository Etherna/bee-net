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

using Etherna.BeeNet.Chunks;
using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Models;
using System;

namespace Etherna.BeeNet.Extensions
{
    public static class SwarmCacExtensions
    {
        public static SwarmDecodedCac Decode(
            this SwarmCac chunk,
            SwarmReference reference,
            Hasher hasher)
        {
            ArgumentNullException.ThrowIfNull(chunk);
            
            if (chunk.Hash != reference.Hash)
                throw new ArgumentException($"Chunk's hash {chunk.Hash} does not match reference {reference}");

            // Decrypt and decode span.
            ReadOnlyMemory<byte> plainData;
            RedundancyLevel redundancyLevel;
            ulong spanLength;
            if (reference.IsEncrypted)
            {
                // Decrypt span.
                var spanBuffer = new byte[SwarmCac.SpanSize];
                var dataBuffer = new byte[SwarmCac.DataSize];
                var dataLength = ChunkEncrypter.DecryptChunk(
                    chunk,
                    reference.EncryptionKey!.Value,
                    spanBuffer,
                    dataBuffer,
                    hasher);
                
                plainData = dataBuffer.AsMemory(0, dataLength);
                redundancyLevel = SwarmCac.SpanToRedundancyLevel(spanBuffer);
                spanLength = SwarmCac.SpanToLength(spanBuffer);
            }
            else
            {
                plainData = chunk.Data;
                redundancyLevel = SwarmCac.SpanToRedundancyLevel(chunk.Span.Span);
                spanLength = SwarmCac.SpanToLength(chunk.Span.Span);
            }

            // Count parities if is an intermediate chunk.
            var parities = spanLength <= SwarmCac.DataSize ? 0 :
                SwarmCac.CountIntermediateReferences(spanLength, redundancyLevel, reference.IsEncrypted).ParityShards;
            
            return new SwarmDecodedCac(reference, redundancyLevel, parities, spanLength, plainData);
        }
    }
}