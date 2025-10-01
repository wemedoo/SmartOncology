using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.FileIO;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.SqlDomain.Implementations;
using System;
using System.Collections.Generic;

namespace Generator
{
    public class FormGenerator : ThesaurusCommon
    {
        private readonly ThesaurusDAL thesaurusDAL;
        private readonly UserData userData;
        int id = 0;
        string[] classIndicators;

        public FormGenerator(IConfiguration configuration, SReportsContext dbContext) 
        {
            this.thesaurusDAL = new ThesaurusDAL(dbContext, configuration);
        }

        public FormGenerator(UserData user) 
        {
            userData = user;
        }

        public Form GetFormFromCsv(string formName)
        {
            List<string[]> csvRows = GetRowsFromCsv();

            Form form = new Form
            {
                State = FormDefinitionState.ReadyForDataCapture,
                Version = new sReportsV2.Domain.Entities.Form.Version() { Major = 1, Minor = 1, Id = Guid.NewGuid().ToString() },
                Language = LanguageConstants.EN,
                Title = formName,
                UserId = userData.Id,
                ThesaurusId = GetNewThesaurus(formName),
                Chapters = new List<FormChapter>()
            };
            form.SetInitialOrganizationId(userData.ActiveOrganization.GetValueOrDefault());

            FormChapter chapter = new FormChapter
            {
                Id = GetNewId(),
                ThesaurusId = GetNewThesaurus(formName),
                Title = formName,
                Pages = new List<FormPage>()
            };

            FormPage page = new FormPage
            {
                Id = GetNewId(),
                ThesaurusId = GetNewThesaurus("."),
                ListOfFieldSets = new List<List<FieldSet>>()
            };

            FieldSet fieldSet = new FieldSet
            {
                Id = GetNewId(),
                ThesaurusId = GetNewThesaurus("."),
                Fields = new List<Field>()
            };

            foreach (string[] row in csvRows)
            {
                FieldSelectable field = new FieldSelectable();
                field.Id = GetNewId();
                field.ThesaurusId = GetNewThesaurus(row[0]);
                field.Type = "radio";
                field.FhirType = "Observation";
                field.Label = row[0];
                field.Values = new List<FormFieldValue>();

                int optionCounter = 0;

                SetFieldValues(field, ref optionCounter, row);
                SetType(field, optionCounter);

                fieldSet.Fields.Add(field);
            }

            page.ListOfFieldSets.Add(new List<FieldSet>() { fieldSet });
            chapter.Pages.Add(page);
            form.Chapters.Add(chapter);

            return form;
        }

        private void SetType(Field field, int optionCounter) 
        {
            if (optionCounter == 1)
            {
                field.Type = "checkbox";
            }
        }

        private void SetFieldValues(FieldSelectable field, ref int optionCounter, string[] row) 
        {
            for (int i = 1; i < row.Length; i++)
            {
                if (row[i] != "-")
                {
                    optionCounter++;
                    string concatenatedValue = $"{classIndicators[i]}-{row[i].Replace("\n", " ")}";
                    int thesaurusId = GetNewThesaurus(concatenatedValue);
                    FormFieldValue value = new FormFieldValue() { Label = concatenatedValue, Value = concatenatedValue, ThesaurusId = thesaurusId };
                    field.Values.Add(value);
                }
            }
        }

        private string GetNewId() 
        {
            id++;
            return id.ToString();
        }

        private int GetNewThesaurus(string label, string description = null) 
        {
            ThesaurusEntry thesaurus =  CreateThesaurus(label, description);
            thesaurusDAL.InsertOrUpdate(thesaurus);

            return thesaurus.ThesaurusEntryId;
        }
        private List<string[]> GetRowsFromCsv()
        {
            List<string[]> allRows = new List<string[]>();
            using (TextFieldParser parser = new TextFieldParser(@"PATH_TO_CTCAE_CSF_FILE"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //Process row
                    string[] fieldss = parser.ReadFields();
                    allRows.Add(fieldss);
                }
            }

            classIndicators = allRows[1];
            //remove first 2 line in rows its comment and class indicator
            allRows.RemoveAt(0);
            allRows.RemoveAt(0);

            return allRows;
        }

    }
}
