using sReportsV2.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.DTOs.DTOs.FormInstance.DataIn
{
    public class FormInstanceLockUnlockRequestDataIn
    {
        [Required]
        public string FormInstanceId { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public FormState FormInstanceNextState { get; set; }
        [Required]
        public DateTime LastUpdate { get; set; }

        public bool IsLocked()
        {
            return FormInstanceNextState == FormState.Locked;
        }
    }
}
