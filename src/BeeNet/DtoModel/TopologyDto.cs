﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModel
{
    public class TopologyDto
    {
        // Constructors.
        public TopologyDto(Clients.v1_4.DebugApi.Response22 response22)
        {
            if (response22 is null)
                throw new ArgumentNullException(nameof(response22));

            BaseAddr = response22.BaseAddr;
            Population = response22.Population;
            Connected = response22.Connected;
            Timestamp = response22.Timestamp;
            NnLowWatermark = response22.NnLowWatermark;
            Depth = response22.Depth;
            Bins = response22.Bins.ToDictionary(
                i => i.Key,
                i => new AnonymousDto(i.Value));
        }


        // Properties.
        public string BaseAddr { get; }
        public int Population { get; }
        public int Connected { get; }
        public string Timestamp { get; }
        public int NnLowWatermark { get; }
        public int Depth { get; }
        public IDictionary<string, AnonymousDto> Bins { get; }
    }
}
