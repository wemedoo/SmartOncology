using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;

namespace sReportsV2.Domain.Sql
{
    public interface IReplaceThesaurusEntity
    {
        void ReplaceThesauruses(ThesaurusMerge thesaurusMerge);
    }
}
