using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.EntitiesBase;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using sReportsV2.Common.Extensions;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    public class Code : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("CodeId")]
        public int CodeId { get; set; }
        public int ThesaurusEntryId { get; set; }
        [ForeignKey("CodeSetId")]
        public int? CodeSetId { get; set; }
        public virtual ThesaurusEntry.ThesaurusEntry ThesaurusEntry { get; set; }

        public Code()
        {
        }

        public Code(int? createdById, string organizationTimeZone = null) : base(createdById, organizationTimeZone)
        {
        }

        public void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.ThesaurusEntryId = this.ThesaurusEntryId == oldThesaurus ? newThesaurus : this.ThesaurusEntryId;
        }

        public void Copy(Code code, string organizationTimeZone = null)
        {
            this.ThesaurusEntryId = code.ThesaurusEntryId;
            this.SetLastUpdate(organizationTimeZone);
            this.ActiveFrom = code.ActiveFrom;
            this.ActiveTo = code.ActiveTo;
        }
    }
}
