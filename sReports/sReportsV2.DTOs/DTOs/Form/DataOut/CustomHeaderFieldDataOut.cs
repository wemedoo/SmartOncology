using sReportsV2.Common.Constants;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.Form.DataOut
{
    public class CustomHeaderFieldDataOut
    {
        public string FieldId { get; set; }
        public string Label { get; set; }
        public string CustomLabel { get; set; }
        public int Order { get; set; }
        public int? DefaultHeaderCode { get; set; }

        public string GetCustomLabelOrLabel()
        {
            return !string.IsNullOrWhiteSpace(CustomLabel) ? CustomLabel : Label;
        }

        public static List<CustomHeaderFieldDataOut> GetDefaultHeaders()
        {
            List<CustomHeaderFieldDataOut> headers = new List<CustomHeaderFieldDataOut>();
            int order = 0;
            foreach(var val in CustomHeaderConstants.GetCustomHeaderConstantList())
            {
                headers.Add(new CustomHeaderFieldDataOut() { Label = val.Label, DefaultHeaderCode= val.DefaultHeaderCode, Order = order});
                order++;
            }
            return headers;
        }

    }
}
