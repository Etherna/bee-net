using Epoche;
using System;

namespace Etherna.BeeNet.Feeds
{
    public class EpochIndex : IFeedIndex
    {
        // Consts.
        public const byte MaxLevel = 32; //valid from 01/01/1970 to 16/03/2242

        // Constructor.
        /// <param name="start">Epoch start in seconds</param>
        /// <param name="level">Epoch level</param>
        public EpochIndex(ulong start, byte level)
        {
            if (start >= (ulong)1 << MaxLevel + 1)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (level > MaxLevel)
                throw new ArgumentOutOfRangeException(nameof(level));

            //normalize start clearing less relevent bits
            start = start >> level << level;

            Level = level;
            Start = start;
        }

        // Properties.
        public bool IsLeft => (Start & Length) == 0;

        public EpochIndex Left => IsLeft ? this : new(Start - Length, Level);

        public EpochIndex Right => !IsLeft ? this : new(Start + Length, Level);

        /// <summary>
        /// Epoch length in seconds
        /// </summary>
        public ulong Length => (ulong)1 << Level;

        /// <summary>
        /// Epoch level
        /// </summary>
        public byte Level { get; }

        /// <summary>
        /// Index represenentation as keccak256 hash
        /// </summary>
        public byte[] MarshalBinary
        {
            get
            {
                var epochBytes = BitConverter.GetBytes(Start);
                if (BitConverter.IsLittleEndian) Array.Reverse(epochBytes);

                var newArray = new byte[epochBytes.Length + 1];
                epochBytes.CopyTo(newArray, 0);
                newArray[epochBytes.Length] = Level;

                return Keccak256.ComputeHash(newArray);
            }
        }

        /// <summary>
        /// Epoch start in seconds
        /// </summary>
        public ulong Start { get; }

        // Methods.
        public EpochIndex GetChildAt(ulong at)
        {
            if (Level == 0)
                throw new InvalidOperationException();
            if (at < Start || at >= Start + Length)
                throw new ArgumentOutOfRangeException(nameof(at));

            var childStart = Start;
            var childLength = Length >> 1;

            if ((at & childLength) > 0)
                childStart |= childLength;

            return new EpochIndex(childStart, (byte)(Level - 1));
        }

        public IFeedIndex GetNext(ulong at)
        {
            if (at < Start)
                throw new ArgumentOutOfRangeException(nameof(at));

            return Start + Length > at ?
                GetChildAt(at) :
                LowestCommonAncestor(Start, at).GetChildAt(at);
        }

        public EpochIndex GetParent()
        {
            if (Level == MaxLevel)
                throw new InvalidOperationException();

            var parentLevel = (byte)(Level + 1);
            var parentStart = Start >> parentLevel << parentLevel;
            return new EpochIndex(parentStart, parentLevel);
        }

        public override string ToString() => $"{Start}/{Level}";

        // Static methods.
        /// <summary>
        /// Calculates the lowest common ancestor epoch given two unix times
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns>Lowest common ancestor epoch index</returns>
        public static EpochIndex LowestCommonAncestor(ulong t0, ulong t1)
        {
            byte level = 0;
            while (t0 >> level != t1 >> level)
            {
                level++;
                if (level > MaxLevel)
                    throw new InvalidOperationException();
            }
            var start = t1 >> level << level;
            return new(start, level);
        }
    }
}
