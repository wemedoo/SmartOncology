using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using MongoDB.Driver.Linq;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.MongoDb.Entities.FormInstance;
using sReportsV2.Domain.Entities.FieldEntity;

namespace sReportsV2.Domain.Entities.FormInstance
{
    [BsonIgnoreExtraElements]
    public class FormInstance : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FormDefinitionId { get; set; }
        public string Title { get; set; }
        public sReportsV2.Domain.Entities.Form.Version Version { get; set; }
        public int EncounterRef { get; set; }
        public int EpisodeOfCareRef { get; set; }
        public string Notes { get; set; }
        public FormState? FormState { get; set; }
        public DateTime? Date { get; set; }
        public int ThesaurusId { get; set; }
        public string Language { get; set; }
        public int? ProjectId { get; set; }
        public List<FieldInstance> FieldInstances { get; set; }

        public List<string> Referrals { get; set; }

        public int UserId { get; set; }
        
        public int OrganizationId { get; set; }

        public int PatientId { get; set; }
        public List<FormInstanceStatus> WorkflowHistory { get; set; }
        public List<ChapterInstance> ChapterInstances { get; set; }
        public Guid? OomniaDocumentInstanceExternalId { get; set; }

        [BsonIgnore]
        public double NonEmptyValuePercentage { get; set; }
        //IMPORTANT! Fields can be deleted when M_202308231355_FieldInstanceValue_Property is executed
        [BsonIgnoreIfNull]
        public List<FieldInstance> Fields { get; set; }

        public FormInstance() { }
        public FormInstance(Form.Form form)
        {
            form = Ensure.IsNotNull(form, nameof(form));

            this.FormDefinitionId = form.Id.ToString();
            this.Title = form.Title;
            this.Version = form.Version;
            this.EntryDatetime = form.EntryDatetime;
            this.Language = form.Language;
            this.ThesaurusId = form.ThesaurusId;
            SetLastUpdate();
        }

        public void Copy(FormInstance entity, FormInstanceStatus formInstanceStatus)
        {
            base.Copy(entity);
            this.CopyChapterPageWorkflowHistory(entity);
            this.WorkflowHistory = entity?.WorkflowHistory ?? new List<FormInstanceStatus>();
            this.RecordLatestWorkflowChangeAndPropagate(formInstanceStatus);
            this.OomniaDocumentInstanceExternalId = entity?.OomniaDocumentInstanceExternalId;
        }

        public void SetValueByThesaurusId(int thesaurusId, string value)
        {
            FieldInstance field = this.FieldInstances.Find(x => x.ThesaurusId == thesaurusId);
            field?.AddValue(value);
        }

        public string GetFieldValueById(string fieldId)
        {
            return GetFieldInstanceByFieldId(fieldId)?.FieldInstanceValues?.GetFirstValue();
        }

        public FieldInstance GetFieldInstanceByFieldId(string fieldId, string fieldSetInstanceRepetitionId = null)
        {
            if (string.IsNullOrEmpty(fieldSetInstanceRepetitionId))
            {
                return this.FieldInstances.Find(x => x.FieldId.Equals(fieldId));
            }
            else
            {
                return this.FieldInstances.Find(x => x.FieldId.Equals(fieldId) && fieldSetInstanceRepetitionId == x.FieldSetInstanceRepetitionId);
            }
        }

        public int GetUserIdWhoMadeAction()
        {
            return this.WorkflowHistory?.LastOrDefault()?.CreatedById ?? this.UserId;
        }

        public void AddMissingFieldSetRepetitions(int inputFieldSetCount, FieldSet fieldSet)
        {
            List<string> fieldSetInstanceRepetitionIds = this.GetFieldSetInstanceRepetitionIds(fieldSet.Id);
            for (int i = fieldSetInstanceRepetitionIds.Count; i < inputFieldSetCount; i++)
            {
                string fieldSetInstanceRepetitionId = GuidExtension.NewGuidStringWithoutDashes();
                foreach (Field field in fieldSet.Fields)
                {
                    this.FieldInstances.Add(new FieldInstance(field, fieldSet.Id, fieldSetInstanceRepetitionId));
                }
            }
        }

