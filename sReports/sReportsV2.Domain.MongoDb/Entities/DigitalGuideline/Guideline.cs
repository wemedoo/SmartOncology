using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Domain.Entities.DigitalGuideline
{
    [BsonIgnoreExtraElements]
    public class Guideline : Entity
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        public int UserId { get; set; }
        public GuidelineElements GuidelineElements { get; set; }
        public sReportsV2.Domain.Entities.Form.Version Version { get; set; }

        public Tuple<List<GuidelineEdgeElementData>, List<GuidelineEdgeElementData>> GetEdges(string nodeId)
        {
            List<GuidelineEdgeElementData> guidelineNextEdges = new List<GuidelineEdgeElementData>();
            List<GuidelineEdgeElementData> guidelinePreviusEdges = new List<GuidelineEdgeElementData>();
            foreach (GuidelineElementData item in this.GuidelineElements.Edges.Select(c => c.Data).ToList()) 
            { 
                GuidelineEdgeElementData castedElementData = (GuidelineEdgeElementData)item;
                if (castedElementData.Source == nodeId)
                    guidelineNextEdges.Add(castedElementData);
                if (castedElementData.Target == nodeId)
                    guidelinePreviusEdges.Add(castedElementData);
            }
            return Tuple.Create(guidelineNextEdges, guidelinePreviusEdges);
        }

        public void InitializeVersionIfMissing()
        {
            if (this.Version == null)
            {
                this.Version = new Form.Version();
            }

            var major = this.Version.Major >= 1 ? this.Version.Major : 1;
            var minor = this.Version.Minor >= 0 ? this.Version.Minor : 0;

            this.Version = new Form.Version()
            {
                Major = major,
                Minor = minor,
                Id = Guid.NewGuid().ToString()
            };
        }
    }

    public class GuidelineElements
    {
        public List<GuidelineElement> Nodes { get; set; }
        public List<GuidelineElement> Edges { get; set; }
    }
}
