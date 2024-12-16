using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using sReportsV2.Domain.Sql.Entities.Common;

namespace sReportsV2.Domain.Sql.Entities.CodeEntry
{
    public class FormCodeRelation : EntitiesBase.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int FormCodeRelationId { get; set; }

        public int? CodeCD { get; set; }
        [ForeignKey("CodeCD")]
        public virtual Code Code { get; set; }
        public string FormId { get; set; }

        public FormCodeRelation()
        {
        }

        public FormCodeRelation(int? createdById, string organizationTimeZone = null) : base(createdById, organizationTimeZone)
        {
        }
    }
}
