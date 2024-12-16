using sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IPersonnelTeamDAL
    {
        PersonnelTeam GetById(int id);
        PersonnelTeam GetByIdJoinActiveRelations(int id);
        void InsertOrUpdate(PersonnelTeam personnelTeam);
        List<PersonnelTeam> GetAll(PersonnelTeamFilter filter);
        int GetAllEntriesCount(PersonnelTeamFilter filter);
        IQueryable<PersonnelTeam> FilterByName(string name);
        void Delete(int id);
    }
}
