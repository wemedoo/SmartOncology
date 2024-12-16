using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.FormConsensus.DataIn
{
    public class RemindUserDataIn
    {
        public int UserId { get; set; }
        public string ConsensusId { get; set; }
        public bool IsOutsideUser { get; set; }
        public string IterationId { get; set; }
    }
}
