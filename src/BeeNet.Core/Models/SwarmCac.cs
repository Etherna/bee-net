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

using Etherna.BeeNet.Extensions;
using System;
using System.Buffers.Binary;

namespace Etherna.BeeNet.Models
{
    public sealed class SwarmCac : SwarmChunk
    {
        // Consts.
        public const int DataSize = 4096;
        public const int SpanDataSize = SpanSize + DataSize;
        public const int SpanSize = 8;
        
        // Fields.
        private RedundancyLevel? _redundancyLevel;
        
        // Constructors.
        public SwarmCac(SwarmHash hash, ReadOnlyMemory<byte> spanData)
        {
            if (spanData.Length < SpanSize)
                throw new ArgumentOutOfRangeException(nameof(spanData),
                    $"Data with span can't be shorter than {SpanSize} bytes");
            if (spanData.Length > SpanDataSize)
                throw new ArgumentOutOfRangeException(nameof(spanData),
                    $"Data with span can't be longer than {SpanDataSize} bytes");
            
            Hash = hash;
            SpanData = spanData;
        }
        
        public SwarmCac(SwarmHash hash, ReadOnlyMemory<byte> span, ReadOnlyMemory<byte> data)
        {
            if (span.Length != SpanSize)
                throw new ArgumentOutOfRangeException(nameof(span), $"Span size must be {SpanSize} bytes");
            if (data.Length > DataSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Data can't be longer than {DataSize} bytes");
            
            Hash = hash;
            
            var spanDataBuffer = new byte[SpanSize + data.Length];
            span.CopyTo(spanDataBuffer);
            data.CopyTo(spanDataBuffer.AsMemory(SpanSize));
            SpanData = spanDataBuffer;
        }
        
        // Static builders.
        public static SwarmCac BuildFromData(SwarmHash hash, ReadOnlyMemory<byte> data)
        {
            if (data.Length > DataSize)
                throw new ArgumentOutOfRangeException(nameof(data), $"Data can't be longer than {DataSize} bytes");
            
            var spanDataBuffer = new byte[SpanSize + data.Length];
            LengthToSpan((ulong)data.Length).CopyTo(spanDataBuffer, 0);
            data.CopyTo(spanDataBuffer.AsMemory(SpanSize));

            return new(hash, spanDataBuffer);
        }

        // Properties.
        public override SwarmHash Hash { get; }
        public ReadOnlyMemory<byte> Span => SpanData[..SpanSize];
        public ReadOnlyMemory<byte> Data => SpanData[SpanSize..];
        public ReadOnlyMemory<byte> SpanData { get; }
        public RedundancyLevel RedundancyLevel => _redundancyLevel ??= GetSpanRedundancyLevel(Span.Span);
        public ulong SpanLength => SpanToLength(
            IsSpanEncoded(Span.Span) ?
                DecodeSpan(Span.Span) :
                Span.Span);

        // Methods.
        public (int DataShards, int ParityShards) CountIntermediateReferences(bool encryptedDataReferences) =>
            CountIntermediateReferences(SpanLength, RedundancyLevel, encryptedDataReferences);
        public override ReadOnlyMemory<byte> GetFullPayload() => SpanData;
        public override byte[] GetFullPayloadToByteArray() => SpanData.ToArray();
        public SwarmShardReference[] GetIntermediateReferences(bool encryptedDataReferences)
        {
            var (_, parities) = CountIntermediateReferences(encryptedDataReferences);
            return GetIntermediateReferencesFromData(Data.Span, parities, encryptedDataReferences);
        }

        // Static methods.
        public static int CalculatePlainDataLength(
            ulong spanLength,
            RedundancyLevel redundancyLevel,
            bool encryptedDataReferences)
        {
            // If is data chunk.
            if (spanLength <= DataSize)
                return (int)spanLength;
            
            // If is intermediate chunk.
            var (dataShards, parities) = CountIntermediateReferences(spanLength, redundancyLevel, encryptedDataReferences);
            return (encryptedDataReferences ? SwarmReference.EncryptedSize : SwarmReference.PlainSize) * dataShards +
                   parities * SwarmHash.HashSize;
        }

        public static (int DataShards, int ParityShards) CountIntermediateReferences(
            ReadOnlySpan<byte> span,
            bool isEncrypted)
        {
            var spanLength = SpanToLength(IsSpanEncoded(span) ? DecodeSpan(span) : span);
            var redundancyLevel = GetSpanRedundancyLevel(span);
            return CountIntermediateReferences(spanLength, redundancyLevel, isEncrypted);
        }
        
        public static (int DataShards, int ParityShards) CountIntermediateReferences(
            ulong spanLength,
            RedundancyLevel redundancyLevel,
            bool isEncrypted)
        {
            if (spanLength <= DataSize)
                throw new ArgumentOutOfRangeException(nameof(spanLength), $"Span length is not from intermediate chunk, it must be greater than {DataSize} bytes");
            
            /*
             * Assume we have a trie of size `span` then we can assume that all the forks except for
             * the last one on the right are of equal size, this is due to how the splitter wraps levels.
             * First the algorithm will search for a BMT level where span can be included,
             * then identify how large data one reference can hold on that level,
             * then count how many references can satisfy span,
             * and finally how many parity shards should be on that level.
             */
            
            //branching factor is how many data shard references can fit into one intermediate chunk
            var branching = (ulong)redundancyLevel.GetMaxDataShards(isEncrypted);
            
            // Search for branch level big enough to include span.
            var branchLevel = 1;
            ulong branchSize = DataSize;
            for (; branchSize < spanLength; branchLevel++)
                branchSize *= branching;
            
            // Span in one full reference. referenceSize = branching ^ (branchLevel - 1)
            ulong referenceSize = DataSize;
            for (var i = 1; i < branchLevel - 1; i++)
                referenceSize *= branching;

            var dataShards = (int)(spanLength / referenceSize);
            if (spanLength % referenceSize != 0)
                dataShards++;

            var parityShards = redundancyLevel.GetParitiesAmount(isEncrypted, dataShards);

            return (dataShards, parityShards);
        }
        
        /// <summary>
        /// Remove redundancy level from span keeping the real byte count for the chunk
        /// </summary>
        /// <param name="span">Span to decode</param>
        /// <returns>Decoded span</returns>
        public static byte[] DecodeSpan(ReadOnlySpan<byte> span)
        {
            var decodedSpan = new byte[SpanSize];
            span.CopyTo(decodedSpan);
            DecodeSpan(decodedSpan);
            return decodedSpan;
        }
        
        /// <summary>
        /// Remove redundancy level from span keeping the real byte count for the chunk
        /// </summary>
        /// <param name="span">Span to decode</param>
        public static void DecodeSpan(Span<byte> span)
        {
            if (span.Length != SpanSize)
                throw new ArgumentException("Span length must be " + SpanSize);

            // Remove redundancy level from the most significant byte.
            span[SpanSize - 1] = 0;
        }

        /// <summary>
        /// Encodes redundancy level into span
        /// </summary>
        /// <param name="span">Span to encode</param>
        /// <param name="level">Redundancy level to encode into span</param>
        /// <returns>Encoded span</returns>
        public static byte[] EncodeSpan(ReadOnlySpan<byte> span, RedundancyLevel level)
        {
            var encodedSpan = new byte[SpanSize];
            span.CopyTo(encodedSpan);
            EncodeSpan(encodedSpan, level);
            return encodedSpan;
        }

        /// <summary>
        /// Encodes redundancy level into span
        /// </summary>
        /// <param name="span">Span to encode</param>
        /// <param name="level">Redundancy level to encode into span</param>
        public static void EncodeSpan(Span<byte> span, RedundancyLevel level)
        {
            if (span.Length != SpanSize)
                throw new ArgumentException("Span length must be " + SpanSize);

            // Set redundancy level in the most significant byte.
            span[SpanSize - 1] = (byte)(level + 128);
        }

        public static SwarmShardReference[] GetIntermediateReferencesFromData(
            ReadOnlySpan<byte> data,
            int parityReferencesAmount,
            bool encryptedDataReferences)
        {
            var parityRefLength = SwarmReference.PlainSize;
            var parityRefsLength = parityReferencesAmount * parityRefLength;
            if (data.Length < parityRefsLength)
                throw new ArgumentException("Data can't be shorter than parity references length: " + parityRefsLength);
            
            var dataRefLength = encryptedDataReferences ? SwarmReference.EncryptedSize : SwarmReference.PlainSize;
            var dataRefsLength = data.Length - parityRefsLength;
            if (dataRefsLength % dataRefLength != 0)
                throw new ArgumentException($"Data length must be multiple of {dataRefLength}");

            var dataReferencesAmount = dataRefsLength / dataRefLength;
            var references = new SwarmShardReference[dataReferencesAmount + parityReferencesAmount];
            var cursor = 0;
            for (int i = 0; i < references.Length; i++)
            {
                if (i < dataReferencesAmount) //data reference
                {
                    references[i] = new SwarmShardReference(data[cursor..(cursor + dataRefLength)].ToArray(), false);
                    cursor += dataRefLength;
                }
                else //parity reference
                {
                    references[i] = new SwarmShardReference(data[cursor..(cursor + parityRefLength)].ToArray(), true);
                    cursor += parityRefLength;
                }
            }

            return references;
        }

        public static SwarmShardReference[] GetIntermediateReferencesFromSpanData(
            ReadOnlySpan<byte> spanData,
            bool encryptedDataReferences)
        {
            var span = spanData[..SpanSize];
            var spanLength = SpanToLength(IsSpanEncoded(span) ? DecodeSpan(span) : span);
            var redundancyLevel = GetSpanRedundancyLevel(span);
            var (_, parities) = CountIntermediateReferences(spanLength, redundancyLevel, encryptedDataReferences);
            return GetIntermediateReferencesFromData(spanData[SpanSize..], parities, encryptedDataReferences);
        }
        
        public static RedundancyLevel GetSpanRedundancyLevel(ReadOnlySpan<byte> span)
        {
            if (span.Length != SpanSize)
                throw new ArgumentException("Span length must be " + SpanSize);
            
            if (!IsSpanEncoded(span))
                return RedundancyLevel.None;
            return (RedundancyLevel)(span[SpanSize - 1] & 127);
        }

        public static bool IsSpanEncoded(ReadOnlySpan<byte> span)
        {
            if (span.Length != SpanSize)
                throw new ArgumentException("Span length must be " + SpanSize);
            
            return span[SpanSize - 1] > 128;
        }
        
        public static bool IsValid(SwarmHash hash, ReadOnlyMemory<byte> spanData, SwarmChunkBmt swarmChunkBmt)
        {
            ArgumentNullException.ThrowIfNull(swarmChunkBmt, nameof(swarmChunkBmt));

            if (spanData.Length is < SpanSize or > SpanDataSize)
                return false;
            return hash == swarmChunkBmt.Hash(spanData);
        }
        
        public static byte[] LengthToSpan(ulong length)
        {
            var span = new byte[SpanSize];
            WriteSpan(length, span);
            return span;
        }

        public static ulong SpanToLength(ReadOnlySpan<byte> span) =>
            BinaryPrimitives.ReadUInt64LittleEndian(span);

        public static void WriteSpan(ulong length, Span<byte> outputSpan) =>
            BinaryPrimitives.WriteUInt64LittleEndian(outputSpan, length);
    }
}