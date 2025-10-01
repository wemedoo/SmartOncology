using Microsoft.Extensions.Configuration;
using sReportsV2.BusinessLayer.Components.Interfaces;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using sReportsV2.DTOs.DTOs.PatientQuery.DataIn;
using sReportsV2.DTOs.DTOs.PatientQuery.DataOut;
using sReportsV2.DTOs.DTOs.ThesaurusEntry.DataIn;
using sReportsV2.DTOs.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Writing;

namespace sReportsV2.BusinessLayer.Components.Implementations
{
    public class SkosConnector : ISkosConnector
    {
        private readonly string graphDbEndpoint;
        private readonly IDictionary<string, string> _namespaces;

        public SkosConnector(IConfiguration configuration) 
        {
            this.graphDbEndpoint = configuration["GraphDbUrl"];
            this._namespaces = new Dictionary<string, string>
            {
                { SkosConstants.ThesaurusNamespace, SkosConstants.ThesaurusNamespaceUrl },
                { "dcterms", "http://purl.org/dc/terms/" },
                { "rdfs", "http://www.w3.org/2000/01/rdf-schema#" },
                { SkosConstants.MondoNamespace, "http://purl.obolibrary.org/obo/MONDO_" },
                { "skos", "http://www.w3.org/2004/02/skos/core#" }
            };
        }

        public bool UseSkosData()
        {
            return true;
        }

        public async Task InsertConcept(ThesaurusEntry thesaurusEntry, SkosDataIn skosData)
        {
            if (skosData == null) return;
            Graph graph = InitializeGraphAndNamespace();
            await DeleteConcept(thesaurusEntry.ThesaurusEntryId);
            SetConcept(graph, thesaurusEntry, skosData);
            LogResponse(await SendRequest(graph).ConfigureAwait(false));
        }

        public async Task DeleteConcept(int thesaurusEntryId)
        {
            LogResponse(await SendRequest(GetDeleteQuery(thesaurusEntryId), isDelete: true));
        }

        public async Task InsertConcepts(Tuple<Dictionary<string, ThesaurusEntry>, List<ParentChildDTO>> results)
        {
            Graph graph = InitializeGraphAndNamespace();
            Dictionary<string, ThesaurusEntry> thesauruses = results.Item1;
            List<ParentChildDTO> parentChildren = results.Item2;
            foreach (KeyValuePair<string, ThesaurusEntry> thesaurusEntryKeyValuePair in thesauruses)
            {
                SkosDataIn skosData = new SkosDataIn
                {
                    NarrowerThesaurusIds = parentChildren
                    .Where(x => x.Parent == thesaurusEntryKeyValuePair.Key)
                    .Select(x => thesauruses[x.Child].ThesaurusEntryId)
                    .ToList()
                };
                SetConcept(graph, thesaurusEntryKeyValuePair.Value, skosData);
            }
            LogResponse(await SendRequest(graph).ConfigureAwait(false));
        }

        public async Task<PatientQueryResultDataOut> GetNarrowerConcepts(PatientQueryFilterDataIn patientQueryFilterDataIn)
        {
            if (patientQueryFilterDataIn.IsEmptyQuery()) return new PatientQueryResultDataOut();
            SparqlResultSet queryResults = await GetResponse(GetNarrowerConceptsQuery(patientQueryFilterDataIn.DiagnoseId));
            return HandlePatientQueryResult(queryResults, patientQueryFilterDataIn.DiagnoseId);
        }

        public async Task<List<SkosLabelAutocompleteDataOut>> SearchByPrefAndAltTerm(string searchTerm)
        {
            SparqlResultSet queryResults = await GetResponse(GetSearchByPrefAndAltTermQuery(searchTerm));
            List<SkosLabelAutocompleteDataOut> results = new List<SkosLabelAutocompleteDataOut>();
            foreach (var result in queryResults)
            {
                results.Add(new SkosLabelAutocompleteDataOut
                {
                    id = GetThesaurusId(result["subject"]).ToString(),
                    text = GetLiteralValue(result["label"]),
                    issynonym = result["relation"].ToString().Equals(SkosConstants.AltLabelUrl)
                });
            }
            return results;
        }

