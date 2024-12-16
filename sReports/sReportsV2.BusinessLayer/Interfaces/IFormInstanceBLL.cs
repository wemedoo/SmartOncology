using sReportsV2.Common.Constants;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs;
using sReportsV2.DTOs.DTOs.FieldInstanceHistory.DataOut;
using sReportsV2.DTOs.DTOs.FieldInstanceHistory.FieldInstanceHistoryDataIn;
using sReportsV2.DTOs.DTOs.FieldInstance.DataIn;
using sReportsV2.DTOs.DTOs.FieldInstance.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.DTOs.Oomnia.DTO;
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
        Task<PaginationDataOut<FormInstanceTableDataOut, FormInstanceFilterDataIn>> ReloadData(FormInstanceFilterDataIn dataIn, List<EnumDTO> languages, UserCookieData userCookieData);
        Task<string> InsertOrUpdateAsync(FormInstance formInstance, FormInstanceStatus formInstanceStatus, UserCookieData userCookieData);
        FormInstance GetById(string id);
        Task<FormInstance> GetByIdAsync(string id);
        List<FormInstance> GetByIds(List<string> ids);
        Task<List<FormInstance>> GetByIdsAsync(List<string> ids);
        void Delete(string formInstanceId, DateTime lastUpdate, int userId);
        Task DeleteAsync(string formInstanceId, DateTime lastUpdate, int userId);
        List<FormInstanceDataOut> GetByEpisodeOfCareId(int episodeOfCareId, int organizationId);
        List<FormInstanceDataOut> SearchByTitle(int episodeOfCare, string title);
        void LockUnlockFormInstance(FormInstanceLockUnlockRequest formInstanceSign, UserCookieData userCookieData);
        IList<FormInstanceStatusDataOut> GetWorkflowHistory(List<FormInstanceStatus> formInstanceStatuses);
        Common.Helpers.DataCaptureChartUtility GetPlottableFieldsByThesaurusId(FormInstancePlotDataIn dataIn, List<FieldDataOut> fieldsDataOut);
        FormInstanceMetadataDataOut GetFormInstanceKeyDataFirst(int createdById);
        void LockUnlockChapterOrPage(FormInstancePartialLock formInstancePartialLock, UserCookieData userCookieData);
        FormInstance GetFormInstanceSet(Form form, FormInstanceDataIn formInstanceDataIn, UserCookieData userCookieData, bool setFieldsFromRequest = true);
        List<FieldInstance> ParseFormInstanceFields(List<FieldInstanceDTO> fieldInstances);
        void PassDataToPocNLPApi(FormInstance formInstance);
        bool SendIntegrationEngineRequest(string requestEndpoint, string port, Object requestBody);
        void PassDataToOomniaApi(LockActionToOomniaApiDTO lockAction, UserCookieData userCookieData);
        void SendExportedFiles(List<FormInstanceDownloadData> formInstancesForDownload, UserCookieData userCookieData, string tableFormat, string fileFormat, bool callAsyncRunner = false);
        void SendHL7Message(FormInstance formInstance, UserCookieData userCookieData);
        void WriteFieldsAndMetadataToStream(FormInstance formInstance, TextWriter tw, string language, string dateFormat);

        #region FieldHistory
        Task InsertOrUpdateManyFieldHistoriesAsync(FormInstance formInstance);
        Task<PaginationDataOut<FieldInstanceHistoryDataOut, FieldInstanceHistoryFilterDataIn>> GetAllFieldHistoriesFiltered(FieldInstanceHistoryFilterDataIn fieldInstanceHistoryFilter);
        Task UpdateManyFieldHistoriesOnDeleteAsync(string formInstanceId, int userId);
        #endregion
    }
}
