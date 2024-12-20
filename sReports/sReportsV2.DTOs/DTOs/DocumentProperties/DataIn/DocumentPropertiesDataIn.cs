﻿using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Domain.Entities.DocumentProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.DocumentProperties.DataIn
{
    public class DocumentPropertiesDataIn
    {
        public DocumentClass Class { get; set; }
        public DocumentPurposeDataIn Purpose { get; set; }
        public DocumentScopeOfValidityDataIn ScopeOfValidity { get; set; }
        public List<int?> ClinicalDomain { get; set; }
        public DocumentClinicalContext ClinicalContext { get; set; }
        public AdministrativeContext? AdministrativeContext { get; set; }
        public string Description { get; set; }
    }
}