        public async Task GetSkosData(ThesaurusEntry thesaurusEntry, ThesaurusEntryDataOut viewModel)
        {
            viewModel.SkosData.AllConceptSchemes = await GetConceptSchemes().ConfigureAwait(false);
            await GetBroaderNarrower(thesaurusEntry, viewModel).ConfigureAwait(false);
            if(thesaurusEntry.GetPreferredTermByActiveLanguage(LanguageConstants.EN) != "Solid Tumor")
            {
                await GetSubjectHierarchy(thesaurusEntry, viewModel).ConfigureAwait(false);
            }
        }

        public async Task<List<AutocompleteDataOut>> GetConceptSchemes()
        {
            SparqlResultSet results = await GetResponse(GetConceptSchemesQuery());
            List<AutocompleteDataOut> output = new List<AutocompleteDataOut>();
            foreach (var result in results)
            {
                string conceptSchemeName = GetLiteralValue(result["label"]);
                if (!string.IsNullOrEmpty(conceptSchemeName))
                {
                    output.Add(new AutocompleteDataOut
                    {
                        id = result["conceptSchema"].ToString(),
                        text = conceptSchemeName
                    });
                }
            }
            return output;
        }

        public async Task<string> ExportSkos(int thesaurusEntryId)
        {
            return await GetRawResponse(GetAllRelationsFor(thesaurusEntryId));
        }

        private async Task GetBroaderNarrower(ThesaurusEntry thesaurusEntry, ThesaurusEntryDataOut viewModel)
        {
            SparqlResultSet results = await GetResponse(GetBroaderNarrowerQuery(thesaurusEntry.ThesaurusEntryId));

            foreach (var result in results)
            {
                HandleSingleResult(result, viewModel);
            }
        }

        private void HandleSingleResult(ISparqlResult result, ThesaurusEntryDataOut viewModel)
        {
            if (result["relation"].ToString().Contains("inScheme"))
            {
                AutocompleteDataOut selectedConceptScheme = viewModel.SkosData.AllConceptSchemes.Find(cS => cS.id == result["target"].ToString());
                if (selectedConceptScheme != null)
                {
                    viewModel.SkosData.SelectedConceptSchemes.Add(selectedConceptScheme);
                }
            }
            else
            {
                int thesaurusId = GetThesaurusId(result["target"]);
                string targetLabel = GetLiteralValue(result["targetLabel"]);

                if (string.IsNullOrEmpty(targetLabel))
                {
                    viewModel.SkosData.MissingThesaurusIds.Add(thesaurusId);
                }
                else
                {
                    viewModel.SkosData.SelectedThesauruses.Add(new ThesaurusEntryViewDataOut { ThesaurusEntryId = thesaurusId, PreferredTerm = targetLabel });
                }

                if (result["relation"].ToString().Contains("broader"))
                {
                    viewModel.SkosData.BroaderThesaurusIds.Add(thesaurusId);
                }
                else
                {
                    viewModel.SkosData.NarrowerThesaurusIds.Add(thesaurusId);
                }
            }
        }

        private async Task GetSubjectHierarchy(ThesaurusEntry thesaurusEntry, ThesaurusEntryDataOut viewModel)
        {
            SparqlResultSet results = await GetResponse(GetSubjectHierarchyQuery(thesaurusEntry.ThesaurusEntryId));
            SubjectHierarchyDataOut handledResult = HandleSubjectHierarchyResult(results);
            viewModel.SkosData.MissingThesaurusIds.AddRange(handledResult.MissingThesaurusIds);
            viewModel.SkosData.Roots = handledResult.GetRoots();
            viewModel.SkosData.MissingThesaurusIds.AddRange(viewModel.SkosData.Roots.Select(x => x.Id));
        }

        private SubjectHierarchyDataOut HandleSubjectHierarchyResult(SparqlResultSet results)
        {
            SubjectHierarchyDataOut subjectHierarchyDataOut = new SubjectHierarchyDataOut();
            foreach (var result in results)
            {
                HandleSingleResult(subjectHierarchyDataOut, result);
            }
            return subjectHierarchyDataOut;
        }

        private PatientQueryResultDataOut HandlePatientQueryResult(SparqlResultSet results, int diagnoseId)
        {
            PatientQueryResultDataOut patientQueryResultDataOut = new PatientQueryResultDataOut();
            foreach (var result in results)
            {
                HandleSingleResult(patientQueryResultDataOut, result, diagnoseId);
            }
            return patientQueryResultDataOut;
        }

