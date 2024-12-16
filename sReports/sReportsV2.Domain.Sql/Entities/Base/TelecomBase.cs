using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    public abstract class TelecomBase : EntitiesBase.Entity
    {
        public int? SystemCD { get; set; }

        [ForeignKey("SystemCD")]
        public Code System { get; set; }
        public int? UseCD { get; set; }

        [ForeignKey("UseCD")]
        public Code Use { get; set; }
        public string Value { get; set; }

        public void Copy(TelecomBase telecom)
        {
            this.Value = telecom.Value;
            this.SystemCD = telecom.SystemCD;
            this.UseCD = telecom.UseCD;
            CopyRowVersion(telecom);
        }
    }
}
