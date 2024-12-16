using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System.Collections.Generic;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IThesaurusMergeDAL
    {
        void InsertOrUpdate(ThesaurusMerge thesaurusMerge);
        List<ThesaurusMerge> GetAllByState(int? state);
    }
}
