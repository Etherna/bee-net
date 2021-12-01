using System;
using System.Collections.Generic;
using System.Text;

namespace TestAdapter.Dtos.GatewayApi
{
    public class PinsPOSTResponse : BaseResponse
    {
        public string Message { get; set; }

        public int Code { get; set; }
    }
}
