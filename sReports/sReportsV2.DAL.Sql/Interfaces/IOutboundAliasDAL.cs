using sReportsV2.Domain.Sql.Entities.Aliases;
using System.Collections.Generic;
using System;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IOutboundAliasDAL
    {
        int InsertOutbound(OutboundAlias alias);
        OutboundAlias GetById(int aliasId);
        void Delete(int aliasId);
    }
}
