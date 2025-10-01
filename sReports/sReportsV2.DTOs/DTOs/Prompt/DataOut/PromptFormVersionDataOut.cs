using System;
using System.Collections.Generic;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.User.DataOut;

namespace sReportsV2.DTOs.DTOs.Prompt.DataOut
{
    public class PromptFormVersionDataOut
    {
        public DateTime CreatedOn { get; set; }
        public string FormName { get; set; }
        public UserShortInfoDataOut CreatedBy { get; set; }
        public string Prompt { get; set; }
        public VersionDTO Version { get; set; }
        public List<PromptFieldDataOut> PromptFields { get; set; }

        public int TotalCDEs { get; set; }
        public int TotalPrompts { get; set; }

        public void PreparePrompts(FormDataOut form)
        {
            List<FieldDataOut> fields = form.GetAllFields();
            TotalCDEs = fields.Count + (string.IsNullOrEmpty(Prompt) ? 0 : 1);
            int totalPrompts = string.IsNullOrEmpty(Prompt) ? 0 : 1;
            foreach (PromptFieldDataOut promptField in PromptFields)
            {
                FieldDataOut field = fields.Find(f => f.Id == promptField.FieldId);
                if (field != null && !string.IsNullOrEmpty(promptField.Prompt))
                {
                    promptField.Field = field;
                    totalPrompts++;
                }
            }
            TotalPrompts = totalPrompts;
        }
    }
}
