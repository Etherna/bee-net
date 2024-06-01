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

using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Util;
using System;

namespace Etherna.BeeNet.Models
{
    public readonly struct PostageBatchId : IEquatable<PostageBatchId>
    {
        // Consts.
        public const int BatchIdSize = 32;
        
        // Fields.
        private readonly byte[] byteId;

        // Constructors.
        public PostageBatchId(byte[] batchId)
        {
            ArgumentNullException.ThrowIfNull(batchId, nameof(batchId));
            if (batchId.Length != BatchIdSize)
                throw new ArgumentOutOfRangeException(nameof(batchId));
            
            byteId = batchId;
        }
        
        public PostageBatchId(string batchId)
        {
            ArgumentNullException.ThrowIfNull(batchId, nameof(batchId));
            
            byteId = batchId.HexToByteArray();
            
            if (byteId.Length != BatchIdSize)
                throw new ArgumentOutOfRangeException(nameof(batchId));
        }
        
        // Static properties.
        public static PostageBatchId Zero { get; } = new byte[BatchIdSize];

        // Methods.
        public bool Equals(PostageBatchId other) => ByteArrayComparer.Current.Equals(byteId, other.byteId);
        public override bool Equals(object? obj) => obj is PostageBatchId other && Equals(other);
        public override int GetHashCode() => ByteArrayComparer.Current.GetHashCode(byteId);
        public byte[] ToByteArray() => (byte[])byteId.Clone();
        public ReadOnlyMemory<byte> ToReadOnlyMemory() => byteId;
        public override string ToString() => byteId.ToHex();
        
        // Static methods.
        public static PostageBatchId FromByteArray(byte[] value) => new(value);
        public static PostageBatchId FromString(string value) => new(value);
        
        // Operator methods.
        public static bool operator ==(PostageBatchId left, PostageBatchId right) => left.Equals(right);
        public static bool operator !=(PostageBatchId left, PostageBatchId right) => !(left == right);
        
        // Implicit conversion operator methods.
        public static implicit operator PostageBatchId(string value) => new(value);
        public static implicit operator PostageBatchId(byte[] value) => new(value);
        
        public static explicit operator byte[](PostageBatchId value) => value.ToByteArray();
        public static explicit operator ReadOnlyMemory<byte>(PostageBatchId value) => value.ToReadOnlyMemory();
        public static explicit operator string(PostageBatchId value) => value.ToString();
    }
}