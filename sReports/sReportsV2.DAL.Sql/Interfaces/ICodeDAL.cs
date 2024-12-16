using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.CodeEntry;
using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DAL.Sql.Interfaces
{
    public interface ICodeDAL
    {
        int Insert(Code code, string organizationTimeZone = null);
        int Update(Code code);
        void Delete(int codeId);
        IQueryable<Code> GetAll();
        IQueryable<Code> GetCodeFiltered(CodeFilter filter);
        List<Code> GetAll(CodeFilter filter);
        List<Code> GetAllAssociationsFiltered(CodeFilter filter);
        int UpdateManyWithThesaurus(int oldThesaurus, int newThesaurus);
        bool ThesaurusExist(int thesaurusId);
        bool CodesExist(int codeSetId);
        void InsertMany(List<int> bulkedThesauruses, int? codeSetId);
        Code GetByPreferredTerm(string preferredTerm, int? codeSetId, string language = LanguageConstants.EN);
        Code GetByPreferredTerm(string preferredTerm, string codeSet, string language = LanguageConstants.EN);
        int GetAllEntriesCount(CodeFilter filter);
        int GetAllAssociationsCount(CodeFilter filter);
        Code GetById(int codeId);
        List<Code> GetByIds(List<int> codeIds);
        List<Code> GetCommunicationSystems(int codeSetId);
        List<Code> GetByCodeSet(string codeSet, string language = LanguageConstants.EN);
        List<Code> GetByCodeSetId(int codeSetId);
        int GetIdByPreferredTerm(string preferredTerm);
        int GetByCodeSetIdAndPreferredTerm(int codeSetId, string preferredTerm);
        int? GetIdByPreferredTermAndNullCodeset(string preferredTerm, string language = LanguageConstants.EN);
        int UpdateCodesetByCodeId(int codeId, int newCodeSetId);

        void SetCodeToInactive(int? codeId, string organizationTimeZone = null);
        Code GetByCodeSetIdAndThesaurusId(int? codeSetId, int thesaurusId);
        int InsertOrUpdateTaskDocumentCode(Code code, string organizationTimeZone = null);
    }
}
