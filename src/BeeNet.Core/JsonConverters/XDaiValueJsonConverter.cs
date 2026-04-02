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

using Etherna.BeeNet.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Etherna.BeeNet.JsonConverters
{
    public sealed class XDaiValueJsonConverter(NumericFormat valueFormat)
        : JsonConverter<XDaiValue>
    {
        public override XDaiValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            reader.TokenType switch
            {
                JsonTokenType.Number when valueFormat == NumericFormat.AsFloat =>
                    XDaiValue.FromDouble(reader.GetDouble()),
                JsonTokenType.Number when valueFormat == NumericFormat.AsInteger =>
                    XDaiValue.FromWeiLong(reader.GetInt64()),
                JsonTokenType.String => XDaiValue.FromWeiString(reader.GetString()!),
                _ => throw new JsonException()
            };

        public override void Write(Utf8JsonWriter writer, XDaiValue value, JsonSerializerOptions options)
        {
            ArgumentNullException.ThrowIfNull(writer);
            switch (valueFormat)
            {
                case NumericFormat.AsFloat:
                    writer.WriteNumberValue(value.ToDouble());
                    break;
                case NumericFormat.AsInteger:
                    writer.WriteNumberValue(value.ToWeiLong());
                    break;
                case NumericFormat.AsString:
                    writer.WriteStringValue(value.ToWeiString());
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported format: {valueFormat}");
            }
        }
    }
}