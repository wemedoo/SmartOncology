using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.Patient.DataOut;
using sReportsV2.DTOs.User.DataOut;
using System;
using System.Collections.Generic;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using System.Linq;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.DTOs.Form.DataOut;
using sReportsV2.DTOs.DTOs.ProjectManagement.DataOut;

namespace sReportsV2.DTOs.FormInstance.DataOut
{
    public class FormInstanceTableDataOut
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public sReportsV2.Domain.Entities.Form.Version Version { get; set; }
        public string Language { get; set; }
        public DateTime? EntryDatetime { get; set; }
        public DateTime LastUpdate { get; set; }
        public UserShortInfoDataOut User { get; set; }
        public PatientTableDataOut Patient { get; set; }
        public int UserId { get; set; }
        public int PatientId { get; set; }
        public int? ProjectId { get; set; }
        public ProjectTableDataOut Project { get; set; }
        public List<FieldDataOut> FieldsToDisplay { get; set; } = new List<FieldDataOut>();
        public Dictionary<string, string> SpecialValues { get; set; } = new Dictionary<string, string>();

        public string GetFieldValueToDisplay(string fieldId)
        {
            FieldDataOut fieldDataOut = FieldsToDisplay.Find(x => x.Id == fieldId);

            string displayValue = string.Empty;

            if (fieldDataOut != null)
            {
                if (fieldDataOut.IsSpecialValue())
                    displayValue = fieldDataOut.GetSpecialValueLabel(SpecialValues);
                else
                    displayValue = fieldDataOut.GetValueLabel();
            }

            return displayValue;
        }

        public string GetDefaultColumnValue(CustomHeaderFieldDataOut customHeaderField, string TimeZoneOffset, string DateFormat)
        {
            string value = string.Empty;
            if(customHeaderField != null)
            {
                switch (customHeaderField.Label)
                {
                    case CustomHeaderConstants.User:
                        value = User != null ? User.Name : string.Empty;
                        break;
                    case CustomHeaderConstants.Version:
                        value = Version != null ? Version.GetFullVersionString() : string.Empty;
                        break;
                    case CustomHeaderConstants.Language:
                        value = Language ?? string.Empty;
                        break;
                    case CustomHeaderConstants.Patient:
                        value = Patient != null ? Patient.GetPatientShortInfoString(DateFormat) : CustomHeaderConstants.Unknown;
                        break;
                    case CustomHeaderConstants.LastUpdate:
                        value = LastUpdate.ToTimeZoned(TimeZoneOffset, DateFormat);
                        break;
                    case CustomHeaderConstants.ProjectName:
                        value = Project != null ? Project.ProjectName : string.Empty;
                        break;
                }
            }
            return value;
        }

    }
}