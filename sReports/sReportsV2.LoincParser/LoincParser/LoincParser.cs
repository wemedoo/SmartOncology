using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using sReportsV2.Common.Entities.User;
using sReportsV2.Domain.Sql.Entities.User;

namespace sReportsV2.LoincParser.LoincParser
{
    public class LoincParser
    {
        private readonly Dictionary<string, string> dictionary = new Dictionary<string, string>();
        private readonly List<LoincDocument> documents = new List<LoincDocument>();
        private readonly string basePath = string.Empty;

        public void GenerateCsvFromResult(string path) 
        {
            Type itemType = typeof(LoincDocument);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .OrderBy(p => p.Name);

            using (var writer = new StreamWriter(Path.Combine(path, "Test.csv")))
            {
                writer.WriteLine(string.Join(", ", props.Select(p => p.Name)));

                foreach (var item in documents)
                {
                    writer.WriteLine(string.Join(", ", props.Select(p => p.GetValue(item, null))));
                }
            }
        }

        public void ParseDocuments() 
        {
            string currentItem = string.Empty;
            LoincDocument document = null;

            var path = $@"{basePath}/DocumentOntology.csv";
            var webRequest = WebRequest.Create(path);

            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (TextFieldParser parser = new TextFieldParser(content))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //Process row
                    string[] fields = parser.ReadFields();
                    if(fields[0] != currentItem) 
                    {
                        currentItem = fields[0];
                        if(document != null)
                        {
                            documents.Add(document);
                        }
                        document = new LoincDocument
                        {
                            LoincIdentifier = fields[0]
                        };
                    }

                    document.SetProperty(fields[2], fields[4]);
                }
            }
        }

        public void ParseDocumentsDomain(string path)
        {
            using (TextFieldParser parser = new TextFieldParser(Path.Combine(path, "DocumentOntology.csv")))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //Process row
                    string[] fields = parser.ReadFields();
                    if(fields[2] == "Document.SubjectMatterDomain" && !dictionary.ContainsKey(fields[0])) 
                    {
                        dictionary.Add(fields[0], fields[4]);
                    }
                }
            }

            List<string> allDomains = dictionary
                .Select(x => string.Concat(ToUpperFirstLetter(x.Value.Replace(',', ' ').Replace('-', ' ')).Where(c => !Char.IsWhiteSpace(c))) )
                .Distinct()
                .OrderBy(x => x)
                .ToList();

           //foreach(var enumValue in Enum.GetValues(typeof(DocumentClinicalDomain)).Cast<DocumentClinicalDomain>().ToList())
           // {
           //     if (allDomains.Contains(enumValue.ToString()))
           //     {
           //         allDomains.Remove(enumValue.ToString());
           //         Console.WriteLine();

           //     };
           // }
            System.IO.File.WriteAllLines(Path.Combine(path, "domains.txt"), allDomains);
        }

        public  string ToUpperFirstLetter(string source)
        {
            return Regex.Replace(source, @"(^\w)|(\s\w)", m => m.Value.ToUpper());
        }

        public void SetNamesForLoincDocuments() 
        {
            var keyValuePairs = GetKeyValuePairs();

            foreach (var doc in documents) 
            {
                if (keyValuePairs.Find(x => x.Key.Replace(" ","") == doc.LoincIdentifier) != null) 
                {
                    doc.Name = keyValuePairs.Find(x => x.Key.Replace(" ", "") == doc.LoincIdentifier).Value;
                }
            }
        }
        //public void ParseOwl()
        //{
        //    ThesaurusDAL thesaurusDAL = new ThesaurusDAL(new SReportsContext());
        //    FormDAL formService = new FormDAL();
        //    //UserService userService = new UserService();
        //    PersonnelDAL userDAL = new PersonnelDAL(new SReportsContext()); 
        //    Personnel user = userDAL.GetByUsername("smladen");

        //    ParseDocuments();
        //    SetNamesForLoincDocuments();

        //    foreach (var doc in documents)
        //    {
        //        ThesaurusEntry thesaurus = new ThesaurusEntry
        //        {
        //            Translations = new List<ThesaurusEntryTranslation>() {
        //            new ThesaurusEntryTranslation() {
        //                Language = LanguageConstants.EN,
        //                PreferredTerm = doc.Name,
        //            }
        //        },

        //            Codes = new List<O4CodeableConcept>() {
        //            new O4CodeableConcept() {
        //                Code = doc.LoincIdentifier,

        //                VersionPublishDate = DateTime.Now.Date
        //            }
        //        },
        //            State = ThesaurusState.Draft
        //        };

        //        thesaurusDAL.InsertOrUpdate(thesaurus);

        //        Form form = new Form
        //        {
        //            ThesaurusId = thesaurus.ThesaurusEntryId,
        //            Title = doc.Name,
        //            Version = new Domain.Entities.Form.Version() { Major = 1, Minor = 1, Id = Guid.NewGuid().ToString() },
        //            State = FormDefinitionState.DesignPending,
        //            UserId = user.PersonnelId,
        //            Language = LanguageConstants.EN,
        //            DisablePatientData = true
        //        };
        //        form.SetInitialOrganizationId(1);

        //        if (!string.IsNullOrWhiteSpace(doc.SubjectMatterDomain)) 
        //        {
        //            form.DocumentProperties = new DocumentProperties
        //            {
        //                ClinicalDomain = new List<int?>
        //            {
        //                //(DocumentClinicalDomain)Enum.Parse(typeof(DocumentClinicalDomain), ToUpperFirstLetter(doc.SubjectMatterDomain).Replace(" ", ""))
     
        //            }
        //            };
        //        }
        //        formService.InsertOrUpdate(form, GetUserDataOut(user));
        //    }

        //    Console.WriteLine("test");

        //}

        public UserData GetUserDataOut(Personnel user) 
        {
            return new  UserData() 
            { 
                FirstName = user.FirstName,
                LastName = user.LastName,
                ActiveOrganization = user.PersonnelConfig.ActiveOrganizationId,
                Id = user.PersonnelId,
                Organizations = user.GetOrganizationRefs(),
                Username = user.Username 
            };            
        }

        public List<CustomKeyValuePair> GetKeyValuePairs() 
        {
            List<string> lines = new List<string>();
            var path = $@"{basePath}/DocumentOntology.owl";
            var webRequest = WebRequest.Create(path);

            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            var formNames = lines.Where(x => x.Contains("# Class")).Select(x => x.Split(':').LastOrDefault()).ToList();
            return formNames.Select(x =>
                new CustomKeyValuePair {
                    Key = x.Split('(')[0],
                    Value = x.Split('(')[1].Replace(')', ' ') 
                }
                ).ToList();
        }
            
    }
}
