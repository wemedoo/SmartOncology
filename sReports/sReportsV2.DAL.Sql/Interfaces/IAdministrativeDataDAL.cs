using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System.Collections.Generic;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IAdministrativeDataDAL
    {
        void ExecuteCustomSqlCommand(string script);
        IEnumerable<AdministrativeData> GetAll();
        void InsertManyVersions(List<ThesaurusEntry> thesauruses, List<int> bulkedThesauruses);
    }
}
