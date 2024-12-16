using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.OrganizationEntities
{
    public class OrganizationRelation : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("OrganizationRelationId")]
        public int OrganizationRelationId { get; set; }
        public int ParentId { get; set; }
        public Organization Parent { get; set; }
        public int ChildId { get; set; }
        public Organization Child { get; set; }
    }
}
