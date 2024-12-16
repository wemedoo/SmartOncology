using sReportsV2.Domain.Sql.Entities.CodeSetEntry;
using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface ICodeSetDAL
    {
        void Insert(CodeSet codeSet);
        Task InsertAsync(CodeSet codeSet);
        IQueryable<CodeSet> GetAll();
        int GetIdByPreferredTerm(string preferredTerm);
        CodeSet GetByPreferredTerm(string preferredTerm);
        int GetAllEntriesCount(CodeSetFilter filter);
        List<CodeSet> GetAll(CodeSetFilter filter);
        CodeSet GetById(int codeSetId);
        Task<CodeSet> GetByIdAsync(int codeSetId);
        int GetThesaurusId(int codeSetId);
        bool ThesaurusExist(int thesaurusId);
        bool ExistCodeSet(int codeSetId);
        bool ExistCodeSetByPreferredTerm(string preferredTerm);
        int GetIdByThesaurusId(int thesaurusId);
        int UpdateManyWithThesaurus(int oldThesaurus, int newThesaurus);
        int GetNextCodeSetId();
        IQueryable<CodeSet> GetAllByPreferredTerm(string preferredTerm);
        void Delete(int codeSetId);
        Task<PaginationData<CodeSet>> GetAllForAutoCompleteNameAndCount(CodeSetFilter filter);
    }
}
