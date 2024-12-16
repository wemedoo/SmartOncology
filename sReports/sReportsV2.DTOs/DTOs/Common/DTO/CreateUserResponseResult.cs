using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common.DTO
{
    public class CreateUserResponseResult : CreateResponseResult
    {
        public string Password { get; set; }
    }
}