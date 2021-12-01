using System;
using System.Collections.Generic;
using System.Text;

namespace TestAdapter.Dtos.GatewayApi
{
    public class BodyDto : BaseResponse
    {
        public string Role { get; set; }

        public int Expiry { get; set; }
    }
}
