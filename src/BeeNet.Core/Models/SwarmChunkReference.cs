namespace Etherna.BeeNet.Models
{
    public class SwarmChunkReference(SwarmHash hash, XorEncryptKey? encryptionKey, bool useRecursiveEncryption)
    {
        public XorEncryptKey? EncryptionKey { get; } = encryptionKey;
        public SwarmHash Hash { get; } = hash;
        public bool UseRecursiveEncryption { get; } = useRecursiveEncryption;
    }
}