using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DTOs.DTOs.Organization.DataIn
{
    public class OrganizationCommunicationEntityDataIn
    {
        public int OrgCommunicationEntityId { get; set; }
        public string DisplayName { get; set; }
        public int? OrgCommunicationEntityCD { get; set; }
        public int? PrimaryCommunicationSystemCD { get; set; }
        public int? SecondaryCommunicationSystemCD { get; set; }
        public int? OrganizationId { get; set; }
        public DateTimeOffset ActiveFrom { get; set; }
        public DateTimeOffset ActiveTo { get; set; }
    }
}
