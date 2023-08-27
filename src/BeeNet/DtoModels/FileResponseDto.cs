using System;
using System.IO;

namespace Etherna.BeeNet.DtoModels
{
    public class FileResponseDto
    {
        // Constructors.
        internal FileResponseDto(Clients.GatewayApi.V5_0_0.FileResponse response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Stream = response.Stream;
            IsFeed = response.Headers.ContainsKey("Swarm-Feed-Index");
        }

        // Properties.
        public bool IsFeed { get; }
        public Stream Stream { get; }
    }
}
