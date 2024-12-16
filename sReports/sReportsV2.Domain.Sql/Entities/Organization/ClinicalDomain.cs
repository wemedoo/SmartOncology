using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.OrganizationEntities
{
    //This table is deprecated, but we shouldn't delete it until we are sure that we have migrated clinical domain to codes in all instances
    public class ClinicalDomain
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("ClinicalDomainId")]
        public int ClinicalDomainId { get; set; }
        public string Name { get; set; }
        public int CodeId { get; set; }
    }
}
