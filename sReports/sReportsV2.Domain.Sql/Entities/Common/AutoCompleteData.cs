using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    public class AutoCompleteUserData
    {
        public int PersonnelId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }

        public override string ToString()
        {
            return $"{UserName} ({FirstName} {LastName})";
        }
    }

    public class AutoCompleteData
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }
}
