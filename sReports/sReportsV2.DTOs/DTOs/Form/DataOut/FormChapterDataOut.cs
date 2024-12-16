using sReportsV2.Common.CustomAttributes;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormChapterDataOut
    {
        [DataProp]
        public string Id { get; set; }
        [DataProp]
        public string Title { get; set; }
        [DataProp]
        public string Description { get; set; }
        [DataProp]
        public int ThesaurusId { get; set; }
        [DataProp]
        public bool IsReadonly { get; set; }
        public List<FormPageDataOut> Pages { get; set; } = new List<FormPageDataOut>();
        public bool DoesAllMandatoryFieldsHaveValue { get; set; }
        public bool IsLocked { get; set; }

        public string GetHtmlId()
        {
            return $"chapter-{Id}";
        }

        public bool ShouldLockActionBeShown(bool hasPermissionToLock)
        {
            return DoesAllMandatoryFieldsHaveValue && !IsLocked && hasPermissionToLock;
        }
    }
}