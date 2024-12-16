using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Domain.Entities.FormInstance
{
    public class ChapterInstance
    {
        public string ChapterId { get; set; }
        public List<ChapterPageInstanceStatus> WorkflowHistory { get; set; }
        public List<PageInstance> PageInstances { get; set; }

        public ChapterInstance(string chapterId, int? createdById, DateTime createdOn)
        {
            ChapterId = chapterId;
            PageInstances = new List<PageInstance>();
            WorkflowHistory = new List<ChapterPageInstanceStatus> {
                new ChapterPageInstanceStatus(ChapterPageState.DataEntryOnGoing, createdById, createdOn)
            };
        }

        public ChapterPageInstanceStatus GetLastChange()
        {
            return WorkflowHistory?.LastOrDefault();
        }

        public PageInstance GetPageInstance(string pageId)
        {
            return PageInstances.Find(pI => pI.PageId == pageId);
        }

        public void RecordLatestWorkflowChangeState(ChapterPageInstanceStatus latestChangeState)
        {
            WorkflowHistory.Add(latestChangeState);
        }
    }
}
