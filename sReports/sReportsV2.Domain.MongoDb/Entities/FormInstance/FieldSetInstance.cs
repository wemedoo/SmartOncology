using sReportsV2.Domain.Entities.FormInstance;

namespace sReportsV2.Domain.MongoDb.Entities.FormInstance
{
    public class FieldSetInstance : ChapterPageFieldSetInstanceBase
    {
        public string FieldSetInstanceRepetitionId { get; set; }
        public FieldSetInstance(string fieldSetInstanceRepetitionId, int? createdById, DateTime createdOn) : base(createdById, createdOn)
        {
            FieldSetInstanceRepetitionId = fieldSetInstanceRepetitionId;
        }

        public override void RecordLatestWorkflowChangeStateAndPropagate(ChapterPageFieldSetInstanceStatus latestChangeState)
        {
            this.RecordLatestWorkflowChangeState(latestChangeState);
        }
    }
}
