// Copyright 2021-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Etherna.BeeNet.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Etherna.BeeNet.JsonConverters
{
    public class BzzBalanceJsonConverter : JsonConverter<BzzBalance>
    {
        public override BzzBalance Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.Number)
                throw new JsonException();

            var decimalValue = reader.GetDecimal();

            return new BzzBalance(decimalValue);
        }

        public override void Write(
            Utf8JsonWriter writer,
            BzzBalance value,
            JsonSerializerOptions options)
        {
            ArgumentNullException.ThrowIfNull(writer, nameof(writer));
            writer.WriteNumberValue(value.ToDecimal());
        }
    }
}