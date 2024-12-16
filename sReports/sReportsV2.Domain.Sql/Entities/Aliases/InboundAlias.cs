using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Aliases
{
    [Table("InboundAliases")]
    public class InboundAlias : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int AliasId { get; set; }
        public string Alias { get; set; }
        public int CodeId { get; set; }
        [ForeignKey("CodeId")]
        public virtual Code Code { get; set; }
        public string System { get; set; }
        public int? OutboundAliasId { get; set; }
        [ForeignKey("OutboundAliasId")]
        public virtual OutboundAlias OutboundAlias { get; set; }

        public void Copy(InboundAlias inboundAlias)
        {
            Alias = inboundAlias.Alias;
            System = inboundAlias.System;
            ActiveFrom = inboundAlias.ActiveFrom;
            ActiveTo = inboundAlias.ActiveTo;
        }
    }
}