        private void HandleSingleResult(SubjectHierarchyDataOut subjectHierarchyDataOut, ISparqlResult result)
        {
            int parentThesaurusId = GetThesaurusId(result["parent"]);
            int childThesaurusId = GetThesaurusId(result["child"]);
            string label = GetLiteralValue(result["label"]);
            
            if (string.IsNullOrEmpty(label))
            {
                subjectHierarchyDataOut.MissingThesaurusIds.Add(childThesaurusId);
            }

            subjectHierarchyDataOut.ChildrenThesaurusIds.Add(childThesaurusId);
            if (subjectHierarchyDataOut.AllNodes.TryGetValue(childThesaurusId, out TreeNodeDataOut treeNode))
            {
                treeNode.UpdateLabel(label);
            }
            else
            {
                subjectHierarchyDataOut.AllNodes[childThesaurusId] = new TreeNodeDataOut { Id = childThesaurusId, Label = label };
            }

            if (!subjectHierarchyDataOut.AllNodes.ContainsKey(parentThesaurusId))
            {
                subjectHierarchyDataOut.AllNodes[parentThesaurusId] = new TreeNodeDataOut { Id = parentThesaurusId };
            }
                
            subjectHierarchyDataOut.AllNodes[parentThesaurusId].Children.Add(subjectHierarchyDataOut.AllNodes[childThesaurusId]);
        }

        private void HandleSingleResult(PatientQueryResultDataOut patientQueryResultDataOut, ISparqlResult result, int diagnoseId)
        {
            int parentThesaurusId = GetThesaurusId(result["parent"]);
            int childThesaurusId = GetThesaurusId(result["child"]);
            patientQueryResultDataOut.MatchingFieldInstanceThesaurusIds.Add(parentThesaurusId);
            patientQueryResultDataOut.MatchingFieldInstanceThesaurusIds.Add(childThesaurusId);
            string label = GetLiteralValue(result["label"]);
            string parentLabel = GetLiteralValue(result["parentLabel"]);

            patientQueryResultDataOut.GraphData.StartingMatchingThesaurusId = diagnoseId;
            patientQueryResultDataOut.GraphData.AddNode(parentThesaurusId, parentLabel);
            patientQueryResultDataOut.GraphData.AddNode(childThesaurusId, label);
            patientQueryResultDataOut.GraphData.AddEdge(parentThesaurusId, childThesaurusId);
        }

        #region Query Methods
        private string GetDeleteQuery(int thesaurusEntryId)
        {
            string subject = GetSubject(thesaurusEntryId);
            string query = $@"
                {_namespaces.GetGraphDbQueryHeader()}
                DELETE {{?s ?p ?o}} 
                WHERE {{
                  {{
                    BIND({subject} AS ?s)
                    ?s ?p ?o .
                  }}
                  UNION
                  {{
                    BIND({subject} AS ?o)
                    ?s ?p ?o .
                  }}
                }}
                ";
            return query;
        }

        private string GetBroaderNarrowerQuery(int thesaurusEntryId)
        {
            string subject = GetSubject(thesaurusEntryId);
            return $@"
                {_namespaces.GetGraphDbQueryHeader()}
                SELECT ?relation ?target ?targetLabel
                WHERE {{
                  {{
                    # Direct relations
                    {subject} ?relation ?target .
                    FILTER(?relation IN (skos:broader, skos:narrower, skos:inScheme))
                  }}
                  OPTIONAL {{
                    ?target skos:prefLabel ?label .
                    FILTER(LANG(?label) = ""en"")
                    BIND(?label AS ?targetLabel)
                    }}
                }}
                ";
        }

        private string GetSubjectHierarchyQuery(int thesaurusEntryId)
        {
            string subject = GetSubject(thesaurusEntryId);
            return $@"
                {_namespaces.GetGraphDbQueryHeader()}
                SELECT DISTINCT ?parent ?child ?label
                WHERE {{
                  {{
                    # Downward traversal: normal use of skos:narrower
                    {subject} skos:narrower+ ?child .
                    ?parent skos:narrower ?child .
                  }}
                  UNION
                  {{
                    # Upward traversal: reverse the skos:narrower path
                    ?child skos:narrower+ {subject} .
                    ?parent skos:narrower ?child .
                  }}
                  UNION
                  {{
                    # Add the direct parent of explicitly
                    ?parent skos:narrower {subject} .
                    BIND({subject} AS ?child)
                  }}
                  OPTIONAL {{ ?child skos:prefLabel ?label }}
                }}
            ";
        }

