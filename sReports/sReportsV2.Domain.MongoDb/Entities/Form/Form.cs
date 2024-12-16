using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Extensions;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Entities.User;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using sReportsV2.Domain.Entities.FormInstance;

namespace sReportsV2.Domain.Entities.Form
{
    [BsonIgnoreExtraElements]
    public class Form : Entity
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
        public Form(FormInstance.FormInstance formValue, Form form)
        {
            formValue = Ensure.IsNotNull(formValue, nameof(formValue));
            form = Ensure.IsNotNull(form, nameof(form));

            this.Id = formValue.FormDefinitionId;
            this.About = form.About;
            this.Chapters = form.Chapters;
            this.Title = formValue.Title;
            this.Version = formValue.Version;
            this.Language = formValue.Language;
            this.ThesaurusId = form.ThesaurusId;
            this.Notes = formValue.Notes;
            this.Date = formValue.Date;
            this.FormState = formValue.FormState;
            this.UserId = formValue.UserId;
            this.SetFieldInstances(formValue.FieldInstances);
            this.CustomHeaderFields = form.CustomHeaderFields;
            this.SetInitialOrganizationId(formValue.OrganizationId);
        }

        #region Getters
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

        public IEnumerable<FieldSet> GetFieldSetIdsInChapter(string chapterId)
        {
            return this.Chapters
                            .Where(chapter => chapter.Id == chapterId)
                            .SelectMany(chapter => chapter.Pages
                                .SelectMany(page => page.ListOfFieldSets
                                    .SelectMany(listOfFS => listOfFS)
                                )
                            );
        }

        public IEnumerable<FieldSet> GetFieldSetIdsInPage(string chapterId, string pageId)
        {

            return this.Chapters
                            .Where(chapter => chapter.Id == chapterId)
                            .SelectMany(chapter => chapter.Pages)
                            .Where(page => page.Id == pageId)
                            .SelectMany(page => page.ListOfFieldSets
                                    .SelectMany(listOfFS => listOfFS)
                                )
                            ;
        }

