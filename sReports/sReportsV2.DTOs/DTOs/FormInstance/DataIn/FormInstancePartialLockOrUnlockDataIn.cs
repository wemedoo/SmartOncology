using sReportsV2.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.DTOs.DTOs.FormInstance.DataIn
{
    public class FormInstancePartialLockOrUnlockDataIn
    {
        public string FormInstanceId { get; set; }   
        public string ChapterId { get; set; }   
        public string PageId { get; set; }   
        public string ActionEndpoint { get; set; }   
        public DateTime LastUpdate { get; set; }
        public ChapterPageState ChapterPageNextState { get; set; }
        [Required]
        public string Password { get; set; }

        public bool IsLockAction()
        {
            return ChapterPageNextState == ChapterPageState.Locked;
        }
    }
}
