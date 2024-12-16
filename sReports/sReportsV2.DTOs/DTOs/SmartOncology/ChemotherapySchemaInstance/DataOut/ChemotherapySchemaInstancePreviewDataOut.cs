using sReportsV2.Common.Entities.User;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataOut
{
    public class ChemotherapySchemaInstancePreviewDataOut
    {
        public int Id { get; set; }
        public DateTime? StartDate { get; set; }
        public UserData Creator { get; set; }
        public DateTimeOffset EntryDatetime { get; set; }
        public ChemotherapySchemaPreviewDataOut ChemotherapySchema {get; set; }
        public int? StateCD { get; set; }

        public string ConvertInstanceStateCDToDisplayName(List<CodeDataOut> states, string language)
        {
            if (this.StateCD != null && this.StateCD.HasValue)
                return states.Find(x => x.Id == this.StateCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);

            return "";
        }
    }
}
