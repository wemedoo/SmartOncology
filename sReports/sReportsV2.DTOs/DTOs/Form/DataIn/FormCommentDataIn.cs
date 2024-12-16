using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.DTOs.Form.DataIn
{
    public class FormCommentDataIn
    {
        public string Id { get; set; }
        public int? CommentStateCD { get; set; }
        [DataType(DataType.Html)]
        public string Value { get; set; }
        public string ItemRef { get; set; }
        public string CommentRef { get; set; }
        public string FormRef { get; set; }
        public int UserId { get; set; }
        public List<int> TaggedUsers { get; set; }
    }
}