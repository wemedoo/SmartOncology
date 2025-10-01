using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.MongoDb.Entities.Base;
using sReportsV2.Domain.MongoDb.Entities.DocumentProperties;
using sReportsV2.Domain.MongoDb.Extensions;
using sReportsV2.Domain.Sql;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;

namespace sReportsV2.Domain.Entities.Form
{
    [BsonIgnoreExtraElements]
    public partial class Form : Entity, IFormThesaurusEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public FormAbout About { get; set; }
        public string Title { get; set; }
        public Version Version { get; set; }
        public List<FormChapter> Chapters { get; set; } = new List<FormChapter>();
        public FormDefinitionState State { get; set; }
        public string Language { get; set; }
        [BsonRepresentation(BsonType.Int32, AllowTruncation = true)]
        public int ThesaurusId { get; set; }
        public string Notes { get; set; }
        public FormState? FormState { get; set; }
        public DateTime? Date { get; set; }
        public DocumentProperties.DocumentProperties DocumentProperties { get; set; }
        public DocumentLoincProperties DocumentLoincProperties { get; set; }
        public List<FormStatus> WorkflowHistory { get; set; }
        public FormEpisodeOfCare EpisodeOfCare { get; set; }
        public bool DisablePatientData { get; set; }
        public int DocumentsCount { get; set; }
        public int UserId { get; set; }
        public List<int> OrganizationIds { get; set; } = new List<int>();
        public List<int> ThesaurusIdsList { get; set; }
        public string OomniaId { get; set; }
        public bool AvailableForTask { get; set; }
        public List<int> NullFlavors { get; set; } = new List<int>();
        public bool Invalid { get; set; }

        public List<CustomHeaderField> CustomHeaderFields { get; set; } = new List<CustomHeaderField>();

        public Form() 
        {
            WorkflowHistory = new List<FormStatus>();
        }

        #region Getters
        public string GetTitleWithVersion()
        {
            return $"{Title} ({Version.GetFullVersionString()})";
        }

        public List<FormPage> GetAllPages()
        {
            return this.Chapters
                .SelectMany(x => x.Pages).ToList();
        }

        #region FieldSets
        public List<FieldSet> GetAllFieldSets()
        {
            return CollectAllFieldSets().ToList();
        }

        public IEnumerable<FieldSet> GetFieldSetsInChapter(string chapterId)
        {
            return this.Chapters
                            .Where(chapter => chapter.Id == chapterId)
                            .SelectMany(chapter => chapter.Pages
                                .SelectMany(page => page.ListOfFieldSets
                                    .SelectMany(listOfFS => listOfFS)
                                )
                            );
        }

        public IEnumerable<FieldSet> GetFieldSetsInPage(string chapterId, string pageId)
        {
            return this.Chapters
                    .Where(chapter => chapter.Id == chapterId)
                    .SelectMany(chapter => chapter.Pages)
                    .Where(page => page.Id == pageId)
                    .SelectMany(page => page.ListOfFieldSets
                        .SelectMany(listOfFS => listOfFS)
                    );
        }

        public List<FieldSet> GetListOfFieldSetsByFieldSetId(string fsId)
        {
            return this.Chapters
                 .SelectMany(chapter => chapter.Pages
                     .SelectMany(page => page.ListOfFieldSets.GetAllFieldSets())
                 )
                 .Where(fieldSet => fieldSet.Id == fsId)
                 .ToList();
        }

