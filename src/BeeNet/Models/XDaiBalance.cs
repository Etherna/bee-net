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
    public struct XDaiBalance : IEquatable<XDaiBalance>
    {
        // Consts.
        public const int DecimalPrecision = 18;

        // Fields.
        private readonly decimal _balance;

        // Constructors.
        public XDaiBalance(decimal balance)
        {
            _balance = decimal.Round(balance, DecimalPrecision)
                       / 1.000000000000000000000000000000000m; //remove final zeros
        }

        // Methods.
        public int CompareTo(XDaiBalance other) => _balance.CompareTo(other._balance);

        public override bool Equals(object? obj) =>
            obj is XDaiBalance xDaiObj &&
            Equals(xDaiObj);

        public bool Equals(XDaiBalance other) => _balance == other._balance;
        public override int GetHashCode() => _balance.GetHashCode();
        public decimal ToDecimal() => _balance;
        public override string ToString() => _balance.ToString(CultureInfo.InvariantCulture);

        // Static methods.
        public static XDaiBalance Add(XDaiBalance left, XDaiBalance right) => left + right;
        public static XDaiBalance Decrement(XDaiBalance balance) => --balance;
        public static XDaiBalance Divide(XDaiBalance left, XDaiBalance right) => left / right;
        public static XDaiBalance FromDecimal(decimal value) => new(value);
        public static XDaiBalance FromDouble(double value) => new((decimal)value);
        public static XDaiBalance FromInt32(int value) => new(value);
        public static XDaiBalance Increment(XDaiBalance balance) => ++balance;
        public static XDaiBalance Multiply(XDaiBalance left, XDaiBalance right) => left * right;
        public static XDaiBalance Negate(XDaiBalance balance) => -balance;
        public static XDaiBalance Subtract(XDaiBalance left, XDaiBalance right) => left - right;

        // Operator methods.
        public static XDaiBalance operator +(XDaiBalance left, XDaiBalance right) =>
            new(left._balance + right._balance);

        public static XDaiBalance operator -(XDaiBalance left, XDaiBalance right) =>
            new(left._balance - right._balance);

        public static XDaiBalance operator *(XDaiBalance left, XDaiBalance right) =>
            new(left._balance * right._balance);

        public static XDaiBalance operator /(XDaiBalance left, XDaiBalance right) =>
            new(left._balance / right._balance);

        public static bool operator ==(XDaiBalance left, XDaiBalance right) => left.Equals(right);
        public static bool operator !=(XDaiBalance left, XDaiBalance right) => !(left == right);
        public static bool operator >(XDaiBalance left, XDaiBalance right) => left._balance > right._balance;
        public static bool operator <(XDaiBalance left, XDaiBalance right) => left._balance < right._balance;
        public static bool operator >=(XDaiBalance left, XDaiBalance right) => left._balance >= right._balance;
        public static bool operator <=(XDaiBalance left, XDaiBalance right) => left._balance <= right._balance;
        public static XDaiBalance operator -(XDaiBalance value) => new(-value._balance);
        public static XDaiBalance operator ++(XDaiBalance value) => new(value._balance + 1);
        public static XDaiBalance operator --(XDaiBalance value) => new(value._balance - 1);

        // Implicit conversion operator methods.
        public static implicit operator XDaiBalance(decimal value) => new(value);
        public static implicit operator XDaiBalance(double value) => new((decimal)value);
        public static implicit operator XDaiBalance(int value) => new(value);

        public static explicit operator decimal(XDaiBalance value) => value.ToDecimal();
    }
}