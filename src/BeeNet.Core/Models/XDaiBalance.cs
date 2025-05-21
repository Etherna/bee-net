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
    [TypeConverter(typeof(XDaiBalanceTypeConverter))]
    public readonly struct XDaiBalance : IEquatable<XDaiBalance>
    {
        // Consts.
        public const int DecimalPrecision = 18;
        public static readonly decimal WeisInXDai = (decimal)Math.Pow(10, DecimalPrecision);

        // Fields.
        private readonly decimal balance;

        // Constructors.
        public XDaiBalance(decimal balance)
        {
            this.balance = decimal.Round(balance, DecimalPrecision)
                           / 1.000000000000000000000000000000000m; //remove final zeros
        }

        // Builders.
        public static XDaiBalance FromDecimal(decimal value) => new(value);
        public static XDaiBalance FromDouble(double value) => new((decimal)value);
        public static XDaiBalance FromInt32(int value) => new(value);
        public static XDaiBalance FromWeiLong(long weiValue) => decimal.Divide(weiValue, WeisInXDai);
        public static XDaiBalance FromWeiString(string weiValue) =>
            FromWeiLong(long.Parse(weiValue, CultureInfo.InvariantCulture));

        // Methods.
        public int CompareTo(XDaiBalance other) => balance.CompareTo(other.balance);
        public override bool Equals(object? obj) =>
            obj is XDaiBalance xDaiObj &&
            Equals(xDaiObj);
        public bool Equals(XDaiBalance other) => balance == other.balance;
        public override int GetHashCode() => balance.GetHashCode();
        public decimal ToDecimal() => balance;
        public override string ToString() => balance.ToString(CultureInfo.InvariantCulture);
        public long ToWeiLong()=> (long)decimal.Multiply(balance, WeisInXDai);
        public string ToWeiString() => decimal.Multiply(balance, WeisInXDai).ToString("F0", CultureInfo.InvariantCulture);

        // Static methods.
        public static XDaiBalance Add(XDaiBalance left, XDaiBalance right) => left + right;
        public static XDaiBalance Decrement(XDaiBalance balance) => --balance;
        public static decimal Divide(XDaiBalance left, XDaiBalance right) => left.balance / right.balance;
        public static XDaiBalance Divide(XDaiBalance left, decimal right) => left.balance / right;
        public static XDaiBalance Increment(XDaiBalance balance) => ++balance;
        public static XDaiBalance Multiply(XDaiBalance left, decimal right) => left.balance * right;
        public static XDaiBalance Multiply(decimal left, XDaiBalance right) => left * right.balance;
        public static XDaiBalance Negate(XDaiBalance balance) => -balance;
        public static XDaiBalance Subtract(XDaiBalance left, XDaiBalance right) => left - right;

        // Operator methods.
        public static XDaiBalance operator +(XDaiBalance left, XDaiBalance right) => left.balance + right.balance;
        public static XDaiBalance operator -(XDaiBalance left, XDaiBalance right) => left.balance - right.balance;
        public static XDaiBalance operator *(XDaiBalance left, decimal right) => left.balance * right;
        public static XDaiBalance operator *(decimal left, XDaiBalance right) => left * right.balance;
        public static decimal operator /(XDaiBalance left, XDaiBalance right) => left.balance / right.balance;
        public static XDaiBalance operator /(XDaiBalance left, decimal right) => left.balance / right;

        public static bool operator ==(XDaiBalance left, XDaiBalance right) => left.Equals(right);
        public static bool operator !=(XDaiBalance left, XDaiBalance right) => !(left == right);
        public static bool operator >(XDaiBalance left, XDaiBalance right) => left.balance > right.balance;
        public static bool operator <(XDaiBalance left, XDaiBalance right) => left.balance < right.balance;
        public static bool operator >=(XDaiBalance left, XDaiBalance right) => left.balance >= right.balance;
        public static bool operator <=(XDaiBalance left, XDaiBalance right) => left.balance <= right.balance;
        public static XDaiBalance operator -(XDaiBalance value) => new(-value.balance);
        public static XDaiBalance operator ++(XDaiBalance value) => new(value.balance + 1);
        public static XDaiBalance operator --(XDaiBalance value) => new(value.balance - 1);

        // Implicit conversion operator methods.
        public static implicit operator XDaiBalance(decimal value) => new(value);
        public static implicit operator XDaiBalance(double value) => new((decimal)value);
        public static implicit operator XDaiBalance(int value) => new(value);

        public static explicit operator decimal(XDaiBalance value) => value.ToDecimal();
    }
}