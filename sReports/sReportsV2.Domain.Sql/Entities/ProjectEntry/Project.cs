using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace sReportsV2.Domain.Sql.Entities.ProjectEntry
{
    public class Project : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("ProjectId")]
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int? ProjectTypeCD { get; set; }
        [ForeignKey("ProjectTypeCD")]
        public Code ProjectType { get; set; }
        public DateTimeOffset? ProjectStartDateTime { get; set; }
        public DateTimeOffset? ProjectEndDateTime { get; set; }

        public virtual List<ProjectPersonnelRelation> ProjectPersonnelRelations { get; set; } = new List<ProjectPersonnelRelation> { };
        public virtual List<ProjectDocumentRelation> ProjectDocumentRelations { get; set; } = new List<ProjectDocumentRelation> { };
        public virtual List<ProjectPatientRelation> ProjectPatientRelations { get; set; } = new List<ProjectPatientRelation> { };

        public void Copy(Project trial)
        {
            this.ProjectName = trial.ProjectName;
            this.ProjectTypeCD = trial.ProjectTypeCD;
            this.ProjectStartDateTime = trial.ProjectStartDateTime;
            this.ProjectEndDateTime = trial.ProjectEndDateTime;
            this.ProjectPersonnelRelations = trial.ProjectPersonnelRelations;
            this.ProjectDocumentRelations = trial.ProjectDocumentRelations;
            this.ProjectPatientRelations = trial.ProjectPatientRelations;
            this.ProjectId = trial.ProjectId;
        }

        public void AddPersonnel(ProjectPersonnelRelation personnelProject)
        {
            if (!ProjectPersonnelRelations.Where(x => x.IsActive())
                            .Select(x => x.PersonnelId).Contains(personnelProject.PersonnelId))
            {
                ProjectPersonnelRelations.Add(personnelProject);
            }
        }
    }
}
