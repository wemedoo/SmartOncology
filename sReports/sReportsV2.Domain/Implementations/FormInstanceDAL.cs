using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Entities;
using sReportsV2.Domain.Entities.CustomFieldFilters;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.FieldFilters;
using sReportsV2.Domain.FieldFilters.Implementations;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Implementations
{
    public class FormInstanceDAL : IFormInstanceDAL
    {
        private readonly IMongoCollection<FormInstance> Collection;
        private readonly IMongoCollection<Form> CollectionForm;
        private readonly bool SupportsTransaction;

        public FormInstanceDAL()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<FormInstance>(MongoCollectionNames.FormInstance);
            CollectionForm = MongoDatabase.GetCollection<Form>(MongoCollectionNames.Form);
            SupportsTransaction = !string.IsNullOrEmpty(MongoDatabase.Client.Settings.ReplicaSetName);
        }

        public FormInstance GetById(string id, List<string> projectionPropertyList = null)
        {
            var query = Collection
                .Find(x => !x.IsDeleted && x.Id.Equals(id));
            query = ImplementProjectionIfNecessary(query, projectionPropertyList);
            return query
                .FirstOrDefault();
        }

        public async Task<FormInstance> GetByIdAsync(string formInstanceId)
        {
            return await Collection
                .Find(x => !x.IsDeleted && x.Id.Equals(formInstanceId))
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public string InsertOrUpdate(FormInstance formInstance, FormInstanceStatus formInstanceStatus)
        {
            formInstance = Ensure.IsNotNull(formInstance, nameof(formInstance));

            if (formInstance.Id == null)
            {
                formInstance.Copy(null, formInstanceStatus);
                InitOrUpdateChapterPageWorkflowHistory(formInstance, formInstanceStatus?.CreatedById);
                Collection.InsertOne(formInstance);
                IncrementFormDocumentCount(formInstance.FormDefinitionId, 1);
            }
            else
            {
                FormInstance formInstanceDb = GetFormInstanceFromDBAndDoConcurrencyCheck(formInstance.Id, formInstance.LastUpdate.Value);
                formInstance.Copy(formInstanceDb, formInstanceStatus);
                InitOrUpdateChapterPageWorkflowHistory(formInstance, formInstanceStatus?.CreatedById);
                FindAndReplace(formInstance);
            }

            return formInstance.Id;
        }

        public async Task<string> InsertOrUpdateAsync(FormInstance formInstance, FormInstanceStatus formInstanceStatus)
        {
            formInstance = Ensure.IsNotNull(formInstance, nameof(formInstance));

            if (formInstance.Id == null)
            {
                formInstance.Copy(null, formInstanceStatus);
                InitOrUpdateChapterPageWorkflowHistory(formInstance, formInstanceStatus?.CreatedById);
                await Collection.InsertOneAsync(formInstance).ConfigureAwait(false);
                IncrementFormDocumentCount(formInstance.FormDefinitionId, 1);
            }
            else
            {
                FormInstance formInstanceDb = GetFormInstanceFromDBAndDoConcurrencyCheck(formInstance.Id, formInstance.LastUpdate.Value);
                formInstance.Copy(formInstanceDb, formInstanceStatus);
                InitOrUpdateChapterPageWorkflowHistory(formInstance, formInstanceStatus?.CreatedById);
                await FindAndReplaceAsync(formInstance).ConfigureAwait(false);
            }

            return formInstance.Id;
        }

        public bool Delete(string id, DateTime lastUpdate)
        {
            FormInstance formInstanceForDelete = GetById(id);
            Entity.DoConcurrencyCheckForDelete(formInstanceForDelete);
            formInstanceForDelete.DoConcurrencyBeforeDeleteCheck(lastUpdate);

            var filter = Builders<FormInstance>.Filter.Eq(x => x.Id, id);
            var update = Builders<FormInstance>.Update.Set(x => x.IsDeleted, true).Set(x => x.LastUpdate, DateTime.Now);
            IncrementFormDocumentCount(formInstanceForDelete.FormDefinitionId, -1);
            return Collection.UpdateOne(filter, update).IsAcknowledged;
        }

        public async Task<bool> DeleteAsync(string id, DateTime lastUpdate)
        {
            FormInstance formInstanceForDelete = GetById(id);
            Entity.DoConcurrencyCheckForDelete(formInstanceForDelete);
            formInstanceForDelete.DoConcurrencyBeforeDeleteCheck(lastUpdate);

            var filter = Builders<FormInstance>.Filter.Eq(x => x.Id, id);
            var update = Builders<FormInstance>.Update.Set(x => x.IsDeleted, true).Set(x => x.LastUpdate, DateTime.Now);
            await IncrementFormDocumentCountAsync(formInstanceForDelete.FormDefinitionId, -1).ConfigureAwait(false);

            return (await Collection.UpdateOneAsync(filter, update).ConfigureAwait(false)).IsAcknowledged;
        }

        public async Task<bool> DeleteByPatientIdAsync(int patientId)
        {
            var filter = Builders<FormInstance>.Filter.Eq(x => x.PatientId, patientId);
            return await DeleteAsyncInBatches(filter).ConfigureAwait(false);
        }

        public async Task<bool> DeleteByEpisodeOfCareIdAsync(int episodeOfCareId)
        {
            var filter = Builders<FormInstance>.Filter.Eq(x => x.EpisodeOfCareRef, episodeOfCareId);
            return await DeleteAsyncInBatches(filter).ConfigureAwait(false);
        }

        public async Task<bool> DeleteByEncounterIdAsync(int encounterId)
        {
            var filter = Builders<FormInstance>.Filter.Eq(x => x.EncounterRef, encounterId);
            return await DeleteAsyncInBatches(filter).ConfigureAwait(false);
        }

        public List<FormInstance> GetByAllByDefinitionId(string id, int organizationId, int batchSize, int offset)
        {
            return Collection
                 .AsQueryable()
                 .Where(x => x.FormDefinitionId.Equals(id) && x.OrganizationId == organizationId  && !x.IsDeleted)
                 .Skip(offset)
                 .Take(batchSize)
                 .ToList();
        }

        public int CountByDefinition(string id)
        {
            return Collection
                .AsQueryable()
                .Where(x => x.FormDefinitionId.Equals(id))
                .Count();
        }

        public List<FormInstance> GetByDefinitionId(string id, int limit, int offset)
        {
            return Collection.AsQueryable()
                .Where(x => x.FormDefinitionId.Equals(id))
                .Skip(offset)
                .Take(limit)
                .ToList();
        }

        public bool ExistsFormInstance(string id)
        {
            return Collection
                .Find(x => !x.IsDeleted && x.Id.Equals(id))
                .CountDocuments() > 0;
        }

        public IQueryable<FormInstance> GetByIds(List<string> ids)
        {
            if (ids == null)
            {
                ids = new List<string>();
            }
            //TO DO IMPLEMENT THE METHOD
            return Collection.AsQueryable().Where(x => ids.Contains(x.Id));
        }

        public async Task<List<FormInstance>> GetByIdsAsync(List<string> ids)
        {
            if (ids == null)
            {
                ids = new List<string>();
            }

            return await Collection.Find(x => ids.Contains(x.Id)).ToListAsync().ConfigureAwait(false);

        }

        public List<FormInstance> GetByEpisodeOfCareId(int episodeOfCareId, int organizationId)
        {
            return Collection
                .AsQueryable()
                .Where(x => x.EpisodeOfCareRef == episodeOfCareId && x.OrganizationId == organizationId)
                .ToList();
        }

        public int CountAllEOCDocuments(int episodeOfCareId, int patientId)
        {
            return Collection
                .AsQueryable()
                .Where(x => x.EpisodeOfCareRef == episodeOfCareId && x.PatientId == patientId)
                .Count();
        }

        public async Task<int> CountAllEOCDocumentsAsync(int episodeOfCareId, int patientId)
        {
            return await Collection
                .AsQueryable()
                .Where(x => x.EpisodeOfCareRef == episodeOfCareId && x.PatientId == patientId && !x.IsDeleted)
                .CountAsync()
                .ConfigureAwait(false);
        }

        public Task<List<FormInstance>> GetAllByEpisodeOfCareIdAsync(int episodeOfCareId, int organizationId)
        {
            return Collection.Find(x => x.EpisodeOfCareRef == episodeOfCareId && x.OrganizationId == organizationId).ToListAsync();
        }

        public bool ExistsById(string id)
        {
            return Collection.AsQueryable().Any(x => x.Id == id);
        }

        public List<FormInstance> GetByParameters(FormInstanceFhirFilter formInstancesFilter)
        {

            return Collection.Find(x => (formInstancesFilter.Encounter == null || x.EncounterRef.Equals(formInstancesFilter.Encounter))
                                            && (formInstancesFilter.Performer == 0 || x.UserId == formInstancesFilter.Performer)
                                            && !x.IsDeleted).ToList();
        }

        public List<FormInstance> GetAllByCovidFilter(FormInstanceCovidFilter filter)
        {
            return Collection.AsQueryable().Where(x => !x.IsDeleted
                                            && (x.LastUpdate > filter.LastUpdate)
                                            && (x.ThesaurusId.Equals(filter.ThesaurusId))
                                            ).ToList();
        }

        public async Task<List<FormInstance>> GetAllFieldsByCovidFilter()
        {
            FindOptions options = new FindOptions
            {
                BatchSize = 1000,
                NoCursorTimeout = false
            };
            var filter = Builders<FormInstance>.Filter.Eq("ThesaurusId", "14573");
            var projection = Builders<FormInstance>.Projection
                .Include("Chapters.Pages.FieldSets.Fields.Label")
                .Include("Chapters.Pages.FieldSets.Fields.Type")
                .Include("Chapters.Pages.FieldSets.Fields.Value")
                .Include("Chapters.Pages.FieldSets.Fields.ThesaurusId")
                .Include("Chapters.Pages.FieldSets.Fields.Values");
            var result = await Collection.Find(filter, options).Project<FormInstance>(projection).ToListAsync().ConfigureAwait(false);

            return result;
        }

        public void InsertMany(List<FormInstance> formInstances)
        {
            formInstances = Ensure.IsNotNull(formInstances, nameof(formInstances));

            IncrementFormDocumentCount(formInstances.FirstOrDefault()?.FormDefinitionId, formInstances.Count);
            Collection.InsertMany(formInstances);
        }

        public List<FormInstance> GetAll()
        {
            return Collection.AsQueryable().Where(x => !x.IsDeleted).ToList();
        }

        public bool ThesaurusExist(int thesaurusId)
        {
            return Collection.AsQueryable()
                            .Any(instance => !instance.IsDeleted &&
                                instance.FieldInstances.Any(field =>
                                    field.ThesaurusId == thesaurusId)
                                || instance.ThesaurusId.Equals(thesaurusId));
        }

        public void UpdateManyWithThesaurus(int oldThesaurus, int newThesaurus)
        {
            var filter = Builders<FormInstance>.Filter;
            var formInstanceAndFieldThesaurusId = filter.And(
            filter.ElemMatch(x => x.FieldInstances, c => c.ThesaurusId == oldThesaurus));

            var update = Builders<FormInstance>.Update;
            var fieldsLevelSetter = update.Set("FieldInstances.$.ThesaurusId", newThesaurus);
            Collection.UpdateMany(formInstanceAndFieldThesaurusId, fieldsLevelSetter);

            var filterThesaurus = Builders<FormInstance>.Filter.Eq("ThesaurusId", oldThesaurus);

            var formInstanceSetter = update.Set("ThesaurusId", newThesaurus);
            Collection.UpdateMany(filterThesaurus, formInstanceSetter);

        }

        public List<FormInstance> GetAllByEncounter(int encounterId, int organizationId)
        {
            return this.Collection.AsQueryable()
                .Where(x => !x.IsDeleted && x.EncounterRef.Equals(encounterId) && x.OrganizationId == organizationId)
                .Select(x => new FormInstance()
                {
                    Id = x.Id,
                    ThesaurusId = x.ThesaurusId,
                    FormDefinitionId = x.FormDefinitionId,
                    Title = x.Title,
                    LastUpdate = x.LastUpdate,
                    UserId = x.UserId,
                    EntryDatetime = x.EntryDatetime
                })
                .OrderByDescending(x => x.EntryDatetime)
                .ToList();
        }

        public async Task<List<FormInstance>> GetAllByEncounterAsync(int encounterId, int organizationId)
        {
            var formInstances = await Collection.AsQueryable().Where(x => !x.IsDeleted && x.EncounterRef.Equals(encounterId) && x.OrganizationId == organizationId)
                .Select(x => new FormInstance()
                {
                    Id = x.Id,
                    ThesaurusId = x.ThesaurusId,
                    FormDefinitionId = x.FormDefinitionId,
                    Title = x.Title,
                    EntryDatetime = x.EntryDatetime,
                    Date = x.Date,
                    LastUpdate = x.LastUpdate,
                    UserId = x.UserId,
                    FieldInstances = x.FieldInstances
                })
                .OrderByDescending(x => x.EntryDatetime)
                .ToListAsync()
                .ConfigureAwait(false);

            formInstances.ForEach(formInstance =>
            {
                formInstance.NonEmptyValuePercentage = CalculatePercentage(formInstance);
            });

            return formInstances;
        }

        public List<FormInstance> SearchByTitle(int episodeOfCareId, string title)
        {
            return Collection.AsQueryable().Where(x => x.EpisodeOfCareRef == episodeOfCareId && x.Title.ToUpper().Contains(title.ToUpper()))
               .OrderByDescending(x => x.EntryDatetime)
               .ToList();
        }

        public List<BsonDocument> GetPlottableFieldsByThesaurusId(string formId, int organizationId, int thesaurusId)
        {
            var stage1 = new BsonDocument {
                { "FormDefinitionId", formId },
                { "OrganizationId", organizationId}
            };
            var stage2 =
                    new BsonDocument
                        {
                            { "Date", 1 },
                            { "EntryDatetime", 1 },
                            { "FieldInstances",
                                new BsonDocument("$filter",
                                    new BsonDocument
                                    {
                                        { "input", "$FieldInstances" },
                                        { "as", "fieldInstance" },
                                        { "cond",
                                            new BsonDocument("$eq",
                                                new BsonArray
                                                {
                                                    "$$fieldInstance.ThesaurusId",
                                                    thesaurusId
                                                }
                                            ) 
                                        }
                                    }
                                ) 
                            }
                        };
            var stage3 =
                    new BsonDocument
                    {
                        { "Date", 1 },
                        { "EntryDatetime", 1 },
                        { "FieldInstanceValue",
                            new BsonDocument("$reduce",
                                new BsonDocument
                                {
                                    { "input", "$FieldInstances.FieldInstanceValues.Values" },
                                    { "initialValue", new BsonArray() },
                                    { "in",
                                        new BsonDocument("$concatArrays",
                                            new BsonArray 
                                            {
                                                "$$value",
                                                "$$this"
                                            }
                                        ) 
                                    }
                                }
                            ) 
                        }
                    };

            var stage4 =
                    new BsonDocument
                    {
                        { "Date", 1 },
                        { "EntryDateTimeValue", "$EntryDatetime.DateTime" },
                        { "FieldInstanceValue",
                            new BsonDocument("$reduce",
                                new BsonDocument
                                {
                                    { "input", "$FieldInstanceValue" },
                                    { "initialValue", new BsonArray() },
                                    { "in",
                                        new BsonDocument("$concatArrays",
                                            new BsonArray
                                            {
                                                "$$value",
                                                "$$this"
                                            }
                                        ) 
                                    }
                                }
                            ) 
                        }
                    };

            var aggregationResult = Collection.Aggregate()
                .Match(stage1)
                .Project(stage2)
                .Project(stage3)
                .Project(stage4)
                .ToList();

            return aggregationResult;
        }

        public async Task<PaginationData<FormInstancePreview>> GetFormInstancesFilteredAsync(FormInstanceFilterData filterData)
        {
            IMongoQueryable<FormInstance> queryResult = Collection.AsQueryable(new AggregateOptions() { AllowDiskUse = true });

            // Text Search needs to be first pipeline step
            queryResult = DoContentFilter(queryResult, ref filterData);
            queryResult = DoBasicFilter(queryResult, filterData);
            queryResult = DoAdditionalFilter(queryResult, filterData);

            return await FormatFormInstancesFiltered(queryResult, filterData).ConfigureAwait(false);
        }

        public FormInstanceMetadata GetFormInstanceKeyDataFirst(int createdById)
        {
            return Collection
                .AsQueryable()
                .Where(x => x.UserId == createdById)
                .Select(x => new FormInstanceMetadata
                {
                    VersionId = x.Version.Id,
                    FormInstanceId = x.Id
                })
                .FirstOrDefault()
            ;
        }

        public void LockUnlockChapterOrPage(FormInstancePartialLock formInstancePartialLock)
        {
            FormInstance formInstance = GetFormInstanceFromDBAndDoConcurrencyCheck(formInstancePartialLock.FormInstanceId, formInstancePartialLock.LastUpdate);
            InitOrUpdateChapterPageWorkflowHistory(formInstance, formInstancePartialLock.CreateById);
            formInstance.RecordLatestChapterOrPageChangeState(formInstancePartialLock);
            FindAndReplace(formInstance);
        }

        public void LockUnlockFormInstance(FormInstanceLockUnlockRequest formInstanceSign)
        {
            FormInstance formInstance = GetFormInstanceFromDBAndDoConcurrencyCheck(formInstanceSign.FormInstanceId, formInstanceSign.LastUpdate);
            formInstance.FormState = formInstanceSign.FormInstanceNextState;
            formInstance.RecordLatestWorkflowChange(formInstance.GetCurrentFormInstanceStatus(formInstanceSign.CreatedById, isSigned: true));
            InitOrUpdateChapterPageWorkflowHistory(formInstance, formInstanceSign.CreatedById);
            formInstance.DoPropagation(new FormInstancePartialLock(formInstanceSign.FormInstanceNextState));
            FindAndReplace(formInstance);
        }

        public DateTime? GetLastUpdateById(string formInstanceId)
        {
            if (!string.IsNullOrWhiteSpace(formInstanceId))
            {
                return Collection
                .Find(x => !x.IsDeleted && x.Id.Equals(formInstanceId))
                .FirstOrDefault()?.LastUpdate;
            }
            else
            {
                return null;
            }
        }

        public List<FormInstance> GetByPatient(int patientId)
        {
            return Collection.AsQueryable().Where(fI => !fI.IsDeleted && fI.PatientId == patientId).ToList();
        }

        private IFindFluent<FormInstance, FormInstance> ImplementProjectionIfNecessary(IFindFluent<FormInstance, FormInstance> query, List<string> projectionPropertyList = null)
        {
            if (projectionPropertyList != null)
            {
                ProjectionDefinition<FormInstance> projectionDefinition = Builders<FormInstance>.Projection.Include(projectionPropertyList[0]);
                for(int i = 1; i < projectionPropertyList.Count; i++)
                {
                    projectionDefinition = projectionDefinition.Include(projectionPropertyList[i]);
                }
                query = query.Project<FormInstance>(projectionDefinition);
            }
            return query;
        }

        private IMongoQueryable<FormInstance> GetByCustomFieldFilters(IMongoQueryable<FormInstance> formInstancesQueryableCollection, FormInstanceFilterData filterData)
        {
            List<CustomFieldFilter> fieldFilters = new List<CustomFieldFilter>();
            foreach (CustomFieldFilterData f in filterData.CustomFieldFiltersData)
                fieldFilters.Add(CustomFieldFiltersFactory.Create(f));

            FilterDefinition<FormInstance> compoundFilter;
            switch (filterData.FieldFiltersOverallOperator)
            {
                case LogicalOperators.OR:
                    {
                        compoundFilter = fieldFilters.AllOr();
                        break;
                    }
                case LogicalOperators.AND:
                    {
                        compoundFilter = fieldFilters.AllAnd();
                        break;
                    }
                default:
                    {
                        compoundFilter = fieldFilters.AllOr();
                        break;
                    }
            }
            return formInstancesQueryableCollection.Where(x => x.FormDefinitionId == filterData.FormId && compoundFilter.Inject());
        }

        private void IncrementFormDocumentCount(string formDefinitionId, int value)
        {
            var filterForm = Builders<Form>.Filter.Eq(f => f.Id, formDefinitionId);
            var updateForm = Builders<Form>.Update.Inc(f => f.DocumentsCount, value);
            CollectionForm.FindOneAndUpdate(filterForm, updateForm);
        }

        private async Task IncrementFormDocumentCountAsync(string formDefinitionId, int value)
        {
            var filterForm = Builders<Form>.Filter.Eq(f => f.Id, formDefinitionId);
            var updateForm = Builders<Form>.Update.Inc(f => f.DocumentsCount, value);
            await CollectionForm.FindOneAndUpdateAsync(filterForm, updateForm).ConfigureAwait(false);
        }

        private IMongoQueryable<FormInstance> TextSearch(IMongoQueryable<FormInstance> formInstancesQueryableCollection, string content)
        {
            FilterDefinition<FormInstance> contentFilter = Builders<FormInstance>.Filter.Text(content.PrepareForMongoStrictTextSearch());
            return formInstancesQueryableCollection.Where(x => contentFilter.Inject());
        }

        private IMongoQueryable<FormInstance> DoContentFilter(IMongoQueryable<FormInstance> queryResult, ref FormInstanceFilterData filterData)
        {
            if (!string.IsNullOrWhiteSpace(filterData.Content))
            {
                filterData.Content = filterData.Content.RemoveDiacritics().ToLower();
                queryResult = TextSearch(queryResult, filterData.Content);
            }

            return queryResult;
        }

        private IMongoQueryable<FormInstance> DoBasicFilter(IMongoQueryable<FormInstance> queryResult, FormInstanceFilterData filterData)
        {
            FilterDefinition<FormInstance> basicFilter = new BsonDocument
            {
                { "ThesaurusId", filterData.ThesaurusId },
                { "IsDeleted", false },
                { "Version._id", filterData.VersionId },
                { "OrganizationId", filterData.OrganizationId }
            };

            return queryResult.Where(x => basicFilter.Inject());
        }

        private IMongoQueryable<FormInstance> DoAdditionalFilter(IMongoQueryable<FormInstance> queryResult, FormInstanceFilterData filterData)
        {
            if (filterData.UserIds != null && filterData.UserIds.Count != 0)
            {
                FilterDefinition<FormInstance> userFilter =
                    new BsonDocument("UserId", new BsonDocument("$in", new BsonArray(filterData.UserIds)));
                queryResult = queryResult.Where(x => userFilter.Inject());
            }

            if (filterData.PatientIds != null && filterData.PatientIds.Count != 0)
            {
                FilterDefinition<FormInstance> patientFilter =
                    new BsonDocument("PatientId", new BsonDocument("$in", new BsonArray(filterData.PatientIds)));
                queryResult = queryResult.Where(x => patientFilter.Inject());
            }

            if (filterData.CustomFieldFiltersData != null && filterData.CustomFieldFiltersData.Count > 0 && filterData.FieldFiltersOverallOperator != null)
            {
                queryResult = GetByCustomFieldFilters(queryResult, filterData);
            }

            if (filterData.ProjectId != null) 
            {
                queryResult = queryResult.Where(x => x.ProjectId == filterData.ProjectId);
            }

            queryResult = queryResult.Where(x => x.ProjectId == null || filterData.PersonnelProjectsIds.Any(ppId => ppId == x.ProjectId));

            return queryResult;
        }

        private async Task<PaginationData<FormInstancePreview>> FormatFormInstancesFiltered(IMongoQueryable<FormInstance> queryResult, FormInstanceFilterData filterData)
        {
            int numOfAllMatchedElements = await queryResult.CountAsync().ConfigureAwait(false);

            List<string> customHeaderIds = filterData.CustomHeaderFields.Select(y => y.Id).ToList();

            List<FormInstancePreview> returnedElements;

            if (filterData.ColumnName != null && (filterData.ColumnName == AttributeNames.User || filterData.ColumnName == AttributeNames.Patient || filterData.ColumnName == AttributeNames.ProjectName))
            {
                returnedElements = queryResult
                    .AsEnumerable()
                    .Select(x => MapToFormInstancePreview(x, customHeaderIds))
                    .ToList();
            }
            else
            {
                IQueryable<FormInstance> sortedQuery = filterData.ColumnName != null ? SortQuery(queryResult, filterData) : queryResult.OrderByDescending(x => x.EntryDatetime);

                returnedElements = sortedQuery
                    .Skip((filterData.Page - 1) * filterData.PageSize)
                    .Take(filterData.PageSize)
                    .AsEnumerable()
                    .Select(x => MapToFormInstancePreview(x, customHeaderIds))
                    .ToList();
            }

            return new PaginationData<FormInstancePreview>(numOfAllMatchedElements, returnedElements);
        }

        private FormInstancePreview MapToFormInstancePreview(FormInstance instance, List<string> customHeaderIds)
        {
            return new FormInstancePreview
            {
                Id = instance.Id,
                Title = instance.Title,
                Version = instance.Version,
                Language = instance.Language,
                UserId = instance.UserId,
                OrganizationId = instance.OrganizationId,
                PatientId = instance.PatientId,
                EntryDatetime = instance.EntryDatetime,
                LastUpdate = instance.LastUpdate,
                FieldInstancesToDisplay = instance.FieldInstances.Where(field => customHeaderIds.Contains(field.FieldId)),
                ProjectId = instance.ProjectId
            };
        }

        private IQueryable<FormInstance> SortQuery(IMongoQueryable<FormInstance> result, FormInstanceFilterData filterData)
        {
            switch (filterData.ColumnName)
            {
                case AttributeNames.Version:
                    return filterData.IsAscending ?
                        result.OrderBy(x => x.Version.Major).ThenBy(x => x.Version.Minor) :
                        result.OrderByDescending(x => x.Version.Major).ThenByDescending(x => x.Version.Minor);
                case AttributeNames.Language:
                    var customOrder = new Dictionary<string, int>();
                    var orderedLanguages = filterData.Languages.OrderBy(x => x.Value);
                    int order = 0;

                    foreach (var kvp in orderedLanguages)
                    {
                        customOrder.Add(kvp.Key, order);
                        order++;
                    }

                    Func<FormInstance, object> orderLanguageByExpression = x =>
                    {
                        return customOrder.FirstOrDefault(co => co.Key == x.Language).Value;
                    };

                    return filterData.IsAscending ?
                               result.OrderBy(orderLanguageByExpression).AsQueryable() :
                               result.OrderByDescending(orderLanguageByExpression).AsQueryable();
                default:
                    Guid parsedGuid;
                    bool isValidGuid = Guid.TryParse(filterData.ColumnName, out parsedGuid);
                    if (isValidGuid)
                    {
                        Func<FormInstance, object> orderByExpression = x =>
                        {
                            var fieldInstance = x.FieldInstances.Find(fi => fi.FieldId == filterData.ColumnName);
                            var type = fieldInstance?.Type;
                            var fieldInstanceValue = fieldInstance?.FieldInstanceValues.FirstOrDefault();
                            var valueLabel = fieldInstanceValue != null
                                ? fieldInstanceValue.IsSpecialValue
                                    ? filterData.SpecialValues.FirstOrDefault(fiv => fiv.Key == fieldInstanceValue.Values.FirstOrDefault()).Value
                                    : fieldInstanceValue.ValueLabel
                                : null;

                            if (type == FieldTypes.Number && double.TryParse(valueLabel, out double numericValue))
                            {
                                return numericValue;
                            }
                            else
                            {
                                return valueLabel;
                            }
                        };

                        var sortedResult = filterData.IsAscending ?
                                            result.OrderBy(orderByExpression).AsQueryable() :
                                            result.OrderByDescending(orderByExpression).AsQueryable();

                        return sortedResult;
                    }
                    else
                    {
                        return SortTableHelper.OrderByField(result, filterData.ColumnName, filterData.IsAscending);
                    }

            }
        }

        private Form GetFormDefinition(string formDefinitionId)
        {
            return CollectionForm.AsQueryable().FirstOrDefault(x => !x.IsDeleted && x.Id.Equals(formDefinitionId));
        }

        private void InitOrUpdateChapterPageWorkflowHistory(FormInstance formInstance, int? createdById)
        {
            Form form = GetFormDefinition(formInstance.FormDefinitionId);
            formInstance.InitOrUpdateChapterPageWorkflowHistory(form, createdById);
        }

        private FormInstance GetFormInstance(string formInstanceId)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(formInstanceId));
        }

        private FormInstance GetFormInstanceFromDBAndDoConcurrencyCheck(string formInstanceId, DateTime incomingLastUpdate)
        {
            FormInstance formInstanceDb = GetFormInstance(formInstanceId);
            formInstanceDb.DoConcurrencyCheck(incomingLastUpdate);

            return formInstanceDb;
        }

        private async Task<bool> DeleteAsyncInBatches(FilterDefinition<FormInstance> filter)
        {
            int batchSize = 100;
            var totalCount = await Collection.CountDocumentsAsync(filter).ConfigureAwait(false);
            var remainingCount = totalCount;

            while (remainingCount > 0)
            {
                var batchFilter = filter;
                var currentBatchSize = Math.Min(batchSize, (int)remainingCount);
                var formInstancesToDelete = await Collection.Find(batchFilter).Limit(currentBatchSize).ToListAsync().ConfigureAwait(false);

                foreach (var formInstance in formInstancesToDelete)
                {
                    Entity.DoConcurrencyCheckForDelete(formInstance);
                }

                var update = Builders<FormInstance>.Update
                    .Set(x => x.IsDeleted, true)
                    .Set(x => x.LastUpdate, DateTime.Now);

                var result = await Collection.UpdateManyAsync(batchFilter, update).ConfigureAwait(false);
                if (!result.IsAcknowledged)
                {
                    return false;
                }

                remainingCount -= currentBatchSize;
            }

            return true;
        }

        private double CalculatePercentage(FormInstance formInstance)
        {
            if (formInstance == null || formInstance.FieldInstances == null || formInstance.FieldInstances.Exists(fi => fi.FieldInstanceValues == null))
                return 0;

            var totalValuesCount = formInstance.FieldInstances.SelectMany(f => f.FieldInstanceValues).Count();
            var nonEmptyValuesCount = formInstance.FieldInstances.SelectMany(f => f.FieldInstanceValues).Sum(v => v.Values.Count(value => !string.IsNullOrEmpty(value)));
            double percentage = (double)nonEmptyValuesCount / totalValuesCount * 100;

            return Math.Round(percentage, MidpointRounding.AwayFromZero);
        }

        public long FindAndReplace(FormInstance formInstanceForInsert)
        {
            var filter = Builders<FormInstance>.Filter.Eq(s => s.Id, formInstanceForInsert.Id);
            return Collection.ReplaceOne(filter, formInstanceForInsert).ModifiedCount;
        }

        public async Task<long> FindAndReplaceAsync(FormInstance formInstanceForInsert)
        {
            var filter = Builders<FormInstance>.Filter.Eq(s => s.Id, formInstanceForInsert.Id);
            return (await Collection.ReplaceOneAsync(filter, formInstanceForInsert).ConfigureAwait(false)).ModifiedCount;
        }

        public void UpdateOomniaExternalDocumentInstanceId(FormInstance formInstance)
        {
            if (SupportsTransaction)
            {
                using (var session = Collection.Database.Client.StartSession())
                {
                    try
                    {
                        session.StartTransaction(new TransactionOptions(readConcern: ReadConcern.Majority, writeConcern: WriteConcern.WMajority));
                        (FilterDefinition<FormInstance> filter, UpdateDefinition<FormInstance> update) = PrepareUpdateOomniaExternalDocumentInstanceId(formInstance);
                        Collection.UpdateOne(session, filter, update);

                        session.CommitTransaction();

                    }
                    catch (Exception ex)
                    {
                        session.AbortTransaction();
                        throw;
                    }
                }
            }
            else
            {
                (FilterDefinition<FormInstance> filter, UpdateDefinition<FormInstance> update) = PrepareUpdateOomniaExternalDocumentInstanceId(formInstance);
                Collection.UpdateOne(filter, update);
            }
        }

        public Field GetFieldByThesaurus(FormInstance formInstance, int thesaurusId)
        {
            return GetAllFields(formInstance).Find(x => x.ThesaurusId == thesaurusId);
        }

        private (FilterDefinition<FormInstance>, UpdateDefinition<FormInstance>) PrepareUpdateOomniaExternalDocumentInstanceId(FormInstance formInstance)
        {
            return (
                Builders<FormInstance>.Filter.Eq(x => x.Id, formInstance.Id), 
                Builders<FormInstance>.Update.Set(x => x.OomniaDocumentInstanceExternalId, formInstance.OomniaDocumentInstanceExternalId)
                );
        }

        private List<Field> GetAllFields(FormInstance formInstance)
        {
            FormDAL service = new FormDAL();
            Form form = new Form(formInstance, service.GetForm(formInstance.FormDefinitionId));
            return form.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(fieldSet => fieldSet
                                    .SelectMany(set => set.Fields
                                    )
                                )
                            )
            ).ToList();
        }
    }
}
