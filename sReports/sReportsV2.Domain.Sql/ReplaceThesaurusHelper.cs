using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;

namespace sReportsV2.Domain.Sql
{
    public static class ReplaceThesaurusHelper
    {
        public static int ReplaceThesaurus(this int currentThesaurus, ThesaurusMerge thesaurusMerge)
        {
            return currentThesaurus == thesaurusMerge.OldThesaurus ? thesaurusMerge.NewThesaurus : currentThesaurus;
        }

        public static int? ReplaceThesaurus(this int? currentThesaurus, ThesaurusMerge thesaurusMerge)
        {
            return currentThesaurus.HasValue ? currentThesaurus.Value.ReplaceThesaurus(thesaurusMerge) : currentThesaurus;
        }
    }
}
