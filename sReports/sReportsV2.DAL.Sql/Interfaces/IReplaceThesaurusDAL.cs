using sReportsV2.Common.Entities.User;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IReplaceThesaurusDAL
    {
        int ReplaceThesaurus(ThesaurusMerge thesaurusMerge, UserData userData = null);
        bool ThesaurusExist(int thesaurusId);
    }
}
