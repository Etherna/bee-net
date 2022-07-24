using System;

namespace Etherna.BeeNet.Clients.GatewayApi.Fixer
{
    abstract public class BaseGateway : IAuthentication
    {
        // Properties.
        protected string? AuthenticatedToken { get; private set; }

        // Methods.
        public void SetAuthToken(string token)
        {
            AuthenticatedToken = token;
        }

        static protected void PrepareBasicAuthRequest(System.Net.Http.HttpRequestMessage request, string username, string password)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var authenticationString = $"{username}:{password}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
            request.Headers.Add("Authorization", $"Basic {base64EncodedAuthenticationString}");
        }

        protected void PrepareBearAuthRequest(System.Net.Http.HttpRequestMessage request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));
            if (AuthenticatedToken is null)
                throw new InvalidOperationException("AuthenticatedToken is null");

            request.Headers.Add("Authorization", $"Bearer {AuthenticatedToken}");
        }
    }
}
