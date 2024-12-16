using sReportsV2.DTOs.CodeEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common.DTO
{
    public class CommunicationDTO
    {
        public int Id { get; set; }
        public bool Preferred { get; set; }
        public int? LanguageCD { get; set; }
        public CommunicationDTO() { }

        public CommunicationDTO(int? languageCD, bool preferred)
        {
            this.LanguageCD = languageCD;
            this.Preferred = preferred;
        }

        public string ConvertLanguageCDToDisplayName(List<CodeDataOut> languages, string language)
        {
            return languages.Find(x => x.Id == this.LanguageCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);
        }
    }
}