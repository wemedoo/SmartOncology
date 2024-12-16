using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.Domain.Sql.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Common
{
    public class ReferalInfo
    {
        public string Id { get; set; }
        public string VersionId { get; set; }
        public string Title { get; set; }
        public int ThesaurusId { get; set; }
        public int UserId { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int OrganizationId { get; set; }
        public List<KeyValue> ReferrableFields { get; set; }

        public ReferalInfo()
        {
        }

        public ReferalInfo(ReferralForm referral)
        {
            this.Id = referral.Id;
            this.VersionId = referral.VersionId;
            this.Title = referral.Title;
            this.ThesaurusId = referral.ThesaurusId;
            this.LastUpdate = referral.LastUpdate;
            this.UserId = referral.UserId;
            this.OrganizationId = referral.OrganizationId;
            this.ReferrableFields = new List<KeyValue>();
        }

        public ReferalInfo(Form.Form referral)
        {
            this.Id = referral.Id;
            this.VersionId = referral.Version.Id;
            this.Title = referral.Title;
            this.ThesaurusId = referral.ThesaurusId;
            this.LastUpdate = referral.LastUpdate;
            this.UserId = referral.UserId;
            this.OrganizationId = referral.GetInitialOrganizationId();
            this.ReferrableFields = new List<KeyValue>();
        }
    }
}
