using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IPersonnelTeamRelationDAL
    {
        PersonnelTeamRelation GetById(int id);
        void InsertOrUpdate(PersonnelTeamRelation personnelTeamRelation);
        void Delete(int personnelTeamRelationId);
        IQueryable<PersonnelTeamRelation> FilterByName(string name);
        List<PersonnelTeamRelation> GetAll(PersonnelTeamRelationFilter filter);
        int GetAllEntriesCount(PersonnelTeamRelationFilter filter);
    }
}
