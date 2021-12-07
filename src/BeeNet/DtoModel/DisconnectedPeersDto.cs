﻿namespace Etherna.BeeNet.DtoModel
{
    public class DisconnectedPeersDto
    {
        public DisconnectedPeersDto(
            string address, 
            MetricsDto metrics)
        {
            Address = address;
            Metrics = metrics;
        }
        
        public string Address { get; set; } = default!;
        
        public MetricsDto Metrics { get; set; } = default!;
    }
}
