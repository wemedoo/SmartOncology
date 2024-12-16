using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.DTOs.Organization.DataOut;
using sReportsV2.DTOs.DTOs.PersonnelTeam.DataOut;
using System;
using System.Collections.Generic;

namespace sReportsV2.DTOs.Organization
{
    public class OrganizationDataOut
    {
        public int Id { get; set; }
        public string RowVersion { get; set; }
        public string Description { get; set; }
        public string Impressum { get; set; }
        public List<string> Type { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public List<TelecomDTO> Telecoms { get; set; }
        public AddressDTO Address { get; set; }
        public OrganizationDataOut Parent { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public string LogoUrl { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public List<IdentifierDataOut> Identifiers { get; set; }
        public List<OrganizationClinicalDomainDataOut> ClinicalDomains { get; set; }
        public string Email { get; set; }
        public List<PersonnelTeamOrganizationRelationDataOut> PersonnelTeamOrganizationRelations { get; set; }
        public List<OrganizationCommunicationEntityDataOut> OrganizationCommunicationEntities { get; set; }
        public string TimeZone { get; set; }
        public string TimeZoneOffset { get; set; }
    }
}