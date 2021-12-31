using System;

namespace Etherna.BeeNet.DtoModel
{
    public class ReserveStateDto
    {
        // Constructors.
        public ReserveStateDto(Clients.v1_4_1.DebugApi.Response12 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Radius = response.Radius;
            Available = response.Available;
            Outer = response.Outer;
            Inner = response.Inner;
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
