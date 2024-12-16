using sReportsV2.Domain.Entities.CustomFieldFilters;
using System.Collections.Generic;
using System.Text;

namespace sReportsV2.DTOs.DTOs.Field.DataOut.Custom
{
    public class CustomFieldFilterDataOut
    {
        public string CustomFieldFiltersId { get; set; }
        public List<CustomFieldFilterData> CustomFieldFiltersData { get; set; }
        public string FormDefinitonId { get; set; }
        public string OverallOperator { get; set; }

        public string RenderCustomFilterText()
        {
            StringBuilder textBuilder = new StringBuilder();
            for (int i = 0; i < CustomFieldFiltersData.Count; i++)
            {
                textBuilder.Append($"{CustomFieldFiltersData[i].FieldLabel} is {CustomFieldFiltersData[i].FilterOperator} to {CustomFieldFiltersData[i].Value}");
                if (i < CustomFieldFiltersData.Count - 1)
                    textBuilder.Append($" {OverallOperator.ToUpper()} ");
            }
            return textBuilder.ToString();
        }
    }

    
}
