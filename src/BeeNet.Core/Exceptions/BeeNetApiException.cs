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
    public class BeeNetApiException : Exception
    {
        // Constructor.
        public BeeNetApiException(
            string message,
            int statusCode,
            string? response,
            IReadOnlyDictionary<string, IEnumerable<string>> headers,
            Exception? innerException)
            : base($"""
                    {message}
                    
                    Status: {statusCode}
                    Response:
                    {(response == null ? "(null)" : response[..(response.Length >= 512 ? 512 : response.Length)])}
                    """, innerException)
        {
            StatusCode = statusCode;
            Response = response;
            Headers = headers;
        }

        // Properties.
        public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; private set; }
        public string? Response { get; private set; }
        public int StatusCode { get; private set; }

        // Methods.
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }

    [SuppressMessage("Design", "CA1032:Implement standard exception constructors")]
    public class BeeNetApiException<TResult> : BeeNetApiException
    {
        // Constructor.
        public BeeNetApiException(
            string message,
            int statusCode,
            string? response,
            IReadOnlyDictionary<string, IEnumerable<string>> headers,
            TResult result,
            Exception? innerException)
            : base(message, statusCode, response, headers, innerException)
        {
            Result = result;
        }

        // Properties.
        public TResult Result { get; private set; }
    }
}
