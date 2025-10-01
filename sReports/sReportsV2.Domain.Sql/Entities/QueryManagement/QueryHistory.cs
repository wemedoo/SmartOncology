using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.User;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace sReportsV2.Domain.Sql.Entities.QueryManagement
{
    [Table("QueryHistories")]
    public class QueryHistory : EntitiesBase.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int QueryHistoryId { get; set; }

        public int QueryId { get; set; }
        [ForeignKey("QueryId")]
        public Query Query { get; set; }

        public string Comment { get; set; }

        public int StatusCD { get; set; }
        [ForeignKey("StatusCD")]
        public Code Status { get; set; }

        public int? LastUpdateById { get; set; }
        [ForeignKey("LastUpdateById")]
        public Personnel LastUpdateBy { get; set; }
    }
}
