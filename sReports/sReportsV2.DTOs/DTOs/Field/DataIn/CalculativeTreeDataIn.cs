using sReportsV2.DTOs.Common;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Field.DataIn
{
    public class CalculativeTreeDataIn : IViewModeDataIn
    {
        public List<CalculativeTreeItemDTO> Data { get; set; }
        public bool IsReadOnlyViewMode { get; set; }
    }
}