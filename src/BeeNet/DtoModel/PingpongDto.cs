namespace Etherna.BeeNet.DtoModel
{
    public class PingpongDto
    {
        public PingpongDto(Clients.v1_4.DebugApi.Response17 response17)
        {
            if (response17 is null)
            {
                return;
            }

            Rtt = response17.Rtt;
        }

        public string Rtt { get; set; } = default!;
    }
}
