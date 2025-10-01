using sReportsV2.Common.Constants;
using sReportsV2.Common.Helpers;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using System.Collections.Generic;
using System.IO;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace sReportsV2.LoincParser.LoincParser
{
    public class DocumentPropertiesTurtleParser
    {
        private readonly string _turtleFilePath;

        public DocumentPropertiesTurtleParser()
        {
            this._turtleFilePath = Path.Combine(DirectoryHelper.AppDataFolder, ResourceTypes.OntologyFolder, "Document", "DocumentOntology.ttl");
        }

        public Dictionary<string, List<LoincPropertyAutocompleteDataOut>> GetLoincDatasource()
        {
            Graph graph = LoadGraph();
            Dictionary<string, List<CustomKeyValuePair>> queryResults = ProcessQuery(graph);
            Dictionary<string, string> objectProperties = GetObjectProperties(graph);

            return TransformToDataOut(queryResults, objectProperties);
        }

        private Graph LoadGraph()
        {
            Graph graph = new Graph();
            TurtleParser parser = new TurtleParser();
            parser.Load(graph, _turtleFilePath);
            return graph;
        }

        private string GetSparqQuery()
        {
            // Query to get subclass relationships and labels
            return @"
                PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
                PREFIX loinc: <https://loinc.org/>

                SELECT ?child ?childLabel ?parent ?parentLabel WHERE {
                    ?child rdfs:subClassOf ?parent .
                    ?child rdfs:label ?childLabel .
                    ?parent rdfs:label ?parentLabel .
                }
                ORDER BY ?childLabel
                ";
        }

        private Dictionary<string, List<CustomKeyValuePair>> ProcessQuery(Graph graph) 
        {
            SparqlResultSet results = (SparqlResultSet)graph.ExecuteQuery(GetSparqQuery());

            Dictionary<string, List<CustomKeyValuePair>> hierarchy = new Dictionary<string, List<CustomKeyValuePair>>();

            foreach (SparqlResult result in results)
            {
                if (result.HasValue("parent"))
                {
                    string parentUri = result["parent"].ToString();
                    string childUri = result["child"].ToString();
                    string childLabel = ExtractFromBegining(result["childLabel"].ToString());

                    if (!hierarchy.ContainsKey(parentUri))
                    {
                        hierarchy[parentUri] = new List<CustomKeyValuePair>();
                    }
                    hierarchy[parentUri].Add(new CustomKeyValuePair { Key = childUri, Value = childLabel});
                }
            }

            return hierarchy;
        }

        private Dictionary<string, string> GetObjectProperties(Graph graph)
        {
            string sparqlQuery = @"
                PREFIX owl: <http://www.w3.org/2002/07/owl#>
                PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>

                SELECT ?property ?label ?domain ?range WHERE {
                    ?property a owl:ObjectProperty .
                    ?property rdfs:range ?range .
                }"
            ;

            SparqlResultSet results = (SparqlResultSet)graph.ExecuteQuery(sparqlQuery);

            Dictionary<string, string> objectProperties = new Dictionary<string, string>();

            foreach (var result in results)
            {
                string property = ExtractFromTheEnd(ExtractFromTheEnd(result["property"].ToString()), '.');
                string range = result["range"].ToString();
                objectProperties[property] = range;
            }

            return objectProperties;
        }

        private string ExtractFromBegining(string value)
        {
            int labelSeparatorIndex = value.IndexOf('^');
            string extractedValue = string.Empty;
            if (labelSeparatorIndex >= 0)
            {
                extractedValue = value.Substring(0, labelSeparatorIndex);
                extractedValue = extractedValue.Replace("(LP)", "");
            }
            return extractedValue;
        }

        private string ExtractFromTheEnd(string value, char separator = '/')
        {
            int lastIndexOfSlash = value.LastIndexOf(separator);
            string extractedValue = string.Empty;
            if (lastIndexOfSlash >= 0)
            {
                extractedValue = value.Substring(lastIndexOfSlash + 1);
            }
            return extractedValue;
        }

        private Dictionary<string, List<LoincPropertyAutocompleteDataOut>> TransformToDataOut(Dictionary<string, List<CustomKeyValuePair>> queryResults, 
            Dictionary<string, string> objectProperties)
        {
            Dictionary<string, List<LoincPropertyAutocompleteDataOut>> loincProperties = new Dictionary<string, List<LoincPropertyAutocompleteDataOut>>();
            foreach (var item in objectProperties)
            {
                List<LoincPropertyAutocompleteDataOut> loincPropertyPerName = new List<LoincPropertyAutocompleteDataOut>();
                CollectDeepInHierarchy(queryResults, item.Value, string.Empty, 0, loincPropertyPerName);
                loincProperties[item.Key] = loincPropertyPerName;
            }

            return loincProperties;
        }

        private void CollectDeepInHierarchy(Dictionary<string, List<CustomKeyValuePair>> hierarchy, string parentUri, string parentLabel, int level, List<LoincPropertyAutocompleteDataOut> loincPropertyPerName)
        {
            if (hierarchy.TryGetValue(parentUri, out List<CustomKeyValuePair> value))
            {
                foreach (CustomKeyValuePair entry in value)
                {
                    loincPropertyPerName.Add(new LoincPropertyAutocompleteDataOut
                    {
                        id = ExtractFromTheEnd(entry.Key),
                        text = entry.Value,
                        level = level.ToString()
                    });
                    CollectDeepInHierarchy(hierarchy, entry.Key, entry.Value, level + 1, loincPropertyPerName);
                }
            }
        }
    }
}
