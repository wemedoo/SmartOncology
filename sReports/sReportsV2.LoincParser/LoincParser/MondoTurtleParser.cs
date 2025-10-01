using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Sql.Entities.CodeSystem;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DTOs.Common.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace sReportsV2.LoincParser.LoincParser
{
    public class MondoTurtleParser
    {
        private readonly string _turtleFilePath;
        private readonly int? _draftStateCD;
        private readonly Graph _graph;
        private readonly IDictionary<string, string> _namespaces;

        public MondoTurtleParser(int? draftStateCD, string fileName)
        {
            this._turtleFilePath = Path.Combine(DirectoryHelper.AppDataFolder, ResourceTypes.OntologyFolder, fileName);
            this._draftStateCD = draftStateCD;
            this._graph = LoadGraph();
            this._namespaces = new Dictionary<string, string>
            {
                { SkosConstants.ThesaurusNamespace, SkosConstants.ThesaurusNamespaceUrl },
                { SkosConstants.MondoNamespace, "http://purl.obolibrary.org/obo/MONDO_" },
                { "obo",  "http://purl.obolibrary.org/obo/" },
                { "rdfs", "http://www.w3.org/2000/01/rdf-schema#" },
                { "skos", "http://www.w3.org/2004/02/skos/core#" }  
            };
        }

        public List<CodeSystem> GetCodeSystems()
        {
            SparqlResultSet codeSystemResults = (SparqlResultSet)_graph.ExecuteQuery(GetSparqQueryForCodeSystems());
            List<CodeSystem> codeSystems = new List<CodeSystem>();
            foreach (SparqlResult codeSystemResult in codeSystemResults)
            {
                if (codeSystemResult["subjectName"] is LiteralNode literal)
                {
                    string subject = literal.Value;
                    codeSystems.Add(new CodeSystem
                    {
                        Value = subject,
                        Label = subject
                    });
                }

            }
            return codeSystems;
        }

        public Tuple<Dictionary<string, ThesaurusEntry>, List<ParentChildDTO>> GetOntology(List<CodeSystem> codeSystems)
        {
            SparqlResultSet results = (SparqlResultSet)_graph.ExecuteQuery(GetSparqQueryForAllData());
            Dictionary<string, ThesaurusEntry> thesauruses = new Dictionary<string, ThesaurusEntry>();
            List<ParentChildDTO> parentChildren = new List<ParentChildDTO>();
            foreach (SparqlResult result in results)
            {
                string subject = ExtractSubject(result["subject"]);
                if (!string.IsNullOrEmpty(subject))
                {
                    ThesaurusEntry thesaurusEntry;
                    if (!thesauruses.TryGetValue(subject, out thesaurusEntry))
                    {
                        thesaurusEntry = new ThesaurusEntry
                        {
                            StateCD = _draftStateCD,
                            Translations = new List<ThesaurusEntryTranslation>
                            {
                                new ThesaurusEntryTranslation
                                {
                                    Language = LanguageConstants.EN,
                                    Abbreviations = new List<string>(),
                                    Synonyms = new List<string>()
                                }
                            }
                        };
                        thesauruses.Add(subject, thesaurusEntry);
                    }
                    UpdateEntry(result, thesaurusEntry, codeSystems, subject, parentChildren);
                }

            }
            return new Tuple<Dictionary<string, ThesaurusEntry>, List<ParentChildDTO>>(thesauruses, parentChildren);
        }

        private Graph LoadGraph()
        {
            Graph graph = new Graph();
            TurtleParser parser = new TurtleParser();
            parser.Load(graph, _turtleFilePath);
            return graph;
        }

        private string GetSparqQueryForCodeSystems()
        {
            // Query to get subclass relationships and labels
            return $@"
                {_namespaces.GetGraphDbQueryHeader()}

                SELECT ?subject ?subjectName ?relation
                WHERE {{
                  {{
                    ?subject ?relation skos:ConceptScheme .
                    ?subject rdfs:label ?subjectName .
                  }}
                }}
            ";
        }

        private string GetSparqQueryForAllData()
        {
            // Query to get subclass relationships and labels
            return $@"
                {_namespaces.GetGraphDbQueryHeader()}

                SELECT ?subject ?predicate ?object WHERE {{
                    ?subject ?subjectType skos:Concept .
                    ?subject ?predicate ?object .
                }}
            ";
        }

        private void UpdateEntry(SparqlResult result, ThesaurusEntry thesaurusEntry, List<CodeSystem> codeSystems, string codeValue, List<ParentChildDTO> parentChildren)
        {
            string predicate = result["predicate"].ToString();
            INode nodeValue = result["object"];
            ThesaurusEntryTranslation englishTranslation = thesaurusEntry.Translations.First();
            string literalValue = string.Empty;
            if (nodeValue is LiteralNode literal)
            {
                literalValue = literal.Value;
            }
            switch (predicate)
            {
                case "http://www.w3.org/2004/02/skos/core#inScheme":
                    CodeSystem ontologyCodeSystem = codeSystems.FirstOrDefault();
                    if (ontologyCodeSystem != null)
                    {
                        thesaurusEntry.Codes = new List<O4CodeableConcept>
                        {
                            new O4CodeableConcept{
                                Code = codeValue,
                                Value = codeValue,
                                Version = "1.0",
                                System = ontologyCodeSystem,
                                VersionPublishDate = DateTime.Now,
                                EntryDateTime = DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone()
                            }
                        };
                    }
                    break;
                case "http://www.w3.org/2004/02/skos/core#broader":
                    parentChildren.Add(new ParentChildDTO
                    {
                        Parent = ExtractSubject(nodeValue),
                        Child = codeValue
                    });
                    break;
                case "http://www.w3.org/2004/02/skos/core#narrower":
                    parentChildren.Add(new ParentChildDTO
                    {
                        Parent = codeValue,
                        Child = ExtractSubject(nodeValue)
                    });
                    break;
                case "http://www.w3.org/2004/02/skos/core#definition":
                    englishTranslation.Definition = literalValue;
                    break;
                case "http://www.w3.org/2004/02/skos/core#prefLabel":
                    englishTranslation.PreferredTerm = literalValue;
                    break;
                case SkosConstants.AltLabelUrl:
                    englishTranslation.Synonyms.Add(literalValue);
                    break;
                default:
                    break;
            }
        }

        private string ExtractSubject(INode subjectNode)
        {
            string subject = subjectNode.ToString();
            int lastIndexOfSeparator = subject.LastIndexOf('_');
            string codeValue = string.Empty;
            if (lastIndexOfSeparator != -1)
            {
                codeValue = subject.Substring(lastIndexOfSeparator + 1);
            }
            return codeValue;
        }
    }
}