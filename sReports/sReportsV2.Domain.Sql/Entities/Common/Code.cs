using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.EntitiesBase;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using sReportsV2.Common.Extensions;
using System.Linq;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    public class Code : Entity, IReplaceThesaurusEntity
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

        public void ReplaceThesauruses(ThesaurusMerge thesaurusMerge)
        {
            this.ThesaurusEntryId = this.ThesaurusEntryId.ReplaceThesaurus(thesaurusMerge);
        }

        public void Copy(Code code, string organizationTimeZone = null)
        {
            this.ThesaurusEntryId = code.ThesaurusEntryId;
            this.SetLastUpdate(organizationTimeZone);
            this.ActiveFrom = code.ActiveFrom;
            this.ActiveTo = code.ActiveTo;
        }

        public string GetDisplayAutocompleteValue(string activeLanguage)
        {
            string codeableConcept = ThesaurusEntry.Codes.FirstOrDefault()?.Code;
            string displayValue = ThesaurusEntry.GetPreferredTermByTranslationOrDefault(activeLanguage);
            if (!string.IsNullOrEmpty(codeableConcept))
            {
                displayValue = $"{codeableConcept} {displayValue}";
            }
            return displayValue;
        }
    }
}
