using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.CustomAttributes;
using sReportsV2.DTOs.DTOs.Organization.DataIn;
using sReportsV2.DTOs.DTOs.PersonnelTeam.DataIn;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.DTOs.Organization
{
    public class OrganizationDataIn
    {
        public int? Id { get; set; }
        public string RowVersion { get; set; }
        [Required]
        public string Email { get; set; }
        public string Description { get; set; }
        public string Impressum { get; set; }
        public List<string> Type { get; set; }
        [Display(Name = "Organization Name")]
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        [Required]
        public string Alias { get; set; }
        public List<OrganizationTelecomDataIn> Telecom { get; set; }
        [ObjectPropertiesRequired(new string[] { "City", "State", "PostalCode", "Street", "CountryCD" }, new string[] { "City", "State", "Postal Code", "Street", "Country" })]
        public AddressDTO Address { get; set; }
        public int? AddressId { get; set; }
        public int? ParentId { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public string LogoUrl { get; set; }
        public List<OrganizationIdentifierDataIn> Identifiers { get; set; }
        public List<OrganizationClinicalDomainDataIn> ClinicalDomains { get; set; }
        public List<PersonnelTeamDataIn> PersonnelTeams { get; set; }
        public List<OrganizationCommunicationEntityDataIn> OrganizationCommunicationEntities { get; set; }
        [Required]
        public string TimeZone { get; set; }
    }
}