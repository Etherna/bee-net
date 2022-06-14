using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Etherna.BeeNet.DtoModels
{
    public class FileResponseDto
    {
        // Constructors.
        public FileResponseDto(Clients.GatewayApi.V3_0_2.FileResponse response)
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
