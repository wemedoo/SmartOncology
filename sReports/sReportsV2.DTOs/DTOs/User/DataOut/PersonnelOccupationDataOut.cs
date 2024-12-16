using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.User.DataOut
{
    public class PersonnelOccupationDataOut
    {
        public int PersonnelOccupationId { get; set; }
        public int PersonnelId { get; set; }
        public int? OccupationCategoryCD { get; set; }
        public int? OccupationSubCategoryCD { get; set; }
        public int? OccupationCD { get; set; }
        public int? PersonnelSeniorityCD { get; set; }
        public DateTimeOffset ActiveFrom { get; set; }
        public DateTimeOffset ActiveTo { get; set; }
    }
}
