using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using sReportsV2.Domain.Sql.Entities.Common;

namespace sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser
{
    public class GlobalThesaurusUser : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("GlobalThesaurusUserId")]
        public int GlobalThesaurusUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        [ForeignKey("SourceCD")]
        public virtual Code SourceCode { get; set; }
        public int? SourceCD { get; set; }

        public string Affiliation { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }

        [ForeignKey("StatusCD")]
        public virtual Code StatusCode { get; set; }
        public int? StatusCD { get; set; }

        public virtual List<GlobalThesaurusUserRole> GlobalThesaurusUserRoles { get; set; } = new List<GlobalThesaurusUserRole>();

        public void UpdateRoles(List<int> newSelectedRoles)
        {
            if (RolesHaveChanged(newSelectedRoles))
            {
                AddUserRoles(newSelectedRoles);
                RemoveUserRoles(newSelectedRoles);
            }
        }

        public bool HasRole(string roleName)
        {
            return GlobalThesaurusUserRoles
                .Where(r => r.IsActive())
                .Select(r => r.GlobalThesaurusRole)
                .Any(r => r.Name == roleName);
        }

        private bool RolesHaveChanged(List<int> newRoles)
        {
            return !newRoles.SequenceEqual(GetRolesIds());
        }

        private List<int> GetRolesIds()
        {
            return GlobalThesaurusUserRoles.Where(x => x.IsActive()).Select(x => x.GlobalThesaurusRoleId).ToList();
        }

        private void AddUserRoles(List<int> newRoles)
        {
            if (newRoles != null)
            {
                foreach (var userRoleId in newRoles)
                {
                    GlobalThesaurusUserRole userRole = GlobalThesaurusUserRoles.Find(x => x.GlobalThesaurusRoleId == userRoleId && x.IsActive());
                    if (userRole == null)
                    {
                        GlobalThesaurusUserRoles.Add(new GlobalThesaurusUserRole()
                        {
                            GlobalThesaurusUserId = GlobalThesaurusUserId,
                            GlobalThesaurusRoleId = userRoleId
                        });
                    }
                }
            }
        }

        private void RemoveUserRoles(List<int> newRoles)
        {
            foreach (GlobalThesaurusUserRole userRole in GlobalThesaurusUserRoles)
            {
                int roleCD = newRoles.Find(x => x == userRole.GlobalThesaurusRoleId);
                if (roleCD == 0)
                {
                    userRole.Delete();
                }
            }
        }

    }
}
