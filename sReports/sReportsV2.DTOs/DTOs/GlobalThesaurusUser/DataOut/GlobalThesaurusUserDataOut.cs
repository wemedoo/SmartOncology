using sReportsV2.Common.Enums;
using sReportsV2.DTOs.CodeEntry.DataOut;
using sReportsV2.DTOs.DTOs.AccessManagment.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace sReportsV2.DTOs.DTOs.GlobalThesaurusUser.DataOut
{
    public class GlobalThesaurusUserDataOut
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
        public DateTimeOffset EntryDatetime { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public int? StatusCD { get; set; }
        public virtual List<RoleDataOut> Roles { get; set; } = new List<RoleDataOut>();

        public List<Claim> GetClaims()
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, Email),
                new Claim(ClaimTypes.Name, Email)
            };

            if (this.Roles != null)
            {
                foreach (var role in this.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }
            }

            return claims;
        }

        public bool IsRoleChecked(int roleId)
        {
            return Roles.Exists(r => r.Id == roleId);
        }

        public string ConvertStatusCDToDisplayName(List<CodeDataOut> statuses, string language)
        {
            if (this.StatusCD != null && this.StatusCD.HasValue)
                return statuses.Find(x => x.Id == this.StatusCD)?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);

            return "";
        }
    }
}
