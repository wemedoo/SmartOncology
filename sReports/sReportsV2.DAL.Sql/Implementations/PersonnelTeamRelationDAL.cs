using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using sReportsV2.Common.Helpers;
using Microsoft.EntityFrameworkCore;

namespace sReportsV2.SqlDomain.Implementations
{
    public class PersonnelTeamRelationDAL : IPersonnelTeamRelationDAL
    {
        private readonly SReportsContext context;

        public PersonnelTeamRelationDAL(SReportsContext context)
        {
            this.context = context;
        }

        public PersonnelTeamRelation GetById(int id)
        {
            PersonnelTeamRelation personnelTeamRelationQuery = context.PersonnelTeamRelations
                .Include(x => x.RelationType)
                .Include(x => x.Personnel)
                .FirstOrDefault(x=> x.PersonnelTeamRelationId == id);

            return personnelTeamRelationQuery;
        }

        public void InsertOrUpdate(PersonnelTeamRelation personnelTeamRelation)
        {
            if (personnelTeamRelation.PersonnelTeamRelationId == 0)
            {
                context.PersonnelTeamRelations.Add(personnelTeamRelation);
            }
            else
            {   
                personnelTeamRelation.SetLastUpdate();
            }

            context.SaveChanges();
        }

        public void Delete(int personnelTeamRelationId)
        {
            PersonnelTeamRelation personnelTeamRelation = GetById(personnelTeamRelationId);
            if(personnelTeamRelation != null)
            {
                personnelTeamRelation.Delete();
                context.SaveChanges();
            }
        }

        public IQueryable<PersonnelTeamRelation> FilterByName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
                return context.PersonnelTeamRelations
                    .Include(x => x.RelationType)
                    .Include(x => x.Personnel)
                    .WhereEntriesAreActive()
                    .Where(x => x.Personnel.FirstName.ToLower().Contains(name.ToLower()) || x.Personnel.LastName.ToLower().Contains(name.ToLower()));
            else
                return context.PersonnelTeamRelations
                    .Include(x => x.RelationType)
                    .Include(x => x.Personnel)
                    .WhereEntriesAreActive();
        }

        public List<PersonnelTeamRelation> GetAll(PersonnelTeamRelationFilter filter)
        {
            IQueryable<PersonnelTeamRelation> result = GetPersonnelTeamFiltered(filter);

            result = result.OrderBy(x => x.PersonnelTeamId)
                .Skip(filter.GetHowManyElementsToSkip())
                .Take(filter.PageSize);

            return result.ToList();
        }

        public int GetAllEntriesCount(PersonnelTeamRelationFilter filter)
        {
            return GetPersonnelTeamFiltered(filter).Count();
        }

        private IQueryable<PersonnelTeamRelation> GetPersonnelTeamFiltered(PersonnelTeamRelationFilter filter)
        {
            IQueryable<PersonnelTeamRelation> personnelTeamRelationQuery = context.PersonnelTeamRelations
                .Include(x => x.RelationType)
                .Include(x => x.Personnel)
                .WhereEntriesAreActive()
                .Where(x => x.PersonnelTeamId == filter.PersonnelTeamId);

            if (filter.PersonnelId != null)
            {
                personnelTeamRelationQuery = personnelTeamRelationQuery.Where(x => x.PersonnelId == filter.PersonnelId);
            }

            if (filter.RelationTypeCD != null)
            {
                personnelTeamRelationQuery = personnelTeamRelationQuery.Where(x => x.RelationTypeCD == filter.RelationTypeCD);
            }

            return personnelTeamRelationQuery;
        }
    }
}
