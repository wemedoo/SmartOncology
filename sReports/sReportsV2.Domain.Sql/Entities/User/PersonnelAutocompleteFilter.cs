namespace sReportsV2.Domain.Sql.Entities.User
{
    public class PersonnelAutocompleteFilter
    {
        public int OrganizationId { get; set; }
        public bool FilterByDoctors { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ArchivedUserStateId { get; set; }
    }
}
