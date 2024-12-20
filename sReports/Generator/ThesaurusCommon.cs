﻿using sReportsV2.Common.Constants;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    public class ThesaurusCommon
    {
        protected ThesaurusEntry CreateThesaurus(string label, string description = null)
        {
            var translations = new List<ThesaurusEntryTranslation>();
            translations.Add(new ThesaurusEntryTranslation()
            {
                PreferredTerm = label != null ? label : string.Empty,
                Definition = string.IsNullOrWhiteSpace(description) ? "." : description,
                Language = LanguageConstants.EN
            });
            ThesaurusEntry entry = new ThesaurusEntry
            {
                Translations = translations
            };

            return entry;
        }

    }
}
