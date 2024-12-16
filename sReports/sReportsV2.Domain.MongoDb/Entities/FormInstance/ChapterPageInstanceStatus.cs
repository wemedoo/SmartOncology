using sReportsV2.Common.Enums;
using System;

namespace sReportsV2.Domain.Entities.FormInstance
{
    public class ChapterPageInstanceStatus
    {
        public ChapterPageState Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CreatedById { get; set; }
        public bool IsSigned { get; set; }
        public PropagationType? PropagationType { get; set; }

        public ChapterPageInstanceStatus(ChapterPageState status, int? createdById, DateTime createdOn) 
        {
            CreatedOn = createdOn;
            Status = status;
            CreatedById = createdById;
        }

        public ChapterPageInstanceStatus(FormInstancePartialLock formInstancePartialLock)
        {
            this.Status = formInstancePartialLock.NextState;
            this.CreatedOn = DateTime.Now;
            this.CreatedById = formInstancePartialLock.CreateById;
            this.IsSigned = formInstancePartialLock.IsSigned;
            this.PropagationType = formInstancePartialLock.ActionType;
        }

        public bool IsLocked()
        {
            return Status == ChapterPageState.Locked;
        }
    }
}
