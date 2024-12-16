namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class AddLanguageCodes : DbMigration
    {
        public override void Up()
        {
            SReportsContext dbContext = new SReportsContext();
            bool hasEntities = dbContext.Modules.Any() || dbContext.Permissions.Any();
            if (hasEntities)
            {
                int codeSetId = CreateCodeSet(dbContext, CodeSetAttributeNames.Language, 45);

                foreach (string term in Languages)
                    GetOrCreateCodeByPreferredTerm(dbContext, codeSetId, term);
            }
        }
        
        public override void Down()
        {
        }

        private int GetOrCreateCodeByPreferredTerm(SReportsContext dbContext, int codeSetId, string codePreferredTerm)
        {
            int existingCode = GetCodeIdByPreferredTerm(dbContext, codePreferredTerm);
            if (existingCode > 0)
            {
                UpdateCodeSetId(dbContext, existingCode, codeSetId);
                return existingCode;
            }
            else
            {
                int thesaurusId = GetOrCreateThesaurusId(dbContext, codePreferredTerm);

                int codeId = dbContext.Database.SqlQuery<int>($@"SELECT TOP (1) CodeId FROM Codes ORDER BY CodeId DESC").FirstOrDefault() + 1;

                dbContext.Database.ExecuteSqlCommand($@"
                SET IDENTITY_INSERT dbo.Codes ON;
                INSERT INTO Codes (CodeId, ThesaurusEntryId, CodeSetId, EntryDatetime, EntityStateCD, TypeCD) values ({codeId}, {thesaurusId}, {codeSetId}, GETDATE(), 2001, 0);
                SET IDENTITY_INSERT dbo.Codes OFF;");

                return codeId;
            }
        }

        private int CreateCodeSet(SReportsContext dbContext, string codeSetName, int codeSetId)
        {
            int thesaurusId = GetOrCreateThesaurusId(dbContext, codeSetName);

            dbContext.Database.ExecuteSqlCommand($@"
            insert into CodeSets (CodeSetId, ThesaurusEntryId, EntryDatetime, EntityStateCD) values ({codeSetId}, {thesaurusId}, GETDATE(), 2001);
            ");

            return codeSetId;
        }

        private int GetOrCreateThesaurusId(SReportsContext dbContext, string preferredTerm, string definition = null)
        {
            int thesaurusId = dbContext.Database.SqlQuery<int>($@"SELECT ThesaurusEntryId FROM ThesaurusEntryTranslations WHERE PreferredTerm = '{preferredTerm}'").FirstOrDefault();
            if (thesaurusId <= 0)
            {
                thesaurusId = (int)dbContext.Database.SqlQuery<Decimal>($@"
                    INSERT INTO ThesaurusEntries (EntryDatetime) Values (GETDATE())
                    SELECT SCOPE_IDENTITY()
                    
                ").FirstOrDefault();

                dbContext.Database.ExecuteSqlCommand($@"INSERT INTO ThesaurusEntryTranslations (ThesaurusEntryId, Language, PreferredTerm, Definition) VALUES ({thesaurusId}, '{LanguageConstants.EN}', '{preferredTerm}', '{definition ?? preferredTerm}')");
            }
            return thesaurusId;
        }

        private int GetCodeIdByPreferredTerm(SReportsContext dbContext, string preferredTerm)
        {
            return dbContext.Database.SqlQuery<int>(
                $@"SELECT TOP(1) code.CodeId
                from [dbo].[Codes] code
                inner join [dbo].[ThesaurusEntryTranslations] tranThCode on tranThCode.ThesaurusEntryId = code.ThesaurusEntryId
                WHERE PreferredTerm = '{preferredTerm}' AND code.EntityStateCD != 2003").FirstOrDefault();
        }

        private void UpdateCodeSetId(SReportsContext dbContext, int codeId, int codeSetId)
        {
            dbContext.Database.ExecuteSqlCommand(
                   $@"UPDATE [dbo].Codes
                    SET CodeSetId = {codeSetId}
                    WHERE CodeId = '{codeId}'");
        }

        public static List<string> Languages { get; set; } = new List<string>()
        {
             "Abkhazian" ,
             "Afar" ,
             "Afrikaans" ,
             "Akan" ,
             "Albanian" ,
             "Amharic" ,
             "Arabic" ,
             "Aragonese" ,
             "Armenian" ,
             "Assamese" ,
             "Avaric" ,
             "Avestan" ,
             "Aymara" ,
             "Azerbaijani" ,
             "Bambara" ,
             "Bashkir" ,
             "Basque" ,
             "Belarusian" ,
             "Bengali" ,
             "Bislama" ,
             "Bosnian" ,
             "Breton" ,
             "Bulgarian" ,
             "Burmese" ,
             "Catalan" ,
             "Chamorro" ,
             "Chechen" ,
             "Chichewa" ,
             "Chinese" ,
             "ChurchSlavonic" ,
             "Chuvash" ,
             "Cornish" ,
            "Corsican" ,
            "Cree" ,
            "Croatian" ,
            "Czech" ,
            "Danish" ,
            "Dhivehi" ,
            "Dutch" ,
            "Dzongkha" ,
            "English" ,
            "Esperanto" ,
            "Estonian" ,
            "Ewe" ,
            "Faroese",
            "Fijian",
            "Finnish",
            "French",
            "WesternFrisian",
            "Fulah",
            "Gaelic",
            "Galician",
            "Ganda",
            "Georgian",
            "German",
            "Greek",
            "Kalaallisut",
            "Guarani",
            "Gujarati",
            "Haitian",
            "Hausa",
            "Hebrew",
            "Herero",
            "Hindi",
            "HiriMotu",
            "Hungarian",
            "Icelandic",
            "Ido",
            "Igbo",
            "Indonesian",
            "Interlingua",
            "Interlingue",
            "Inuktitut",
            "Inupiaq",
            "Irish",
            "Italian",
            "Japanese",
            "Javanese",
            "Kannada",
            "Kanuri",
            "Kashmiri",
            "Kazakh",
            "CentralKhmer",
            "Kikuyu",
            "Kinyarwanda",
            "Kirghiz",
            "Komi",
            "Kongo",
            "Korean",
            "Kuanyama",
            "Kurdish",
            "Lao",
            "Latin",
            "Latvian",
            "Limburgan",
            "Lingala",
            "Lithuanian",
            "LubaKatanga",
            "Luxembourgish",
            "Macedonian",
            "Malagasy",
            "Malay",
            "Malayalam",
            "Maltese",
            "Manx",
            "Maori",
            "Marathi",
            "Marshallese",
            "Mongolian",
            "Nauru",
            "Navajo",
            "NorthNdebele",
            "SouthNdebele",
            "Ndonga",
            "Nepali",
            "Norwegian",
            "NorwegianBokmål",
            "NorwegianNynorsk",
            "SichuanYi",
            "Occitan",
            "Ojibwa",
            "Oriya",
            "Oromo",
            "Ossetian",
            "Pali",
            "Pashto",
            "Persian",
            "Polish",
            "Portuguese",
            "Punjabi",
            "Quechua",
            "Romanian",
            "Romansh",
            "Rundi",
            "Russian",
            "NorthernSami",
            "Samoan",
            "Sango",
            "Sanskrit",
            "Sardinian",
            "Serbian",
            "Shona",
            "Sindhi",
            "Sinhala",
            "Slovak",
            "Slovenian",
            "Somali",
            "SouthernSotho",
            "Spanish",
            "Sundanese",
            "Swahili",
            "Swati",
            "Swedish",
            "Tagalog",
            "Tahitian",
            "Tajik",
            "Tamil",
            "Tatar",
            "Telugu",
            "Thai",
            "Tibetan",
            "Tigrinya",
            "Tonga",
            "Tsonga",
            "Tswana",
            "Turkish",
            "Turkmen",
            "Twi",
            "Uighur",
            "Ukrainian",
            "Urdu",
            "Uzbek",
            "Venda",
            "Vietnamese",
            "Volapük",
            "Walloon",
            "Welsh",
            "Wolof",
            "Xhosa",
            "Yiddish",
            "Yoruba",
            "Zhuang",
            "Zulu"
        };
    }
}
