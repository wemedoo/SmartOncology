using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.DigitalGuideline;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Domain.Entities.DigitalGuidelineInstance
{
    public class GuidelineInstance : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string DigitalGuidelineId { get; set; }
        public int EpisodeOfCareId { get; set; }
        public Period Period { get; set; }
        public string Title { get; set; }
        public List<NodeValue> NodeValues { get; set; } = new List<NodeValue>();

        public NodeValue GetNodeValueById(string nodeId)
        {
            return this.NodeValues.Find(x => x.Id.Equals(nodeId));
        }

        public void SetStartNode(List<GuidelineEdgeElementData> edges)
        {
            this.NodeValues[0].State = NodeState.Completed;
            foreach (GuidelineEdgeElementData item in edges)
            {
                NodeValue nodeValue = this.NodeValues.Find(x => x.Id.Equals(item.Target));
                nodeValue.State = NodeState.Active;
            }
        }

        public void SetNodeValue(string value, string nodeId) 
        {
            NodeValue nodeValue = this.NodeValues.Find(x => x.Id.Equals(nodeId));
            if (!string.IsNullOrEmpty(value))
            {
                this.NodeValues.Find(x => x.Id.Equals(nodeId)).Value = value;
                nodeValue.State = NodeState.Completed;
            }
        }

        public void SetNextNodeState(string nodeId, Guideline guideline)
        {
            foreach (GuidelineEdgeElementData item in guideline.GetEdges(nodeId).Item1)
            {
                NodeValue nodeValue = this.NodeValues.Find(x => x.Id.Equals(item.Target));
                string nodeType = guideline.GuidelineElements.Nodes.Find(x => x.Data.Id.Equals(item.Target)).Data.Type;
                if (nodeType == "Decision")
                    SetStateForDecisionNode(item, nodeValue, guideline);
                else
                    nodeValue.State = NodeState.Active;
            }
        }

        public void SetStateForDecisionNode(GuidelineEdgeElementData edgeElement, NodeValue nodeValue, Guideline guideline)
        {
            List<NodeState> nodeStates = new List<NodeState>();
            foreach (GuidelineEdgeElementData item in guideline.GetEdges(edgeElement.Target).Item2)
                nodeStates.Add(this.NodeValues.Find(x => x.Id.Equals(item.Source)).State);

            if (nodeStates.TrueForAll(x => x.Equals(NodeState.Completed)))
                nodeValue.State = NodeState.Active;
        }
    }
}