        public List<string> GetFieldSetInstanceRepetitionIds(string fieldSetId)
        {
            return this.FieldInstances
                .Where(fI => fI.FieldSetId == fieldSetId)
                .Select(fI => fI.FieldSetInstanceRepetitionId)
                .Distinct().ToList();
        }

        public void ParseOrAddLastUpdate(string lastUpdate)
        {
            if (string.IsNullOrWhiteSpace(lastUpdate))
            {
                this.SetLastUpdate();
            }
            else
            {
                this.LastUpdate = Convert.ToDateTime(lastUpdate);
            }
        }

        #region Workflow History

        public FormInstanceStatus GetCurrentFormInstanceStatus(int? userId, bool isSigned = false)
        {
            FormInstanceStatus formInstanceStatus = null;

            if (userId.HasValue)
            {
                formInstanceStatus = new FormInstanceStatus(FormState.Value, userId.Value, isSigned);
            }

            return formInstanceStatus;
        }

        public void InitOrUpdateChapterPageFieldSetWorkflowHistory(Form.Form form, int? createdById)
        {
            if (this.ChapterInstances == null)
            {
                this.ChapterInstances = new List<ChapterInstance>();
            }

            if (this.FieldInstances == null)
            {
                this.FieldInstances = new List<FieldInstance>();
            }

            IDictionary<string, List<string>> fieldSetInstanceRepetitions = FieldInstances
                .GroupBy(x => x.FieldSetId)
                .ToDictionary(x => x.Key, x => x.Select(x => x.FieldSetInstanceRepetitionId).Distinct().ToList());

            DateTime createdOn = DateTime.Now;
            foreach (FormChapter chapter in form.Chapters)
            {
                ChapterInstance chapterInstance = GetChapterInstance(chapter.Id);
                bool chapterInstanceDoesntExist = ItemDoesNotExist(chapterInstance);
                if (chapterInstanceDoesntExist)
                {
                    chapterInstance = new ChapterInstance(chapter.Id, createdById, createdOn);
                }

                foreach (FormPage page in chapter.Pages)
                {
                    PageInstance pageInstance = chapterInstance.GetPageInstance(page.Id);
                    bool pageInstanceDoesntExist = ItemDoesNotExist(pageInstance);

                    if (pageInstanceDoesntExist)
                    {
                        pageInstance = new PageInstance(page.Id, createdById, createdOn);
                    }

                    if (pageInstance.FieldSetInstances == null)
                    {
                        pageInstance.FieldSetInstances = new List<FieldSetInstance>();
                    }

                    foreach (FieldSet fieldSet in page.ListOfFieldSets.SelectMany(x => x))
                    {
                        if (fieldSetInstanceRepetitions.TryGetValue(fieldSet.Id, out List<string> fieldSetInstanceRepetitionsIds))
                        {
                            foreach (string fieldSetInstanceRepetitionId in fieldSetInstanceRepetitionsIds)
                            {
                                FieldSetInstance fieldSetInstance = pageInstance.GetFieldSetInstance(fieldSetInstanceRepetitionId);
                                if (ItemDoesNotExist(fieldSetInstance))
                                {
                                    pageInstance.FieldSetInstances.Add(
                                        new FieldSetInstance(fieldSetInstanceRepetitionId, createdById, createdOn)
                                    );
                                }
                            }
                        }
                    }

                    if (pageInstanceDoesntExist)
                    {
                        chapterInstance.PageInstances.Add(pageInstance);
                    }
                }

                if (chapterInstanceDoesntExist)
                {
                    this.ChapterInstances.Add(chapterInstance);
                }
            }
        }

