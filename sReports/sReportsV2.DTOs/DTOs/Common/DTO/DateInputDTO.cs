using System.Collections.Generic;

namespace sReportsV2.DTOs.Common
{
    public class DateInputDTO
    {
        public Dictionary<object, object> InputAttributes { get; set; }
        public Dictionary<object, object> InputBtnAttributes { get; set; }

        public string GetBtnAttribute(string name, string defaultValue)
        {
            return InputBtnAttributes != null && InputBtnAttributes.ContainsKey(name) ? InputBtnAttributes[name].ToString() : defaultValue;
        }
    }

    public class TimeInputDTO : DateInputDTO
    {
        public bool FullWidthHeight { get; set; }
        public string ContainerId { get; set; }
    }
}
