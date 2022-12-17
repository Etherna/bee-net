using System;

namespace Etherna.BeeNet.DtoModels
{
    public class AccountDto
    {
        // Constructors.
        public AccountDto(Clients.DebugApi.V4_0_0.Anonymous anonymous)
        {
            if (anonymous is null)
                throw new ArgumentNullException(nameof(anonymous));

            Balance = anonymous.Balance;
            ThresholdReceived = anonymous.ThresholdReceived;
            ThresholdGiven = anonymous.ThresholdGiven;
            SurplusBalance = anonymous.SurplusBalance;
            ReservedBalance = anonymous.ReservedBalance;
            ShadowReservedBalance = anonymous.ShadowReservedBalance;
            GhostBalance = anonymous.GhostBalance;
        }

        // Properties.
        public string Balance { get; set; }
        public string ThresholdReceived { get; set; }
        public string ThresholdGiven { get; set; }
        public string SurplusBalance { get; set; }
        public string ReservedBalance { get; set; }
        public string ShadowReservedBalance { get; set; }
        public string GhostBalance { get; set; }
    }
}
