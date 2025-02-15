// Copyright 2021-present Etherna SA
// This file is part of Bee.Net.
// 
// Bee.Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Bee.Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Bee.Net.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.BeeNet.AspNet.TypeConverters;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace Etherna.BeeNet.AspNet
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBeeNet(this IServiceCollection services)
        {
            // Register global TypeConverters.
            TypeDescriptor.AddAttributes(typeof(EthAddress), new TypeConverterAttribute(typeof(EthAddressTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(PostageBatchId), new TypeConverterAttribute(typeof(PostageBatchIdTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(SwarmAddress), new TypeConverterAttribute(typeof(SwarmAddressTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(SwarmHash), new TypeConverterAttribute(typeof(SwarmHashTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(SwarmUri), new TypeConverterAttribute(typeof(SwarmUriTypeConverter)));
            TypeDescriptor.AddAttributes(typeof(TagId), new TypeConverterAttribute(typeof(TagIdTypeConverter)));
            
            // Register services.
            services.AddScoped<IChunkService, ChunkService>();
            services.AddScoped<IFeedService, FeedService>();

            return services;
        }
    }
}