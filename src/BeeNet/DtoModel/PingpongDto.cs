using System;

namespace Etherna.BeeNet.DtoModel
{
    public class PingPongDto
    {
        // Constructors.
        public PingPongDto(Clients.v1_4_1.DebugApi.Response17 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Rtt = response.Rtt;
        }


        // Properties.
        public string Rtt { get; }
    }
}
