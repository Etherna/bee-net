using System;

namespace Etherna.BeeNet.DtoModels
{
    public class RedistributionStateDto
    {
        // Constructors.
        internal RedistributionStateDto(Clients.DebugApi.V4_0_0.Response33 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            IsFrozen = response.IsFrozen;
            IsFullySynced = response.IsFullySynced;
            Round = response.Round;
            LastWonRound = response.LastWonRound;
            LastPlayedRound = response.LastPlayedRound;
            LastFrozenRound = response.LastFrozenRound;
            Block = response.Block;
            Reward = response.Reward;
            Fees = response.Fees;
        }

        // Properties.
        public bool IsFrozen { get; set; } = default!;
        public bool IsFullySynced { get; set; } = default!;
        public int Round { get; set; } = default!;
        public int LastWonRound { get; set; } = default!;
        public int LastPlayedRound { get; set; } = default!;
        public int LastFrozenRound { get; set; } = default!;
        public int Block { get; set; } = default!;
        public string Reward { get; set; } = default!;
        public string Fees { get; set; } = default!;
    }
}
