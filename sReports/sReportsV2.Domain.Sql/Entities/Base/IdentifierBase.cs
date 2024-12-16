using sReportsV2.Domain.Sql.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.Base
{
    public abstract class IdentifierBase : EntitiesBase.Entity
    {
        [StringLength(128)]
        public string IdentifierValue { get; set; }

        public int? IdentifierTypeCD { get; set; }
        [ForeignKey("IdentifierTypeCD")]
        public Code IdentifierType { get; set; }

        public int? IdentifierPoolCD { get; set; }
        [ForeignKey("IdentifierPoolCD")]
        public Code IdentifierPool { get; set; }

        public int? IdentifierUseCD { get; set; }
        [ForeignKey("IdentifierUseCD")]
        public Code IdentifierUse { get; set; }

        protected IdentifierBase()
        {
        }

        protected IdentifierBase(int? createdById, string organizationTimeZone = null) : base(createdById, organizationTimeZone)
        {

        }

        protected IdentifierBase(int? identifierTypeCD, string value, int? identifierUseCD = null)
        {
            this.IdentifierTypeCD = identifierTypeCD;
            this.IdentifierValue = value;
            this.IdentifierUseCD = identifierUseCD;
        }

        public void Copy(IdentifierBase identifierBase)
        {
            this.IdentifierTypeCD = identifierBase.IdentifierTypeCD;
            this.IdentifierValue = identifierBase.IdentifierValue;
            this.IdentifierUseCD = identifierBase.IdentifierUseCD;
            CopyRowVersion(identifierBase);
        }
    }
}
