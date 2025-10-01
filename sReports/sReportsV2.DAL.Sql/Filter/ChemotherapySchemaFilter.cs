using sReportsV2.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Filter
{
    public class ChemotherapySchemaFilter : EntityFilter
    {
        public string Indication { get; set; }
        public string State { get; set; }
        public string ClinicalConstelation { get; set; }
        public string Name { get; set; }
    }
}
