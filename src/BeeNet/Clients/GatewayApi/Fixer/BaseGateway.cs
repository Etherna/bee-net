//   Copyright 2021-present Etherna SA
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

using Etherna.BeeNet.InputModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            if (AuthenticatedToken is null)
                throw new InvalidOperationException("AuthenticatedToken is null");

            request.Headers.Add("Authorization", $"Bearer {AuthenticatedToken}");
        }

        // Protected static methods.
        protected static void PrepareBasicAuthRequest(HttpRequestMessage request, string username, string password)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            var authenticationString = $"{username}:{password}";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
            request.Headers.Add("Authorization", $"Basic {base64EncodedAuthenticationString}");
        }
    }
}
