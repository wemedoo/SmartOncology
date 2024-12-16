using Microsoft.Extensions.Configuration;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System.Collections.Generic;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IO4CodeableConceptDAL
    {
        void InsertMany(List<ThesaurusEntry> thesauruses, List<int> bulkedThesauruses, IConfiguration configuration);
    }
}
