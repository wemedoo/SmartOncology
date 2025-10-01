using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using System.Collections.Generic;
using System.Linq;

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

        public string GetVersionId()
        {
            return string.IsNullOrWhiteSpace(this.EditVersionId) ? this.VersionId : this.EditVersionId;
        }

        public void UpdateCachedData(FormInstanceDataIn incoming)
        {
            this.Notes = incoming.Notes;
            this.FormState = incoming.FormState;
            this.Date = incoming.Date;
            this.LastUpdate = incoming.LastUpdate;

            foreach (FieldInstanceDTO incomingFieldInstance in incoming.FieldInstances)
            {
                FieldInstanceDTO existingFieldInstance = this.FieldInstances.SingleOrDefault(fI => fI.FieldInstanceRepetitionId == incomingFieldInstance.FieldInstanceRepetitionId);
                if (existingFieldInstance != null)
                {
                    existingFieldInstance.UpdateCachedData(incomingFieldInstance);
                }
                else
                {
                    this.FieldInstances.Add(incomingFieldInstance);
                }
            }
        }
    }
}