// Copyright 2021-present Etherna SA
// This file is part of SwarmSDK.
//
// SwarmSDK is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
//
// SwarmSDK is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with SwarmSDK.
// If not, see <https://www.gnu.org/licenses/>.

using System;

namespace Etherna.SwarmSdk.Extensions
{
    public static class HexExtensions
    {
#if !NET9_0_OR_GREATER
        private const string LowerHexAlphabet = "0123456789abcdef";
#endif

        public static bool HasHexPrefix(this string value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return value.StartsWith("0x", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parse an hex string (with or without "0x" prefix) into its bytes.
        /// </summary>
        /// <exception cref="FormatException">Thrown when the string is not valid hex.</exception>
        public static byte[] HexToByteArray(this string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var hex = value.RemoveHexPrefix();
            if (hex.Length % 2 != 0)
                hex = "0" + hex;
            return Convert.FromHexString(hex);
        }

        public static bool IsHex(this string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var hex = value.RemoveHexPrefix();
            if (hex.Length == 0)
                return false;
            foreach (var c in hex)
                if (!Uri.IsHexDigit(c))
                    return false;
            return true;
        }

        public static string RemoveHexPrefix(this string value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return value.HasHexPrefix() ? value[2..] : value;
        }

        public static string ToHex(this byte[] value, bool prefix = false)
        {
            ArgumentNullException.ThrowIfNull(value);
            return ToHex((ReadOnlySpan<byte>)value, prefix);
        }

        public static string ToHex(this ReadOnlySpan<byte> value, bool prefix = false)
        {
#if NET9_0_OR_GREATER
            var hex = Convert.ToHexStringLower(value);
            return prefix ? "0x" + hex : hex;
#else
            var offset = prefix ? 2 : 0;
            var chars = new char[offset + (value.Length * 2)];
            if (prefix)
            {
                chars[0] = '0';
                chars[1] = 'x';
            }

            for (var i = 0; i < value.Length; i++)
            {
                var b = value[i];
                chars[offset + (i * 2)] = LowerHexAlphabet[b >> 4];
                chars[offset + (i * 2) + 1] = LowerHexAlphabet[b & 0xF];
            }

            return new string(chars);
#endif
        }
    }
}
