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

using Etherna.BeeNet.Extensions;
using Etherna.BeeNet.Hashing;
using System;

namespace Etherna.BeeNet.Models
{
    public sealed class SwarmEpochFeedIndex : SwarmFeedIndexBase
    {
        // Consts.
        public const byte MaxLevel = 32; //valid from 01/01/1970 to 16/03/2242
        public const ulong MaxUnixTimeStamp = ((ulong)1 << (MaxLevel + 1)) - 1;
        public const ulong MinLevel = 0;
        public const ulong MinUnixTimeStamp = 0;

        // Fields.
        private readonly IHasher hasher;
        
        // Constructor.
        /// <param name="start">Epoch start in seconds</param>
        /// <param name="level">Epoch level</param>
        /// <param name="hasher">The hash provider</param>
        public SwarmEpochFeedIndex(ulong start, byte level, IHasher hasher)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(start, (ulong)1 << MaxLevel + 1);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(level, MaxLevel);

            //normalize start clearing less relevant bits
            start = start >> level << level;

            this.hasher = hasher;
            Level = level;
            Start = start;
        }

        // Properties.
        public bool IsLeft => (Start & Length) == 0;

        public bool IsRight => !IsLeft;

        public SwarmEpochFeedIndex Left => IsLeft ? this : new(Start - Length, Level, hasher);

        public SwarmEpochFeedIndex Parent
        {
            get
            {
                if (Level == MaxLevel)
                    throw new InvalidOperationException();

                var parentLevel = (byte)(Level + 1);
                var parentStart = Start >> parentLevel << parentLevel;
                return new SwarmEpochFeedIndex(parentStart, parentLevel, hasher);
            }
        }

        public SwarmEpochFeedIndex Right => IsRight ? this : new(Start + Length, Level, hasher);

        /// <summary>
        /// Epoch length in seconds
        /// </summary>
        public ulong Length => (ulong)1 << Level;

        /// <summary>
        /// Epoch level
        /// </summary>
        public byte Level { get; }

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
            if (obj is not SwarmEpochFeedIndex epochObj) return false;
            return Level == epochObj.Level && Start == epochObj.Start;
        }

        public SwarmEpochFeedIndex GetChildAt(DateTimeOffset at) =>
            GetChildAt((ulong)at.ToUnixTimeSeconds());

        public SwarmEpochFeedIndex GetChildAt(ulong at)
        {
            if (Level == 0)
                throw new InvalidOperationException();
            if (at < Start || at >= Start + Length)
                throw new ArgumentOutOfRangeException(nameof(at));

            var childStart = Start;
            var childLength = Length >> 1;

            if ((at & childLength) > 0)
                childStart |= childLength;

            return new SwarmEpochFeedIndex(childStart, (byte)(Level - 1), hasher);
        }

        public override int GetHashCode() =>
            Level.GetHashCode() ^ Start.GetHashCode();

        /// <summary>
        /// Index representation as keccak256 hash
        /// </summary>
        public override Memory<byte> MarshalBinary()
        {
            var epochBytes = Start.UnixDateTimeToByteArray();
            var newArray = new byte[epochBytes.Length + 1];
            epochBytes.CopyTo(newArray, 0);
            newArray[epochBytes.Length] = Level;

            return hasher.ComputeHash(newArray);
        }

        public override SwarmFeedIndexBase GetNext(ulong at)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(at, Start);

            return Start + Length > at ?
                GetChildAt(at) :
                LowestCommonAncestor(Start, at, hasher).GetChildAt(at);
        }

        public override string ToString() => $"{Start}/{Level}";

        // Static methods.
        /// <summary>
        /// Calculates the lowest common ancestor epoch given two unix times
        /// </summary>
        /// <returns>Lowest common ancestor epoch index</returns>
        public static SwarmEpochFeedIndex LowestCommonAncestor(ulong t0, ulong t1, IHasher hasher)
        {
            byte level = 0;
            while (t0 >> level != t1 >> level)
            {
                level++;
                if (level > MaxLevel)
                    throw new InvalidOperationException();
            }
            var start = t1 >> level << level;
            return new(start, level, hasher);
        }
    }
}
