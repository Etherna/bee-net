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

namespace Etherna.BeeNet.DtoModel
{
    public class MessageResponseDto
    {
        // Constructors.
        public MessageResponseDto(Clients.v1_4_1.DebugApi.Response9 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Message = response.Message;
            Code = response.Code;
        }

        public MessageResponseDto(Clients.v1_4_1.DebugApi.Response10 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Message = response.Message;
            Code = response.Code;
        }

        public MessageResponseDto(Clients.v1_4_1.DebugApi.Response16 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Message = response.Message;
            Code = response.Code;
        }

        public MessageResponseDto(Clients.v1_4_1.GatewayApi.Response10 response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));

            Message = response.Message;
            Code = response.Code;
        }

        public MessageResponseDto(Clients.v1_4_1.GatewayApi.Response12 response)
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
