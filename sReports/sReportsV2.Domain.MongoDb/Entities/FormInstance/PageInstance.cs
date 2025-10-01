using sReportsV2.Domain.MongoDb.Entities.FormInstance;

namespace sReportsV2.Domain.Entities.FormInstance
{
    public class PageInstance : ChapterPageFieldSetInstanceBase
    {
        public string PageId { get; set; }
        public List<FieldSetInstance> FieldSetInstances { get; set; }

        public PageInstance(string pageId, int? createdById, DateTime createdOn) : base(createdById, createdOn)
        {
            PageId = pageId;
            FieldSetInstances = new List<FieldSetInstance>();
        }

        public FieldSetInstance GetFieldSetInstance(string fieldSetInstanceRepetitionId)
        {
            return FieldSetInstances.Find(pI => pI.FieldSetInstanceRepetitionId == fieldSetInstanceRepetitionId);
        }

        public override void RecordLatestWorkflowChangeStateAndPropagate(ChapterPageFieldSetInstanceStatus latestChangeState)
        {
            this.RecordLatestWorkflowChangeState(latestChangeState);
            foreach (var fieldSetInstance in FieldSetInstances)
            {
                fieldSetInstance.RecordLatestWorkflowChangeStateAndPropagate(latestChangeState);
            }
        }
    }
}
