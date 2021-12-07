﻿namespace Etherna.BeeNet.DtoModel
{
    public class MessageResponseDto
    {
        public MessageResponseDto(string message, int code)
        {
            Message = message;
            Code = code;
        }

        public string Message { get; set; } = default!;
        
        public int Code { get; set; } = default!;
    }
}
