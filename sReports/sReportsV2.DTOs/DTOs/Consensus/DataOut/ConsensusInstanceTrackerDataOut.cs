using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Consensus.DataOut
{
    public class ConsensusInstanceTrackerDataOut
    {
        public int UserRef { get; set; }
        public string UserName { get; set; }
        public bool IsOutsideUser { get; set; }
        public DateTimeOffset? EntryDateTime { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public double PercentDone { get; set; }

        public ConsensusInstanceTrackerDataOut() { }

        public ConsensusInstanceTrackerDataOut(int userRef, string userName, bool isOutsideUser, DateTimeOffset? entryDateTime, DateTimeOffset? lastUpdate, double percentDone) 
        {
            this.UserRef = userRef;
            this.UserName = userName;
            this.IsOutsideUser = isOutsideUser;
            this.EntryDateTime = entryDateTime;
            this.LastUpdate = lastUpdate;
            this.PercentDone = percentDone;
        }

    }
}