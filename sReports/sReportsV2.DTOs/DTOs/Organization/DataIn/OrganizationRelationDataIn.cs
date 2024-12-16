using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.Organization.DataIn
{
    public class OrganizationRelationDataIn
    {
        public int ParentId { get; set; }
        public int ChildId { get; set; }
    }
}
