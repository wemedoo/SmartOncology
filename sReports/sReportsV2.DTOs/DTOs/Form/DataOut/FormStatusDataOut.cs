using sReportsV2.Common.Enums;
using sReportsV2.DTOs.DTOs.Form.DataOut;
using sReportsV2.DTOs.User.DataOut;
using System;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormStatusDataOut : IFormStatusDataOut
    {
        public FormDefinitionState Status { get; set; }
        public DateTime Created { get; set; }
        public UserShortInfoDataOut User { get; set; }

        public string GetStatusColor(string status) 
        {
            string color;

            if (status == "Disabled")
                color = "rect-disabled";
            else if (status == "ReadyForDataCapture")
                color = "rect-dataCapture";
            else if (status == "ReadyForReview")
                color = "rect-ForReview";
            else
                color = "rect-onGoing";

            return color;
        }

        public dynamic StatusValue
        {
            get
            {
                return Status;
            }
        }

        public DateTime CreatedDateTime
        {
            get
            {
                return Created;
            }
        }

        public string CreatedName
        {
            get
            {
                string name = string.Empty;
                if (User != null)
                {
                    name = User.Name;
                }
                return name;
            }
        }
    }
}