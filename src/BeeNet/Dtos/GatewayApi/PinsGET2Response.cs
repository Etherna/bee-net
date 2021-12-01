using System;
using System.Collections.Generic;
using System.Text;

namespace TestAdapter.Dtos.GatewayApi
{
    public class PinsGET2Response : BaseResponse
    {
        public ICollection<string> Addresses { get; set; }
    }
}
