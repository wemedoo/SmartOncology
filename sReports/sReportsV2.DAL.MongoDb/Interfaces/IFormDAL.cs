using MongoDB.Bson;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IFormDAL : IReplaceThesaurusDAL
    {
        List<Form> GetAll(FormFilterData filterData);
        Task<List<Form>> GetAllByOrganizationAndLanguageAndNameAsync(int organization, string language, string name = "");
        Form GetForm(string formId);
        Task<Form> GetFormAsync(string formId);
        Task<Form> GetFormTask(string formId);
        Form GetFormByThesaurus(int thesaurusId);
        Form GetFormByThesaurusAndVersion(int thesaurusId, string versionId);
        List<FormInstancePerDomain> GetFormInstancePerDomain();
        Form GetFormByThesaurusAndLanguage(int thesaurusId, string language);
        Form GetFormByThesaurusAndLanguageAndVersionAndOrganization(int thesaurusId, int organizationId, string activeLanguage, string versionId);
        bool Delete(string formId, DateTime lastUpdate);
        bool ExistsForm(string formId);
        bool ExistsFormByThesaurus(int thesaurusId);
        Form InsertOrUpdate(Form form, UserData user, bool updateVersion = true);
        long GetAllFormsCount(FormFilterData filterData);
        List<Form> GetFilteredDocumentsByThesaurusAppeareance(int o4mtId, string searchTerm, int thesaurusPageNum, int? organizationId);
        long GetThesaurusAppereanceCount(int o4mtId, string searchTerm, int? organizationId = null);
        DocumentProperties GetDocumentProperties(string formId);
        long GetFormByThesaurusAndLanguageAndVersionAndOrganizationCount(int thesaurusId, int organizationId, string activeLanguage, sReportsV2.Domain.Entities.Form.Version version);
        Form GetFormWithGreatestVersion(int thesaurusId, int activeOrganization, string activeLanguage);
        List<Form> GetByFormIdsList(List<string> ids);
        Task<List<Form>> GetByFormIdsListAsync(List<string> id);
        void DisableFormsByThesaurusAndLanguageAndOrganization(int thesaurus, int organizationId, string activeLanguage);
        List<string> GetByClinicalDomains(List<int> clinicalDomains);
        List<BsonDocument> GetPlottableFields(string formId);
        Task<string> InsertOrUpdateCustomHeaderFieldsAsync(Form form, UserData user, bool updateVersion = true);
        Task<List<Form>> GetByTitleForAutoComplete(FormFilterData formFilterData);
        Task<long> CountByTitle(FormFilterData formFilterData);
        Task<List<Form>> GetByTitle(string title);
        Task<IEnumerable<FieldSet>> GetAllFieldSetsByFormId(string formId);
        bool IsNullFlavorUsedInAnyField(string formId, int nullFlavorId);
        List<int> GetFormNullFlavors(string formId);
        Task<List<string>> GetGeneratedLanguages(int thesaurusId, int organizationId, Entities.Form.Version version);
        IEnumerable<FieldSet> GetFieldSetsByFieldLabels(string formId, List<string> fieldLabels, string fieldType);
    }
}
