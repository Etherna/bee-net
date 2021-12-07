namespace Etherna.BeeNet.DtoModel.DebugApi
{
    public class PingpongDto
    {
        public PingpongDto(string rtt)
        {
            Rtt = rtt;
        }

        public string Rtt { get; set; } = default!;
    }
}
