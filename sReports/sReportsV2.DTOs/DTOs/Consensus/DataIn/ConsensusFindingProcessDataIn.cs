using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Form.DataIn
{
    public class ConsensusFindingProcessDataIn
    {
        public List<int> UsersIds { get; set; } = new List<int>();
        public List<int> OutsideUsersIds { get; set; } = new List<int>();
        public string ConsensusId { get; set; }
        public string EmailMessage { get; set; }
    }
}