        public void RecordLatestChapterOrPageOrFieldSetChangeState(FormInstancePartialLock formInstancePartialLock)
        {
            ChapterInstance chapterInstance = GetChapterInstance(formInstancePartialLock?.ChapterId);
            if (chapterInstance != null)
            {
                formInstancePartialLock.SetPartialLockPropagationType();
                ChapterPageFieldSetInstanceStatus chapterPageFieldSetInstanceStatus = new ChapterPageFieldSetInstanceStatus(formInstancePartialLock);
                switch (chapterPageFieldSetInstanceStatus.PropagationType)
                {
                    case PropagationType.Chapter:
                        chapterInstance.RecordLatestWorkflowChangeStateAndPropagate(chapterPageFieldSetInstanceStatus);
                        break;
                    case PropagationType.Page:
                        PageInstance pageInstance = chapterInstance.GetPageInstance(formInstancePartialLock.PageId);
                        pageInstance.RecordLatestWorkflowChangeStateAndPropagate(chapterPageFieldSetInstanceStatus);
                        break;
                    case PropagationType.FieldSet:
                        FieldSetInstance fieldSetInstance = chapterInstance
                            .GetPageInstance(formInstancePartialLock.PageId)
                            .GetFieldSetInstance(formInstancePartialLock.FieldSetInstanceRepetitionId);
                        fieldSetInstance.RecordLatestWorkflowChangeStateAndPropagate(chapterPageFieldSetInstanceStatus);
                        break;
                    default:
                        throw new InvalidOperationException($"Not supported propagation type, propagation type: {formInstancePartialLock.ActionType}");
                }
                DoBackwardPropagation(formInstancePartialLock, chapterPageFieldSetInstanceStatus);
            }
        }

        private void DoBackwardPropagation(FormInstancePartialLock formInstancePartialLock, ChapterPageFieldSetInstanceStatus latestChangeToPropagate)
        {
            switch (formInstancePartialLock.ActionType)
            {
                case PropagationType.FieldSet:
                    DoBackwardPropagationAfterFieldSetStateChange(formInstancePartialLock, latestChangeToPropagate);
                    break;
                case PropagationType.Page:
                    DoBackwardPropagationAfterPageStateChange(formInstancePartialLock, latestChangeToPropagate);
                    break;
                case PropagationType.Chapter:
                    DoBackwardPropagationAfterChapterStateChange(formInstancePartialLock, latestChangeToPropagate);
                    break;
                default:
                    break;
            }
        }

        public FormInstanceItemLockingStatus ExamineIfChaptersAndPagesAndFieldsetsAreLocked()
        {
            FormInstanceItemLockingStatus formInstanceLockingStatus = new FormInstanceItemLockingStatus(false);
            if (ChapterInstances != null)
            {
                foreach (ChapterInstance chapterInstance in ChapterInstances)
                {
                    ChapterPageFieldSetInstanceStatus lastChange = chapterInstance.GetLastChange();
                    FormInstanceItemLockingStatus chapterInstanceItemLockingStatus = new FormInstanceItemLockingStatus(IsItemLocked(lastChange));
                    foreach (PageInstance pageInstance in chapterInstance.PageInstances)
                    {
                        lastChange = pageInstance.GetLastChange();
                        FormInstanceItemLockingStatus pageInstanceItemLockingStatus = new FormInstanceItemLockingStatus(IsItemLocked(lastChange));

                        if (pageInstance.FieldSetInstances != null)
                        {
                            foreach (FieldSetInstance fieldSetInstance in pageInstance.FieldSetInstances)
                            {
                                lastChange = fieldSetInstance.GetLastChange();
                                FormInstanceItemLockingStatus fieldSetInstanceItemLockingStatus = new FormInstanceItemLockingStatus(IsItemLocked(lastChange));
                                pageInstanceItemLockingStatus.UpdateChildLockingStatus(fieldSetInstance.FieldSetInstanceRepetitionId, fieldSetInstanceItemLockingStatus);
                            }
                        }
                        chapterInstanceItemLockingStatus.UpdateChildLockingStatus(pageInstance.PageId, pageInstanceItemLockingStatus);
                    }
                    formInstanceLockingStatus.UpdateChildLockingStatus(chapterInstance.ChapterId, chapterInstanceItemLockingStatus);
                }
            }

            return formInstanceLockingStatus;
        }

