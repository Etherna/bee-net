using System;

namespace Etherna.BeeNet.DtoModel
{
    public class ReservestateDto
    {
        // Constructors.
        public ReservestateDto(Clients.v1_4.DebugApi.Response12 response12)
        {
            if (response12 is null)
                throw new ArgumentNullException(nameof(response12));

            Radius = response12.Radius;
            Available = response12.Available;
            Outer = response12.Outer;
            Inner = response12.Inner;
        }


        // Properties.
        public int Radius { get; }
        public int Available { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Outer { get; }
        /// <summary>Numeric string that represents integer which might exceeds `Number.MAX_SAFE_INTEGER` limit (2^53-1)</summary>
        public string Inner { get; }
    }
}
