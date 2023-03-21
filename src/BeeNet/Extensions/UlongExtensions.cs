using System;

namespace Etherna.BeeNet.Extensions
{
    public static class UlongExtensions
    {
        public static byte[] UnixDateTimeToByteArray(this ulong unixDateTime)
        {
            var byteArrayDateTime = BitConverter.GetBytes(unixDateTime);
            if (BitConverter.IsLittleEndian) Array.Reverse(byteArrayDateTime);
            return byteArrayDateTime;
        }
    }
}
