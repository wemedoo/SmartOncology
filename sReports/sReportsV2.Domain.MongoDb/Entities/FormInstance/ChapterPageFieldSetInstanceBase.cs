using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.FormInstance;

namespace sReportsV2.Domain.MongoDb.Entities.FormInstance
{
    public abstract class ChapterPageFieldSetInstanceBase
    {
        public List<ChapterPageFieldSetInstanceStatus> WorkflowHistory { get; set; }

        protected ChapterPageFieldSetInstanceBase(int? createdById, DateTime createdOn)
        {
            WorkflowHistory = new List<ChapterPageFieldSetInstanceStatus> {
                new ChapterPageFieldSetInstanceStatus(ChapterPageFieldSetState.DataEntryOnGoing, createdById, createdOn)
            };
        }

        public ChapterPageFieldSetInstanceStatus GetLastChange()
        {
            return WorkflowHistory?.LastOrDefault();
        }

        public void RecordLatestWorkflowChangeState(ChapterPageFieldSetInstanceStatus latestChangeState)
        {
            WorkflowHistory.Add(latestChangeState);
        }

        public abstract void RecordLatestWorkflowChangeStateAndPropagate(ChapterPageFieldSetInstanceStatus latestChangeState);
    }
}
