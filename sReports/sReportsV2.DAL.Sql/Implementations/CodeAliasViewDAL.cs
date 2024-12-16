using sReportsV2.Common.Helpers;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Aliases;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.SqlDomain.Implementations
{
    public class CodeAliasViewDAL : BaseDisposalDAL, ICodeAliasViewDAL
    {
        public CodeAliasViewDAL(SReportsContext context) : base(context)
        {
        }

        public IQueryable<CodeAliasView> GetAll(CodeAliasFilter filter)
        {
            return context.CodeAliasViews
                .Where(x => x.CodeId == filter.CodeId);
        }

        public List<CodeAliasView> GetAllAlias(CodeAliasFilter filter)
        {
            IQueryable<CodeAliasView> result = this.GetAll(filter);

            if (filter.ColumnName != null)
                result = SortTableHelper.OrderByField(result, filter.ColumnName, filter.IsAscending)
                            .Skip((filter.Page - 1) * filter.PageSize)
                            .Take(filter.PageSize);
            else
                result = result.OrderByDescending(x => x.EntryDatetime)
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize);

            return result.ToList();
        }

        public int GetAllEntriesCount(CodeAliasFilter filter)
        {
            return this.GetAll(filter).Count();
        }

        public CodeAliasView GetById(int aliasId)
        {
            return context.CodeAliasViews
                .FirstOrDefault(x => x.AliasId == aliasId);
        }

        public bool SystemExist(int codeId, string system, string alias, bool isInbound)
        {
            if (isInbound)
                return context.CodeAliasViews
                    .Any(x => x.CodeId == codeId && x.System == system && x.InboundAlias == alias);
            else
                return context.CodeAliasViews
                    .Any(x => x.CodeId == codeId && x.System == system && x.OutboundAlias == alias);
        }

        public List<CodeAliasView> GetAllAvailableAliases()
        {
            return context.CodeAliasViews.ToList();
        }
    }
}
