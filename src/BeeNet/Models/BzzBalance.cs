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

using System;
using System.Globalization;

namespace Etherna.BeeNet.Models
{
    public readonly struct BzzBalance : IEquatable<BzzBalance>
    {
        // Consts.
        public const int DecimalPrecision = 16;
        public static readonly decimal PlursInBzz = (decimal)Math.Pow(10, DecimalPrecision);

        // Fields.
        private readonly decimal balance;

        // Constructors.
        public BzzBalance(decimal balance)
        {
            this.balance = decimal.Round(balance, DecimalPrecision)
                           / 1.000000000000000000000000000000000m; //remove final zeros
        }
        
        // Builders.
        public static BzzBalance FromDecimal(decimal value) => new(value);
        public static BzzBalance FromDouble(double value) => new((decimal)value);
        public static BzzBalance FromInt32(int value) => new(value);
        public static BzzBalance FromPlurLong(long plurValue) => decimal.Divide(plurValue, PlursInBzz);
        public static BzzBalance FromPlurString(string plurValue) =>
            FromPlurLong(long.Parse(plurValue, CultureInfo.InvariantCulture));

        // Methods.
        public int CompareTo(BzzBalance other) => balance.CompareTo(other.balance);
        public override bool Equals(object? obj) =>
            obj is BzzBalance xDaiObj &&
            Equals(xDaiObj);
        public bool Equals(BzzBalance other) => balance == other.balance;
        public override int GetHashCode() => balance.GetHashCode();
        public decimal ToDecimal() => balance;
        public long ToPlurLong() => (long)decimal.Multiply(balance, PlursInBzz);
        public string ToPlurString() => decimal.Multiply(balance, PlursInBzz).ToString("F0", CultureInfo.InvariantCulture);
        public override string ToString() => balance.ToString(CultureInfo.InvariantCulture);

        // Static methods.
        public static BzzBalance Add(BzzBalance left, BzzBalance right) => left + right;
        public static BzzBalance Decrement(BzzBalance balance) => --balance;
        public static decimal Divide(BzzBalance left, BzzBalance right) => left.balance / right.balance;
        public static BzzBalance Divide(BzzBalance left, decimal right) => left.balance / right;
        public static BzzBalance Increment(BzzBalance balance) => ++balance;
        public static BzzBalance Multiply(BzzBalance left, decimal right) => left.balance * right;
        public static BzzBalance Multiply(decimal left, BzzBalance right) => left * right.balance;
        public static BzzBalance Negate(BzzBalance balance) => -balance;
        public static BzzBalance Subtract(BzzBalance left, BzzBalance right) => left - right;

        // Operator methods.
        public static BzzBalance operator +(BzzBalance left, BzzBalance right) => left.balance + right.balance;
        public static BzzBalance operator -(BzzBalance left, BzzBalance right) => left.balance - right.balance;
        public static BzzBalance operator *(BzzBalance left, decimal right) => left.balance * right;
        public static BzzBalance operator *(decimal left, BzzBalance right) => left * right.balance;
        public static decimal operator /(BzzBalance left, BzzBalance right) => left.balance / right.balance;
        public static BzzBalance operator /(BzzBalance left, decimal right) => left.balance / right;

        public static bool operator ==(BzzBalance left, BzzBalance right) => left.Equals(right);
        public static bool operator !=(BzzBalance left, BzzBalance right) => !(left == right);
        public static bool operator >(BzzBalance left, BzzBalance right) => left.balance > right.balance;
        public static bool operator <(BzzBalance left, BzzBalance right) => left.balance < right.balance;
        public static bool operator >=(BzzBalance left, BzzBalance right) => left.balance >= right.balance;
        public static bool operator <=(BzzBalance left, BzzBalance right) => left.balance <= right.balance;
        public static BzzBalance operator -(BzzBalance value) => new(-value.balance);
        public static BzzBalance operator ++(BzzBalance value) => new(value.balance + 1);
        public static BzzBalance operator --(BzzBalance value) => new(value.balance - 1);

        // Implicit conversion operator methods.
        public static implicit operator BzzBalance(decimal value) => new(value);
        public static implicit operator BzzBalance(double value) => new((decimal)value);
        public static implicit operator BzzBalance(int value) => new(value);

        public static explicit operator decimal(BzzBalance value) => value.ToDecimal();
    }
}