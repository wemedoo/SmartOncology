﻿using sReportsV2.Common.SmartOncologyEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataIn
{
    public class ChemotherapySchemaInstanceFilterDataIn : Common.DataIn
    {
        public int PatientId { get; set; }
        public string Indication { get; set; }
        public int? StateCD { get; set; }
        public string ClinicalConstelation { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }
    }
}