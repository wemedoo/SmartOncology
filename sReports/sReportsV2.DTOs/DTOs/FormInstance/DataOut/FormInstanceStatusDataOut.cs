using sReportsV2.Common.Enums;
using sReportsV2.DTOs.DTOs.Form.DataOut;
using sReportsV2.DTOs.User.DataOut;
using System;

namespace sReportsV2.DTOs.DTOs.FormInstance.DataOut
{
    public class FormInstanceStatusDataOut : IFormStatusDataOut
    {
        public int CreatedById { get; set; }
        public UserShortInfoDataOut CreatedBy { get; set; }
        public string CreatedByActiveOrganization { get; set; }
        public DateTime CreatedOn { get; set; }
        public FormState FormInstanceStatus { get; set; }
        public bool IsSigned { get; set; }

        public dynamic StatusValue
        {
            get
            {
                return FormInstanceStatus;
            }
        }

        public DateTime CreatedDateTime
        {
            get
            {
                return CreatedOn;
            }
        }

        public string CreatedName
        {
            get
            {
                string name = string.Empty;
                if (CreatedBy != null)
                {
                    name = CreatedBy.Name;
                }
                return name;
            }
        }
    }
}
