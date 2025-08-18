using Etherna.BeeNet.Models;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Hashing.Pipeline
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public class CompactedChunkAttemptResult(
        EncryptionKey256 chunkKey,
        ReadOnlyMemory<byte> encryptedSpanData,
        SwarmHash hash)
    {
        public EncryptionKey256 ChunkKey { get; } = chunkKey;
        public ReadOnlyMemory<byte> EncryptedSpanData { get; } = encryptedSpanData;
        public SwarmHash Hash { get; } = hash;
    }
}