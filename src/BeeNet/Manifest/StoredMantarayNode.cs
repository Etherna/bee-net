using Etherna.BeeNet.Hasher.Store;
using Etherna.BeeNet.Models;

namespace Etherna.BeeNet.Manifest
{
    public class StoredMantarayNode : MantarayNode
    {
        // Fields.
        private readonly IChunkStore chunkStore;

        // Constructor.
        public StoredMantarayNode(SwarmChunk chunk, IChunkStore chunkStore)
        {
            this.chunkStore = chunkStore;
            Chunk = chunk;
            
            // Initialize node state.
            //TODO
        }
        
        // Properties.
        public SwarmChunk Chunk { get; }
    }
}