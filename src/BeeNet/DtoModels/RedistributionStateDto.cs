using System;

namespace Etherna.BeeNet.DtoModels
{
    public class RedistributionStateDto
    {
        // Constructors.
        internal RedistributionStateDto(Clients.DebugApi.V4_0_0.Response31 response)
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
        public bool IsFrozen { get; set; }
        public bool IsFullySynced { get; set; }
        public int Round { get; set; } 
        public int LastWonRound { get; set; } 
        public int LastPlayedRound { get; set; } 
        public int LastFrozenRound { get; set; } 
        public int Block { get; set; } 
        public string Reward { get; set; }
        public string Fees { get; set; } 
    }
}
