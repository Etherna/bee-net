using System;

namespace Etherna.BeeNet.DtoModel
{
    public class SettlementDataDto
    {
        // Constructors.
        public SettlementDataDto(Clients.v1_4_1.DebugApi.Settlements settlement)
        {
            if (settlement is null)
                throw new ArgumentNullException(nameof(settlement));

            Peer = settlement.Peer;
            Received = settlement.Received;
            Sent = settlement.Sent;
        }

        public SettlementDataDto(Clients.v1_4_1.DebugApi.Settlements2 settlements)
        {
            if (settlements is null)
                throw new ArgumentNullException(nameof(settlements));

            Peer = settlements.Peer;
            Received = settlements.Received;
            Sent = settlements.Sent;
        }


        // Properties.
        public string Peer { get; }
        public int Received { get; }
        public int Sent { get; }
    }
}
