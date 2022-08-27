using Etherna.BeeNet.InputModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Clients.GatewayApi.Fixer
{
    public abstract class BaseGateway : IAuthentication
    {
        // Protected properties.
        protected string? AuthenticatedToken { get; private set; }

        // Public methods.
        public void SetAuthToken(string token)
        {
            AuthenticatedToken = token;
        }

        // Protected methods.
        protected void PrepareBearAuthRequest(HttpRequestMessage request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));
            if (AuthenticatedToken is null)
                throw new InvalidOperationException("AuthenticatedToken is null");

            request.Headers.Add("Authorization", $"Bearer {AuthenticatedToken}");
        }

        // Protected static methods.
        protected static void PrepareBasicAuthRequest(HttpRequestMessage request, string username, string password)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var authenticationString = $"{username}:{password}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
            request.Headers.Add("Authorization", $"Basic {base64EncodedAuthenticationString}");
        }

        protected static async Task PrepareUploadBzzFilesAsync(HttpRequestMessage httpRequestMessage, IEnumerable<FileParameterInput> fileParameterInputs)
        {
            if (fileParameterInputs == null ||
                !fileParameterInputs.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(fileParameterInputs));
            }
            if (httpRequestMessage == null)
            {
                throw new ArgumentNullException(nameof(httpRequestMessage));
            }

            HttpContent content_;
            if (fileParameterInputs.First().ContentType == "text/plain")
            {
#pragma warning disable CA2000 // Dispose objects before losing scope
                var reader = new StreamReader(fileParameterInputs.First().Data);
#pragma warning restore CA2000 // Dispose objects before losing scope
                content_ = new StringContent(await reader.ReadToEndAsync().ConfigureAwait(false));
            }
            else
            {
                content_ = new StreamContent(fileParameterInputs.First().Data);
                content_.Headers.TryAddWithoutValidation("Content-Type", fileParameterInputs.First().ContentType);
            }


            content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(fileParameterInputs.First().ContentType);
            httpRequestMessage.Content = content_;
            httpRequestMessage.Method = new HttpMethod("POST");
            httpRequestMessage.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("*/*"));
        }
    }
}
