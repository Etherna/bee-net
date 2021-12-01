using System;
using System.Collections.Generic;
using System.Text;

namespace TestAdapter.Dtos.GatewayApi
{
    public class TagsGETResponse : BaseResponse
    {
        public ICollection<TagsDto> Tags { get; set; }
    }
}
