using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Sql;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Distribution
{
    public class FormDistribution : Entity, IReplaceThesaurusEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        public string VersionId { get; set; }
        public List<FormFieldDistribution> Fields { get; set; }

        public void ReplaceThesauruses(ThesaurusMerge thesaurusMerge)
        {
            this.ThesaurusId = this.ThesaurusId.ReplaceThesaurus(thesaurusMerge);

            if (this.Fields != null)
            {
                foreach (FormFieldDistribution field in this.Fields)
                {
                    field.ThesaurusId = field.ThesaurusId.ReplaceThesaurus(thesaurusMerge);
                    if (field.ValuesAll != null)
                    {
                        foreach (var val in field.ValuesAll)
                        {
                            if (val.Values != null)
                            {
                                foreach (var v in val.Values)
                                {
                                    if (v != null)
                                    {
                                        v.ThesaurusId = v.ThesaurusId.ReplaceThesaurus(thesaurusMerge);
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }
    }
}
