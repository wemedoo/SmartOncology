using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.User
{
    [Table("PersonnelOrganizations")]
    public class PersonnelOrganization
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int PersonnelOrganizationId { get; set; }
        public bool? IsPracticioner { get; set; }
        public string Qualification { get; set; }
        public string SeniorityLevel { get; set; }
        public string Speciality { get; set; }
        public string SubSpeciality { get; set; }
        public int PersonnelId { get; set; }
        [ForeignKey("PersonnelId")]
        public Personnel Personnel { get; set; }
        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        [ForeignKey("StateCD")]
        public virtual Code StateCode { get; set; }
        public int? StateCD { get; set; }

        public void Copy(PersonnelOrganization userOrganization)
        {
            this.IsPracticioner = userOrganization.IsPracticioner;
            this.Qualification = userOrganization.Qualification;
            this.SeniorityLevel = userOrganization.SeniorityLevel;
            this.Speciality = userOrganization.Speciality;
            this.StateCD = userOrganization.StateCD;
            this.SubSpeciality = userOrganization.SubSpeciality;
        }
    }
}
