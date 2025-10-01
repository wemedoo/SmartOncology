using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common.DataOut;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.DTOs.ThesaurusEntry.DataOut
{
    public class SkosDataOut
    {
        public List<ThesaurusEntryViewDataOut> SelectedThesauruses { get; set; } = new List<ThesaurusEntryViewDataOut>();
        public List<int> MissingThesaurusIds { get; set; } = new List<int>();
        public List<int> BroaderThesaurusIds { get; set; } = new List<int>();
        public List<int> NarrowerThesaurusIds { get; set; } = new List<int>();
        public List<TreeNodeDataOut> Roots { get; set; } = new List<TreeNodeDataOut>();
        public List<AutocompleteDataOut> AllConceptSchemes { get; set; } = new List<AutocompleteDataOut>();
        public List<AutocompleteDataOut> SelectedConceptSchemes { get; set; } = new List<AutocompleteDataOut>();
        public void AddMissingSelectedThesauruses(List<sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry> thesaurusEntries, string activeLanguage)
        {
            SelectedThesauruses
            .AddRange(
                thesaurusEntries
                .Select(x => new ThesaurusEntryViewDataOut
                {
                    ThesaurusEntryId = x.ThesaurusEntryId,
                    PreferredTerm = x.GetPreferredTermByActiveLanguage(activeLanguage)
                }
                )
            );
        }

        public IEnumerable<AutocompleteDataOut> GetChosenThesauruses(List<int> selectedValues)
        {
            return SelectedThesauruses
                .Where(x => selectedValues.Contains(x.ThesaurusEntryId))
                .DistinctBy(x => x.ThesaurusEntryId)
                .Select(x => new AutocompleteDataOut
                {
                    id = x.ThesaurusEntryId.ToString(),
                    text = x.PreferredTerm
                });
        }
    }
}
