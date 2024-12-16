using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using MongoDB.Driver.Linq;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Form;

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
            this.RecordLatestWorkflowChange(formInstanceStatus);
            this.OomniaDocumentInstanceExternalId = entity?.OomniaDocumentInstanceExternalId;
        }

        public void SetValueByThesaurusId(int thesaurusId, string value)
        {
            FieldInstance field = this.FieldInstances.Find(x => x.ThesaurusId == thesaurusId);
            field?.AddValue(value);
        }

        public string GetFieldValueById(string id)
        {
            return this.FieldInstances.Find(x => x.FieldId.Equals(id))?.FieldInstanceValues?.GetFirstValue();
        }

        public int GetUserIdWhoMadeAction()
        {
            return this.WorkflowHistory?.LastOrDefault()?.CreatedById ?? this.UserId;
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

        public void InitOrUpdateChapterPageWorkflowHistory(Form.Form form, int? createdById)
        {
            if (this.ChapterInstances == null)
            {
                this.ChapterInstances = new List<ChapterInstance>();
            }

            DateTime createdOn = DateTime.Now;
            foreach (FormChapter chapter in form.Chapters)
            {
                ChapterInstance chapterInstance = GetChapterInstance(chapter.Id);
                bool doesChapterInstanceAlreadyExist = chapterInstance != null;
                if (!doesChapterInstanceAlreadyExist)
                {
                    chapterInstance = new ChapterInstance(chapter.Id, createdById, createdOn);
                }

                foreach (FormPage page in chapter.Pages)
                {
                    PageInstance pageInstance = chapterInstance.GetPageInstance(page.Id);
                    bool doesPageInstanceAlreadyExist = pageInstance != null;
                    if (!doesPageInstanceAlreadyExist)
                    {
                        chapterInstance.PageInstances.Add(
                            new PageInstance(page.Id, createdById, createdOn)
                        );
                    }
                }

                if (!doesChapterInstanceAlreadyExist)
                {
                    this.ChapterInstances.Add(chapterInstance);
                }
            }
        }

        public void RecordLatestChapterOrPageChangeState(FormInstancePartialLock formInstancePartialLock)
        {
            ChapterInstance chapterInstance = GetChapterInstance(formInstancePartialLock?.ChapterId);
            if (chapterInstance != null)
            {
                bool pageAction = formInstancePartialLock.IsPageAction();
                if (pageAction)
                {
                    formInstancePartialLock.ActionType = PropagationType.Page;
                    PageInstance pageInstance = chapterInstance.GetPageInstance(formInstancePartialLock.PageId);
                    pageInstance.RecordLatestWorkflowChangeState(new ChapterPageInstanceStatus(formInstancePartialLock));
                }
                else
                {
                    formInstancePartialLock.ActionType = PropagationType.Chapter;
                    chapterInstance.RecordLatestWorkflowChangeState(new ChapterPageInstanceStatus(formInstancePartialLock));
                }
                DoPropagation(formInstancePartialLock);
            }
        }

        public void DoPropagation(FormInstancePartialLock formInstancePartialLock)
        {
            switch (formInstancePartialLock.ActionType)
            {
                case PropagationType.Page:
                    DoPropagationAfterPageStateChange(formInstancePartialLock);
                    break;
                case PropagationType.Chapter:
                    DoPropagationAfterChapterStateChange(formInstancePartialLock);
                    break;
                case PropagationType.FormInstance:
                    DoPropagationAfterFormInstanceStateChange(formInstancePartialLock);
                    break;
                default:
                    break;
            }
        }

        public (Dictionary<string, bool>, Dictionary<string, bool>, Dictionary<string, List<string>> chapterHierarchy) ExamineIfChaptersAndPagesAreLocked()
        {
            Dictionary<string, bool> chaptersState = new Dictionary<string, bool>();
            Dictionary<string, bool> pagesState = new Dictionary<string, bool>();
            Dictionary<string, List<string>> chapterHierarchy = new Dictionary<string, List<string>>();

            if (ChapterInstances != null)
            {
                foreach (ChapterInstance chapterInstance in ChapterInstances)
                {
                    ChapterPageInstanceStatus lastChange = chapterInstance.GetLastChange();
                    chaptersState.Add(chapterInstance.ChapterId, IsItemLocked(lastChange));
                    List<string> pageIdsWithinChapter = new List<string>();
                    foreach (PageInstance pageInstance in chapterInstance.PageInstances)
                    {
                        lastChange = pageInstance.GetLastChange();
                        pagesState.Add(pageInstance.PageId, IsItemLocked(lastChange));
                        pageIdsWithinChapter.Add(pageInstance.PageId);
                    }
                    chapterHierarchy.Add(chapterInstance.ChapterId, pageIdsWithinChapter);
                }
            }

            return (chaptersState, pagesState, chapterHierarchy);
        }

        public void RecordLatestWorkflowChange(FormInstanceStatus formInstanceStatus)
        {
            if (WorkflowHistory == null)
            {
                WorkflowHistory = new List<FormInstanceStatus>();
            }
            if (formInstanceStatus != null)
            {
                WorkflowHistory.Add(formInstanceStatus);
            }
        }

        private void CopyChapterPageWorkflowHistory(FormInstance formInstance)
        {
            if (formInstance != null)
            {
                this.ChapterInstances = formInstance.ChapterInstances;
            }
        }

        private bool IsItemLocked(ChapterPageInstanceStatus lastChange)
        {
            return lastChange != null && lastChange.IsLocked();
        }

        private void DoPropagationAfterPageStateChange(FormInstancePartialLock formInstancePartialLock)
        {
            ChapterPageInstanceStatus latestChangeToPropagate = new ChapterPageInstanceStatus(formInstancePartialLock);
            ChapterInstance chapterInstance = GetChapterInstance(formInstancePartialLock.ChapterId);
            bool shouldUpdateFormInstanceState = false;
            bool shouldUpdateChapterInstanceState = false;
            if (formInstancePartialLock.IsLockAction())
            {
                (Dictionary<string, bool> chaptersState, Dictionary<string, bool> pagesState, Dictionary<string, List<string>> chapterHierarchy) = this.ExamineIfChaptersAndPagesAreLocked();

                List<string> pageIdsWithinChapter = chapterHierarchy[chapterInstance.ChapterId];
                bool allPagesWithinChapterAreLocked = pageIdsWithinChapter.Where(pagesState.ContainsKey).Select(x => pagesState[x]).All(x => x);
                shouldUpdateChapterInstanceState = allPagesWithinChapterAreLocked;
                if (allPagesWithinChapterAreLocked)
                {
                    chaptersState[chapterInstance.ChapterId] = true;
                }

                bool allChaptersAreLocked = chaptersState.Values.All(x => x);
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
            DoPropagationUntilFormInstance(latestChangeToPropagate, shouldUpdateFormInstanceState);
        }

        private void DoPropagationAfterChapterStateChange(FormInstancePartialLock formInstancePartialLock)
        {
            ChapterPageInstanceStatus latestChangeToPropagate = new ChapterPageInstanceStatus(formInstancePartialLock);
            ChapterInstance chapterInstance = GetChapterInstance(formInstancePartialLock.ChapterId);
            if (chapterInstance != null)
            {
                foreach (PageInstance pageInstance in chapterInstance.PageInstances)
                {
                    pageInstance.RecordLatestWorkflowChangeState(latestChangeToPropagate);
                }
            }
            bool shouldUpdateFormInstanceState = false;
            if (formInstancePartialLock.IsLockAction())
            {
                (Dictionary<string, bool> chaptersState, _, _) = this.ExamineIfChaptersAndPagesAreLocked();
                bool allChaptersAreLocked = chaptersState.Values.All(x => x);
                shouldUpdateFormInstanceState = allChaptersAreLocked;
            }
            else
            {
                shouldUpdateFormInstanceState = IsFormInstanceLocked();
            }
            DoPropagationUntilFormInstance(latestChangeToPropagate, shouldUpdateFormInstanceState);
        }

        private void DoPropagationUntilFormInstance(ChapterPageInstanceStatus latestChangeToPropagate, bool shouldUpdateFormInstanceState)
        {
            if (shouldUpdateFormInstanceState)
            {
                FormInstanceStatus latestFormInstanceChange = new FormInstanceStatus(latestChangeToPropagate);
                this.FormState = latestFormInstanceChange.Status;
                RecordLatestWorkflowChange(latestFormInstanceChange);
            }
        }

        private void DoPropagationAfterFormInstanceStateChange(FormInstancePartialLock formInstancePartialLock)
        {
            ChapterPageInstanceStatus latestChangeToPropagate = new ChapterPageInstanceStatus(formInstancePartialLock);
            foreach (ChapterInstance chapterInstance in ChapterInstances)
            {
                chapterInstance.WorkflowHistory.Add(latestChangeToPropagate);
                foreach (PageInstance pageInstance in chapterInstance.PageInstances)
                {
                    pageInstance.RecordLatestWorkflowChangeState(latestChangeToPropagate);
                }
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
