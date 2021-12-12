using System;

namespace Etherna.BeeNet.DtoModel
{
    public class SettlementDataDto
    {
        // Constructors.
        public SettlementDataDto(Clients.v1_4.DebugApi.Settlements settlement)
        {
            if (settlement is null)
                throw new ArgumentNullException(nameof(settlement));

            Peer = settlement.Peer;
            Received = settlement.Received;
            Sent = settlement.Sent;
        }

        public SettlementDataDto(Clients.v1_4.DebugApi.Settlements2 settlements2)
        {
            if (settlements2 is null)
                throw new ArgumentNullException(nameof(settlements2));

            Peer = settlements2.Peer;
            Received = settlements2.Received;
            Sent = settlements2.Sent;
        }


        // Properties.
        public string Peer { get; }
        public int Received { get; }
        public int Sent { get; }
    }
}
