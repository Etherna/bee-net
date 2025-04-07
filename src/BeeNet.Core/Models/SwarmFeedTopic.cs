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

using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using System;

namespace Etherna.BeeNet.Models
{
    public readonly struct SwarmFeedTopic : IEquatable<SwarmFeedTopic>
    {
        // Consts.
        public const int TopicSize = SwarmHash.HashSize;
        
        // Fields.
        private readonly ReadOnlyMemory<byte> byteTopic;

        // Constructors.
        public SwarmFeedTopic(ReadOnlyMemory<byte> topic)
        {
            if (!IsValidTopic(topic))
                throw new ArgumentOutOfRangeException(nameof(topic));
            
            byteTopic = topic;
        }

        public SwarmFeedTopic(string topic)
        {
            ArgumentNullException.ThrowIfNull(topic, nameof(topic));
            
            try
            {
                byteTopic = topic.HexToByteArray();
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid hex", nameof(topic));
            }
            
            if (!IsValidTopic(byteTopic))
                throw new ArgumentOutOfRangeException(nameof(topic));
        }

        // Methods.
        public bool Equals(SwarmFeedTopic other) => byteTopic.Span.SequenceEqual(other.byteTopic.Span);
        public override bool Equals(object? obj) => obj is SwarmFeedTopic other && Equals(other);
        public override int GetHashCode() => ByteArrayComparer.Current.GetHashCode(byteTopic.ToArray());
        public byte[] ToByteArray() => byteTopic.ToArray();
        public ReadOnlyMemory<byte> ToReadOnlyMemory() => byteTopic;
        public override string ToString() => byteTopic.ToArray().ToHex();
        
        // Static methods.
        public static SwarmFeedTopic FromByteArray(byte[] value) => new(value);
        public static SwarmFeedTopic FromString(string value) => new(value);
        public static bool IsValidTopic(ReadOnlyMemory<byte> value) => value.Length == TopicSize;
        public static bool IsValidTopic(string value)
        {
            try
            {
                return IsValidTopic(value.HexToByteArray());
            }
            catch (FormatException)
            {
                return false;
            }
        }
        
        // Operator methods.
        public static bool operator ==(SwarmFeedTopic left, SwarmFeedTopic right) => left.Equals(right);
        public static bool operator !=(SwarmFeedTopic left, SwarmFeedTopic right) => !(left == right);
        
        // Implicit conversion operator methods.
        public static implicit operator SwarmFeedTopic(string value) => new(value);
        public static implicit operator SwarmFeedTopic(byte[] value) => new(value);
        
        // Explicit conversion operator methods.
        public static explicit operator string(SwarmFeedTopic value) => value.ToString();
        public static explicit operator ReadOnlyMemory<byte>(SwarmFeedTopic value) => value.ToReadOnlyMemory();
        public static explicit operator byte[](SwarmFeedTopic value) => value.ToByteArray();
    }
}