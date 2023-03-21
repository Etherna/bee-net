using Etherna.BeeNet.Feeds.Models;
using System;

namespace Etherna.BeeNet.Extensions
{
    public static class ByteArrayExtensions
    {
        public static ulong ByteArrayToUnixDateTime(this byte[] dateTimeByteArray)
        {
            if (dateTimeByteArray.Length != FeedChunk.TimeStampByteSize)
                throw new ArgumentOutOfRangeException(nameof(dateTimeByteArray), "Invalid date time byte array length");

            var fixedDateTimeByteArray = new byte[dateTimeByteArray.Length]; //don't reverse original
            Array.Copy(dateTimeByteArray, fixedDateTimeByteArray, fixedDateTimeByteArray.Length);
            if (BitConverter.IsLittleEndian) Array.Reverse(fixedDateTimeByteArray);
            return BitConverter.ToUInt64(fixedDateTimeByteArray, 0);
        }
    }
}
