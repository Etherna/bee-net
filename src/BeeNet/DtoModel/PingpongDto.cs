using System;

namespace Etherna.BeeNet.DtoModel
{
    public class PingpongDto
    {
        // Constructors.
        public PingpongDto(Clients.v1_4.DebugApi.Response17 response17)
        {
            if (response17 is null)
                throw new ArgumentNullException(nameof(response17));

            Rtt = response17.Rtt;
        }


        // Properties.
        public string Rtt { get; }
    }
}
