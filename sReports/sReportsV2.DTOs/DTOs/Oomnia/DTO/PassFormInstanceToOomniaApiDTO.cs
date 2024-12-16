using sReportsV2.Common.Constants;
using System;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.Oomnia.DTO
{
    public class PassFormInstanceToOomniaApiDTO : SaveDocumentRequest
    {
        public Guid ExternalOrganizationId { get; set; }
        public string ParticipantIdentifier { get; set; }

        public PassFormInstanceToOomniaApiDTO() { }
        public PassFormInstanceToOomniaApiDTO(string externalOrganizationId, string oomniaDocumentExternalId)
        {
            this.SystemIdentificator = ResourceTypes.ApplicationName;
            this.ExternalOrganizationId = new Guid(externalOrganizationId);
            this.RequestData = new SaveDocumentExternalRequest
            {
                ExternalDocumentId = new Guid(oomniaDocumentExternalId),
                Fields = new List<SaveFieldData>()
            };
        }
    }

    public class SaveDocumentRequest
    {
        public SaveDocumentExternalRequest RequestData { get; set; }

        public string SystemIdentificator { get; set; }
    }

    public class SaveDocumentExternalRequest
    {
        public Guid? ExternalDocumentId { get; set; }
        public Guid? ExternalDocumentInstanceId { get; set; }
        public IList<SaveFieldData> Fields { get; set; }
    }

    public class SaveFieldData
    {
        public string Name { get; set; }
        public FieldValue Value { get; set; }
        public int? ChapterSequenceNumber { get; set; }
        public int? GroupSequenceNumber { get; set; }
        public int? FieldSequenceNumber { get; set; }
    }

    public class FieldValue
    {
        public string Text { get; set; }
        public IList<string> SelectedOptions { get; set; }
    }

    public class SaveSReportsDocumentResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public IEnumerable<SaveSReportsDocumentResponseItem> SavedDocuments { get; set; }

    }

    public class SaveSReportsDocumentResponseItem
    {
        public Guid ExternalDocumentInstanceId { get; set; }
        public string ParticipantId { get; set; }
        public string ScreeningNumber { get; set; }

    }
}
