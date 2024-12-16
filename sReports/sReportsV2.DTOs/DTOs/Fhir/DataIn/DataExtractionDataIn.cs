using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.Fhir.DataIn
{
    public class DataExtractionDataIn
    {
        public string FormInstanceId { get; set; }
        public string FieldIdWithDataToExtract { get; set; }
        public string FieldInstanceIdWithDataToExtract { get; set; }
    }
}