        private string GetNarrowerConceptsQuery(int thesaurusEntryId)
        {
            string subject = GetSubject(thesaurusEntryId);
            return $@"
                {_namespaces.GetGraphDbQueryHeader()}
                SELECT DISTINCT ?parent ?child ?label ?parentLabel
                WHERE
                    {{
                        {subject} skos:narrower+ ?child .
  	                    ?parent skos:narrower ?child .
                        ?child skos:prefLabel ?label .
                        ?parent skos:prefLabel ?parentLabel .
                        FILTER(STRSTARTS(STR(?parent), ""{SkosConstants.ThesaurusNamespaceUrl}""))
                    }}
                ";
        }

        private string GetSearchByPrefAndAltTermQuery(string searchTerm)
        {
            searchTerm = !string.IsNullOrEmpty(searchTerm) ? searchTerm.ToLower() : string.Empty;
            return $@"
                {_namespaces.GetGraphDbQueryHeader()}
                    SELECT ?subject ?relation ?label
                    WHERE
                        {{
                        ?subject ?relation ?label . 
                        FILTER(?relation IN (skos:prefLabel, skos:altLabel))
                        FILTER(STRSTARTS(STR(?subject), ""{SkosConstants.ThesaurusNamespaceUrl}""))
                        FILTER(STRSTARTS(LCASE(?label), ""{searchTerm}""))
                    }}
                    LIMIT 15
                ";
        }

        private string GetConceptSchemesQuery()
        {
            return $@"
                {_namespaces.GetGraphDbQueryHeader()}
                select ?conceptSchema ?label where {{
                    ?conceptSchema ?is skos:ConceptScheme .
                    ?conceptSchema ?labelPredicate ?label .
                    FILTER(?labelPredicate in (dcterms:title))
                }}
                ";
        }

        private string GetAllRelationsFor(int thesaurusEntryId)
        {
            string subject = GetSubject(thesaurusEntryId);
            return $@"
                {_namespaces.GetGraphDbQueryHeader()}
                SELECT DISTINCT ?predicate ?object
                WHERE
                    {{
                        {subject} ?predicate ?object
                    }}
                ";
        }
        #endregion /Query Methods

        #region Helper Methods
        private Graph InitializeGraphAndNamespace()
        {
            var graph = new Graph();
            foreach (KeyValuePair<string, string> _namespace in _namespaces)
            {
                graph.NamespaceMap.AddNamespace(_namespace.Key, new Uri(_namespace.Value));
            }
            return graph;
        }

        private void AddHierarchy(Graph graph, List<int> thesaurusIds, IUriNode subject, IUriNode relation, IUriNode inverseRelation)
        {
            foreach (int thesaurusId in thesaurusIds)
            {
                IUriNode objectRelation = graph.CreateUriNode(GetSubject(thesaurusId));
                graph.Assert(subject, relation, objectRelation);
                graph.Assert(objectRelation, inverseRelation, subject);
            }
        }

        private async Task<HttpResponseMessage> SendRequest(Graph graph, bool isDelete = false)
        {
            var ttlWriter = new CompressingTurtleWriter();
            var stringWriter = new System.IO.StringWriter();
            ttlWriter.Save(graph, stringWriter);
            string turtleData = stringWriter.ToString();
            return await SendRequest(stringWriter.ToString(), isDelete);
        }

