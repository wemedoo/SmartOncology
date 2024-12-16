namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using sReportsV2.Domain.Sql.Entities.Common;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class UpdateLanguageCDValues : DbMigration
    {
        public override void Up()
        {
            Dictionary<string, int> dictionaryCodeIdLanguage = new Dictionary<string, int>();
            using (var dbContext = new SReportsContext())
            {
                foreach (var item in Languages)
                {
                    var result = dbContext.Codes.Where(x => x.ThesaurusEntry.Translations
                        .Any(m => m.PreferredTerm == item.Value) && x.CodeSetId == 45).FirstOrDefault();
                    if (result != null)
                    {
                        var codeId = result.CodeId;
                        dictionaryCodeIdLanguage.Add(item.Key, codeId);
                    }
                }
            }

            if (dictionaryCodeIdLanguage.Count > 0)
            { 
                var values = string.Join(",", dictionaryCodeIdLanguage.Select(kv => $"('{kv.Key.Replace("'", "''")}', {kv.Value})"));

                SReportsContext dbContext2 = new SReportsContext();
                var script = $@" BEGIN
                                    DECLARE @dictionaryCodeIdLanguage TABLE (LanguageCode VARCHAR(50), LanguageId INT)

                                    -- populate the dictionaryCodeIdLanguage table
                                    INSERT INTO @dictionaryCodeIdLanguage (LanguageCode, LanguageId)
                                    VALUES {values} -- add more languages as needed

                                    -- update the Communications table
                                    UPDATE c
                                    SET c.[LanguageCD] = l.LanguageId
                                    FROM Communications c
                                    JOIN @dictionaryCodeIdLanguage l ON c.Language = l.LanguageCode
                                END
                                ";
                dbContext2.Database.ExecuteSqlCommand(script);
                dbContext2.SaveChanges();
            }
        }
        
        public override void Down()
        {
        }

        public static Dictionary<string, string> Languages { get; set; } = new Dictionary<string, string>()
        {
            { "ab", "Abkhazian" },
            { "aa", "Afar" },
            { "af", "Afrikaans" },
            { "ak", "Akan" },
            { "sq", "Albanian" },
            { "am", "Amharic" },
            { "ar", "Arabic" },
            { "an", "Aragonese" },
            { "hy", "Armenian" },
            { "as", "Assamese" },
            { "av", "Avaric" },
            { "ae", "Avestan" },
            { "ay", "Aymara" },
            { "az", "Azerbaijani" },
            { "bm", "Bambara" },
            { "ba", "Bashkir" },
            { "eu", "Basque" },
            { "be", "Belarusian" },
            { "bn", "Bengali" },
            { "bi", "Bislama" },
            { "bs", "Bosnian" },
            { "br", "Breton" },
            { "bg", "Bulgarian" },
            { "my", "Burmese" },
            { "ca", "Catalan" },
            { "ch", "Chamorro" },
            { "ce", "Chechen" },
            { "ny", "Chichewa" },
            { "zh", "Chinese" },
            { "cu", "ChurchSlavonic" },
            { "cv", "Chuvash" },
            { "kw", "Cornish" },
            { "co", "Corsican" },
            { "cr", "Cree" },
            { "hr", "Croatian" },
            { "cs", "Czech" },
            { "da", "Danish" },
            { "dv", "Dhivehi" },
            { "nl", "Dutch" },
            { "dz", "Dzongkha" },
            { "en", "English" },
            { "eo", "Esperanto" },
            { "et", "Estonian" },
            { "ee", "Ewe" },
            {"fo", "Faroese"},
            {"fj", "Fijian"},
            {"fi", "Finnish"},
            {"fr", "French"},
            {"fy", "WesternFrisian"},
            {"ff", "Fulah"},
            {"gd", "Gaelic"},
            {"gl", "Galician"},
            {"lg", "Ganda"},
            {"ka", "Georgian"},
            {"de", "German"},
            {"el", "Greek"},
            {"kl", "Kalaallisut"},
            {"gn", "Guarani"},
            {"gu", "Gujarati"},
            {"ht", "Haitian"},
            {"ha", "Hausa"},
            {"he", "Hebrew"},
            {"hz", "Herero"},
            {"hi", "Hindi"},
            {"ho", "HiriMotu"},
            {"hu", "Hungarian"},
            {"is", "Icelandic"},
            {"io", "Ido"},
            {"ig", "Igbo"},
            {"id", "Indonesian"},
            {"ia", "Interlingua"},
            {"ie", "Interlingue"},
            {"iu", "Inuktitut"},
            {"ik", "Inupiaq"},
            {"ga", "Irish"},
            {"it", "Italian"},
            {"ja", "Japanese"},
            {"jv", "Javanese"},
            {"kn", "Kannada"},
            {"kr", "Kanuri"},
            {"ks", "Kashmiri"},
            {"kk", "Kazakh"},
            {"km", "CentralKhmer"},
            {"ki", "Kikuyu"},
            {"rw", "Kinyarwanda"},
            {"ky", "Kirghiz"},
            {"kv", "Komi"},
            {"kg", "Kongo"},
            {"ko", "Korean"},
            {"kj", "Kuanyama"},
            {"ku", "Kurdish"},
            {"lo", "Lao"},
            {"la", "Latin"},
            {"lv", "Latvian"},
            {"li", "Limburgan"},
            {"ln", "Lingala"},
            {"lt", "Lithuanian"},
            {"lu", "LubaKatanga"},
            {"lb", "Luxembourgish"},
            {"mk", "Macedonian"},
            {"mg", "Malagasy"},
            {"ms", "Malay"},
            {"ml", "Malayalam"},
            {"mt", "Maltese"},
            {"gv", "Manx"},
            {"mi", "Maori"},
            {"mr", "Marathi"},
            {"mh", "Marshallese"},
            {"mn", "Mongolian"},
            {"na", "Nauru"},
            {"nv", "Navajo"},
            {"nd", "NorthNdebele"},
            {"nr", "SouthNdebele"},
            {"ng", "Ndonga"},
            {"ne", "Nepali"},
            {"no", "Norwegian"},
            {"nb", "NorwegianBokmål"},
            {"nn", "NorwegianNynorsk"},
            {"ii", "SichuanYi"},
            {"oc", "Occitan"},
            {"oj", "Ojibwa"},
            {"or", "Oriya"},
            {"om", "Oromo"},
            {"os", "Ossetian"},
            {"pi", "Pali"},
            {"ps", "Pashto"},
            {"fa", "Persian"},
            {"pl", "Polish"},
            {"pt", "Portuguese"},
            {"pa", "Punjabi"},
            {"qu", "Quechua"},
            {"ro", "Romanian"},
            {"rm", "Romansh"},
            {"rn", "Rundi"},
            {"ru", "Russian"},
            {"se", "NorthernSami"},
            {"sm", "Samoan"},
            {"sg", "Sango"},
            {"sa", "Sanskrit"},
            {"sc", "Sardinian"},
            {"sr", "Serbian"},
            {"sn", "Shona"},
            {"sd", "Sindhi"},
            {"si", "Sinhala"},
            {"sk", "Slovak"},
            {"sl", "Slovenian"},
            {"so", "Somali"},
            {"st", "SouthernSotho"},
            {"es", "Spanish"},
            {"su", "Sundanese"},
            {"sw", "Swahili"},
            {"ss", "Swati"},
            {"sv", "Swedish"},
            {"tl", "Tagalog"},
            {"ty", "Tahitian"},
            {"tg", "Tajik"},
            {"ta", "Tamil"},
            {"tt", "Tatar"},
            {"te", "Telugu"},
            {"th", "Thai"},
            {"bo", "Tibetan"},
            {"ti", "Tigrinya"},
            {"to", "Tonga"},
            {"ts", "Tsonga"},
            {"tn", "Tswana"},
            {"tr", "Turkish"},
            {"tk", "Turkmen"},
            {"tw", "Twi"},
            {"ug", "Uighur"},
            {"uk", "Ukrainian"},
            {"ur", "Urdu"},
            {"uz", "Uzbek"},
            {"ve", "Venda"},
            {"vi", "Vietnamese"},
            {"vo", "Volapük"},
            {"wa", "Walloon"},
            {"cy", "Welsh"},
            {"wo", "Wolof"},
            {"xh", "Xhosa"},
            {"yi", "Yiddish"},
            {"yo", "Yoruba"},
            {"za", "Zhuang"},
            {"zu", "Zulu"}
        };
    }
}
