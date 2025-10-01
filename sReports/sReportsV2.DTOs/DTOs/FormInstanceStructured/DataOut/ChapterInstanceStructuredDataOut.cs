namespace sReportsV2.DTOs.Form.DataOut
{
    public partial class FormChapterDataOut
    {
        public bool DoesAllMandatoryFieldsHaveValue { get; set; }
        public bool IsLocked { get; set; }

        public bool ShouldLockActionBeShown(bool hasPermissionToLock)
        {
            return DoesAllMandatoryFieldsHaveValue && IsLocked && hasPermissionToLock;
        }
    }
}
