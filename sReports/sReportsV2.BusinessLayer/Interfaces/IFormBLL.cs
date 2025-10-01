using Microsoft.AspNetCore.Mvc;
using sReportsV2.BusinessLayer.Implementations;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.CTCAE.DataIn;
using sReportsV2.DTOs.DTOs.FieldInstance.DataIn;
using sReportsV2.DTOs.DTOs.FieldInstance.DataOut;
using sReportsV2.DTOs.DTOs.Form.DataIn;
using sReportsV2.DTOs.DTOs.Form.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.Field.DataIn;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Form.DataOut.Tree;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IFormBLL
    {
        PaginationDataOut<FormDataOut, FormFilterDataIn> ReloadData(FormFilterDataIn dataIn, UserCookieData userCookieData);
        Form GetFormByThesaurusAndLanguage(int thesaurusId, string language);
        FormDataOut GetFormDataOut(Form form, UserCookieData userCookieData);
        FormDataOut GetFormDataOutById(string formId, UserCookieData userCookieData);
        Form GetFormByThesaurusAndLanguageAndVersionAndOrganization(int thesaurusId, int organizationId, string activeLanguage, string versionId);
        List<FormInstancePerDomainDataOut> GetFormInstancePerDomain(string activeLanguage);
        FormDataOut SetFormDependablesAndReferrals(Form form, FormInstanceReferralDTO formInstanceReferralDTO = null);
        FormDataOut GetFormDataOut(FormInstance formInstance, UserCookieData userCookieData, FormInstanceReloadDataIn formInstanceReloadData);
        List<Form> GetFormsFromReferrals(List<FormInstance> referrals);
        Form GetFormById(string formId);
        Task<Form> GetFormByIdAsync(string formId);
        TreeDataOut GetTreeDataOut(int thesaurusId, int thesaurusPageNum, string searchTerm, UserCookieData userCookieData = null);
        bool TryGenerateNewLanguage(string formId, string language, UserCookieData userCookieData);
        void DisableActiveFormsIfNewVersion(Form form, UserCookieData userCookieData);
        ResourceCreatedDTO UpdateFormState(UpdateFormStateDataIn updateFormStateDataIn, UserCookieData userCookieData);
        List<FieldDataOut> GetPlottableFields(string formId);
        Task<string> InsertOrUpdateCustomHeaderFieldsAsync(string formId, List<CustomHeaderFieldDataIn> customHeaderFieldsDataIn, UserCookieData userCookieData);
        Task<FormDataOut> GetFormForGeneralInfoAsync(FormDataIn dataIn);
        Form InsertOrUpdate(Form form, UserCookieData userCookieData, bool executeBackgroundTask = true);
        Task<AutocompleteResultDataOut> GetTitleDataForAutocomplete(AutocompleteDataIn dataIn, UserCookieData userCookieData);
        Task<FormDataOut> PasteElements<T>(List<T> elements, string destinationFormId, string destinationElementId, bool afterDestination, UserCookieData userCookieData);
        Task<bool> Delete(string formId, DateTime lastUpdate, string organizationTimeZone);
        void ExecuteFormBackgroundTasksAfterSave(Form form, FormBackgroundTaskDataIn formBackgroundTask, UserCookieData userCookieData);
        bool IsNullFlavorUsedInAnyField(string formId, int nullFlavorId);
        List<int> GetFormNullFlavors(string formId);
        FieldInstanceDependenciesDataOut ExecuteDependenciesFormulas(FieldInstanceDependenciesDataIn dataIn);
        FormFieldSetDataOut AddFieldsetRepetition(string formId, string fieldSetId, List<FieldInstance> fieldInstances);
        bool ValidateFormula(DependentOnInfoDataIn dataIn);
        Task<FormGenerateNewLanguageDataOut> GetGenerateNewLanguage(string formId, int activeOrganization);
        Task GenerateReport(string formId, UserCookieData userCookieData);
        IEnumerable<FieldSet> GetFieldSetsByFieldLabels(string formId, FormFieldSetDataIn fieldset);
        string GetFormJson(FormDataIn formDataIn);
        long GetThesaurusAppereanceCount(int o4mtId, string searchTerm, int? organizationId = null);
        DocumentProperties GetDocumentProperties(string formId);
        FormInstance SetFormInstanceIdForUpdate(CTCAEPatient patient, FormInstance formInstance);
        bool ExistsFormByThesaurus(int thesaurusId);
        bool ExistsForm(string formId);
        Form GetFormByThesaurus(int thesaurusId);
        Form GetFormByThesaurusAndVersion(int thesaurusId, string versionId);
    }
}
