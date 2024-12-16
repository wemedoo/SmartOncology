using MongoDB.Bson;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IFormInstanceDAL
    {
        FormInstance GetById(string formValueId, List<string> projectionPropertyList = null);
        Task<FormInstance> GetByIdAsync(string formInstanceId);
        string InsertOrUpdate(FormInstance formInstance, FormInstanceStatus formInstanceStatus);
        Task<string> InsertOrUpdateAsync(FormInstance formInstance, FormInstanceStatus formInstanceStatus);
        bool Delete(string id, DateTime lastUpdate);
        Task<bool> DeleteAsync(string id, DateTime lastUpdate);
        Task<bool> DeleteByPatientIdAsync(int patientId);
        Task<bool> DeleteByEpisodeOfCareIdAsync(int episodeOfCareId);
        Task<bool> DeleteByEncounterIdAsync(int encounterId);
        List<FormInstance> GetByEpisodeOfCareId(int episodeOfCareId, int organizationId);
        int CountAllEOCDocuments(int episodeOfCareId, int patientId);
        Task<int> CountAllEOCDocumentsAsync(int episodeOfCareId, int patientId);
        Task<List<FormInstance>> GetAllByEpisodeOfCareIdAsync(int episodeOfCareId, int organizationId);
        List<FormInstance> GetAllByCovidFilter(FormInstanceCovidFilter filter);
        Task<List<FormInstance>> GetAllFieldsByCovidFilter();
        bool ExistsFormInstance(string formValueId);
        IQueryable<FormInstance> GetByIds(List<string> ids);
        Task<List<FormInstance>> GetByIdsAsync(List<string> ids);
        List<FormInstance> GetByParameters(FormInstanceFhirFilter patientFilter);
        void InsertMany(List<FormInstance> formInstances);
        bool ExistsById(string id);
        List<FormInstance> GetByAllByDefinitionId(string id, int organizationId, int batchSize, int offset);
        List<FormInstance> GetByDefinitionId(string id, int limit, int offset);
        List<FormInstance> SearchByTitle(int episodeOfCareId, string title);
        List<FormInstance> GetAll();
        int CountByDefinition(string id);
        bool ThesaurusExist(int thesaurusId);

        void UpdateManyWithThesaurus(int oldThesaurus, int newThesaurus);
        List<FormInstance> GetAllByEncounter(int encounterId, int organizationId);
        Task<List<FormInstance>> GetAllByEncounterAsync(int encounterId, int organizationId);
        List<BsonDocument> GetPlottableFieldsByThesaurusId(string formId, int organizationId, int thesaurusId);
        Task<PaginationData<FormInstancePreview>> GetFormInstancesFilteredAsync(FormInstanceFilterData filterData);
        FormInstanceMetadata GetFormInstanceKeyDataFirst(int createdById);
        void LockUnlockChapterOrPage(FormInstancePartialLock formInstancePartialLock);
        void LockUnlockFormInstance(FormInstanceLockUnlockRequest formInstanceSign);
        DateTime? GetLastUpdateById(string formInstanceId);
        void UpdateOomniaExternalDocumentInstanceId(FormInstance formInstance);
        List<FormInstance> GetByPatient(int patientId);
        Field GetFieldByThesaurus(FormInstance formInstance, int thesaurusId);
    }
}
