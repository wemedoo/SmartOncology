using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.CTCAE.DataIn;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using sReportsV2.DTOs.DTOs.Fhir.DataIn;
using sReportsV2.DTOs.DTOs.FieldInstanceHistory.DataOut;
using sReportsV2.DTOs.DTOs.FieldInstanceHistory.FieldInstanceHistoryDataIn;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.DTOs.FormInstanceChart.DataOut;
using sReportsV2.DTOs.DTOs.Oomnia.DTO;
using sReportsV2.DTOs.DTOs.PatientQuery.DataIn;
using sReportsV2.DTOs.DTOs.PatientQuery.DataOut;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.FormInstance;
using sReportsV2.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IFormInstanceBLL
    {
        Task<PaginationDataOut<FormInstanceTableDataOut, FormInstanceFilterDataIn>> ReloadData(FormInstanceFilterDataIn dataIn, UserCookieData userCookieData);
        Task<string> InsertOrUpdateAsync(FormInstance formInstance, FormInstanceStatus formInstanceStatus, UserCookieData userCookieData);
        FormInstance GetById(string id);
        Task<FormInstance> GetByIdAsync(string id);
        List<FormInstance> GetByIds(List<string> ids);
        Task<List<FormInstance>> GetByIdsAsync(List<string> ids);
        void Delete(string formInstanceId, DateTime lastUpdate, int userId);
        Task DeleteAsync(string formInstanceId, DateTime lastUpdate, int userId);
        List<AutocompleteOptionDataOut> SearchByTitle(int episodeOfCare, string title, UserCookieData userCookieData);
        void LockUnlockFormInstance(FormInstanceLockUnlockRequest formInstanceSign, UserCookieData userCookieData);
        IList<FormInstanceStatusDataOut> GetWorkflowHistory(List<FormInstanceStatus> formInstanceStatuses);
        FormInstanceChartDataOut GetPlottableFieldsByThesaurusId(FormInstancePlotDataIn dataIn, List<FieldDataOut> fieldsDataOut);
        FormInstanceMetadataDataOut GetFormInstanceKeyDataFirst(int createdById);
        void LockUnlockChapterOrPageOrFieldSet(FormInstancePartialLock formInstancePartialLock, UserCookieData userCookieData);
        FormInstance GetFormInstanceSet(Form form, FormInstanceDataIn formInstanceDataIn, UserCookieData userCookieData, bool setFieldsFromRequest = true);
        List<FieldInstance> ParseFormInstanceFields(List<FieldInstanceDTO> fieldInstances);
        void PassDataToPocNLPApi(FormInstance formInstance);
        bool SendIntegrationEngineRequest(string requestEndpoint, string port, Object requestBody);
        void PassDataToOomniaApi(LockActionToOomniaApiDTO lockAction, UserCookieData userCookieData);
        void SendExportedFiles(List<FormInstanceDownloadData> formInstancesForDownload, UserCookieData userCookieData, string tableFormat, string fileFormat, bool callAsyncRunner = false);
        void SendHL7Message(FormInstance formInstance, UserCookieData userCookieData);
        void WriteFieldsAndMetadataToStream(FormInstance formInstance, TextWriter tw, string language, string dateFormat);
        Task<bool> GenerateAIDataExtraction(DataExtractionDataIn dataExtractionDataIn, UserCookieData userCookieData);
        void SetCTCAEPatient(Form form, FormInstance formInstance, CTCAEPatient patient, UserCookieData userCookieData);
        void SetFormInstanceAdditionalData(Form form, FormInstance formInstance, UserCookieData userCookieData);
        int GetEncounterFromRequestOrCreateDefault(int episodeOfCareId, int encounterId);
        void InsertListOfFormInstances(List<FormInstance> formInstances);
        int CountByDefinition(string id);
        Task<PatientSemanticQueryResultDataOut> GetPatientSemanticResult(PatientQueryFilterDataIn patientQueryFilterDataIn, UserCookieData userCookieData);
        Task<AutocompleteResultDataOut<SkosLabelAutocompleteDataOut>> GetDataForAutocomplete(AutocompleteDataIn autocompleteDataIn);
        #region FieldHistory
        Task InsertOrUpdateManyFieldHistoriesAsync(FormInstance formInstance);
        Task<PaginationDataOut<FieldInstanceHistoryDataOut, FieldInstanceHistoryFilterDataIn>> GetAllFieldHistoriesFiltered(FieldInstanceHistoryFilterDataIn fieldInstanceHistoryFilter);
        Task UpdateManyFieldHistoriesOnDeleteAsync(string formInstanceId, int userId);
        #endregion  
    }
}
