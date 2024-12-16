using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common
{
    public class TelecomDTO
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int? SystemCD { get; set; }
        public int? UseCD { get; set; }
        public string RowVersion { get; set; }
    }
}