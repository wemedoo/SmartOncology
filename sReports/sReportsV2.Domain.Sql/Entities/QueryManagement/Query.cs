using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.User;
using System.Collections.Generic;
using System;
using sReportsV2.Common.Extensions;

namespace sReportsV2.Domain.Sql.Entities.QueryManagement
{
    public class Query : EntitiesBase.Entity
    {
        public Query()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int QueryId { get; set; }
        public string FieldId { get; set; }
        public string FormInstanceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }

        public int? LastUpdateById { get; set; }
        [ForeignKey("LastUpdateById")]
        public Personnel LastUpdateBy { get; set; }

        public int ReasonCD { get; set; }
        [ForeignKey("ReasonCD")]
        public Code Reason { get; set; }

        public int StatusCD { get; set; }
        [ForeignKey("StatusCD")]
        public Code Status { get; set; }

        public virtual List<QueryHistory> History { get; set; } = new List<QueryHistory>();

        public void Copy(Query query)
        {
            this.QueryId = query.QueryId;
            this.FieldId = query.FieldId;
            this.FormInstanceId = query.FormInstanceId;
            this.Title = query.Title;
            this.Description = query.Description;
            this.Comment = query.Comment;
            this.ReasonCD = query.ReasonCD;
            this.StatusCD = query.StatusCD;
        }

        public override void Delete(DateTimeOffset? activeTo = null, bool setLastUpdateProperty = true, string organizationTimeZone = null)
        {
            var activeToDate = activeTo ?? DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone(organizationTimeZone);

            foreach (var history in History)
                history.Delete(activeToDate);

            this.ActiveTo = activeTo ?? DateTimeOffset.UtcNow.ConvertToOrganizationTimeZone(organizationTimeZone);
        }
    }
}
