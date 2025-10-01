using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Entities;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Implementations
{
    public class FormDAL : IFormDAL
    {
        private readonly IMongoCollection<Form> Collection;
        private readonly IMongoCollection<FormInstance> FormInstanceCollection;

        public FormDAL()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<Form>(MongoCollectionNames.Form);
            FormInstanceCollection = MongoDatabase.GetCollection<FormInstance>(MongoCollectionNames.FormInstance);
        }

        public bool ExistsForm(string id)
        {
            return Collection.Find(x => x.Id.Equals(id) && !x.IsDeleted).CountDocuments() > 0;
        }

        public List<Form> GetAll(FormFilterData filterData)
        {
            IQueryable<Form> result = GetFormsFiltered(filterData);

            if (filterData != null)
            {
                if (filterData.ColumnName != null)
                    result = SortByField(result, filterData);
                else
                    result = result.Skip(filterData.GetHowManyElementsToSkip())
                        .Take(filterData.PageSize);
            }

            return result.ToList();
        }

        public List<FormInstancePerDomain> GetFormInstancePerDomain()
        {
            var result = Collection.Aggregate()
                                   .Unwind<Form,FormOverridden>(x => x.DocumentProperties.ClinicalDomain)
                                   .Project(x => new FormInstancePerDomain()
                                   {
                                       Domain = x.DocumentProperties.ClinicalDomain,
                                       Count = x.DocumentsCount
                                   })
                                   .ToList()
                                   .GroupBy(x => x.Domain)
                                    .Select(x => new FormInstancePerDomain()
                                    {
                                        Domain = x.Key,
                                        Count = x.Count()
                                    })
                                    .ToList();
            return result.Where(x => x.Count != 0).ToList();
        }

        public Form GetForm(string id)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(id) && !x.IsDeleted);
        }

        public async Task<Form> GetFormAsync(string id)
        {
            return await GetFormTask(id).ConfigureAwait(false);
        }

        public Task<Form> GetFormTask(string id)
        {
            return Collection.AsQueryable().FirstOrDefaultAsync(x => x.Id.Equals(id) && !x.IsDeleted);
        }

        public Form GetFormByThesaurus(int thesaurusId)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.ThesaurusId.Equals(thesaurusId) && !x.IsDeleted);
        }

        public Form GetFormByThesaurusAndVersion(int thesaurusId,string versionId)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.ThesaurusId.Equals(thesaurusId) && !x.IsDeleted && x.Version.Id == versionId);
        }

        public Form GetFormByThesaurusAndLanguage(int thesaurusId, string language)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.ThesaurusId.Equals(thesaurusId) && x.Language.Equals(language) && !x.IsDeleted);
        }

        public bool Delete(string formId, DateTime lastUpdate)
        {
            Form formForDelete = GetForm(formId);
            Entity.DoConcurrencyCheckForDelete(formForDelete);
            formForDelete.DoConcurrencyBeforeDeleteCheck(lastUpdate);

            var filter = Builders<Form>.Filter.Eq(x => x.Id, formId);
            var update = Builders<Form>.Update.Set(x => x.IsDeleted, true).Set(x => x.LastUpdate, DateTime.Now);
            return Collection.UpdateOne(filter, update).IsAcknowledged;
        }

        public List<Form> GetFilteredDocumentsByThesaurusAppeareance(int o4mtId, string searchTerm, int thesaurusPageNum, int? organizationId)
        {
            return GetThesaurusAppereance(o4mtId, searchTerm, organizationId)
                  .Skip(thesaurusPageNum).Take(FilterConstants.DefaultPageSize).ToList();
        }

        public long GetThesaurusAppereanceCount(int o4mtId, string searchTerm, int? organizationId = null)
        {
            return GetThesaurusAppereance(o4mtId, searchTerm, organizationId).Count();
        }

        public Form InsertOrUpdate(Form form, UserData user, bool updateVersion = true)
        {
            form = Ensure.IsNotNull(form, nameof(form));
            user = Ensure.IsNotNull(user, nameof(user));
            form.ThesaurusIdsList = form.GetAllThesaurusIds().Distinct().ToList();

            if (string.IsNullOrEmpty(form.Id))
            {
                form.Version.Id = updateVersion ? Guid.NewGuid().ToString() : form.Version.Id;
                form.Copy(user, null);

                Collection.InsertOne(form);
            }
            else
            {
                Form formForUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(form.Id));
                formForUpdate.DoConcurrencyCheck(form.LastUpdate.Value);
                form.Copy(user, formForUpdate);
                form.UpdateCustomHeadersWhenFormUpdated();
                form.DocumentsCount = formForUpdate.DocumentsCount;
                var filter = Builders<Form>.Filter.Where(s => s.Id.Equals(form.Id));
                Collection.ReplaceOne(filter, form);
                UpdateFormIfModified(form);
            }

            return form;
        }

        private void UpdateFormIfModified(Form form)
        {
            bool isModified = false;

            foreach (var page in form.GetAllPages())
            {
                bool pageModified = RemoveDuplicateFieldSets(page);
                if (pageModified)
                    isModified = true;
            }

            if (isModified)
            {
                var filter = Builders<Form>.Filter.Where(s => s.Id.Equals(form.Id));
                Collection.ReplaceOne(filter, form);
            }
        }

        private bool RemoveDuplicateFieldSets(FormPage formPage)
        {
            if (formPage.ListOfFieldSets == null || formPage.ListOfFieldSets.Count == 0)
                return false;

            var fieldSetsInOtherLists = new HashSet<string>();
            bool modified = false;

            foreach (var fieldSetList in formPage.ListOfFieldSets)
            {
                foreach (var fieldSet in fieldSetList)
                {
                    foreach (var subFieldSet in fieldSet.ListOfFieldSets)
                    {
                        fieldSetsInOtherLists.Add(subFieldSet.Id);
                    }
                }
            } 

            foreach (var fieldSetList in formPage.ListOfFieldSets)
            {
                int originalCount = fieldSetList.Count;
                fieldSetList.RemoveAll(fs => fieldSetsInOtherLists.Contains(fs.Id));

                if (fieldSetList.Count != originalCount)
                    modified = true;
            }

            int listCountBefore = formPage.ListOfFieldSets.Count;
            formPage.ListOfFieldSets.RemoveAll(list => list.Count == 0);

            if (formPage.ListOfFieldSets.Count != listCountBefore)
                modified = true;

            return modified;
        }

        public long GetAllFormsCount(FormFilterData filterData)
        {
            return this.GetFormsFiltered(filterData).Count();
        }

        public bool ExistsFormByThesaurus(int thesaurusId)
        {
            return Collection
                .Find(x => x.ThesaurusId.Equals(thesaurusId) && !x.IsDeleted)
                .CountDocuments() > 0;
        }

        public Form GetFormByThesaurusAndLanguageAndVersionAndOrganization(int thesaurusId, int organizationId, string activeLanguage, string versionId)
        {
            return Collection
                .Find(x => !x.IsDeleted
                    && x.ThesaurusId.Equals(thesaurusId)
                    && x.Language.Equals(activeLanguage)
                    && x.Version.Id.Equals(versionId)
                    && x.OrganizationIds.Contains(organizationId))
                .FirstOrDefault();
        }

        public long GetFormByThesaurusAndLanguageAndVersionAndOrganizationCount(int thesaurusId, int organizationId, string activeLanguage, sReportsV2.Domain.Entities.Form.Version version)
        {
            return Collection
                .Find(x => !x.IsDeleted
                    && x.ThesaurusId.Equals(thesaurusId)
                    && x.Language.Equals(activeLanguage)
                    && x.Version.Major.Equals(version.Major)
                    && x.Version.Id != version.Id 
                    && x.Version.Minor.Equals(version.Minor) 
                    && x.OrganizationIds.Contains(organizationId))
                .CountDocuments();
        }

        public DocumentProperties GetDocumentProperties(string id)
        {
            return Collection
                .AsQueryable()
                .FirstOrDefault(x => x.Id.Equals(id) && !x.IsDeleted)
                ?.DocumentProperties;

        }

        public Task<List<Form>> GetAllByOrganizationAndLanguageAndNameAsync(int organization, string language, string name = "")
        {
            return Collection
                .Find(x => !x.IsDeleted 
                    && x.OrganizationIds.Contains(organization)
                    && x.Language == language
                    && !x.Invalid
                    && x.State == FormDefinitionState.ReadyForDataCapture
                    && (string.IsNullOrEmpty(name) || x.Title.ToLower().StartsWith(name.ToLower())))
                .SortBy(x => x.Title)
                .Project(x => new Form()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Version = x.Version,
                    ThesaurusId = x.ThesaurusId,
                    EntryDatetime = x.EntryDatetime
                })
                .ToListAsync();
        }

        private List<Form> GetFormsByThesaurusAndLanguageAndOrganization(int thesaurus, int organizationId, string activeLanguage)
        {
            return Collection.AsQueryable()
                           .Where(x => !x.IsDeleted
                               && x.ThesaurusId.Equals(thesaurus)
                               && x.Language.Equals(activeLanguage)
                               && x.OrganizationIds.Contains(organizationId))
                           .Select(x => new Form() {Id = x.Id , Version = x.Version })
                           .ToList();
        }

        public Form GetFormWithGreatestVersion(int thesaurusId, int activeOrganization, string activeLanguage)
        {
            List<Form> forms = this.GetFormsByThesaurusAndLanguageAndOrganization(thesaurusId, activeOrganization, activeLanguage);
            Form result =forms != null && forms.Count > 0 ? forms[0] : null;
            foreach (Form form in forms)
            {
                if (form.Version.IsVersionGreater(result.Version))
                {
                    result = form;
                }
            }

            return result;
        }

        public void DisableFormsByThesaurusAndLanguageAndOrganization(int thesaurus, int organizationId, string activeLanguage)
        {
            var filter = Builders<Form>.Filter.Where(x => !x.IsDeleted
                               && x.ThesaurusId.Equals(thesaurus)
                               && x.Language.Equals(activeLanguage)
                               && x.OrganizationIds.Contains(organizationId));

            var update =Builders<Form>.Update.Set(x => x.State, FormDefinitionState.Archive);

            Collection.UpdateMany(filter, update);
        }

        public int ReplaceThesaurus(ThesaurusMerge thesaurusMerge, UserData userData)
        {
            int i = 0;
            foreach (Form form in GetAll(null))
            {
                form.ReplaceThesauruses(thesaurusMerge);
                InsertOrUpdate(form, userData);
                i++;
            }
            
            return i;
        }

        public bool ThesaurusExist(int thesaurusId)
        {
            return GetDocumentsByThesaurusAppeareance(thesaurusId).Count > 0;
        }

        public List<Form> GetByFormIdsList(List<string> ids)
        {
            return Collection.Find(x => ids.Contains(x.Id)).ToList();
        }

        public Task<List<Form>> GetByFormIdsListAsync(List<string> ids)
        {
            return  Collection.Find(x => ids.Contains(x.Id) && !x.Invalid).ToListAsync();
        }

        public List<string> GetByClinicalDomains(List<int> clinicalDomains)
        {
            return Collection.AsQueryable()
                             .Where(x => x.DocumentProperties.ClinicalDomain.Any(c => clinicalDomains.Contains(c.Value)) && !x.Invalid)
                             .Select(f => f.Id)
                             .Distinct()
                             .ToList();
        }

        private List<Form> GetDocumentsByThesaurusAppeareance(int o4mtId)
        {
            return GetThesaurusAppereance(o4mtId).ToList();
        }

        private IEnumerable<Form> GetThesaurusAppereance(int o4mtId, string searchTerm = null, int? organizationId = null)
        {
            var result = Collection.AsQueryable().Where(x => !x.IsDeleted && x.ThesaurusIdsList.Contains(o4mtId));
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                result = result.Where(x => x.Title != null && x.Title.ToUpper().Contains(searchTerm.ToUpper()));
            }
            if (organizationId.HasValue)
            {
                result = result.Where(x => x.OrganizationIds.Contains(organizationId.Value));
            }
            return result;  
        }

        private IQueryable<Form> GetFormsFiltered(FormFilterData filterData)
        {
            IQueryable<Form> result = Collection.AsQueryable(new AggregateOptions() { AllowDiskUse = true }).Where(x => !x.IsDeleted);
            if (filterData != null)
            {                
                result = result.Where(x => x.Language != null && x.Language.Equals(filterData.ActiveLanguage)
                        && x.OrganizationIds.Contains(filterData.OrganizationId)
                        && (filterData.AdministrativeContext == null || (x.DocumentProperties != null && x.DocumentProperties.AdministrativeContext == filterData.AdministrativeContext))
                        && (filterData.Classes == null || (x.DocumentProperties != null && x.DocumentProperties.Class != null && x.DocumentProperties.Class.Class == filterData.Classes))
                        && (filterData.ClinicalContext == null || (x.DocumentProperties != null && x.DocumentProperties.ClinicalContext != null && x.DocumentProperties.ClinicalContext.ClinicalContext == filterData.ClinicalContext))
                        && (filterData.ClinicalDomain == null || (x.DocumentProperties != null && x.DocumentProperties.ClinicalDomain.Count > 0 && x.DocumentProperties.ClinicalDomain.Contains(filterData.ClinicalDomain)))
                        && (filterData.ExplicitPurpose == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose.ExplicitPurpose == filterData.ExplicitPurpose))
                        && (filterData.GeneralPurpose == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose != null && x.DocumentProperties.Purpose.GeneralPurpose != null && x.DocumentProperties.Purpose.GeneralPurpose.GeneralPurpose == filterData.GeneralPurpose))
                        && (filterData.ContextDependent == null || (x.DocumentProperties != null && x.DocumentProperties.Purpose != null && x.DocumentProperties.Purpose.GeneralPurpose != null && x.DocumentProperties.Purpose.GeneralPurpose.ContextDependent == filterData.ContextDependent))
                        && (filterData.ScopeOfValidity == null || (x.DocumentProperties != null && x.DocumentProperties.ScopeOfValidity != null && x.DocumentProperties.ScopeOfValidity.ScopeOfValidity == filterData.ScopeOfValidity))
                        && (filterData.State == null || x.State == filterData.State)
                        && (filterData.ThesaurusId == 0 || (x.ThesaurusId == filterData.ThesaurusId))
                        && (filterData.DateTimeTo == null || x.EntryDatetime < filterData.DateTimeTo)
                        && (filterData.DateTimeFrom == null || x.EntryDatetime > filterData.DateTimeFrom)
                        && (filterData.Ids == null || filterData.Ids.Count == 0 || filterData.Ids.Contains(x.Id))

                   );

                if (filterData.HideInvalidDocuments) 
                {
                    result = result.Where(x => !x.Invalid);
                }

                if (!string.IsNullOrWhiteSpace(filterData.Title))
                {
                    result = result.Where(x => x.Title.ToLower().Contains(filterData.Title.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(filterData.Content))
                {
                    string content = filterData.Content.RemoveDiacritics().ToLower();
                    List<string> formDefinitionIds = GetFormDefinitonIdsByFormInstanceContent(content);

                    result = result.Where(x => formDefinitionIds.Contains(x.Id));
                }

            }

            return result;
        }

        public List<BsonDocument> GetPlottableFields(string formId)
        {
            var matchStage = new BsonDocument("_id", new ObjectId(formId));

            var projectStage1 = new BsonDocument("ListOfFieldSets",
                                    new BsonDocument("$reduce",
                                    new BsonDocument
                                                {
                                                    { "input", "$Chapters.Pages.ListOfFieldSets" },
                                                    { "initialValue",
                                    new BsonArray() },
                                                    { "in",
                                    new BsonDocument("$concatArrays",
                                    new BsonArray
                                                        {
                                                            "$$value",
                                                            "$$this"
                                                        }) }
                                                }));

            var projectStage2 = new BsonDocument("ListOfFieldSets",
                                    new BsonDocument("$reduce",
                                    new BsonDocument
                                                {
                                                    { "input", "$ListOfFieldSets" },
                                                    { "initialValue",
                                    new BsonArray() },
                                                    { "in",
                                    new BsonDocument("$concatArrays",
                                    new BsonArray
                                                        {
                                                            "$$value",
                                                            "$$this"
                                                        }) }
                                                }));

            var projectStage3 = new BsonDocument("Fields",
                                    new BsonDocument("$reduce",
                                    new BsonDocument
                                                {
                                                    { "input", "$ListOfFieldSets.Fields" },
                                                    { "initialValue",
                                    new BsonArray() },
                                                    { "in",
                                    new BsonDocument("$concatArrays",
                                    new BsonArray
                                                        {
                                                            "$$value",
                                                            "$$this"
                                                        }) }
                                                }));

            //var projectStage4 = new BsonDocument
            //                            {
            //                                { "_id", 0 },
            //                                { "Fields",
            //                        new BsonDocument("$filter",
            //                        new BsonDocument
            //                                    {
            //                                        { "input", "$Fields" },
            //                                        { "as", "field" },
            //                                        { "cond",
            //                        new BsonDocument("$gte",
            //                        new BsonArray
            //                                            {
            //                                                "$$field.Values",
            //                                                BsonNull.Value
            //                                            }) }
            //                                    }) }};

            var matchFinalStage = new BsonDocument("Fields.Values",
                new BsonDocument("$elemMatch",
                new BsonDocument("NumericValue",
                new BsonDocument("$ne", BsonNull.Value))));

            return Collection.Aggregate()
                .Match(matchStage)
                .Project(projectStage1)
                .Project(projectStage2)
                .Project(projectStage2)
                .Project(projectStage3)
                .Unwind<BsonDocument>("Fields")
                .Match(matchFinalStage).ToList();
        }

        public async Task<string> InsertOrUpdateCustomHeaderFieldsAsync(Form form, UserData user, bool updateVersion = true)
        {
            form = Ensure.IsNotNull(form, nameof(form));
            user = Ensure.IsNotNull(user, nameof(user));
            form.ThesaurusIdsList = form.GetAllThesaurusIds().Distinct().ToList();

            var customHeaderFields = form.CustomHeaderFields;

            if (string.IsNullOrEmpty(form.Id))
            {
                form.Version.Id = updateVersion ? Guid.NewGuid().ToString() : form.Version.Id;
                form.Copy(user, null);
                form.CustomHeaderFields = customHeaderFields;
                Collection.InsertOne(form);
            }
            else
            {
                Form formForUpdate = await GetFormAsync(form.Id).ConfigureAwait(false);
                formForUpdate.DoConcurrencyCheck(form.LastUpdate.Value);
                form.Copy(user, formForUpdate);
                form.CustomHeaderFields = customHeaderFields;

                form.DocumentsCount = formForUpdate.DocumentsCount;
                var filter = Builders<Form>.Filter.Where(s => s.Id.Equals(form.Id));
                await Collection.ReplaceOneAsync(filter, form).ConfigureAwait(false);
            }

            return form.Id;
        }
        
        public async Task<IEnumerable<FieldSet>> GetAllFieldSetsByFormId(string formId)
        {
            var query = await Collection.AsQueryable()
                .Where(x => x.Id == formId)
                .SelectMany(x => x.Chapters)
                .SelectMany(x => x.Pages)
                .SelectMany(x => x.ListOfFieldSets).ToListAsync().ConfigureAwait(false);

            return query.SelectMany(listOfFS => listOfFS);
        }

        public Task<List<Form>> GetByTitleForAutoComplete(FormFilterData formFilterData)
        {
            var query = ApplyTitleFilter(formFilterData);

            Task<List<Form>> formsTask = query
                .Skip(formFilterData.GetHowManyElementsToSkip())
                .Limit(formFilterData.PageSize)
                .Project(x => new Form()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Version = x.Version,
                })
                .ToListAsync();

            return formsTask;
        }

        public Task<long> CountByTitle(FormFilterData formFilterData)
        {
            return ApplyTitleFilter(formFilterData).CountDocumentsAsync();
        }

        public async Task<List<Form>> GetByTitle(string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
                return await Collection.AsQueryable().Where(
                    x => !x.IsDeleted && x.Title.ToLower().Contains(title.ToLower()) && !x.Invalid
                    ).ToListAsync().ConfigureAwait(false);
            else
                return null;
        }

        public IEnumerable<FieldSet> GetFieldSetsByFieldLabels(string formId, List<string> fieldLabels, string fieldType)
        {
            if (fieldLabels == null || !fieldLabels.Any() || string.IsNullOrEmpty(fieldType))
            {
                return Enumerable.Empty<FieldSet>();
            }

            var query = Collection.AsQueryable()
                .Where(x => x.Id == formId)
                .SelectMany(x => x.Chapters)
                .SelectMany(x => x.Pages)
                .SelectMany(x => x.ListOfFieldSets)
                .ToList();

            var matchingFieldSets = query.SelectMany(listOfFS => listOfFS)
                .Where(fs =>
                {
                    var dbFieldLabels = fs.Fields.Select(field => field.Label).ToList();
                    var dbFieldTypes = fs.Fields.Select(field => field.Type).Distinct().ToList();

                    return !dbFieldLabels.Except(fieldLabels).Any() && !fieldLabels.Except(dbFieldLabels).Any()
                        && dbFieldTypes.Count == 1 && dbFieldTypes.First() == fieldType;
                })
                .ToList();

            return matchingFieldSets;
        }


        private IOrderedFindFluent<Form, Form> ApplyTitleFilter(FormFilterData filterData)
        {
            return Collection
                .Find(x => !x.IsDeleted && x.Language != null && x.Language.Equals(filterData.ActiveLanguage)
                        && x.OrganizationIds.Contains(filterData.OrganizationId)
                        && !x.Invalid
                        && (string.IsNullOrEmpty(filterData.Title) || x.Title.ToLower().Contains(filterData.Title.ToLower())))
                .SortBy(x => x.Title);
        }

        private IQueryable<Form> SortByField(IQueryable<Form> result, FormFilterData filterData)
        {
            switch (filterData.ColumnName)
            {
                case AttributeNames.Version:
                    if (filterData.IsAscending)
                        return result.OrderBy(x => x.Version.Major)
                                .ThenBy(x => x.Version.Minor)
                                .Skip(filterData.GetHowManyElementsToSkip())
                                .Take(filterData.PageSize);
                    else
                        return result.OrderByDescending(x => x.Version.Major)
                                .ThenByDescending(x => x.Version.Minor)
                                .Skip(filterData.GetHowManyElementsToSkip())
                                .Take(filterData.PageSize);
                case AttributeNames.Title:
                    if (filterData.IsAscending)
                        return result.AsEnumerable().OrderBy(x => x.Title)
                                .ThenBy(x => x.Version.Minor)
                                .Skip(filterData.GetHowManyElementsToSkip())
                                .Take(filterData.PageSize).AsQueryable();
                    else
                        return result.AsEnumerable().OrderByDescending(x => x.Title)
                                .ThenByDescending(x => x.Version.Minor)
                                .Skip(filterData.GetHowManyElementsToSkip())
                                .Take(filterData.PageSize).AsQueryable();
                case AttributeNames.State:
                    string designPending = filterData.FormStates[(int)FormDefinitionState.DesignPending];
                    string design = filterData.FormStates[(int)FormDefinitionState.Design];
                    string reviewPending = filterData.FormStates[(int)FormDefinitionState.ReviewPending];
                    string review = filterData.FormStates[(int)FormDefinitionState.Review];
                    string readyForDataCapture = filterData.FormStates[(int)FormDefinitionState.ReadyForDataCapture];
                    string archive = filterData.FormStates[(int)FormDefinitionState.Archive];

                    if (filterData.IsAscending)               
                        return result.AsEnumerable().OrderBy(x => (int)x.State == 0 ? designPending : (int)x.State == 1 ? design : (int)x.State == 2 ? reviewPending :
                                (int)x.State == 3 ? review : (int)x.State == 4 ? readyForDataCapture : archive)
                                .Skip(filterData.GetHowManyElementsToSkip())
                                .Take(filterData.PageSize).AsQueryable();
                    else
                        return result.AsEnumerable().OrderByDescending(x => (int)x.State == 0 ? designPending : (int)x.State == 1 ? design : (int)x.State == 2 ? reviewPending :
                                (int)x.State == 3 ? review : (int)x.State == 4 ? readyForDataCapture : archive)
                                .Skip(filterData.GetHowManyElementsToSkip())
                                .Take(filterData.PageSize).AsQueryable();
                case AttributeNames.Class:
                    if (filterData.IsAscending)
                        return result.AsEnumerable().OrderBy(x => x.DocumentProperties.Class?.Class?.ToString())
                                .Skip(filterData.GetHowManyElementsToSkip())
                                .Take(filterData.PageSize).AsQueryable();
                    else
                        return result.AsEnumerable().OrderByDescending(x => x.DocumentProperties.Class?.Class?.ToString())
                                .Skip(filterData.GetHowManyElementsToSkip())
                                .Take(filterData.PageSize).AsQueryable();
                case AttributeNames.ExplicitPurpose:
                    if (filterData.IsAscending)
                        return result.AsEnumerable().OrderBy(x => x.DocumentProperties.Purpose?.ExplicitPurpose.ToString())
                                .Skip(filterData.GetHowManyElementsToSkip())
                                .Take(filterData.PageSize).AsQueryable();
                    else
                        return result.AsEnumerable().OrderByDescending(x => x.DocumentProperties.Purpose?.ExplicitPurpose.ToString())
                                .Skip(filterData.GetHowManyElementsToSkip())
                                .Take(filterData.PageSize).AsQueryable();
                case AttributeNames.ClinicalContext:
                    if (filterData.IsAscending)
                        return result.AsEnumerable().OrderBy(x => x.DocumentProperties.ClinicalContext?.ClinicalContext.ToString())
                                .Skip(filterData.GetHowManyElementsToSkip())
                                .Take(filterData.PageSize).AsQueryable();
                    else
                        return result.AsEnumerable().OrderByDescending(x => x.DocumentProperties.ClinicalContext?.ClinicalContext.ToString())
                                .Skip(filterData.GetHowManyElementsToSkip())
                                .Take(filterData.PageSize).AsQueryable();
                default:
                    return SortTableHelper.OrderByField(result, filterData.ColumnName, filterData.IsAscending)
                                .Skip(filterData.GetHowManyElementsToSkip())
                                .Take(filterData.PageSize);
            }
        }

        private List<string> GetFormDefinitonIdsByFormInstanceContent(string content)
        {
            IQueryable<FormInstance> result = FormInstanceCollection.AsQueryable(new AggregateOptions() { AllowDiskUse = true });

            FilterDefinition<FormInstance> contentFilter = Builders<FormInstance>.Filter.Text(content.PrepareForMongoStrictTextSearch());  
            return result
                .Where(x => contentFilter.Inject())
                .Select(x => x.FormDefinitionId).Distinct().ToList();
        }

        public bool IsNullFlavorUsedInAnyField(string formId, int nullFlavorId)
        {
            return Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(formId) && !x.IsDeleted)?
                .GetAllFields()?.Exists(field => field.NullFlavors.Contains(nullFlavorId)) ?? false;
        }

        public List<int> GetFormNullFlavors(string formId)
        {
            return Collection.AsQueryable()
                .Where(x => x.Id.Equals(formId) && !x.IsDeleted && x.NullFlavors != null)
                .Select(x => x.NullFlavors).FirstOrDefault();
        }

        public async Task<List<string>> GetGeneratedLanguages(int thesaurusId, int organizationId, Entities.Form.Version version)
        {
            return await Collection.AsQueryable().Where(x => !x.IsDeleted
                && x.ThesaurusId == thesaurusId
                && x.OrganizationIds.Contains(organizationId)
                && x.Version.Id == version.Id
                && x.Version.Major == version.Major
                && x.Version.Minor == version.Minor)
                .Select(x => x.Language)
                .ToListAsync()
                ;
        }
    }
}
