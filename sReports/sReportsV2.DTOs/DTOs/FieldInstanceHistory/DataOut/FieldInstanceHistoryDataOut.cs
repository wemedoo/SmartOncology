using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.FieldInstanceHistory.DataOut
{
    public class FieldInstanceHistoryDataOut
    {
        public string Id { get; set; }
        public string FormInstanceId { get; set; }
        public string FieldLabel { get; set; }
        public string FieldSetLabel { get; set; }
        public int Revision { get; set; }
        public string ValueLabel { get; set; }
        public int UserId { get; set; }
        public string UserCompleteName { get; set; }
        public DateTime EntryDatetime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }

        public string FieldSetInstanceRepetitionId { get; set; }
        public string FieldSetId { get; set; }
        public string FieldId { get; set; }
        public string FieldInstanceRepetitionId { get; set; }
        public string Type { get; set; }
        public List<string> Values { get; set; }
        public bool IsSpecialValue { get; set; }

        public bool HasValue()
        {
            return Values != null && Values.Count > 0;
        }

        public string GetChangeValue(Dictionary<int, Dictionary<int, string>> missingValueList)
        {
            string changeValue;
            if (IsSpecialValue)
            {
                changeValue = GetCodeMissingValue(Values?.FirstOrDefault(), missingValueList);
            } 
            else
            {
                switch (Type)
                {
                    case FieldTypes.Date:
                        changeValue = ValueLabel.RenderDate();
                        break;
                    case FieldTypes.Datetime:
                        changeValue = ValueLabel.RenderDatetime();
                        break;
                    default:
                        changeValue = ValueLabel;
                        break;
                }
            }

            return changeValue ?? "Empty";
        }

        private string GetCodeMissingValue(string codeIdValue, Dictionary<int, Dictionary<int, string>> missingValues)
        {
            int.TryParse(codeIdValue, out int codeId);
            return missingValues
                        .Where(x => x.Key == GetMissingValueCodeSetId())
                        .SelectMany(c => c.Value)
                        .Where(v => v.Key == codeId)
                        .Select(v => v.Value)
                        .FirstOrDefault();
        }

        protected int GetMissingValueCodeSetId()
        {
            switch (Type)
            {
                case FieldTypes.Number:
                    return (int)CodeSetList.MissingValueNumber;
                case FieldTypes.Date:
                    return (int)CodeSetList.MissingValueDate;
                case FieldTypes.Datetime:
                    return (int)CodeSetList.MissingValueDateTime;
                default:
                    return (int)CodeSetList.NullFlavor;
            }
        }
    }
}