        public List<List<FieldSet>> GetAllListOfFieldSets()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                            )
                        )
                        .ToList();
        }

        private IEnumerable<FieldSet> CollectAllFieldSets()
        {
            return this.Chapters
            .SelectMany(chapter => chapter.Pages
                .SelectMany(page =>
                {
                    var listOfFS = page.ListOfFieldSets;
                    IEnumerable<FieldSet> allFieldSets = Enumerable.Empty<FieldSet>();

                    foreach (var fieldsets in listOfFS)
                    {
                        var firstFieldSet = fieldsets.FirstOrDefault();
                        if (firstFieldSet != null && firstFieldSet.ListOfFieldSets.Any())
                        {
                            allFieldSets = allFieldSets.Concat(fieldsets.SelectMany(fs => fs.ListOfFieldSets));
                        }
                        else
                        {
                            allFieldSets = allFieldSets.Concat(fieldsets);
                        }
                    }

                    return allFieldSets;
                })
            );
        }

        private IEnumerable<Field> CollectAllFields()
        {
            return CollectAllFieldSets().SelectMany(field => field.Fields);
        }
        #endregion /FieldSets

        #region Fields
        public Field GetFieldById(string id)
        {
            return CollectAllFields().FirstOrDefault(x => x.Id == id);
        }

        public List<Field> GetAllFields()
        {
            return CollectAllFields().ToList();
        }

        public List<FieldSelectable> GetAllSelectableFields()
        {
            return CollectAllFields()
             .OfType<FieldSelectable>()
             .ToList();
        }

        public List<Field> GetAllNonPatientFields()
        {
            return this.Chapters.Where(x => !x.ThesaurusId.ToString().Equals(ResourceTypes.PatientThesaurus))
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(listOfFS => listOfFS
                                    .SelectMany(set => set.Fields
                                    )
                                )
                           )
                        ).ToList();
        }
        #endregion /Fields

        public List<FormFieldValue> GetAllFieldValues()
        {
            return GetAllSelectableFields()
                .SelectMany(x => x.Values).ToList();
        }
        #endregion /Getters

        #region Other methods
        public void Copy(UserData user, Form entity)
        {
            base.Copy(entity);
            this.WorkflowHistory = entity?.WorkflowHistory ?? new List<FormStatus>();
            this.SetWorkflow(user, State);
            this.CustomHeaderFields = entity?.CustomHeaderFields;
        }

        #region Thesaurus Methods

        public List<int> GetAllThesaurusIds()
        {
            List<int> thesaurusList = new List<int>
            {
                this.ThesaurusId
            };
            foreach (FormChapter formChapter in this.Chapters)
            {
                thesaurusList.Add(formChapter.ThesaurusId);
                thesaurusList.AddRange(formChapter.GetAllThesaurusIds());
            }

            return thesaurusList;
        }

        public void ReplaceThesauruses(ThesaurusMerge thesaurusMerge)
        {
            this.ThesaurusId = this.ThesaurusId.ReplaceThesaurus(thesaurusMerge);
            foreach (FormChapter chapter in this.Chapters)
            {
                chapter.ReplaceThesauruses(thesaurusMerge);
            }
        }

        public void GenerateTranslation(List<sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry> entries, string language, string activeLanguage)
        {
            this.Language = language;
            this.Title = entries.Find(x => x.ThesaurusEntryId.Equals(ThesaurusId))?.GetPreferredTermByTranslationOrDefault(language, activeLanguage);

            foreach (FormChapter formChapter in this.Chapters)
            {
                formChapter.Title = entries.Find(x => x.ThesaurusEntryId.Equals(formChapter.ThesaurusId))?.GetPreferredTermByTranslationOrDefault(language, activeLanguage);
                formChapter.Description = entries.Find(x => x.ThesaurusEntryId.Equals(formChapter.ThesaurusId))?.GetDefinitionByTranslationOrDefault(language, activeLanguage);
                formChapter.GenerateTranslation(entries, language, activeLanguage);
            }
        }

        #endregion /Thesaurus Methods

        public bool IsVersionChanged(Form formFromDatabase)
        {
            formFromDatabase = Ensure.IsNotNull(formFromDatabase, nameof(formFromDatabase));

            return this.Version.Major != formFromDatabase.Version.Major || this.Version.Minor != formFromDatabase.Version.Minor;
        }

        public void SetInitialOrganizationId(int organizationId)
        {
            if (this.OrganizationIds == null || !this.OrganizationIds.Any())
            {
                this.OrganizationIds = new List<int> { organizationId };
            }
        }

        public void OverrideOrganizationId(int organizationId)
        {
            this.OrganizationIds = new List<int> { organizationId };
        }

        public int GetActiveOrganizationId(int activeOrganizationId)
        {
            return this.OrganizationIds.Find(orgId => orgId == activeOrganizationId);
        }

        public int GetInitialOrganizationId()
        {
            return this.OrganizationIds.FirstOrDefault();
        }

        private void SetWorkflow(UserData user, FormDefinitionState state)
        {
            user = Ensure.IsNotNull(user, nameof(user));

            WorkflowHistory.Add(new FormStatus()
            {
                Created = DateTime.Now,
                Status = state,
                UserId = user.Id
            });
        }
        #endregion /Other methods

        #region Search-Insert Chapter/Page/Fieldset/Field

        public int? SearchChapterIndex(string chapterId)
        {
            return Chapters?.FindIndex(c => c.Id == chapterId);
        }
        public int? SearchPageIndex(string chapterId, string pageId)
        {
            return Chapters?.Find(c => c.Id == chapterId)
                .Pages?.FindIndex(p => p.Id == pageId);
        }
        public int? SearchFieldSetIndex(string chapterId, string pageId, string fieldSetId)
        {
            return Chapters?.Find(c => c.Id == chapterId)?
                .Pages?.Find(p => p.Id == pageId)?
                .ListOfFieldSets?.FindIndex(lof => lof.FirstOrDefault()?.Id == fieldSetId);
        }
        public int? SearchFieldIndex(string chapterId, string pageId, string fieldSetId, string fieldId)
        {
            return Chapters?.Find(c => c.Id == chapterId)?
                .Pages?.Find(p => p.Id == pageId)?
                .ListOfFieldSets?.Find(lof => lof.FirstOrDefault()?.Id == fieldSetId).FirstOrDefault()?
                .Fields?.FindIndex(f => f.Id == fieldId);
        }

        public void InsertChapters(List<FormChapter> chapters, int chapterIndex, bool afterDestination)
        {
            Chapters.InsertRange(chapterIndex + (afterDestination ? 1 : 0), chapters);
        }
        public void InsertPages(List<FormPage> pages, string chapterId, int pageIndex, bool afterDestination)
        {
            Chapters?.Find(c => c.Id == chapterId)?
                .Pages.InsertRange(pageIndex + (afterDestination ? 1 : 0), pages);
        }
        public void InsertFieldSets(List<List<FieldSet>> fieldsets, string chapterId, string pageId, int fieldSetIndex, bool afterDestination)
        {
            Chapters.Find(c => c.Id == chapterId)?
                .Pages?.Find(p => p.Id == pageId)?
                .ListOfFieldSets?.InsertRange(fieldSetIndex + (afterDestination ? 1 : 0), fieldsets);
        }
        public void InsertFields(List<Field> fields, string chapterId, string pageId, string fieldSetId, int fieldIndex, bool afterDestination)
        {
            Chapters.Find(c => c.Id == chapterId)?
                .Pages?.Find(p => p.Id == pageId)?
                .ListOfFieldSets?.Find(lof => lof.FirstOrDefault()?.Id == fieldSetId).FirstOrDefault()?
                .Fields?.InsertRange(fieldIndex + (afterDestination ? 1 : 0), fields);
        }

        #endregion

        #region Custom Headers

        public List<Field> GetFieldsByCustomHeader()
        {
            return GetAllFields().Where(field => CustomHeaderFields != null && CustomHeaderFields.Select(x => x.FieldId).Contains(field.Id)).DistinctByExtension(x => x.Id).ToList();
        }

        public void UpdateCustomHeadersWhenFormUpdated()
        {
            
            if (CustomHeaderFields != null && CustomHeaderFields.Count > 0)
            {
                IEnumerable<Field> fieldsInHeader = GetAllFields().Where(field => CustomHeaderFields.Select(x => x.FieldId).Contains(field.Id)).DistinctByExtension(x => x.Id);

                RemoveCustomHeaderWhenFieldDeleted(fieldsInHeader);

                foreach (Field fieldInHeader in fieldsInHeader)
                {
                    CustomHeaderField customHeaderField = CustomHeaderFields.Find(x => x.FieldId == fieldInHeader.Id);
                    if (fieldInHeader.Label != customHeaderField.Label)
                    {
                        customHeaderField.Label = fieldInHeader.Label;
                        customHeaderField.CustomLabel = fieldInHeader.Label;
                    }
                }

            }
        }

        private void RemoveCustomHeaderWhenFieldDeleted(IEnumerable<Field> fieldsInHeader)
        {
            CustomHeaderFields.RemoveAll(h => h.DefaultHeaderCode == null && !fieldsInHeader.Select(f => f.Id).Contains(h.FieldId));
        }

        #endregion
    }
}
