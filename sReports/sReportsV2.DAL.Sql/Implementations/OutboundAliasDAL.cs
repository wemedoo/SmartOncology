using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Aliases;
using sReportsV2.SqlDomain.Interfaces;
using System.Linq;
using sReportsV2.Common.Enums;
using System.Collections.Generic;
using System;
using sReportsV2.Common.Helpers;

namespace sReportsV2.SqlDomain.Implementations
{
    public class OutboundAliasDAL : BaseDisposalDAL, IOutboundAliasDAL
    {
        public OutboundAliasDAL(SReportsContext context) : base(context)
        {
        }

        public int InsertOutbound(OutboundAlias alias)
        {
            OutboundAlias aliasFromDb = this.GetById(alias.AliasId);

            if (aliasFromDb == null)
            {
                alias.SetActiveFromAndToDatetime();
                context.OutboundAliases.Add(alias);
            }
            else
            {
                aliasFromDb.SetLastUpdate();
            }
            context.SaveChanges();

            return alias.AliasId;
        }

        public OutboundAlias GetById(int aliasId)
        {
            return context.OutboundAliases
                .WhereEntriesAreActive()
                .FirstOrDefault(x => x.AliasId == aliasId);
        }

        public void Delete(int aliasId)
        {
            OutboundAlias fromDb = this.GetById(aliasId);
            if (fromDb != null)
            {
                fromDb.Delete();
                context.SaveChanges();
            }
        }
    }
}
