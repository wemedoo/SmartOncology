using sReportsV2.DTOs.DTOs.TrialManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.User.DTO
{
    public class ClinicalTrialWithUserInfoDataIn
    {
        public ClinicalTrialDataIn ClinicalTrial { get; set; }
        public int? UserId { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
    }
}