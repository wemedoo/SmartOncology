using sReportsV2.Domain.Sql.Entities.Aliases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IInboundAliasDAL
    {
        void InsertInbound(InboundAlias alias);
        InboundAlias GetById(int aliasId);
        void Delete(int aliasId);
    }
}
