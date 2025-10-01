using sReportsV2.Common.Enums;
using System;

namespace sReportsV2.Domain.Entities.FormInstance
{
    public class FormInstancePartialLock
    {
        public string FormInstanceId { get; set; }
        public string ChapterId { get; set; }
        public string PageId { get; set; }
        public string FieldSetInstanceRepetitionId { get; set; }
        public int? CreateById { get; set; }
        public ChapterPageFieldSetState NextState { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsSigned { get; set; }
        public PropagationType? ActionType { get; set; }

        public void SetPartialLockPropagationType()
        {
            if (!string.IsNullOrEmpty(FieldSetInstanceRepetitionId))
            {
                ActionType = PropagationType.FieldSet;
            }
            else if (!string.IsNullOrEmpty(PageId))
            {
                ActionType = PropagationType.Page;
            } 
            else if (!string.IsNullOrEmpty(ChapterId))
            {
                ActionType = PropagationType.Chapter;
            }
        }

        public bool IsLockAction()
        {
            return NextState == ChapterPageFieldSetState.Locked;
        }

        public FormInstancePartialLock() { }

        public FormInstancePartialLock(FormState formInstanceNextState)
        {
            this.ActionType = sReportsV2.Common.Enums.PropagationType.FormInstance;
            this.NextState = formInstanceNextState == FormState.Locked ? ChapterPageFieldSetState.Locked : ChapterPageFieldSetState.DataEntryOnGoing;
            this.IsSigned = true;
        }
    }
}
