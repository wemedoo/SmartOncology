using sReportsV2.Common.CustomAttributes;
using System;

namespace sReportsV2.DTOs.Field.DataOut
{
    public class FieldStringDataOut : FieldDataOut
    {
        [DataProp]
        public bool IsRepetitive { get; set; }
        [DataProp]
        public int NumberOfRepetitions { get; set; }

        public override bool IsFieldRepetitive => IsRepetitive;

        public override Tuple<bool, bool, int> GetRepetitiveInfo()
        {
            bool possibleRepetitiveField = true;
            return new Tuple<bool, bool, int>(possibleRepetitiveField, IsRepetitive, NumberOfRepetitions);
        }
    }
}