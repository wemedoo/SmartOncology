using sReportsV2.Domain.Sql.Entities.Aliases;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface ICodeAliasViewDAL
    {
        IQueryable<CodeAliasView> GetAll(CodeAliasFilter filter);
        int GetAllEntriesCount(CodeAliasFilter filter);
        List<CodeAliasView> GetAllAlias(CodeAliasFilter filter);
        CodeAliasView GetById(int aliasId);
        bool SystemExist(int codeId, string system, string alias, bool isInbound);
        List<CodeAliasView> GetAllAvailableAliases();
    }
}
