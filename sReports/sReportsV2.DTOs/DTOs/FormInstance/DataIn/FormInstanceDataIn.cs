using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.Field.DataIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.FormInstance.DataIn
{
    public class FormInstanceDataIn
    {
        public string FormInstanceId { get; set; }
        public string FormDefinitionId { get; set; }
        public int ThesaurusId { get; set; }
        public string Notes { get; set; }
        public string FormState { get; set; }
        public string Date { get; set; }
        public string LastUpdate { get; set; }
        public List<string> Referrals { get; set; }
        public string VersionId { get; set; }
        public string EditVersionId { get; set; }
        public string Language { get; set; }
        public int? ProjectId { get; set; }
        public List<FieldInstanceDTO> FieldInstances { get; set; }

        #region PatientRelatedData
        public int EncounterId { get; set; } 
        public int PatientId { get; set; } 
        public int EpisodeOfCareId { get; set; } 
        #endregion

    }
}