        public void RecordLatestWorkflowChangeAndPropagate(FormInstanceStatus formInstanceStatus, FormInstancePartialLock formInstancePartialLockForwardPropagate = null)
        {
            if (WorkflowHistory == null)
            {
                WorkflowHistory = new List<FormInstanceStatus>();
            }
            if (formInstanceStatus != null)
            {
                WorkflowHistory.Add(formInstanceStatus);
            }
            if (formInstancePartialLockForwardPropagate != null)
            {
                DoForwardPropagationAfterFormInstanceStateChange(new ChapterPageFieldSetInstanceStatus(formInstancePartialLockForwardPropagate));
            }
        }

        private void CopyChapterPageWorkflowHistory(FormInstance formInstance)
        {
            if (formInstance != null)
            {
                this.ChapterInstances = formInstance.ChapterInstances;
            }
        }

        private bool IsItemLocked(ChapterPageFieldSetInstanceStatus lastChange)
        {
            return lastChange != null && lastChange.IsLocked();
        }

        private bool ItemDoesNotExist<T>(T formInstanceItem) where T : ChapterPageFieldSetInstanceBase
        {
            return formInstanceItem == null;
        }

        private void DoBackwardPropagationAfterFieldSetStateChange(FormInstancePartialLock formInstancePartialLock, ChapterPageFieldSetInstanceStatus latestChangeToPropagate)
        {
            ChapterInstance chapterInstance = GetChapterInstance(formInstancePartialLock.ChapterId);
            PageInstance pageInstance = chapterInstance.GetPageInstance(formInstancePartialLock.PageId);
            bool shouldUpdateFormInstanceState;
            bool shouldUpdateChapterInstanceState;
            bool shouldUpdatePageInstanceState;
            if (formInstancePartialLock.IsLockAction())
            {
                FormInstanceItemLockingStatus formInstanceLockingStatus = this.ExamineIfChaptersAndPagesAndFieldsetsAreLocked();
                FormInstanceItemLockingStatus chapterInstanceLockingStatus = formInstanceLockingStatus.GetChild(chapterInstance.ChapterId);
                FormInstanceItemLockingStatus pageInstanceLockingStatus = chapterInstanceLockingStatus.GetChild(pageInstance.PageId);

                bool allFieldSetsWithinPageAreLocked = pageInstanceLockingStatus.AllChildrenLocked();
                shouldUpdatePageInstanceState = allFieldSetsWithinPageAreLocked;
                if (allFieldSetsWithinPageAreLocked)
                {
                    chapterInstanceLockingStatus.UpdateChildLockingStatus(pageInstance.PageId, true);
                }

                bool allPagesWithinChapterAreLocked = chapterInstanceLockingStatus.AllChildrenLocked();
                shouldUpdateChapterInstanceState = allPagesWithinChapterAreLocked;
                if (allPagesWithinChapterAreLocked)
                {
                    formInstanceLockingStatus.UpdateChildLockingStatus(chapterInstance.ChapterId, true);
                }

                shouldUpdateFormInstanceState = formInstanceLockingStatus.AllChildrenLocked();
            }
            else
            {
                shouldUpdatePageInstanceState = IsItemLocked(pageInstance.GetLastChange());
                shouldUpdateChapterInstanceState = IsItemLocked(chapterInstance.GetLastChange());
                shouldUpdateFormInstanceState = IsFormInstanceLocked();
            }

            if (shouldUpdatePageInstanceState)
            {
                pageInstance.RecordLatestWorkflowChangeState(latestChangeToPropagate);
            }

            if (shouldUpdateChapterInstanceState)
            {
                chapterInstance.RecordLatestWorkflowChangeState(latestChangeToPropagate);
            }
            DoBackwardPropagationUntilFormInstance(latestChangeToPropagate, shouldUpdateFormInstanceState);
        }

