using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System;
using sReportsV2.Common.Helpers;
using Microsoft.EntityFrameworkCore;

namespace sReportsV2.SqlDomain.Implementations
{
    public class PersonnelTeamDAL : IPersonnelTeamDAL
    {
        private readonly SReportsContext context;
        public PersonnelTeamDAL(SReportsContext context)
        {
            this.context = context;
        }

        public PersonnelTeam GetById(int id)
        {
            var personnelTeamQuery = context.PersonnelTeams
                .Include(x => x.Type)
                .Include(x => x.PersonnelTeamRelations)
                .Include(x => x.PersonnelTeamOrganizationRelations)
                .Include("PersonnelTeamRelations.RelationType")
                .WhereEntriesAreActive()
                .FirstOrDefault(x => x.PersonnelTeamId == id);

            return personnelTeamQuery;
        }

        public PersonnelTeam GetByIdJoinActiveRelations(int id)
        {
            var personnelTeamQuery = context.PersonnelTeams
                .Include(x => x.Type)
                .Include(x => x.PersonnelTeamRelations)
                .Include("PersonnelTeamRelations.RelationType")
                .WhereEntriesAreActive()
                .Where(x => x.PersonnelTeamId == id)
                .FirstOrDefault();

            if (personnelTeamQuery != null)
            {
                personnelTeamQuery.PersonnelTeamRelations =
                    personnelTeamQuery.PersonnelTeamRelations
                        .Where(r => r.IsActive() && r.RelationType != null)
                        .ToList();
            }

            return personnelTeamQuery;
        }

        public void InsertOrUpdate(PersonnelTeam personnelTeam)
        {
            if (personnelTeam.PersonnelTeamId == 0)
            {
                context.PersonnelTeams.Add(personnelTeam);
            }
            else
            {
                personnelTeam.SetLastUpdate();
            }
            context.SaveChanges();
        }

        public List<PersonnelTeam> GetAll(PersonnelTeamFilter filter)
        {
            IQueryable<PersonnelTeam> result = GetPersonnelTeamFiltered(filter);

            result = result.OrderBy(x => x.PersonnelTeamId)
                .Skip(filter.GetHowManyElementsToSkip())
                .Take(filter.PageSize);

            result.ToList().ForEach(x => { x.PersonnelTeamRelations = 
                x.PersonnelTeamRelations.Where(y => y.IsActive()).ToList(); });

            return result.ToList();
        }

        public int GetAllEntriesCount(PersonnelTeamFilter filter)
        {
            return GetPersonnelTeamFiltered(filter).Count();
        }

        public IQueryable<PersonnelTeam> FilterByName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
                return context.PersonnelTeams
                    .WhereEntriesAreActive()
                    .Where(x => x.Name.ToLower().Contains(name.ToLower()));
            else
                return context.PersonnelTeams.WhereEntriesAreActive();
        }

        public void Delete(int id)
        {
            var fromDb = GetById(id);
            if (fromDb != null)
            {
                foreach(PersonnelTeamRelation personnelTeamRelation in fromDb.PersonnelTeamRelations)
                {
                    personnelTeamRelation.Delete();
                }
                foreach (PersonnelTeamOrganizationRelation personnelTeamOrganizationRelation in fromDb.PersonnelTeamOrganizationRelations)
                {
                    personnelTeamOrganizationRelation.Delete();
                }
                fromDb.Delete();
                context.SaveChanges();
            }
        }

        private IQueryable<PersonnelTeam> GetPersonnelTeamFiltered(PersonnelTeamFilter filter)
        {
            IQueryable<PersonnelTeam> personnelTeamQuery = context.PersonnelTeams
                .Include(x => x.PersonnelTeamRelations)
                .Include("PersonnelTeamRelations.RelationType")
                .Include(x => x.Type)
                .WhereEntriesAreActive()
                .Where(x => x.PersonnelTeamOrganizationRelations.Any(b => b.OrganizationId == filter.OrganizationId));

            if (!string.IsNullOrEmpty(filter.TeamName))
            {
                personnelTeamQuery = personnelTeamQuery.Where(x => x.Name == filter.TeamName);
            }

            if (filter.TeamType != 0)
            {
                personnelTeamQuery = personnelTeamQuery.Where(x => x.Type.CodeId == filter.TeamType);
            }

            return personnelTeamQuery;
        }

    }
}
