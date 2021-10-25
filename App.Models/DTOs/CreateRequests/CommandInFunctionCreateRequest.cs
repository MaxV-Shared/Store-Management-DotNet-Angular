﻿using MaxV.Base.DTOs;

namespace App.Models.DTOs.CreateRequests
{
    public class CommandInFunctionCreateRequest : BaseCreateRequest
    {
        public string CommandId { get; set; }
        public string FunctionId { get; set; }
    }
}
