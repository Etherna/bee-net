// Copyright 2021-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Globalization;

namespace Etherna.BeeNet.Models
{
    public readonly struct BzzBalance : IEquatable<BzzBalance>
    {
        // Consts.
        public const int DecimalPrecision = 16;

        // Fields.
        private readonly decimal _balance;

        // Constructors.
        public BzzBalance(decimal balance)
        {
            _balance = decimal.Round(balance, DecimalPrecision)
                       / 1.000000000000000000000000000000000m; //remove final zeros
        }
        
        // Builders.
        public static BzzBalance FromDecimal(decimal value) => new(value);
        public static BzzBalance FromDouble(double value) => new((decimal)value);
        public static BzzBalance FromInt32(int value) => new(value);
        public static BzzBalance FromPlurLong(long plurValue) =>
            decimal.Divide(plurValue, (decimal)Math.Pow(10, DecimalPrecision));
        public static BzzBalance FromPlurString(string plurValue) =>
            FromPlurLong(long.Parse(plurValue, CultureInfo.InvariantCulture));
        

        // Methods.
        public int CompareTo(BzzBalance other) => _balance.CompareTo(other._balance);
        public override bool Equals(object? obj) =>
            obj is BzzBalance xDaiObj &&
            Equals(xDaiObj);
        public bool Equals(BzzBalance other) => _balance == other._balance;
        public override int GetHashCode() => _balance.GetHashCode();
        public decimal ToDecimal() => _balance;
        public override string ToString() => _balance.ToString(CultureInfo.InvariantCulture);

        // Static methods.
        public static BzzBalance Add(BzzBalance left, BzzBalance right) => left + right;
        public static BzzBalance Decrement(BzzBalance balance) => --balance;
        public static BzzBalance Divide(BzzBalance left, BzzBalance right) => left / right;
        public static BzzBalance Increment(BzzBalance balance) => ++balance;
        public static BzzBalance Multiply(BzzBalance left, BzzBalance right) => left * right;
        public static BzzBalance Negate(BzzBalance balance) => -balance;
        public static BzzBalance Subtract(BzzBalance left, BzzBalance right) => left - right;

        // Operator methods.
        public static BzzBalance operator +(BzzBalance left, BzzBalance right) =>
            new(left._balance + right._balance);

        public static BzzBalance operator -(BzzBalance left, BzzBalance right) =>
            new(left._balance - right._balance);

        public static BzzBalance operator *(BzzBalance left, BzzBalance right) =>
            new(left._balance * right._balance);

        public static BzzBalance operator /(BzzBalance left, BzzBalance right) =>
            new(left._balance / right._balance);

        public static bool operator ==(BzzBalance left, BzzBalance right) => left.Equals(right);
        public static bool operator !=(BzzBalance left, BzzBalance right) => !(left == right);
        public static bool operator >(BzzBalance left, BzzBalance right) => left._balance > right._balance;
        public static bool operator <(BzzBalance left, BzzBalance right) => left._balance < right._balance;
        public static bool operator >=(BzzBalance left, BzzBalance right) => left._balance >= right._balance;
        public static bool operator <=(BzzBalance left, BzzBalance right) => left._balance <= right._balance;
        public static BzzBalance operator -(BzzBalance value) => new(-value._balance);
        public static BzzBalance operator ++(BzzBalance value) => new(value._balance + 1);
        public static BzzBalance operator --(BzzBalance value) => new(value._balance - 1);

        // Implicit conversion operator methods.
        public static implicit operator BzzBalance(decimal value) => new(value);
        public static implicit operator BzzBalance(double value) => new((decimal)value);
        public static implicit operator BzzBalance(int value) => new(value);

        public static explicit operator decimal(BzzBalance value) => value.ToDecimal();
    }
}