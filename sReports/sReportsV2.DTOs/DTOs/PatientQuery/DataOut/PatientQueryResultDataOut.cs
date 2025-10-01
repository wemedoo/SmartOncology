using System.Collections.Generic;

namespace sReportsV2.DTOs.DTOs.PatientQuery.DataOut
{
    public class PatientQueryResultDataOut
    {
        public List<int> MatchingFieldInstanceThesaurusIds { get; set; }
        public GraphData GraphData { get; set; }

        public PatientQueryResultDataOut()
        {
            MatchingFieldInstanceThesaurusIds = new List<int>();
            GraphData = new GraphData();
        }
    }

    public class GraphData
    {
        public int StartingMatchingThesaurusId { get; set; }
        public List<NodeData> Nodes { get; set; }
        public List<EdgeData> Edges { get; set; }

        public GraphData()
        {
            Nodes = new List<NodeData>();
            Edges = new List<EdgeData>();
        }

        public void AddNode(int thesaurusId, string label)
        {
            NodeData node = Nodes.Find(n => n.Id == thesaurusId);
            if (node != null)
            {
                node.UpdateLabel(label);
            }
            else
            {
                Nodes.Add(new NodeData
                {
                    Id = thesaurusId,
                    Label = label
                });
            }
        }

        public void AddEdge(int parentThesaurusId, int childThesaurusId)
        {
            Edges.Add(new EdgeData(parentThesaurusId, childThesaurusId));
        }

        public bool ShowGraph()
        {
            return Nodes.Find(n => n.Id == this.StartingMatchingThesaurusId)?.Label != "Solid Tumor";
        }
    }

    public class NodeData
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public bool Shown { get; set; }
        public bool Expanded { get; set; }

        public void UpdateLabel(string label)
        {
            if (!string.IsNullOrEmpty(label) && string.IsNullOrEmpty(Label))
            {
                this.Label = label;
            }
        }
    }

    public class EdgeData
    {
        public int From { get; set; }
        public int To { get; set; }
        public string Arrows { get; set; }
        public bool Shown { get; set; }
        public EdgeData(int from, int to)
        {
            From = from;
            To = to;
            Arrows = "to";
        }
    }
}
