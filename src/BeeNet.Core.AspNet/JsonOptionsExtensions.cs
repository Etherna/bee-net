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

using Etherna.BeeNet.JsonConverters;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Etherna.BeeNet.AspNet
{
    public static class JsonOptionsExtensions
    {
        public static void AddBeeNetJsonConverters(
            this JsonOptions options,
            bool writeBalancesAsString = true)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));
            
            options.JsonSerializerOptions.Converters.Add(new BzzBalanceJsonConverter(writeBalancesAsString));
            options.JsonSerializerOptions.Converters.Add(new EncryptionKey256JsonConverter());
            options.JsonSerializerOptions.Converters.Add(new EthAddressJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new EthTxHashJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new PostageBatchIdJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new PostageStampJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new SwarmAddressJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new SwarmFeedTopicJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new SwarmHashJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new SwarmOverlayAddressJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new SwarmReferenceJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new SwarmSocIdentifierJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new SwarmSocSignatureJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new SwarmUriJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new TagIdJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new XDaiBalanceJsonConverter(writeBalancesAsString));
        }
    }
}