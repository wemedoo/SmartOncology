using sReportsV2.Common.Constants;
using sReportsV2.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace sReportsV2.UMLS.Classes
{
    public class CsvExporter
    {
        private static string basePath = string.Empty;
        private static Dictionary<RankKey, string> rankDictionary = new Dictionary<RankKey, string>();
        private static Dictionary<string, List<string>> defDictionary = new Dictionary<string, List<string>>();
        private static Dictionary<string, string> languages = new Dictionary<string, string>()
        {
            {"ENG", LanguageConstants.EN },
            {"FRE", LanguageConstants.FR },
            {"GER", LanguageConstants.DE },
            {"ITA", LanguageConstants.IT },
            {"SPA", LanguageConstants.ES },
            {"POR", LanguageConstants.PT }
        };

        private static List<string> terms = new List<string>();
        private static Dictionary<string, string> semantycTypeDictionary = new Dictionary<string, string>();
        private static List<UmlsCsvEntity> csvEntries = new List<UmlsCsvEntity>();

        public CsvExporter() { }
        public void Import(string path)
        {
            basePath = path;
            LoadMRRANKIntoMemory();
            LoadMRDEFIntoMemory();
            LoadMRSTYIntoMemory();
            LoadMRCONSOIntoMemory();
        }

        private static void LoadMRCONSOIntoMemory()
        {
            string currentConcept = string.Empty;
            int conceptCount = 0;
            UmlsConcept concept = new UmlsConcept();

            try
            {
                var path = $@"{basePath}/MRCONSO.RRF";
                var webRequest = WebRequest.Create(path);

                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        if (currentConcept != parts[0])
                        {
                            AddToCsvEntriesIfValid(concept);
                            conceptCount++;
                            currentConcept = parts[0];
                            concept = new UmlsConcept() 
                            {
                                CUI = parts[0],
                                TTY = parts[12],
                                Atoms = new List<UmlsAtom>()
                            };

                        }
                        if (languages.ContainsKey(parts[1]))
                        {
                            UmlsAtom atom = new UmlsAtom() 
                            {
                                Language = parts[1],
                                Definition = GetDefinition(parts[7]),
                                Name = parts[14],
                                AUI = parts[7],
                                Source = parts[11]
                            };
                            concept.Atoms.Add(atom);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HandleException(e, "MRCONSO.RRF");
            }
        }

        private static void AddToCsvEntriesIfValid(UmlsConcept concept)
        {
            if (concept != null && concept.Atoms != null) 
            {
                foreach (string term in terms) 
                {
                    if(concept.Atoms.Select(y => y.Name).AsEnumerable().Any(z => z.Contains(term))) 
                    {
                        csvEntries.Add(new UmlsCsvEntity()
                        {
                            UI = concept.CUI,
                            Name = concept.Atoms.Find(x => !string.IsNullOrWhiteSpace(x.Name))?.Name,
                            Definition = concept.Atoms.Find(x => !string.IsNullOrWhiteSpace(x.Definition))?.Definition,
                            Atoms = GetAtomsForCsv(concept.Atoms),
                            SemanticType = semantycTypeDictionary[concept.CUI],
                            Term = term
                        });

                        break;
                    }
                }
            }
        }

        private static string GetAtomsForCsv(List<UmlsAtom> atoms)
        {
            StringBuilder textBuilder = new StringBuilder();
            foreach (UmlsAtom atom in atoms)
            {
                textBuilder.Append($"{atom?.Name} ({atom?.AUI}- {atom?.Source} - {atom?.Language})" + "\n");
            }

            return textBuilder.ToString();
        }



        private static string GetDefinition(string atomIdentifier)
        {
            return defDictionary.ContainsKey(atomIdentifier) ? string.Join(Environment.NewLine, defDictionary[atomIdentifier]) : string.Empty;
        }
        private static void LoadMRDEFIntoMemory()
        {
            try
            {
                var path = $@"{basePath}/MRDEF.RRF";
                var webRequest = WebRequest.Create(path);

                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        if (!defDictionary.ContainsKey(parts[1]))
                        {
                            defDictionary.Add(parts[1], new List<string>() { parts[5] });
                        }
                        else
                        {
                            defDictionary[parts[1]].Add(parts[5]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HandleException(e, "MRDEF.RRF");
            }
        }

        private static void LoadMRRANKIntoMemory()
        {
            try
            {
                var path = $@"{basePath}/MRRANK.RRF";
                var webRequest = WebRequest.Create(path);

                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        RankKey key = new RankKey() { SAB = parts[1], TTY = parts[2] };
                        if (!rankDictionary.Any(x => x.Key.Equals(key)))
                        {
                            rankDictionary.Add(key, parts[0]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HandleException(e, "MRRANK.RRF");
            }
        }
        private static void LoadMRSTYIntoMemory()
        {
            try
            {
                var path = $@"{basePath}/MRSTY.RRF";
                var webRequest = WebRequest.Create(path);

                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        if (!semantycTypeDictionary.ContainsKey(parts[0])) 
                        {
                            semantycTypeDictionary.Add(parts[0], parts[3]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HandleException(e, "MRSTY.RRF");
            }
        }

        private static void HandleException(Exception ex, string fileName)
        {
            LogHelper.Error($"The file ({fileName}) could not be read, exception message: {ex.Message}");
            LogHelper.Error(ex.StackTrace);
        }
    }
}
