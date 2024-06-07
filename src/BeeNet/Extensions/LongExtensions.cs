//   Copyright 2021-present Etherna SA
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Buffers.Binary;

namespace Etherna.BeeNet.Extensions
{
    public static class LongExtensions
    {
        public static byte[] UnixDateTimeToByteArray(this long unixDateTime) =>
            ((ulong)unixDateTime).UnixDateTimeToByteArray();
        
        public static byte[] UnixDateTimeToByteArray(this ulong unixDateTime)
        {
            var buffer = new byte[8];
            BinaryPrimitives.WriteUInt64BigEndian(buffer, unixDateTime);
            return buffer;
        }
    }
}
