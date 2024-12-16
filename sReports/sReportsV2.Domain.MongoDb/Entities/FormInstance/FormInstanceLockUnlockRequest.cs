using sReportsV2.Common.Enums;
using System;

namespace sReportsV2.Domain.Entities.FormInstance
{
    public class FormInstanceLockUnlockRequest
    {
        public string FormInstanceId { get; set; }
        public FormState FormInstanceNextState { get; set; }
        public DateTime LastUpdate { get; set; }
        public int? CreatedById { get; set; }

        public bool IsLocked()
        {
            return FormInstanceNextState == FormState.Locked;
        }
    }
}
