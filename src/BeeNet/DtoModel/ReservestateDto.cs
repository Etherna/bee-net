namespace Etherna.BeeNet.DtoModel
{
    public class ReservestateDto
    {
        public ReservestateDto(Clients.v1_4.DebugApi.Response12 response12)
        {
            if (response12 is null)
            {
                return;
            }

            Radius = response12.Radius;
            Available = response12.Available;
            Outer = response12.Outer;
            Inner = response12.Inner;
        }

        public int Radius { get; set; } = default!;
        
        public int Available { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Outer { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Inner { get; set; } = default!;
    }
}
