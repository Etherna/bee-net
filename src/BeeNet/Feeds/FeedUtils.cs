using System;

namespace Etherna.BeeNet.Feeds
{
    public static class FeedUtils
    {
        // Consts.
        public const int MaxChunkPayloadBytesSize = 4096;
        public const int MaxFeedContentPayloadBytesSize = MaxChunkPayloadBytesSize
                                                        - TimeStampByteSize /*creation timestamp*/;
        public const int TimeStampByteSize = sizeof(ulong);

        // Methods.
        public static byte[] BuildFeedChunkPayload(byte[] contentPayload)
        {
            if (contentPayload.Length > MaxFeedContentPayloadBytesSize)
                throw new ArgumentOutOfRangeException(nameof(contentPayload),
                    $"Content payload can't be longer than {MaxFeedContentPayloadBytesSize}");

            var chunkPayload = new byte[TimeStampByteSize + contentPayload.Length];
            var unixNow = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            UnixDateTimeToByteArray(unixNow).CopyTo(chunkPayload, 0);
            contentPayload.CopyTo(chunkPayload, TimeStampByteSize);

            return chunkPayload;
        }

        public static ulong ByteArrayToUnixDateTime(byte[] dateTimeByteArray)
        {
            if (dateTimeByteArray.Length != TimeStampByteSize)
                throw new ArgumentOutOfRangeException(nameof(dateTimeByteArray), "Invalid date time byte array length");

            var fixedDateTimeByteArray = new byte[dateTimeByteArray.Length];
            Array.Copy(dateTimeByteArray, fixedDateTimeByteArray, fixedDateTimeByteArray.Length);
            if (BitConverter.IsLittleEndian) Array.Reverse(fixedDateTimeByteArray);
            return BitConverter.ToUInt64(fixedDateTimeByteArray, 0);
        }

        public static byte[] GetContentPayloadFromFeedChunkPayload(byte[] feedChunkPayload)
        {
            var contentPayload = new byte[feedChunkPayload.Length - TimeStampByteSize];
            Array.Copy(feedChunkPayload, TimeStampByteSize, contentPayload, 0, contentPayload.Length);
            return contentPayload;
        }

        public static DateTimeOffset GetFeedCreationTimeStamp(byte[] feedChunkPayload)
        {
            var timeStampByteArray = new byte[TimeStampByteSize];
            Array.Copy(feedChunkPayload, timeStampByteArray, TimeStampByteSize);
            var unixTimeStamp = ByteArrayToUnixDateTime(timeStampByteArray);
            return DateTimeOffset.FromUnixTimeSeconds((long)unixTimeStamp);
        }

        public static byte[] UnixDateTimeToByteArray(ulong unixDateTime)
        {
            var byteArrayDateTime = BitConverter.GetBytes(unixDateTime);
            if (BitConverter.IsLittleEndian) Array.Reverse(byteArrayDateTime);
            return byteArrayDateTime;
        }
    }
}
