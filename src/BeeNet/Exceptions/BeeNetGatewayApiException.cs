using System;
using System.Collections.Generic;
using System.Globalization;

namespace Etherna.BeeNet.Exceptions
{
#pragma warning disable CA1032 // Implement standard exception constructors
    public partial class BeeNetGatewayApiException : Exception
    {
        // Constructor.
        internal BeeNetGatewayApiException(
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

    public partial class BeeNetGatewayApiException<TResult> : BeeNetGatewayApiException
    {
        // Constructor.
        internal BeeNetGatewayApiException(
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
