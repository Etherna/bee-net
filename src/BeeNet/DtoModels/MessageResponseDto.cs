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

namespace Etherna.BeeNet.DtoModels
{
    public class MessageResponseDto
    {
        // Constructors.
        public MessageResponseDto(Clients.GatewayApi.V3_0_2.Response10 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Message = response.Message;
            Code = response.Code;
        }

        public MessageResponseDto(Clients.GatewayApi.V3_0_2.Response12 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Message = response.Message;
            Code = response.Code;
        }

        public MessageResponseDto(Clients.GatewayApi.V3_0_2.Response27 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Message = response.Message;
            Code = response.Code;
        }

        public MessageResponseDto(Clients.GatewayApi.V3_0_2.Response28 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Message = response.Message;
            Code = response.Code;
        }

        public MessageResponseDto(Clients.GatewayApi.V3_0_2.Response34 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Message = response.Message;
            Code = response.Code;
        }

        // Properties.
        public string Message { get; }
        public int Code { get; }
    }
}