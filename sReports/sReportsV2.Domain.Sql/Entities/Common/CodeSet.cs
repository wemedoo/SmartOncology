using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.EntitiesBase;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    public class CodeSet : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CodeSetId { get; set; }
        [NotMapped]
        public int NewCodeSetId { get; set; }
        public int ThesaurusEntryId { get; set; }
        [DefaultValue(false)]
        public bool ApplicableInDesigner { get; set; }
        public virtual ThesaurusEntry.ThesaurusEntry ThesaurusEntry { get; set; }

        public void Copy(CodeSet codeSet)
        {
            this.ThesaurusEntryId = codeSet.ThesaurusEntryId;
            this.SetLastUpdate();
            this.ActiveFrom = codeSet.ActiveFrom;
            this.ActiveTo = codeSet.ActiveTo;
            this.ApplicableInDesigner = codeSet.ApplicableInDesigner;
        }

        public void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.ThesaurusEntryId = this.ThesaurusEntryId == oldThesaurus ? newThesaurus : this.ThesaurusEntryId;
        }

        public CodeSet()
        {
        }

        public CodeSet(bool isEntityAddedByMigrations) : base(isEntityAddedByMigrations)
        {
        }
    }
}
