using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using System.Collections.Generic;

namespace sReportsV2.Domain.Entities.DocumentProperties
{
    [BsonIgnoreExtraElements]
    public class DocumentProperties
    {
        public DocumentClass Class { get; set; }
        public DocumentPurpose Purpose { get; set; }
        public DocumentScopeOfValidity ScopeOfValidity { get; set; }
        public List<int?> ClinicalDomain { get; set; }
        public DocumentClinicalContext ClinicalContext { get; set; }
        public AdministrativeContext? AdministrativeContext { get; set; }
        public string Description { get; set; }
    }
}
