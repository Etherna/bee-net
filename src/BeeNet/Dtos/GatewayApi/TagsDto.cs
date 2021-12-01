﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TestAdapter.Dtos.GatewayApi
{
    public class TagsDto : BaseResponse
    {
        public int Uid { get; set; }

        public DateTimeOffset StartedAt { get; set; }

        public int Total { get; set; }

        public int Processed { get; set; }

        public int Synced { get; set; }
    }
}
