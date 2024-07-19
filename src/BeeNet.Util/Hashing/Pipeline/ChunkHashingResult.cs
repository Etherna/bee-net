using Etherna.BeeNet.Manifest;
using Etherna.BeeNet.Models;

namespace Etherna.BeeNet.Hashing.Pipeline
{
    public class ChunkHashingResult(SwarmHash hash, XorEncryptKey? encryptionKey)
    {
        public XorEncryptKey? EncryptionKey { get; } = encryptionKey;
        public SwarmHash Hash { get; } = hash;
    }
}