        public List<FieldSet> GetListOfFieldSetsByFieldSetId(string fsId)
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                            )).FirstOrDefault(x => x.Contains(this.GetAllFieldSets().Find(y => y.Id == fsId)));

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

        public (Dictionary<string, string>, Dictionary<string, string>) GetFieldToFieldSetMapping()
        {
            Dictionary<string, string> fieldToFieldSetMapping = new Dictionary<string, string>();
            Dictionary<string, string> fieldSetInstanceRepetitionIds = new Dictionary<string, string>();
            foreach (FieldSet fieldSet in CollectAllFieldSets())
            {
                fieldSetInstanceRepetitionIds.Add(fieldSet.Id, GuidExtension.NewGuidStringWithoutDashes());
                foreach (Field field in fieldSet.Fields)
                {
                    fieldToFieldSetMapping[field.Id] = fieldSet.Id;
                }
            }
            return (fieldToFieldSetMapping, fieldSetInstanceRepetitionIds);
        }

        private IEnumerable<FieldSet> CollectAllFieldSets()
        {
            return this.Chapters
                            .SelectMany(chapter => chapter.Pages
                                .SelectMany(page => page.ListOfFieldSets
                                    .SelectMany(listOfFS => listOfFS)
                                )
                            );
        }

        private IEnumerable<Field> CollectAllFields()
        {
            return CollectAllFieldSets().SelectMany(set => set.Fields);
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

        public void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.ThesaurusId = this.ThesaurusId == oldThesaurus ? newThesaurus : this.ThesaurusId;
            foreach (FormChapter chapter in this.Chapters)
            {
                chapter.ReplaceThesauruses(oldThesaurus, newThesaurus);
            }
        }

        public void GenerateTranslation(List<sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry> entries, string language)
        {
            string activeLanguage = this.Language;
            this.Language = language;
            this.Title = entries.Find(x => x.ThesaurusEntryId.Equals(ThesaurusId))?.GetPreferredTermByTranslationOrDefault(language, activeLanguage);

            foreach (FormChapter formChapter in this.Chapters)
            {
                formChapter.Title = entries.Find(x => x.ThesaurusEntryId.Equals(formChapter.ThesaurusId))?.GetPreferredTermByTranslationOrDefault(language, activeLanguage);
                formChapter.Description = entries.Find(x => x.ThesaurusEntryId.Equals(formChapter.ThesaurusId))?.GetDefinitionByTranslationOrDefault(language, activeLanguage);
                formChapter.GenerateTranslation(entries, language, activeLanguage);
            }
        }

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

        #region Set Form Instance
        public void SetFieldInstances(List<FieldInstance> fieldInstances)
        {
            fieldInstances = Ensure.IsNotNull(fieldInstances, nameof(fieldInstances));

            foreach (List<FieldSet> repetitiveFieldSetList in this.GetAllListOfFieldSets())
            {
                SetFieldInstances(repetitiveFieldSetList, fieldInstances);
            }
        }

        private void SetFieldInstances(List<FieldSet> repetitiveFieldSetList, List<FieldInstance> fieldInstances)
        {
            IEnumerable<FieldInstance> fieldInstancesRelatedToFieldSet = fieldInstances.Where(x => x.FieldSetId == repetitiveFieldSetList.FirstOrDefault()?.Id);

            PrepareFieldSet(repetitiveFieldSetList, fieldInstancesRelatedToFieldSet);

            foreach (Field field in repetitiveFieldSetList.SelectMany(x => x.Fields))
            {
                List<FieldInstanceValue> fieldInstanceValues = fieldInstancesRelatedToFieldSet
                    .FirstOrDefault(fI =>
                        fI.FieldSetInstanceRepetitionId == field.FieldSetInstanceRepetitionId
                        && fI.FieldId == field.Id
                        )
                    ?.FieldInstanceValues
                    ;
                if (fieldInstanceValues.HasAnyFieldInstanceValue())
                {
                    field.FieldInstanceValues = fieldInstanceValues;
                }
                else
                {
                    field.FieldInstanceValues = new List<FieldInstanceValue>()
                    {
                        new FieldInstanceValue(string.Empty)
                    };
                }
            }
        }

        private void PrepareFieldSet(List<FieldSet> repetitiveFieldSetList, IEnumerable<FieldInstance> fieldInstancesRelatedToFieldSet)
        {
            if (repetitiveFieldSetList.Count == 1)
            {
                IList<string> fieldSetInstanceRepetitionIds = fieldInstancesRelatedToFieldSet.Select(x => x.FieldSetInstanceRepetitionId).Distinct().ToList();

                FieldSet firstFieldSetInRepetition = repetitiveFieldSetList.First();

                if (fieldSetInstanceRepetitionIds.Any())
                {
                    firstFieldSetInRepetition.SetFieldSetInstanceRepetitionIds(fieldSetInstanceRepetitionIds[0]);
                    fieldSetInstanceRepetitionIds.RemoveAt(0);
                }
                else
                {
                    firstFieldSetInRepetition.SetFieldSetInstanceRepetitionIds(GuidExtension.NewGuidStringWithoutDashes());
                }

                foreach (string fieldSetInstanceRepetitionId in fieldSetInstanceRepetitionIds)
                {
                    FieldSet repetitiveFieldSet = firstFieldSetInRepetition.Clone();
                    repetitiveFieldSet.SetFieldSetInstanceRepetitionIds(fieldSetInstanceRepetitionId);
                    repetitiveFieldSetList.Add(repetitiveFieldSet);
                }
            }
        }

        #endregion /Set Form Instance

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

        #region Referrable logic
        public List<Field> GetAllFieldsFromNonRepetititveFieldSets()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets.Where(x => !x[0].IsRepetitive)
                                .SelectMany(list => list.SelectMany(set => set.Fields)
                                )
                            )
                        ).ToList();
        }

        public void SetValuesFromReferrals(List<Form> formInstances)
        {
            formInstances = Ensure.IsNotNull(formInstances, nameof(formInstances));
            this.SetMatchedFieldSets(formInstances);
            List<Field> allReferralsFields = formInstances.SelectMany(x => x.GetAllFieldsFromNonRepetititveFieldSets()).ToList();
            SetFieldsFromNonRepetitiveFieldSets(allReferralsFields);
        }

        private void SetFieldsFromNonRepetitiveFieldSets(List<Field> allReferralsFields)
        {
            foreach (Field field in this.GetAllFieldsFromNonRepetititveFieldSets())
            {
                Field referralField = allReferralsFields.Find(x => x.ThesaurusId == field.ThesaurusId && x.Type == field.Type);
                if (referralField != null && referralField.FieldInstanceValues != null)
                {
                    field.FieldInstanceValues = referralField.FieldInstanceValues;
                }
            }
        }

        private void SetMatchedFieldSets(List<Form> formInstances)
        {
            formInstances = Ensure.IsNotNull(formInstances, nameof(formInstances));
            List<List<FieldSet>> referralsRepetiveFieldSets = formInstances.SelectMany(x => x.GetFieldSetsByRepetitivity(true)).ToList();
            foreach (List<FieldSet> formFieldSet in this.GetFieldSetsByRepetitivity(true))
            {
                foreach (List<FieldSet> referralFieldSet in referralsRepetiveFieldSets)
                {
                    if (formFieldSet[0].IsReferable(referralFieldSet[0]))
                    {
                        SetInstanceId(referralFieldSet, formFieldSet);
                        formFieldSet.RemoveAt(0);
                        formFieldSet.AddRange(referralFieldSet);
                        break;
                    }
                }
            }
        }

        private void SetInstanceId(List<FieldSet> referralFieldSet, List<FieldSet> formFieldSet)
        {
            foreach (FieldSet item in referralFieldSet)
            {
                item.Id = formFieldSet[0].Id;
            }
        }

        public List<ReferalInfo> GetValuesFromReferrals(List<Form> formInstances, Dictionary<int, Dictionary<int, string>> missingValuesDict)
        {
            List<ReferalInfo> result = new List<ReferalInfo>();

            result.AddRange(this.GetReferalInfoFromRepetitiveFieldSets(formInstances, missingValuesDict));
            result.AddRange(this.GetReferalInfoFromNonRepetitiveFieldSets(formInstances, missingValuesDict));

            return result;
        }

        private List<ReferalInfo> GetReferalInfoFromRepetitiveFieldSets(List<Form> formInstances, Dictionary<int, Dictionary<int, string>> missingValuesDict)
        {
            formInstances = Ensure.IsNotNull(formInstances, nameof(formInstances));

            List<ReferalInfo> result = new List<ReferalInfo>();
            List<int> thesaurusesAdded = new List<int>();

            foreach (Form instance in formInstances)
            {
                ReferalInfo referalInfo = new ReferalInfo(instance);

                foreach (List<FieldSet> formFieldSet in this.GetFieldSetsByRepetitivity(true))
                {
                    foreach (List<FieldSet> referralFieldSet in instance.GetFieldSetsByRepetitivity(true))
                    {
                        if (formFieldSet[0].IsReferable(referralFieldSet[0]) && !thesaurusesAdded.Contains(formFieldSet[0].ThesaurusId))
                        {
                            thesaurusesAdded.Add(formFieldSet[0].ThesaurusId);
                            AddReferrableFieldsFromRepetitiveFieldSets(referalInfo, referralFieldSet, missingValuesDict);
                        }
                    }
                }

                result.Add(referalInfo);
            }

            return result;
        }

        private void AddReferrableFieldsFromRepetitiveFieldSets(ReferalInfo referalInfo, List<FieldSet> referralFieldSet, Dictionary<int, Dictionary<int, string>> missingValuesDict)
        {
            Ensure.IsNotNull(referalInfo, nameof(referalInfo));
            Ensure.IsNotNull(referralFieldSet, nameof(referralFieldSet));

            for (int i = 0; i < referralFieldSet.Count; i++)
            {
                foreach (Field referralField in referralFieldSet[i].Fields)
                {
                    AddReferrableFieldToReferralInfo(referralField.HasValue(), referalInfo, referralField, missingValuesDict, i);
                }
            }
        }

        private List<ReferalInfo> GetReferalInfoFromNonRepetitiveFieldSets(List<Form> formInstances, Dictionary<int, Dictionary<int, string>> missingValuesDict)
        {
            formInstances = Ensure.IsNotNull(formInstances, nameof(formInstances));
            List<ReferalInfo> result = new List<ReferalInfo>();
            List<ReferralForm> allReferrals = formInstances
                .Select(x => new ReferralForm(x))
                .ToList();


            foreach (ReferralForm referral in allReferrals)
            {
                ReferalInfo referalInfo = new ReferalInfo(referral);

                foreach (Field field in this.GetAllFieldsFromNonRepetititveFieldSets())
                {
                    Field referralField = referral.Fields.Find(x => x.ThesaurusId == field.ThesaurusId && x.Type == field.Type);

                    if (referralField != null/* && !addedThesauruses.Contains(referralField.ThesaurusId)*/)
                    {
                        AddReferrableFieldToReferralInfo(
                            (IsFieldString(referralField) || IsFieldSelectable(referralField)) && referralField.HasValue(), 
                            referalInfo, 
                            referralField,
                            missingValuesDict
                            );
                    }
                }
                result.Add(referalInfo);
            }

            return result;
        }

        private void AddReferrableFieldToReferralInfo(bool addReferralField, ReferalInfo referalInfo, Field referralField, Dictionary<int, Dictionary<int, string>> missingValuesDict, int fieldSetPosition = -1)
        {
            Ensure.IsNotNull(referalInfo, nameof(referalInfo));
            Ensure.IsNotNull(referralField, nameof(referralField));

            if (addReferralField)
            {
                string repetitiveSuffix = fieldSetPosition > -1 ? $"({fieldSetPosition})" : string.Empty;
                referalInfo.ReferrableFields.Add(new KeyValue
                {
                    Key = $"{referralField.Label}{repetitiveSuffix}",
                    Value = referralField.GetReferrableValue(missingValuesDict),
                    ThesaurusId = referralField.ThesaurusId
                });
            }
        }

        private bool IsFieldString(Field referralField)
        {
            return referralField is FieldString;
        }

        private bool IsFieldSelectable(Field referralField)
        {
            return referralField is FieldSelectable;
        }

        private List<List<FieldSet>> GetFieldSetsByRepetitivity(bool isRepetitive)
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                            )
                        ).Where(list => list[0].IsRepetitive == isRepetitive).ToList();

        }

        #endregion /Referrable logic

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
