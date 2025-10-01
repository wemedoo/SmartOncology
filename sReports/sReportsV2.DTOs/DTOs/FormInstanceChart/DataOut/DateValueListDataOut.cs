using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.FormInstanceChart.DataOut
{
    public class DateValueListDataOut
    {
        public DateValueListDataOut Add(double? value, long dateInMilliseconds)
        {
            Value.Add(value);
            Date.Add(dateInMilliseconds);
            return this;
        }
        public List<double?> Value { get; set; } = new List<double?>();
        public List<double> Date { get; set; } = new List<double>();
    }
}
