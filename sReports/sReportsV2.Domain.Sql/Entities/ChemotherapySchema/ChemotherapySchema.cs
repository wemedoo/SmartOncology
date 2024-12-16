using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.EntitiesBase;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace sReportsV2.Domain.Sql.Entities.ChemotherapySchema
{
    public class ChemotherapySchema : Entity, IEditChildEntries<Indication>, IEditChildEntries<LiteratureReference>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("ChemotherapySchemaId")]
        public int ChemotherapySchemaId { get; set; }
        public string Name { get; set; }
        public List<Indication> Indications { get; set; } = new List<Indication>();
        public List<LiteratureReference> LiteratureReferences { get; set; } = new List<LiteratureReference>();
        public List<Medication> Medications { get; set; } = new List<Medication>();
        public int CreatorId { get; set; }
        [ForeignKey("CreatorId")]
        public sReportsV2.Domain.Sql.Entities.User.Personnel Creator { get; set; }
        public int LengthOfCycle { get; set; }
        public int NumOfCycles { get; set; }
        public bool AreCoursesLimited { get; set; }
        public int NumOfLimitedCourses { get; set; }

        public ChemotherapySchema()
        {
        }

        public ChemotherapySchema(int? createdById) : base(createdById)
        {
        }

        public void Copy(ChemotherapySchema chemotherapySchema)
        {
            CopyName(chemotherapySchema.Name);
            CopyGeneralProperties(chemotherapySchema);
            CopyRowVersion(chemotherapySchema);
            //CopyIndications(chemotherapySchema.Indications);
            //CopyLiteratureReference(chemotherapySchema.LiteratureReferences);
        }

        public void CopyName(string name)
        {
            this.Name = name;
        }

        public void CopyGeneralProperties(ChemotherapySchema chemotherapySchema)
        {
            this.LengthOfCycle = chemotherapySchema.LengthOfCycle;
            this.NumOfCycles = chemotherapySchema.NumOfCycles;
            this.AreCoursesLimited = chemotherapySchema.AreCoursesLimited;
            this.NumOfLimitedCourses = chemotherapySchema.NumOfLimitedCourses;
        }

        #region Edit Child entries

        public void CopyEntries(List<Indication> upcomingEntries)
        {
            if (upcomingEntries != null)
            {
                DeleteExistingRemovedEntries(upcomingEntries);
                AddNewOrUpdateOldEntries(upcomingEntries);
            }
        }

        public void DeleteExistingRemovedEntries(List<Indication> upcomingEntries)
        {
            foreach (var indication in Indications)
            {
                var remainingIndication = upcomingEntries.Exists(x => x.IndicationId == indication.IndicationId);
                if (!remainingIndication)
                {
                    indication.IsDeleted = true;
                }
            }
        }

        public void AddNewOrUpdateOldEntries(List<Indication> upcomingEntries)
        {
            foreach (var indication in upcomingEntries)
            {
                if (indication.IndicationId == 0)
                {
                    Indications.Add(indication);
                }
                else
                {
                    var dbIndication = Indications.Find(x => x.IndicationId == indication.IndicationId);
                    if (dbIndication != null)
                    {
                        dbIndication.Copy(indication);
                    }
                }
            }
        }

        public void CopyEntries(List<LiteratureReference> upcomingEntries)
        {
            if (upcomingEntries != null)
            {
                DeleteExistingRemovedEntries(upcomingEntries);
                AddNewOrUpdateOldEntries(upcomingEntries);
            }
        }

        public void DeleteExistingRemovedEntries(List<LiteratureReference> upcomingEntries)
        {
            List<LiteratureReference> remainingLiteratureReferences = new List<LiteratureReference>();
            foreach (var literatureReference in LiteratureReferences)
            {
                var remainingIndication = upcomingEntries.Exists(x => x.LiteratureReferenceId == literatureReference.LiteratureReferenceId);
                if (remainingIndication)
                {
                    remainingLiteratureReferences.Add(literatureReference);
                }
            }
            LiteratureReferences = remainingLiteratureReferences;
        }

        public void AddNewOrUpdateOldEntries(List<LiteratureReference> upcomingEntries)
        {
            foreach (var literatureReference in upcomingEntries)
            {
                if (literatureReference.LiteratureReferenceId == 0)
                {
                    LiteratureReferences.Add(literatureReference);
                }
                else
                {
                    var dbLiteratureReference = LiteratureReferences.Find(x => x.LiteratureReferenceId == literatureReference.LiteratureReferenceId);
                    dbLiteratureReference.Copy(literatureReference);
                }
            }
        }

        #endregion Edit Child entries
    }
}
