using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.User.DataIn
{
    public class PersonnelOccupationDataIn
    {
        public int? PersonnelId { get; set; }
        public int? OccupationCategoryCD { get; set; }
        public int? OccupationSubCategoryCD { get; set; }
        public int? OccupationCD { get; set; }
        public int? PersonnelSeniorityCD { get; set; }
    }
}
