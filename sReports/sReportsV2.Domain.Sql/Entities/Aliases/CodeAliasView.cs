using sReportsV2.Domain.Sql.EntitiesBase;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.Aliases
{
    public class CodeAliasView : Entity
    {
        [Key]
        [Column("AliasId")]
        public int AliasId { get; set; }
        public int CodeId { get; set; }
        public int CodeSetId { get; set; }
        public string System { get; set; }
        public string InboundAlias { get; set; }
        public string OutboundAlias { get; set; }
        public int InboundAliasId { get; set; }
        public int? OutboundAliasId { get; set; }
    }
}
