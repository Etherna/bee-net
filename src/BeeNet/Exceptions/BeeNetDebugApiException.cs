﻿//   Copyright 2021-present Etherna Sagl
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

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Etherna.BeeNet.Exceptions
{
#pragma warning disable CA1032 // Implement standard exception constructors
    public partial class BeeNetDebugApiException : Exception
    {
        // Constructor.
        internal BeeNetDebugApiException(
            string message,
            int statusCode,
            string? response,
            IReadOnlyDictionary<string, IEnumerable<string>> headers,
            Exception? innerException)
            : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + ((response == null) ? "(null)" : response.Substring(0, response.Length >= 512 ? 512 : response.Length)), innerException)
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

    public partial class BeeNetDebugApiException<TResult> : BeeNetDebugApiException
    {
        // Constructor.
        internal BeeNetDebugApiException(
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
#pragma warning restore CA1032 // Implement standard exception constructors
}
