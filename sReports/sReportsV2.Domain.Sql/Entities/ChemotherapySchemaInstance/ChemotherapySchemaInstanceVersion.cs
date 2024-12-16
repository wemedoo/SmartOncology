using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.EntitiesBase;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance
{
    public class ChemotherapySchemaInstanceVersion : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("ChemotherapySchemaInstanceVersionId")]
        public int ChemotherapySchemaInstanceVersionId { get; set; }
        public int ChemotherapySchemaInstanceId { get; set; }
        [ForeignKey("CreatorId")]
        public virtual sReportsV2.Domain.Sql.Entities.User.Personnel Creator { get; set; }
        public int CreatorId { get; set; }
        public int FirstDelayDay { get; set; }
        public int DelayFor { get; set; }
        public string ReasonForDelay { get; set; }
        public string Description { get; set; }
        [ForeignKey("ActionTypeCD")]
        public virtual Code ChemotherapySchemaInstanceActionType { get; set; }
        public int? ActionTypeCD { get; set; }

        public ChemotherapySchemaInstanceVersion()
        {
        }

        public ChemotherapySchemaInstanceVersion(int? createdById) : base(createdById)
        {
        }
    }
}