        private void DoBackwardPropagationAfterPageStateChange(FormInstancePartialLock formInstancePartialLock, ChapterPageFieldSetInstanceStatus latestChangeToPropagate)
        {
            ChapterInstance chapterInstance = GetChapterInstance(formInstancePartialLock.ChapterId);
            bool shouldUpdateFormInstanceState = false;
            bool shouldUpdateChapterInstanceState = false;
            if (formInstancePartialLock.IsLockAction())
            {
                FormInstanceItemLockingStatus formInstanceLockingStatus = this.ExamineIfChaptersAndPagesAndFieldsetsAreLocked();

                FormInstanceItemLockingStatus chapterInstanceLockingStatus = formInstanceLockingStatus.GetChild(chapterInstance.ChapterId);
                bool allPagesWithinChapterAreLocked = chapterInstanceLockingStatus.AllChildrenLocked();
                shouldUpdateChapterInstanceState = allPagesWithinChapterAreLocked;
                if (allPagesWithinChapterAreLocked)
                {
                    formInstanceLockingStatus.UpdateChildLockingStatus(chapterInstance.ChapterId, true);
                }

                bool allChaptersAreLocked = formInstanceLockingStatus.AllChildrenLocked();
                shouldUpdateFormInstanceState = allChaptersAreLocked;
            }
            else
            {
                shouldUpdateChapterInstanceState = IsItemLocked(chapterInstance.GetLastChange());
                shouldUpdateFormInstanceState = IsFormInstanceLocked();
            }

            if (shouldUpdateChapterInstanceState)
            {
                chapterInstance.RecordLatestWorkflowChangeState(latestChangeToPropagate);
            }
            DoBackwardPropagationUntilFormInstance(latestChangeToPropagate, shouldUpdateFormInstanceState);
        }

        private void DoBackwardPropagationAfterChapterStateChange(FormInstancePartialLock formInstancePartialLock, ChapterPageFieldSetInstanceStatus latestChangeToPropagate)
        {
            bool shouldUpdateFormInstanceState = false;
            if (formInstancePartialLock.IsLockAction())
            {
                FormInstanceItemLockingStatus formInstanceLockingStatus = this.ExamineIfChaptersAndPagesAndFieldsetsAreLocked();
                shouldUpdateFormInstanceState = formInstanceLockingStatus.AllChildrenLocked();
            }
            else
            {
                shouldUpdateFormInstanceState = IsFormInstanceLocked();
            }
            DoBackwardPropagationUntilFormInstance(latestChangeToPropagate, shouldUpdateFormInstanceState);
        }

        private void DoBackwardPropagationUntilFormInstance(ChapterPageFieldSetInstanceStatus latestChangeToPropagate, bool shouldUpdateFormInstanceState)
        {
            if (shouldUpdateFormInstanceState)
            {
                FormInstanceStatus latestFormInstanceChange = new FormInstanceStatus(latestChangeToPropagate);
                this.FormState = latestFormInstanceChange.Status;
                RecordLatestWorkflowChangeAndPropagate(latestFormInstanceChange);
            }
        }

        private void DoForwardPropagationAfterFormInstanceStateChange(ChapterPageFieldSetInstanceStatus latestChangeToPropagate)
        {
            foreach (ChapterInstance chapterInstance in ChapterInstances)
            {
                chapterInstance.RecordLatestWorkflowChangeStateAndPropagate(latestChangeToPropagate);
            }
        }

        private ChapterInstance GetChapterInstance(string chapterId)
        {
            return ChapterInstances.Find(cI => cI.ChapterId == chapterId);
        }

        public FormInstanceStatus GetLastChange()
        {
            return WorkflowHistory?.LastOrDefault();
        }

        public bool IsFormInstanceLocked()
        {
            return FormState == sReportsV2.Common.Enums.FormState.Locked;
        }

        #endregion /Worfkow History
    }
}
