using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.DigitalGuideline.DataOut
{
    public class GuidelineDataOut
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        public DateTime? EntryDatetime { get; set; }
        public DateTime? LastUpdate { get; set; }
        public GuidelineElementsDataOut GuidelineElements { get; set; }
        public VersionDTO Version { get; set; }

        public object ToJsonNodeElements()
        {
            return new
            {
                nodes = GuidelineElements?.Nodes,
                edges = GuidelineElements?.Edges,
                title = Title,
                thesaurusId = ThesaurusId,
                version = Version
            }.JsonSerialize(true);
        }

        public object ToExportJson()
        {
            return new
            {
                id = this.Id,
                title = this.Title,
                thesaurusId = this.ThesaurusId,
                entryDatetime = this.EntryDatetime,
                lastUpdate = this.LastUpdate,
                guidelineElements = new
                {
                    nodes = this.GuidelineElements?.Nodes?.Select(n => new
                    {
                        data = new
                        {
                            id = n.Data.Id,
                            state = n.Data.State,
                            thesaurusId = n.Data.ThesaurusId,
                            title = n.Data.Title,
                            type = n.Data.Type
                        },
                        position = n.Position,
                        group = n.Group,
                        removed = n.Removed,
                        selected = n.Selected,
                        selectable = n.Selectable,
                        locked = n.Locked,
                        grabbable = n.Grabbable,
                        pannable = n.Pannable,
                        classes = n.Classes
                    }),
                    edges = this.GuidelineElements?.Edges?.Select(e =>
                    {
                        var edgeData = e.Data as GuidelineEdgeElementDataDataOut;

                        return new
                        {
                            data = new
                            {
                                source = edgeData?.Source,
                                target = edgeData?.Target,
                                id = edgeData?.Id,
                                state = edgeData?.State,
                                thesaurusId = edgeData?.ThesaurusId,
                                title = edgeData?.Title,
                                type = edgeData?.Type
                            },
                            position = e.Position,
                            group = e.Group,
                            removed = e.Removed,
                            selected = e.Selected,
                            selectable = e.Selectable,
                            locked = e.Locked,
                            grabbable = e.Grabbable,
                            pannable = e.Pannable,
                            classes = e.Classes
                        };
                    })
                },
                version = new
                {
                    id = this.Version?.Id,
                    major = this.Version?.Major,
                    minor = this.Version?.Minor
                }
            };
        }
    }

    public class GuidelineElementsDataOut
    {
        public List<GuidelineElementDataOut> Nodes { get; set; }
        public List<GuidelineElementDataOut> Edges { get; set; }
    }
}