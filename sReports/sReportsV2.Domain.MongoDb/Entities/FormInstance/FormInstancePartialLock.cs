using sReportsV2.Common.Enums;
using System;

namespace sReportsV2.Domain.Entities.FormInstance
{
    public class FormInstancePartialLock
    {
        public string FormInstanceId { get; set; }
        public string ChapterId { get; set; }
        public string PageId { get; set; }
        public int? CreateById { get; set; }
        public ChapterPageState NextState { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsSigned { get; set; }
        public PropagationType? ActionType { get; set; }

        public bool IsPageAction()
        {
            return !string.IsNullOrEmpty(PageId);
        }

        public bool IsLockAction()
        {
            return NextState == ChapterPageState.Locked;
        }

        public FormInstancePartialLock() { }

        public FormInstancePartialLock(FormState formInstanceNextState)
        {
            this.ActionType = sReportsV2.Common.Enums.PropagationType.FormInstance;
            this.NextState = formInstanceNextState == FormState.Locked ? ChapterPageState.Locked : ChapterPageState.DataEntryOnGoing;
            this.IsSigned = true;
        }
    }
}
