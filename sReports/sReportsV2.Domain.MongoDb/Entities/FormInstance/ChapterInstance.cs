using sReportsV2.Common.Enums;
using sReportsV2.Domain.MongoDb.Entities.FormInstance;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.Domain.Entities.FormInstance
{
    public class ChapterInstance : ChapterPageFieldSetInstanceBase
    {
        public string ChapterId { get; set; }
        public List<PageInstance> PageInstances { get; set; }

        public ChapterInstance(string chapterId, int? createdById, DateTime createdOn) : base(createdById, createdOn)
        {
            ChapterId = chapterId;
            PageInstances = new List<PageInstance>();
        }

        public PageInstance GetPageInstance(string pageId)
        {
            return PageInstances.Find(pI => pI.PageId == pageId);
        }

        public override void RecordLatestWorkflowChangeStateAndPropagate(ChapterPageFieldSetInstanceStatus latestChangeState)
        {
            this.RecordLatestWorkflowChangeState(latestChangeState);
            foreach (var pageInstance in PageInstances) { 
                pageInstance.RecordLatestWorkflowChangeStateAndPropagate(latestChangeState);
            }
        }
    }
}
