﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataIn
{
    public class FieldTextAreaDataIn : FieldStringDataIn
    {
        public bool DataExtractionEnabled { get; set; }
    }
}