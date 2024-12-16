using sReportsV2.Common.Constants;
using sReportsV2.Domain.Sql.Entities.CodeEntry;
using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface ICodeAssociationDAL
    {
        void Insert(List<CodeAssociation> codeAssociations, DateTimeOffset? activeTo = null);
        IQueryable<CodeAssociation> GetAll();
        int GetAllEntriesCount(CodeAssociationFilter filter);
        CodeAssociation GetById(int codeAssociationId);
        List<CodeAssociation> GetAll(CodeAssociationFilter filter);
        bool ExistAssociation(int parentId, int childId);
        void Delete(int associationId);
        List<CodeAssociation> GetAllByParentId(int parentId);
        List<CodeAssociation> GetAllByParentOrChildId(int codeId);
        List<int> GetByParentId(int parentId);
        Task<PaginationData<CodeAssociation>> GetAllForCodedFieldsAndCount(CodeAssociationFilter filter);
        Dictionary<int, Dictionary<int, string>> InitializeMissingValueList(string language = LanguageConstants.EN);
    }
}
