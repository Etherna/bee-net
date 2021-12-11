namespace Etherna.BeeNet.DtoModel
{
    public class MessageResponseDto
    {
        public MessageResponseDto(Clients.v1_4.DebugApi.Response9 response9)
        {
            if (response9 is null)
            {
                return;
            }

            Message = response9.Message;
            Code = response9.Code;
        }

        public MessageResponseDto(Clients.v1_4.DebugApi.Response10 response10)
        {
            if (response10 is null)
            {
                return;
            }

            Message = response10.Message;
            Code = response10.Code;
        }

        public MessageResponseDto(Clients.v1_4.DebugApi.Response16 response16)
        {
            if (response16 is null)
            {
                return;
            }

            Message = response16.Message;
            Code = response16.Code;
        }

        public MessageResponseDto(Clients.v1_4.GatewayApi.Response10 response10)
        {
            if (response10 is null)
            {
                return;
            }

            Message = response10.Message;
            Code = response10.Code;
        }

        public MessageResponseDto(Clients.v1_4.GatewayApi.Response12 response12)
        {
            if (response12 is null)
            {
                return;
            }

            Message = response12.Message;
            Code = response12.Code;
        }


        public string Message { get; set; } = default!;
        
        public int Code { get; set; } = default!;
    }
}
