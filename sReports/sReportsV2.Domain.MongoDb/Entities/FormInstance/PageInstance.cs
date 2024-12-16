using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Domain.Entities.FormInstance
{
    public class PageInstance
    {
        public string PageId { get; set; }
        public List<ChapterPageInstanceStatus> WorkflowHistory { get; set; }
        public PageInstance(string pageId, int? createdById, DateTime createdOn)
        {
            PageId = pageId;
            WorkflowHistory = new List<ChapterPageInstanceStatus> {
                new ChapterPageInstanceStatus(ChapterPageState.DataEntryOnGoing, createdById, createdOn)
            };
        }
        public ChapterPageInstanceStatus GetLastChange()
        {
            return WorkflowHistory?.LastOrDefault();
        }

        public void RecordLatestWorkflowChangeState(ChapterPageInstanceStatus latestChangeState)
        {
            WorkflowHistory.Add(latestChangeState);
        }
    }
}
