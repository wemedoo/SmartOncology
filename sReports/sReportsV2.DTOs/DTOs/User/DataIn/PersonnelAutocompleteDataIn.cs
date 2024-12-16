using sReportsV2.DTOs.Autocomplete;

namespace sReportsV2.DTOs.DTOs.User.DataIn
{
    public class PersonnelAutocompleteDataIn : AutocompleteDataIn
    {
        public int OrganizationId { get; set; }
        public bool FilterByDoctors { get; set; }
    }
}
