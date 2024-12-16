using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.EntitiesBase;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.CodeEntry
{
    public class CodeAssociation : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int CodeAssociationId { get; set; }
        public int ParentId { get; set; }
        [ForeignKey("ParentId")]
        public virtual Code Parent { get; set; }
        public int? ChildId { get; set; }
        [ForeignKey("ChildId")]
        public virtual Code Child { get; set; }

        public CodeAssociation()
        {
        }

        public CodeAssociation(int? createdById) : base(createdById)
        {
        }

        public void Copy(CodeAssociation codeAssociation)
        {
            this.ParentId = codeAssociation.ParentId;
            this.ChildId = codeAssociation.ChildId;
            this.SetLastUpdate();
        }
    }
}
