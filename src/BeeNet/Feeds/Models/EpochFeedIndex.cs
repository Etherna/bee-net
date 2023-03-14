﻿using Epoche;
using Etherna.BeeNet.Extensions;
using System;

namespace Etherna.BeeNet.Feeds.Models
{
    public class EpochFeedIndex : FeedIndexBase
    {
        // Consts.
        public const byte MaxLevel = 32; //valid from 01/01/1970 to 16/03/2242

        // Constructor.
        /// <param name="start">Epoch start in seconds</param>
        /// <param name="level">Epoch level</param>
        public EpochFeedIndex(ulong start, byte level)
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

        public EpochFeedIndex Left => IsLeft ? this : new(Start - Length, Level);

        public EpochFeedIndex Right => !IsLeft ? this : new(Start + Length, Level);

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
        public override byte[] MarshalBinary
        {
            get
            {
                var epochBytes = Start.UnixDateTimeToByteArray();
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
        public bool ContainsTime(ulong at) =>
            at >= Start && at < Start + Length;

        public EpochFeedIndex GetChildAt(ulong at)
        {
            if (Level == 0)
                throw new InvalidOperationException();
            if (at < Start || at >= Start + Length)
                throw new ArgumentOutOfRangeException(nameof(at));

            var childStart = Start;
            var childLength = Length >> 1;

            if ((at & childLength) > 0)
                childStart |= childLength;

            return new EpochFeedIndex(childStart, (byte)(Level - 1));
        }

        public override FeedIndexBase GetNext(ulong at)
        {
            if (at < Start)
                throw new ArgumentOutOfRangeException(nameof(at));

            return Start + Length > at ?
                GetChildAt(at) :
                LowestCommonAncestor(Start, at).GetChildAt(at);
        }

        public EpochFeedIndex GetParent()
        {
            if (Level == MaxLevel)
                throw new InvalidOperationException();

            var parentLevel = (byte)(Level + 1);
            var parentStart = Start >> parentLevel << parentLevel;
            return new EpochFeedIndex(parentStart, parentLevel);
        }

        public override string ToString() => $"{Start}/{Level}";

        // Static methods.
        /// <summary>
        /// Calculates the lowest common ancestor epoch given two unix times
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <returns>Lowest common ancestor epoch index</returns>
        public static EpochFeedIndex LowestCommonAncestor(ulong t0, ulong t1)
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
