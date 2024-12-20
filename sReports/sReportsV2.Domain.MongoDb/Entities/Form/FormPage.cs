﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.CustomFHIRClasses;

namespace sReportsV2.Domain.Entities.Form
{
    [BsonIgnoreExtraElements]
    public class FormPage
    {
        public O4CodeableConcept Code { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public bool IsVisible { get; set; }
        public string Description { get; set; }

        [BsonRepresentation(BsonType.Int32, AllowTruncation = true)]
        public int ThesaurusId { get; set; }
        public FormPageImageMap ImageMap { get; set; }
        public List<List<FieldSet>> ListOfFieldSets { get; set; } = new List<List<FieldSet>>();

        public LayoutStyle LayoutStyle { get; set; }

        public List<int> GetAllThesaurusIds()
        {
            List<int> thesaurusList = new List<int>();
            foreach (FieldSet fieldSet in ListOfFieldSets.SelectMany(fs => fs))
            {
                var fieldSethesaurusId = fieldSet.ThesaurusId;
                thesaurusList.Add(fieldSethesaurusId);
                thesaurusList.AddRange(fieldSet.GetAllThesaurusIds());
            }

            return thesaurusList;
        }

        public void GenerateTranslation(List<sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry> entries, string language, string activeLanguage)
        {
            foreach (List<FieldSet> listOfFS in ListOfFieldSets)
            {
                foreach(FieldSet fieldSet in listOfFS)
                {
                    fieldSet.Label = entries.Find(x => x.ThesaurusEntryId.Equals(fieldSet.ThesaurusId))?.GetPreferredTermByTranslationOrDefault(language, activeLanguage);
                    fieldSet.Description = entries.Find(x => x.ThesaurusEntryId.Equals(fieldSet.ThesaurusId))?.GetDefinitionByTranslationOrDefault(language, activeLanguage);
                    fieldSet.GenerateTranslation(entries, language, activeLanguage);
                }
            }
        }
        public void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.ThesaurusId = this.ThesaurusId == oldThesaurus ? newThesaurus : this.ThesaurusId;
            foreach (var list in this.ListOfFieldSets)
            {
                list[0].ReplaceThesauruses(oldThesaurus, newThesaurus);
            }
        }

    }
}
