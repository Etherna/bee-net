namespace Etherna.BeeNet.DtoModel
{
    public class ReservestateDto
    {
        public ReservestateDto(int radius, int available, string outer, string inner)
        {
            Radius = radius;
            Available = available;
            Outer = outer;
            Inner = inner;
        }

        public int Radius { get; set; } = default!;
        
        public int Available { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Outer { get; set; } = default!;

        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Inner { get; set; } = default!;
    }
}
