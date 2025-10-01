using sReportsV2.DTOs.Common.DataOut;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.DTOs.DTOs.ThesaurusEntry.DataOut
{
    public class SubjectHierarchyDataOut
    {
        public Dictionary<int, TreeNodeDataOut> AllNodes { get; set; } = new Dictionary<int, TreeNodeDataOut>();
        public List<int> ChildrenThesaurusIds { get; set; } = new List<int>();
        public List<int> MissingThesaurusIds { get; set; } = new List<int>();
        public List<TreeNodeDataOut> GetRoots()
        {
            return AllNodes.Values.Where(n => !ChildrenThesaurusIds.Contains(n.Id)).ToList();
        }
    }
}
