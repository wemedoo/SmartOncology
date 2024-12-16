using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.Domain.Sql.EntitiesBase;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.ProjectEntry;

namespace sReportsV2.Domain.Sql.Entities.ClinicalTrial
{
    public class ClinicalTrial : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("ClinicalTrialId")]
        public int ClinicalTrialId { get; set; }
        public string ClinicalTrialTitle { get; set; }
        public string ClinicalTrialAcronym { get; set; }
        [Column("ClinicalTrialSponsorIdentifier")]
        public string ClinicalTrialSponsorIdentifier { get; set; }
        [Column("ClinicalTrialDataProviderIdentifier")]
        public string ClinicalTrialDataProviderIdentifier { get; set; }
        [Column("ClinicalTrialRecruitmentStatusCD")]
        public int? ClinicalTrialRecruitmentStatusCD { get; set; }
        [ForeignKey("ClinicalTrialRecruitmentStatusCD")]
        public Code ClinicalTrialRecruitmentStatus { get; set; }
        public bool? IsArchived { get; set; }
        [StringLength(60)]
        public string ClinicalTrialIdentifier { get; set; }
        [StringLength(300)]
        public string ClinicalTrialSponsorName { get; set; }
        [StringLength(300)]
        public string ClinicalTrialDataManagementProvider { get; set; }

        public int? ClinicalTrialIdentifierTypeCD { get; set; }
        [ForeignKey("ClinicalTrialIdentifierTypeCD")]
        public virtual Code ClinicalTrialIdentifierType { get; set; }
        public int? ClinicalTrialSponsorIdentifierTypeCD { get; set; }
        [ForeignKey("ClinicalTrialSponsorIdentifierTypeCD")]
        public virtual Code ClinicalTrialSponsorIdentifierType { get; set; }
        public int? ProjectId { get; set; }
        public Project Project { get; set; }

        public void Copy(ClinicalTrial trial)
        {
            this.ClinicalTrialTitle = trial.ClinicalTrialTitle;
            this.ClinicalTrialAcronym = trial.ClinicalTrialAcronym;
            this.ClinicalTrialSponsorIdentifier = trial.ClinicalTrialSponsorIdentifier;
            this.ClinicalTrialDataProviderIdentifier = trial.ClinicalTrialDataProviderIdentifier;
            this.ClinicalTrialRecruitmentStatusCD = trial.ClinicalTrialRecruitmentStatusCD;
            this.IsArchived = trial.IsArchived;
            this.ClinicalTrialIdentifier = trial.ClinicalTrialIdentifier;
            this.ClinicalTrialSponsorName = trial.ClinicalTrialSponsorName;
            this.ClinicalTrialDataManagementProvider = trial.ClinicalTrialDataManagementProvider;
            this.ClinicalTrialIdentifierTypeCD = trial.ClinicalTrialIdentifierTypeCD;
            this.ClinicalTrialSponsorIdentifierTypeCD = trial.ClinicalTrialSponsorIdentifierTypeCD;
            this.ProjectId = trial.ProjectId;
        }
    }
}
