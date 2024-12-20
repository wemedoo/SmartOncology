﻿using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.EpisodeOfCare
{
    public class EpisodeOfCareFilterDataIn : DataIn
    {
        public int? IdentifierType{ get; set; }
        public string IdentifierValue { get; set; }
        public int StatusCD { get; set; }
        public int TypeCD { get; set; }
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public string Description { get; set; }
        public string OrganizationId { get; set; }
    }
}