using Newtonsoft.Json;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.User.DataOut;
using System.Collections.Generic;
using System.Linq;
using TimeZoneConverter;

namespace sReportsV2.DTOs.User.DTO
{
    public class UserCookieData
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ActiveLanguage { get; set; }
        public int ActiveOrganization { get; set; }
        public int PageSize { get; set; }
        public string Email { get; set; }
        public List<RoleDataOut> Roles { get; set; }
        public List<UserOrganizationDataOut> Organizations { get; set; }
        public List<string> SuggestedForms { get; set; }
        public List<PositionPermissionDataOut> PositionPermissions { get; set; }
        public string LogoUrl { get; set; }
        public string TimeZoneOffset { get; set; }
        public string OrganizationTimeZone { get; set; }
        public string OrganizationTimeZoneIana { get; set; }
        public bool FormInstanceLoaded { get; set; }

        public List<OrganizationDataOut> GetNonArchivedOrganizations(int? archivedUserStateCD)
        {
            return Organizations.Where(x => x.StateCD != archivedUserStateCD).Select(x => x.Organization).ToList();
        }

        public string GetActiveOrganizationName()
        {
            return GetActiveOrganizationData()?.Name;
        }

        public bool UserHasPermission(string permissionName, string moduleName)
        {
            return PositionPermissions
                .Exists(p => p.ModuleName.Equals(moduleName) && p.PermissionName.Equals(permissionName));
        }

        public bool UserHasAnyOfRole(params string[] roleNames)
        {
            return Roles.Exists(r => roleNames.ToList().Exists(v => v == r.Name));
        }

        public string GetFirstAndLastName()
        {
            return this.FirstName + " " + this.LastName;
        }

        public string GetUserTimeZoneIana()
        {
            return string.IsNullOrEmpty(this.TimeZoneOffset) ? DateTimeExtension.DefaultTimezone : this.TimeZoneOffset;
        }

        public void UpdateAfterActiveOrganizationChange(Personnel entity, int newActiveOrganizationId)
        {
            this.ActiveOrganization = newActiveOrganizationId;
            this.OrganizationTimeZone = entity.GetActiveOrganizationTimeZoneId();
            this.OrganizationTimeZoneIana = TZConvert.WindowsToIana(this.OrganizationTimeZone);
            this.LogoUrl = entity.PersonnelConfig.ActiveOrganization.LogoUrl;
        }

        private OrganizationDataOut GetActiveOrganizationData()
        {
            return Organizations.Find(x => x.Organization.Id.Equals(ActiveOrganization)).Organization;
        }
    }
}