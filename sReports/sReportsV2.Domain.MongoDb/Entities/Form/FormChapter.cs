﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Domain.Entities.FieldEntity;

namespace sReportsV2.Domain.Entities.Form
{
    [BsonIgnoreExtraElements]
    public class FormChapter
    {
        public O4CodeableConcept Code { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [BsonRepresentation(BsonType.Int32, AllowTruncation = true)]
        public int ThesaurusId { get; set; }
        public bool IsReadonly { get; set; } 
        public List<FormPage> Pages { get; set; } = new List<FormPage>();

        public int GetNumberOfFieldsForChapter()
        {
            return this.Pages
                .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(fieldSet => fieldSet
                                    .SelectMany(set => set.Fields
                                    )
                                )
                 )
                .ToList()
                .Count;
        }

        public List<Field> GetAllFields()
        {
            return this.Pages
                        .SelectMany(page => page.ListOfFieldSets
                            .SelectMany(fieldSet => fieldSet
                                .SelectMany(fieldset => fieldset.Fields    
                                )
                            )
                        )
                        .ToList();
        }

        public List<Field> GetFieldsByList(List<string> fields)
        {
            List<Field> result = Pages.SelectMany(x => x.ListOfFieldSets.SelectMany(list => list.SelectMany(y => y.Fields.Where(f => fields.Contains(f.FhirType))))).ToList();
            foreach (Field field in result) 
            {
                field.FieldInstanceValues = field.FieldInstanceValues.HasAnyFieldInstanceValue() ? field.FieldInstanceValues : null;
            }

            return result;
        }

        public List<int> GetAllThesaurusIds()
        {
            List<int> thesaurusList = new List<int>();
            foreach (FormPage page in Pages)
            {
                var pageThesaurusId = page.ThesaurusId;
                thesaurusList.Add(pageThesaurusId);
                thesaurusList.AddRange(page.GetAllThesaurusIds());
            }

            return thesaurusList;
        }

        public void GenerateTranslation(List<sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry> entries, string language, string activeLanguage)
        {
            foreach (FormPage page in Pages)
            {
                page.Title = entries.Find(x => x.ThesaurusEntryId.Equals(page.ThesaurusId))?.GetPreferredTermByTranslationOrDefault(language, activeLanguage);
                page.Description = entries.Find(x => x.ThesaurusEntryId.Equals(page.ThesaurusId))?.GetDefinitionByTranslationOrDefault(language, activeLanguage);
                page.GenerateTranslation(entries, language, activeLanguage);
            }  
        }

        public void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.ThesaurusId = this.ThesaurusId == oldThesaurus ? newThesaurus : this.ThesaurusId;
            foreach (FormPage page in this.Pages)
            {
                page.ReplaceThesauruses(oldThesaurus, newThesaurus);
            }
        }

    }
}
