using sReportsV2.Domain.Sql.EntitiesBase;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using sReportsV2.Domain.Sql.Entities.Common;

namespace sReportsV2.Domain.Sql.Entities.FormComment
{
    public class Comment : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("CommentId")]
        public int CommentId { get; set; }
        public string Value { get; set; }
        public string ItemRef { get; set; }
        public int? CommentRef { get; set; }
        public string FormRef { get; set; }
        public int PersonnelId { get; set; }

        [ForeignKey("CommentStateCD")]
        public virtual Code CommentStateCode { get; set; }
        public int? CommentStateCD { get; set; }
    }
}
