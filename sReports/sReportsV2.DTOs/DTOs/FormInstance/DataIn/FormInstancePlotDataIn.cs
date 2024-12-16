using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.FormInstance.DataIn
{
    public class FormInstancePlotDataIn
    {
        public string FormDefinitionId { get; set; }
        public List<int> FieldThesaurusIds { get; set; }
        public DateTime? DateTimeFrom { get; set; }
        public DateTime? DateTimeTo { get; set; }  
        public int OrganizationId { get; set; }  
    }
}
