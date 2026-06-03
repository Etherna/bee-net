// Copyright 2021-present Etherna SA
// This file is part of SwarmSDK.
//
// SwarmSDK is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
//
// SwarmSDK is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with SwarmSDK.
// If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Etherna.SwarmSdk.Clients.Beehive
{
    // Native AOT support for the NSwag-generated BeehiveGeneratedClient.
    // See BeeGeneratedClient.Aot.cs for the rationale.
    //
    // Note: unlike the Bee client (which streams uploads as raw octet-stream content), this older
    // NSwag generation JSON-serializes the FileParameter wrapper for its upload operations. That
    // pre-existing behavior is preserved here by registering FileParameter; it is not an AOT change.
    [JsonSerializable(typeof(FileParameter))]
    [JsonSerializable(typeof(BeeErrorDto))]
    [JsonSerializable(typeof(BeehivePinDto))]
    [JsonSerializable(typeof(BeePinsDto))]
    [JsonSerializable(typeof(ChainStateDto))]
    [JsonSerializable(typeof(ChunkReferenceDto))]
    [JsonSerializable(typeof(GlobalPostageBatchesDto))]
    [JsonSerializable(typeof(HealthDto))]
    [JsonSerializable(typeof(NodeDto))]
    [JsonSerializable(typeof(PostageBatchBucketsDto))]
    [JsonSerializable(typeof(PostageBatchDto))]
    [JsonSerializable(typeof(PostageBatchIdWithTxHashDto))]
    [JsonSerializable(typeof(PostageBatchStampListDto))]
    [JsonSerializable(typeof(ReadinessDto))]
    [JsonSerializable(typeof(ICollection<BeehivePinDto>))]
    internal sealed partial class BeehiveJsonSerializerContext : JsonSerializerContext;

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance", "CA1852:Seal internal types",
        Justification = "NSwag-generated client is intentionally unsealed (virtual members / partial extension point).")]
    internal partial class BeehiveGeneratedClient
    {
        static partial void UpdateJsonSerializerSettings(System.Text.Json.JsonSerializerOptions settings) =>
            settings.TypeInfoResolverChain.Add(BeehiveJsonSerializerContext.Default);

        // Helpers.
        // Serializes a request body through the source-generated context (AOT-safe). Resolves the
        // JsonTypeInfo from the body's static type T, so a null body serializes as "null" exactly like
        // the original generated SerializeToUtf8Bytes<T>(body, options) call did.
        private byte[] SerializeBody<T>(T body) =>
            System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(
                body, (System.Text.Json.Serialization.Metadata.JsonTypeInfo<T>)JsonSerializerSettings.GetTypeInfo(typeof(T)));
    }
}
