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

using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Models;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Etherna.BeeNet.Chunks
{
    public static class ChunkEncrypter
    {
        // Methods.
        public static void Decrypt(
            ReadOnlySpan<byte> data,
            EncryptionKey256 key,
            uint initCtr,
            Span<byte> decryptedData,
            Hasher hasher)
        {
            if (data.Length != decryptedData.Length)
                throw new ArgumentException("Data length must be equal to decrypted data length");

            Transform(data, key, initCtr, decryptedData, hasher);
        }

        /// <summary>
        /// Decrypt a chunk
        /// </summary>
        /// <param name="chunkSpanData"></param>
        /// <param name="key"></param>
        /// <param name="decryptedSpan"></param>
        /// <param name="decryptedData"></param>
        /// <param name="hasher"></param>
        /// <returns>Decrypted chunk data length</returns>
        public static int DecryptChunk(
            ReadOnlySpan<byte> chunkSpanData,
            EncryptionKey256 key,
            Span<byte> decryptedSpan,
            Span<byte> decryptedData,
            Hasher hasher)
        {
            if (decryptedSpan.Length != SwarmCac.SpanSize)
                throw new ArgumentException($"{nameof(decryptedSpan)} must have size {SwarmCac.SpanSize}");
            if (decryptedData.Length != SwarmCac.DataSize)
                throw new ArgumentException($"{nameof(decryptedData)} must have size {SwarmCac.DataSize}");
            
            // Decrypt.
            Transform(chunkSpanData[..SwarmCac.SpanSize], key, SwarmCac.DataSize / EncryptionKey256.KeySize, decryptedSpan, hasher);
            Transform(chunkSpanData[SwarmCac.SpanSize..], key, 0, decryptedData, hasher);

            // Calculate real data length removing added padding.
            var (level, decodedSpan) = ChunkRedundancy.DecodeSpanLevel(decryptedSpan);
            var length = SwarmCac.SpanToLength(decodedSpan);
            if (length <= SwarmCac.DataSize)
                return (int)length;
            
            var (dataShards, parities) = SwarmCac.CountIntermediateReferences(length, level, true);
            return SwarmReference.EncryptedSize * dataShards + parities * SwarmHash.HashSize;
        }
        
        public static (EncryptionKey256 Key, byte[] EncSpan, byte[] EncData) EncryptChunk(
            ReadOnlySpan<byte> chunkSpanData,
            EncryptionKey256? key,
            Hasher hasher)
        {
            var encryptedSpan = new byte[SwarmCac.SpanSize];
            var encryptedData = new byte[SwarmCac.DataSize];

            key = EncryptChunk(chunkSpanData, key, encryptedSpan, encryptedData, hasher);
            
            return (key.Value, encryptedSpan, encryptedData);
        }

        public static EncryptionKey256 EncryptChunk(
            ReadOnlySpan<byte> chunkSpanData,
            EncryptionKey256? key,
            Span<byte> encryptedSpan,
            Span<byte> encryptedData,
            Hasher hasher)
        {
            if (encryptedSpan.Length != SwarmCac.SpanSize)
                throw new ArgumentException($"{nameof(encryptedSpan)} must have size {SwarmCac.SpanSize}");
            if (encryptedData.Length != SwarmCac.DataSize)
                throw new ArgumentException($"{nameof(encryptedData)} must have size {SwarmCac.DataSize}");
            
            key ??= EncryptionKey256.BuildNewRandom();
            
            Transform(chunkSpanData[..SwarmCac.SpanSize], key.Value, SwarmCac.DataSize / EncryptionKey256.KeySize, encryptedSpan, hasher);
            Transform(chunkSpanData[SwarmCac.SpanSize..], key.Value, 0, encryptedData, hasher);

            return key.Value;
        }
        
        // Helpers.
        private static void Transform(
            ReadOnlySpan<byte> input,
            EncryptionKey256 key,
            uint initCtr,
            Span<byte> output,
            Hasher hasher)
        {
            var index = 0;
                
            var inputLength = input.Length;
            for (var i = 0; i < inputLength; i += EncryptionKey256.KeySize)
            {
                var l = Math.Min(EncryptionKey256.KeySize, inputLength - i);
                Transcript(input[i..(i + l)], key, index, initCtr, output[i..(i + l)], hasher);
                index++;
            }
            
            // Pad the rest if output is longer.
            RandomNumberGenerator.Fill(output[inputLength..]);
        }

        private static void Transcript(
            ReadOnlySpan<byte> input,
            EncryptionKey256 key,
            int index,
            uint initCtr,
            Span<byte> output,
            Hasher hasher)
        {
            // First hash key with counter (initial counter + index).
            List<byte> dataToHash = [];
            dataToHash.AddRange(key.ToByteArray());

            var ctrBytes = new byte[4];
            BinaryPrimitives.WriteUInt32LittleEndian(ctrBytes, (uint)index + initCtr);
            dataToHash.AddRange(ctrBytes);
            
            var ctrHash = hasher.ComputeHash(dataToHash.ToArray());
            dataToHash.Clear();

            // Second round of hashing for selective disclosure.
            var segmentKey = hasher.ComputeHash(ctrHash);
            
            // XOR input with segmentKey.
            var inputLength = input.Length;
            for (var i = 0; i < inputLength; i++)
                output[i] = (byte)(input[i] ^ segmentKey[i]);
            
            // Pad the rest if output is longer.
            RandomNumberGenerator.Fill(output[inputLength..]);
        }
    }
}