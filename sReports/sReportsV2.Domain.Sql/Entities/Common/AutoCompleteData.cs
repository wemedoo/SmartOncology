using sReportsV2.Domain.Sql.Entities.User;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    public class AutoCompleteUserData
    {
        public int PersonnelId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }

        public AutoCompleteUserData(Personnel personnel)
        {
            PersonnelId = personnel.PersonnelId;
            FirstName = personnel.FirstName;
            LastName = personnel.LastName;
            UserName = personnel.Username;
        }

        public string DisplayName => $"{FirstName} {LastName} ({UserName})";
    }

    public class AutoCompleteData
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }
}
