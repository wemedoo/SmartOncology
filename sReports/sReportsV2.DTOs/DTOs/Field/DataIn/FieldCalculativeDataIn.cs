﻿using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Field.DataIn
{
    public class FieldCalculativeDataIn : FieldDataIn
    {
        public string Formula { get; set; }

        public Dictionary<string, string> IdentifiersAndVariables { get; set; }
        public CalculationFormulaType FormulaType { get; set; }
        public CalculationGranularityType? GranularityType { get; set; }

    }
}