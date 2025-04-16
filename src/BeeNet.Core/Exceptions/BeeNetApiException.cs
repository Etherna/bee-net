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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Etherna.BeeNet.Exceptions
{
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors")]
    public class BeeNetApiException(
        string message,
        int statusCode,
        string? response,
        IReadOnlyDictionary<string, IEnumerable<string>> headers,
        Exception? innerException)
        : Exception($"""
                     {message}

                     Status: {statusCode}
                     Response:
                     {(response == null ? "(null)" : response[..Math.Min(response.Length, 512)])}
                     """, innerException)
    {
        // Properties.
        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; } = headers;
        public string? Response { get; } = response;
        public int StatusCode { get; } = statusCode;

        // Methods.
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }

    [SuppressMessage("Design", "CA1032:Implement standard exception constructors")]
    public class BeeNetApiException<TResult>(
        string message,
        int statusCode,
        string? response,
        IReadOnlyDictionary<string, IEnumerable<string>> headers,
        TResult result,
        Exception? innerException)
        : BeeNetApiException(message, statusCode, response, headers, innerException)
    {
        // Properties.
        public TResult Result { get; } = result;
    }
}
