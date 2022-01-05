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
