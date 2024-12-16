using sReportsV2.DTOs.CodeEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DTO
{
    public class ClinicalDomainDTO
    {
        public int Id { get; set; }
        public string Translation { get; set; }

        public string ConvertClinicalDomainCDToDisplayName(List<CodeDataOut> clinicalDomains, string language)
        {
            return clinicalDomains.Find(x => x.Id == this.Id)?.Thesaurus?.GetPreferredTermByTranslationOrDefault(language) ?? String.Empty;
        }
    }
}