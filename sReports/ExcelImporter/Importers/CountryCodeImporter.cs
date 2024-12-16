using ExcelImporter.Classes;
using Microsoft.Extensions.Configuration;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.CodeEntry;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExcelImporter.Importers
{
    public class CountryCodeImporter : ExcelSaxImporter<O4CodeableConcept>
    {
        private readonly IThesaurusDAL thesaurusDAL;
        private readonly ICodeSystemDAL codeSystemDAL;
        private readonly ICodeDAL codeDAL;
        private readonly ICodeSetDAL codeSetDAL;
        private readonly IThesaurusTranslationDAL translationDAL;
        private readonly IO4CodeableConceptDAL o4codeableConceptDAL;
        private readonly IConfiguration configuration;

        private const string Code = "Code";
        private const string Value = "Display";

        private int countryCodeSystemId;

        public CountryCodeImporter(string fileName, string sheetName, IThesaurusDAL thesaurusDAL, ICodeDAL codeDAL, ICodeSystemDAL codeSystemDAL, IThesaurusTranslationDAL translationDAL, IO4CodeableConceptDAL o4codeableConceptDAL, ICodeSetDAL codeSetDAL, IConfiguration configuration) : base(fileName, sheetName)
        {
            this.thesaurusDAL = thesaurusDAL;
            this.codeDAL = codeDAL;
            this.codeSystemDAL = codeSystemDAL;
            this.translationDAL = translationDAL;
            this.o4codeableConceptDAL = o4codeableConceptDAL;
            this.codeSetDAL = codeSetDAL;
            this.configuration = configuration;
        }

        public override void ImportDataFromExcelToDatabase()
        {
            if (codeDAL.GetCodeFiltered(new CodeFilter { CodeSetId = (int)CodeSetList.Country}).Count() == 0)
            {
                SetCountryCodeSystemId();
                List<O4CodeableConcept> countryCodes = ImportFromExcel();
                InsertDataIntoDatabase(countryCodes);
            }
        }

        protected override List<O4CodeableConcept> ImportFromExcel()
        {
            return ImportRowsFromExcel().Select(row => GetCode(row)).ToList();
        }

        protected override void InsertDataIntoDatabase(List<O4CodeableConcept> entries)
        {
            List<ThesaurusEntry> thesauruses = GetThesauruses(entries);

            int countryCodeSetId = codeSetDAL.GetIdByPreferredTerm("Country");

            if (countryCodeSetId > 0)
            {
                thesaurusDAL.InsertMany(thesauruses);
                var bulkedThesauruses = thesaurusDAL.GetLastBulkInserted(thesauruses.Count);
                translationDAL.InsertMany(thesauruses, bulkedThesauruses, configuration);
                o4codeableConceptDAL.InsertMany(thesauruses, bulkedThesauruses, configuration);
                codeDAL.InsertMany(bulkedThesauruses, countryCodeSetId);
            }
        }

        private void SetCountryCodeSystemId()
        {
            sReportsV2.Domain.Sql.Entities.CodeSystem.CodeSystem codeSystem = codeSystemDAL.GetByValue(ResourceTypes.CountryCodingSystem) 
                    ?? throw new InvalidOperationException($"The code system for '{ResourceTypes.CountryCodingSystem}' could not be found.");
            countryCodeSystemId = codeSystem.CodeSystemId;
        }

        private O4CodeableConcept GetCode(RowInfo dataRow)
        {
            return new O4CodeableConcept()
            {
                Code = dataRow.GetCellValue(GetColumnAddress(Code)),
                Value = dataRow.GetCellValue(GetColumnAddress(Value)),
                CodeSystemId = countryCodeSystemId, 
                VersionPublishDate = DateTime.Now,
                EntryDateTime = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone()
            };
        }

        private List<ThesaurusEntry> GetThesauruses(List<O4CodeableConcept> codes)
        {
            List<ThesaurusEntry> thesaurusEntries = new List<ThesaurusEntry>();
            foreach (IGrouping<string, O4CodeableConcept> codesByCountry in codes.GroupBy(x => x.Value))
            {
                thesaurusEntries.Add(GetThesaurus(codesByCountry));
            }
            return thesaurusEntries;
        }

        private ThesaurusEntry GetThesaurus(IGrouping<string, O4CodeableConcept> codesByCountry)
        {
            List<O4CodeableConcept> countryCodes = codesByCountry.ToList();
            string country = codesByCountry.Key;
            int? draftStateCD = codeDAL.GetByCodeSetIdAndPreferredTerm((int)CodeSetList.ThesaurusState, CodeAttributeNames.Draft);

            return new ThesaurusEntry()
            {
                Translations = new List<ThesaurusEntryTranslation>()
                    {
                        new ThesaurusEntryTranslation()
                        {
                            Language = LanguageConstants.EN,
                            PreferredTerm = country,
                            Definition = country,
                            Abbreviations = countryCodes.Select(c => c.Code).ToList()
                        }
                    },
                Codes = countryCodes,
                StateCD = draftStateCD
            };
        }
    }
}