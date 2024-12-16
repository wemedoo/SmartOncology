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
    [Table("OutboundAliases")]
    public class OutboundAlias : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int AliasId { get; set; }
        public string Alias { get; set; }
        public int CodeId { get; set; }
        [ForeignKey("CodeId")]
        public virtual Code Code { get; set; }
        public string System { get; set; }

        public void Copy(OutboundAlias outboundAlias)
        {
            Alias = outboundAlias.Alias;
            System = outboundAlias.System;
            ActiveFrom = outboundAlias.ActiveFrom;
            ActiveTo = outboundAlias.ActiveTo;
        }
    }
}
