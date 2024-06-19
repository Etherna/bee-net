using Etherna.BeeNet.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Hasher.Store
{
    public class ChunkJoiner
    {
        // Fields.
        private readonly IChunkStore chunkStore;

        // Constructor.
        public ChunkJoiner(IChunkStore chunkStore)
        {
            this.chunkStore = chunkStore;
        }
        
        // Methods.
        public async Task<IEnumerable<byte>> GetJoinedChunkDataAsync(SwarmHash hash, bool isCompacted)
        {
            var chunk = await chunkStore.GetAsync(hash).ConfigureAwait(false);
            var totalDataLength = SwarmChunk.SpanToLength(chunk.Span.Span);
            
            if (totalDataLength <= SwarmChunk.DataSize)
                return isCompacted
                    ? chunk.Data[1..chunk.Data.Length].ToArray()
                    : chunk.Data.ToArray();
            
            var joinedData = new List<byte>();
                
            for (int i = 0; i < chunk.Data.Length; i += SwarmHash.HashSize)
            {
                var childHash = new SwarmHash(chunk.Data[i..(i + SwarmHash.HashSize)].ToArray());
                joinedData.AddRange(await GetJoinedChunkDataAsync(childHash, isCompacted).ConfigureAwait(false));
            }
            
            return joinedData;
        }
    }
}