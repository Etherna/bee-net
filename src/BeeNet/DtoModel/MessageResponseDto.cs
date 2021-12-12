using System;

namespace Etherna.BeeNet.DtoModel
{
    public class MessageResponseDto
    {
        // Constructors.
        public MessageResponseDto(Clients.v1_4.DebugApi.Response9 response9)
        {
            if (response9 is null)
                throw new ArgumentNullException(nameof(response9));

            Message = response9.Message;
            Code = response9.Code;
        }

        public MessageResponseDto(Clients.v1_4.DebugApi.Response10 response10)
        {
            if (response10 is null)
                throw new ArgumentNullException(nameof(response10));

            Message = response10.Message;
            Code = response10.Code;
        }

        public MessageResponseDto(Clients.v1_4.DebugApi.Response16 response16)
        {
            if (response16 is null)
                throw new ArgumentNullException(nameof(response16));

            Message = response16.Message;
            Code = response16.Code;
        }

        public MessageResponseDto(Clients.v1_4.GatewayApi.Response10 response10)
        {
            if (response10 is null)
                throw new ArgumentNullException(nameof(response10));

            Message = response10.Message;
            Code = response10.Code;
        }

        public MessageResponseDto(Clients.v1_4.GatewayApi.Response12 response12)
        {
            if (response12 is null)
                throw new ArgumentNullException(nameof(response12));

            Message = response12.Message;
            Code = response12.Code;
        }


        // Properties.
        public string Message { get; }
        public int Code { get; }
    }
}
