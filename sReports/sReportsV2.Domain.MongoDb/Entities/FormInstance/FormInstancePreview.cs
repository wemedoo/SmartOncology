using sReportsV2.Domain.Entities.FieldEntity;
using System;
using System.Collections.Generic;

namespace sReportsV2.Domain.Entities.FormInstance
{
    public class FormInstancePreview
    { 
        public string Id { get; set; }
        public string Title { get; set; }
        public Form.Version Version { get; set; }
        public string Language { get; set; }
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public int PatientId { get; set; }
        public DateTime EntryDatetime { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int? ProjectId { get; set; }
        public IEnumerable<FieldInstance> FieldInstancesToDisplay { get; set; }
        public IEnumerable<Field> FieldsToDisplay { get; set; }
    }
}
