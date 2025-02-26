using Etherna.BeeNet.Models;
using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Hashing.Pipeline
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public class CompactedChunkAttemptResult(
        XorEncryptKey chunkKey,
        byte[] encryptedData,
        SwarmHash hash)
    {
        public XorEncryptKey ChunkKey { get; } = chunkKey;
        public byte[] EncryptedData { get; } = encryptedData;
        public SwarmHash Hash { get; } = hash;
    }
}