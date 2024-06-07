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

using Epoche;
using Etherna.BeeNet.Extensions;
using System;
using System.Collections.ObjectModel;

namespace Etherna.BeeNet.Feeds
{
    public sealed class EpochFeedIndex : FeedIndexBase
    {
        // Consts.
        public const byte MaxLevel = 32; //valid from 01/01/1970 to 16/03/2242
        public const ulong MaxUnixTimeStamp = ((ulong)1 << (MaxLevel + 1)) - 1;
        public const ulong MinLevel = 0;
        public const ulong MinUnixTimeStamp = 0;

        // Constructor.
        /// <param name="start">Epoch start in seconds</param>
        /// <param name="level">Epoch level</param>
        public EpochFeedIndex(ulong start, byte level)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(start, (ulong)1 << MaxLevel + 1);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(level, MaxLevel);
#else
            if (start >= (ulong)1 << MaxLevel + 1)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (level > MaxLevel)
                throw new ArgumentOutOfRangeException(nameof(level));
#endif

            //normalize start clearing less relevent bits
            start = start >> level << level;

            Level = level;
            Start = start;
        }

        // Properties.
        public bool IsLeft => (Start & Length) == 0;

        public bool IsRight => !IsLeft;

        public EpochFeedIndex Left => IsLeft ? this : new(Start - Length, Level);

        public EpochFeedIndex Parent
        {
            get
            {
                if (Level == MaxLevel)
                    throw new InvalidOperationException();

                var parentLevel = (byte)(Level + 1);
                var parentStart = Start >> parentLevel << parentLevel;
                return new EpochFeedIndex(parentStart, parentLevel);
            }
        }

        public EpochFeedIndex Right => IsRight ? this : new(Start + Length, Level);

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
        public override ReadOnlyCollection<byte> MarshalBinary
        {
            get
            {
                var epochBytes = Start.UnixDateTimeToByteArray();
                var newArray = new byte[epochBytes.Length + 1];
                epochBytes.CopyTo(newArray, 0);
                newArray[epochBytes.Length] = Level;

                return new ReadOnlyCollection<byte>(Keccak256.ComputeHash(newArray));
            }
        }

        /// <summary>
        /// Epoch start in seconds
        /// </summary>
        public ulong Start { get; }

        // Methods.
        public bool ContainsTime(DateTimeOffset at) =>
            ContainsTime((ulong)at.ToUnixTimeSeconds());

        public bool ContainsTime(ulong at) =>
            at >= Start && at < Start + Length;

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not EpochFeedIndex epochObj) return false;
            return Level == epochObj.Level && Start == epochObj.Start;
        }

        public EpochFeedIndex GetChildAt(DateTimeOffset at) =>
            GetChildAt((ulong)at.ToUnixTimeSeconds());

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

        public override int GetHashCode() =>
            Level.GetHashCode() ^ Start.GetHashCode();

        public override FeedIndexBase GetNext(ulong at)
        {
#if NET8_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfLessThan(at, Start);
#else
            if (at < Start)
                throw new ArgumentOutOfRangeException(nameof(at));
#endif

            return Start + Length > at ?
                GetChildAt(at) :
                LowestCommonAncestor(Start, at).GetChildAt(at);
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
