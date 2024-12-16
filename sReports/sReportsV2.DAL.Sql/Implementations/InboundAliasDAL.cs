using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Aliases;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;

namespace sReportsV2.SqlDomain.Implementations
{
    public class InboundAliasDAL : BaseDisposalDAL, IInboundAliasDAL
    {

        public InboundAliasDAL(SReportsContext context) : base(context)
        {
        }

        public void InsertInbound(InboundAlias alias)
        {
            InboundAlias aliasFromDb = this.GetById(alias.AliasId);

            if (aliasFromDb == null)
            {
                alias.SetActiveFromAndToDatetime();
                context.InboundAliases.Add(alias);
            }
            else
            {
                aliasFromDb.SetLastUpdate();
            }
            context.SaveChanges();
        }

        public InboundAlias GetById(int aliasId)
        {
            return context.InboundAliases
                .WhereEntriesAreActive()
                .FirstOrDefault(x => x.AliasId == aliasId);
        }

        public void Delete(int aliasId)
        {
            InboundAlias fromDb = this.GetById(aliasId);
            if (fromDb != null)
            {
                fromDb.Delete();
                context.SaveChanges();
            }
        }
    }
}
