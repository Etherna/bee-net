using System;
using System.IO;

namespace Etherna.BeeNet.DtoModels
{
    public class FileResponseDto
    {
        // Constructors.
        internal FileResponseDto(Clients.GatewayApi.V3_2_0.FileResponse response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            IsPartial = response.IsPartial;
            StatusCode = response.StatusCode;
            Stream = response.Stream;
        }

        // Properties.
        public bool IsPartial { get; }
        public int StatusCode { get; }
        public Stream Stream { get; }
    }
}
