using System;
using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataIn
{
    public class GlobalThesaurusUserDataIn
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? SourceCD { get; set; }
        public string Affiliation { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public List<int> Roles { get; set; } = new List<int>();
    }
}
