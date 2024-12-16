using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.Common.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.Form.DataOut
{
    public class FormCommentDataOut
    {
        public string Id { get; set; }
        public int? CommentStateCD { get; set; }
        public string Value { get; set; }
        public string ItemRef { get; set; }
        public string CommentRef { get; set; }
        public string FormRef { get; set; }
        public int UserId { get; set; }
        public DateTimeOffset EntryDatetime { get; set; }
        public UserDataOut User { get; set; }

        public string ConvertCommentStateCDToDisplayName(List<CodeDataOut> commentStates, string language)
        {
            if (this.CommentStateCD != null && this.CommentStateCD.HasValue)
                return commentStates.Find(x => x.Id == this.CommentStateCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);

            return "";
        }
    }
}