using System;

namespace Chapters.Resources
{
    public class FormPdfMetadata
    {
        public string Title { get; set; }
        public sReportsV2.Domain.Entities.Form.Version Version { get; set; }
        public DateTime EntryDatetime { get; set; }
        public string PatientIdentifier { get; set; }

        public FormPdfMetadata(string title, sReportsV2.Domain.Entities.Form.Version version, DateTime entryDatetime, string patientIdentifier = null)
        {
            Title = title;
            Version = version;
            EntryDatetime = entryDatetime;
            PatientIdentifier = patientIdentifier;
        }
    }
}
