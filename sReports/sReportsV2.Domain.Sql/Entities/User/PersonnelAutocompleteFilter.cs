using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.User
{
    public class PersonnelAutocompleteFilter
    {
        public int OrganizationId { get; set; }
        public bool FilterByDoctors { get; set; }
        public string Name { get; set; }
    }
}
