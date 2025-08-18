using Etherna.BeeNet.Manifest;
using Etherna.BeeNet.Stores;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Models
{
    public sealed class SwarmChunkReference(
        SwarmHash hash,
        EncryptionKey256? encryptionKey,
        bool useRecursiveEncryption)
    {
        // Static builders.
        public static async Task<SwarmChunkReference> ResolveFromAddressAsync(
            SwarmAddress address,
            IReadOnlyChunkStore chunkStore) =>
            (await address.ResolveToResourceInfoAsync(
                chunkStore, ManifestPathResolver.IdentityResolver).ConfigureAwait(false)).Result.ChunkReference;
        public static async Task<SwarmChunkReference> ResolveFromStringAsync(
            string hashOrAddress,
            IReadOnlyChunkStore chunkStore)
        {
            if (SwarmHash.IsValidHash(hashOrAddress))
                return new SwarmChunkReference(SwarmHash.FromString(hashOrAddress), null, false);
            return (await SwarmAddress.FromString(hashOrAddress).ResolveToResourceInfoAsync(
                    chunkStore, ManifestPathResolver.IdentityResolver).ConfigureAwait(false))
                .Result.ChunkReference;
        }

        // Properties.
        public EncryptionKey256? EncryptionKey { get; } = encryptionKey;
        public SwarmHash Hash { get; } = hash;
        public bool UseRecursiveEncryption { get; } = useRecursiveEncryption;
        
        // Methods.
        public static SwarmChunkReference FromSwarmHash(SwarmHash hash) => new(hash, null, false);
        
        // Implicit conversion operator methods.
        public static implicit operator SwarmChunkReference(SwarmHash hash) => new(hash, null, false);
    }
}