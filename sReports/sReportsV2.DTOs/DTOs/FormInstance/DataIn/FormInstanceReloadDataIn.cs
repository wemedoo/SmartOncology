using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.FormInstance.DataIn
{
    public class FormInstanceReloadDataIn
    {
        public FormInstanceViewMode ViewMode { get; set; }
        public string FormInstanceId { get; set; }
        public bool IsReadOnlyViewMode { get; set; }
        public string ActiveChapterId { get; set; }
        public int? ActivePageLeftScroll { get; set; }
        public string ActivePageId { get; set; }
        public bool HiddenFieldsShown { get; set; }
    }
}
