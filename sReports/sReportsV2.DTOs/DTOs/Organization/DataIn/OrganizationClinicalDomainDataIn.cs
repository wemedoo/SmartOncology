using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.Organization.DataIn
{
    public class OrganizationClinicalDomainDataIn
    {
        public int OrganizationClinicalDomainId { get; set; }
        public int ClinicalDomainCD { get; set; }
    }
}
