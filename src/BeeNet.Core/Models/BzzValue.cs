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

using Etherna.BeeNet.TypeConverters;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Etherna.BeeNet.Models
{
    [TypeConverter(typeof(BzzValueTypeConverter))]
    public readonly struct BzzValue(decimal value) : IEquatable<BzzValue>
    {
        // Consts.
        public const int DecimalPrecision = 16;
        public static readonly decimal PlursInBzz = (decimal)Math.Pow(10, DecimalPrecision);

        // Fields.
        private readonly decimal value = decimal.Round(value, DecimalPrecision)
                                         / 1.000000000000000000000000000000000m; //remove final zeros

        // Builders.
        public static BzzValue FromDecimal(decimal value) => new(value);
        public static BzzValue FromDouble(double value) => new((decimal)value);
        public static BzzValue FromInt32(int value) => new(value);
        public static BzzValue FromInt64(long value) => new(value);
        public static BzzValue FromPlurLong(long plurValue) => decimal.Divide(plurValue, PlursInBzz);
        public static BzzValue FromPlurString(string plurValue) =>
            FromPlurLong(long.Parse(plurValue, CultureInfo.InvariantCulture));

        // Methods.
        public int CompareTo(BzzValue other) => value.CompareTo(other.value);
        public override bool Equals(object? obj) =>
            obj is BzzValue xDaiObj &&
            Equals(xDaiObj);
        public bool Equals(BzzValue other) => value == other.value;
        public override int GetHashCode() => value.GetHashCode();
        public decimal ToDecimal() => value;
        public long ToPlurLong() => (long)decimal.Multiply(value, PlursInBzz);
        public string ToPlurString() => decimal.Multiply(value, PlursInBzz).ToString("F0", CultureInfo.InvariantCulture);
        public override string ToString() => value.ToString(CultureInfo.InvariantCulture);

        // Static methods.
        public static BzzValue Add(BzzValue left, BzzValue right) => left + right;
        public static BzzValue Decrement(BzzValue value) => --value;
        public static decimal Divide(BzzValue left, BzzValue right) => left.value / right.value;
        public static BzzValue Divide(BzzValue left, decimal right) => left.value / right;
        public static BzzValue Divide(BzzValue left, double right) => left.value / (decimal)right;
        public static BzzValue Divide(BzzValue left, int right) => left.value / right;
        public static BzzValue Divide(BzzValue left, long right) => left.value / right;
        public static BzzValue Increment(BzzValue value) => ++value;
        public static BzzValue Multiply(BzzValue left, decimal right) => left.value * right;
        public static BzzValue Multiply(BzzValue left, double right) => left.value * (decimal)right;
        public static BzzValue Multiply(BzzValue left, int right) => left.value * right;
        public static BzzValue Multiply(BzzValue left, long right) => left.value * right;
        public static BzzValue Multiply(decimal left, BzzValue right) => left * right.value;
        public static BzzValue Multiply(double left, BzzValue right) => (decimal)left * right.value;
        public static BzzValue Multiply(int left, BzzValue right) => left * right.value;
        public static BzzValue Multiply(long left, BzzValue right) => left * right.value;
        public static BzzValue Negate(BzzValue value) => -value;
        public static BzzValue Subtract(BzzValue left, BzzValue right) => left - right;

        // Operator methods.
        public static BzzValue operator +(BzzValue left, BzzValue right) => left.value + right.value;
        public static BzzValue operator -(BzzValue left, BzzValue right) => left.value - right.value;
        public static BzzValue operator *(BzzValue left, decimal right) => left.value * right;
        public static BzzValue operator *(BzzValue left, double right) => left.value * (decimal)right;
        public static BzzValue operator *(BzzValue left, int right) => left.value * right;
        public static BzzValue operator *(BzzValue left, long right) => left.value * right;
        public static BzzValue operator *(decimal left, BzzValue right) => left * right.value;
        public static BzzValue operator *(double left, BzzValue right) => (decimal)left * right.value;
        public static BzzValue operator *(int left, BzzValue right) => left * right.value;
        public static BzzValue operator *(long left, BzzValue right) => left * right.value;
        public static decimal operator /(BzzValue left, BzzValue right) => left.value / right.value;
        public static BzzValue operator /(BzzValue left, decimal right) => left.value / right;
        public static BzzValue operator /(BzzValue left, double right) => left.value / (decimal)right;
        public static BzzValue operator /(BzzValue left, int right) => left.value / right;
        public static BzzValue operator /(BzzValue left, long right) => left.value / right;

        public static bool operator ==(BzzValue left, BzzValue right) => left.Equals(right);
        public static bool operator !=(BzzValue left, BzzValue right) => !(left == right);
        public static bool operator >(BzzValue left, BzzValue right) => left.value > right.value;
        public static bool operator <(BzzValue left, BzzValue right) => left.value < right.value;
        public static bool operator >=(BzzValue left, BzzValue right) => left.value >= right.value;
        public static bool operator <=(BzzValue left, BzzValue right) => left.value <= right.value;
        public static BzzValue operator -(BzzValue value) => new(-value.value);
        public static BzzValue operator ++(BzzValue value) => new(value.value + 1);
        public static BzzValue operator --(BzzValue value) => new(value.value - 1);

        // Implicit conversion operator methods.
        public static implicit operator BzzValue(decimal value) => new(value);
        public static implicit operator BzzValue(double value) => new((decimal)value);
        public static implicit operator BzzValue(int value) => new(value);
        public static implicit operator BzzValue(long value) => new(value);

        public static explicit operator decimal(BzzValue value) => value.ToDecimal();
    }
}