        private async Task<HttpResponseMessage> SendRequest(string turtleData, bool isDelete = false)
        {
            LogHelper.Info("Generated RDF:\n" + turtleData);
            using var client = new HttpClient();
            var content = new StringContent(turtleData, Encoding.UTF8, isDelete ? "application/sparql-update" : "text/turtle");
            // Add auth header if needed
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes("user:password")));
            var response = await client.PostAsync($"{graphDbEndpoint}/statements", content).ConfigureAwait(false);
            return response;
        }

        private void LogResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                LogHelper.Info("Successfully uploaded to GraphDB.");
            }
            else
            {
                LogHelper.Error($"Upload failed: {response.StatusCode} - {response.Content.ReadAsStringAsync()}");
            }
        }

        private void SetConcept(Graph graph, ThesaurusEntry thesaurusEntry, SkosDataIn skosData)
        {
            ThesaurusEntryTranslation translation = thesaurusEntry.GetTranslation(LanguageConstants.EN);
            IUriNode subject = graph.CreateUriNode(GetSubject(thesaurusEntry.ThesaurusEntryId));
            IUriNode rdfType = graph.CreateUriNode("rdf:type");
            IUriNode type = graph.CreateUriNode("skos:Concept");
            IUriNode prefLabel = graph.CreateUriNode("skos:prefLabel");
            IUriNode exactMatch = graph.CreateUriNode("skos:exactMatch");
            IUriNode inScheme = graph.CreateUriNode("skos:inScheme");
            IUriNode altLabel = graph.CreateUriNode("skos:altLabel");
            IUriNode definition = graph.CreateUriNode("skos:definition");
            IUriNode broader = graph.CreateUriNode("skos:broader");
            IUriNode narrower = graph.CreateUriNode("skos:narrower");
            graph.Assert(subject, rdfType, type);
            graph.Assert(subject, prefLabel, graph.CreateLiteralNode(translation.PreferredTerm, translation.Language));
            graph.Assert(subject, definition, graph.CreateLiteralNode(translation.Definition ?? string.Empty, translation.Language));
            foreach (string synonym in translation.Synonyms)
            {
                graph.Assert(subject, altLabel, graph.CreateLiteralNode(synonym, translation.Language));
            }
            foreach (O4CodeableConcept codeableConcept in thesaurusEntry.Codes)
            {
                if (codeableConcept.System.Value == "Mondo Disease Ontology")
                {
                    graph.Assert(subject, exactMatch, graph.CreateUriNode(GetQualifiedName(codeableConcept.Value, SkosConstants.MondoNamespace)));
                }
            }
            foreach (string extensionUrl in skosData.ConceptSchemes)
            {
                graph.Assert(subject, inScheme, graph.CreateUriNode(GetConceptName(extensionUrl)));
            }
            AddHierarchy(graph, skosData.BroaderThesaurusIds, subject, broader, narrower);
            AddHierarchy(graph, skosData.NarrowerThesaurusIds, subject, narrower, broader);
        }

        private async Task<SparqlResultSet> GetResponse(string query)
        {
            using var client = new HttpClient();
            var sparqlQueryClient = new SparqlQueryClient(client, new Uri(graphDbEndpoint));
            var d1 = sparqlQueryClient.NamedGraphs;
            var d2 = sparqlQueryClient.DefaultGraphs;
            return await sparqlQueryClient.QueryWithResultSetAsync(query).ConfigureAwait(false);
        }

        private async Task<string> GetRawResponse(string query)
        {
            using var http = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, graphDbEndpoint);
            request.Content = new StringContent($"query={Uri.EscapeDataString(query)}",
                                                System.Text.Encoding.UTF8,
                                                "application/x-www-form-urlencoded");
            request.Headers.Add("Accept", "application/sparql-results+json");

            var response = await http.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var rawJson = await response.Content.ReadAsStringAsync();
            return rawJson;
        }

        private string GetQualifiedName(string conceptName, string conceptNamespace)
        {
            return $"{conceptNamespace}:{conceptName}";
        }

        private string GetLiteralValue(INode node)
        {
            return node is ILiteralNode literalNode ? literalNode.Value : string.Empty;
        }

        private string GetSubject(int thesaurusEntryId)
        {
            return $"{SkosConstants.ThesaurusNamespace}:{SkosConstants.ThesaurusSubjectPrefix}{thesaurusEntryId}";
        }

        private string GetConceptName(string input)
        {
            foreach (var namespaceMapping in _namespaces)
            {
                if (input.Contains(namespaceMapping.Value))
                {
                    input = input.Replace(namespaceMapping.Value, $"{namespaceMapping.Key}:");
                    break;
                }
            }
            return input;
        }

        private int GetThesaurusId(INode node)
        {
            Match thesaurusMatch = Regex.Match(node.ToString(), $"{SkosConstants.ThesaurusSubjectPrefix}(\\d+)");
            if (thesaurusMatch.Success)
            {
                return int.Parse(thesaurusMatch.Groups[1].Value);
            }
            else
            {
                throw new Exception("Invalid RDF subject");
            }
        }
        #endregion /Helper Methods
    }
}
