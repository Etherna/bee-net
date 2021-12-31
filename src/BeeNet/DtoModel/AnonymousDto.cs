﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.BeeNet.DtoModel
{
    public class AnonymousDto
    {
        // Constructors.
        public AnonymousDto(Clients.v1_4_1.DebugApi.Anonymous anonymous)
        {
            if (anonymous is null)
                throw new ArgumentNullException(nameof(anonymous));

            Population = anonymous.Population;
            Connected = anonymous.Connected;
            DisconnectedPeers = anonymous.DisconnectedPeers
                .Select(k => new DisconnectedPeersDto(k));
            ConnectedPeers = anonymous.ConnectedPeers
                .Select(k => new ConnectedPeersDto(k));
        }


        // Properties.
        public int Population { get; }
        public int Connected { get; }
        public IEnumerable<DisconnectedPeersDto> DisconnectedPeers { get; }
        public IEnumerable<ConnectedPeersDto> ConnectedPeers { get; }
    }
}
