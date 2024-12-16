using sReportsV2.Common.Enums;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using sReportsV2.SqlDomain.Helpers;

namespace sReportsV2.SqlDomain.Implementations
{
    public class OrganizationRelationDAL : IOrganizationRelationDAL
    {
        private readonly SReportsContext context;
        public OrganizationRelationDAL(SReportsContext context)
        {
            this.context = context;
        }
        public OrganizationRelation GetRelationByChildId(int childId)
        {
            return this.context.OrganizationRelations
                .WhereEntriesAreActive()
                .FirstOrDefault(x => x.ChildId == childId);
        }

        public List<OrganizationRelation> GetOrganizationHierarchies()
        {
            return context.OrganizationRelations
                .WhereEntriesAreActive()
                .Include(x => x.Child)
                    .ThenInclude(x => x.OrganizationAddress)
                .Include(x => x.Parent)
                    .ThenInclude(x => x.OrganizationAddress)
                .ToList();
        }

        public void UnLinkOrganization(int organizationId, int oldParentId)
        {
            OrganizationRelation organizationRelation = context.OrganizationRelations
                .WhereEntriesAreActive()
                .FirstOrDefault(x => x.ChildId == organizationId && x.ParentId == oldParentId);
            if (organizationRelation != null)
            {
                organizationRelation.Delete(setLastUpdateProperty: false);
            }

            context.SaveChanges();
        }

        public void InsertOrUpdate(OrganizationRelation organizationRelation)
        {
            if(organizationRelation.OrganizationRelationId == 0)
            {
                context.OrganizationRelations.Add(organizationRelation);
            }
            else
            {
                context.UpdateEntryMetadata(organizationRelation, setRowVersion: false);
            }
            context.SaveChanges();
        }
    }
}
