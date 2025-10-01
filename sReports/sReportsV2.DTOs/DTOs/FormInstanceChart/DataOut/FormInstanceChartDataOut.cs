using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.FormInstanceChart.DataOut
{
    public class FormInstanceChartDataOut
    {
        public Dictionary<string, DateValueListDataOut> LabelValuesDict { get; set; }

        public FormInstanceChartDataOut()
        {
            LabelValuesDict = new Dictionary<string, DateValueListDataOut>();
        }

        public void AddToKeyIfExists(string label, double? value, long dateInMilliseconds)
        {
            if (LabelValuesDict.TryGetValue(label, out DateValueListDataOut labelValue))
            {
                labelValue.Add(value, dateInMilliseconds);
            }
            else
            {
                LabelValuesDict.Add(label, new DateValueListDataOut().Add(value, dateInMilliseconds));
            }
        }
    }
}
