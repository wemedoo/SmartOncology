using sReportsV2.Domain.Sql.Entities.AccessManagment;
using sReportsV2.Domain.Sql.Entities.Aliases;
using sReportsV2.Domain.Sql.Entities.PersonnelTeamEntities;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance;
using sReportsV2.Domain.Sql.Entities.CodeEntry;
using sReportsV2.Domain.Sql.Entities.CodeSystem;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using sReportsV2.Domain.Sql.Entities.FormComment;
using sReportsV2.Domain.Sql.Entities.GlobalThesaurusUser;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.Domain.Sql.Entities.OutsideUser;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.Domain.Sql.Entities.HL7;
using sReportsV2.Domain.Sql.Entities.TaskEntry;
using sReportsV2.Domain.Sql.Entities.ClinicalTrial;
using sReportsV2.Domain.Sql.Entities.ProjectEntry;
using sReportsV2.Domain.Sql.Entities.ApiRequest;
using sReportsV2.Domain.Sql.Entities.PatientList;
using Microsoft.EntityFrameworkCore;

namespace sReportsV2.DAL.Sql.Sql
{
    public class SReportsContext : DbContext
    {
        public SReportsContext(DbContextOptions<SReportsContext> options) : base(options)
        {
            this.Database.SetCommandTimeout(6000);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.LogTo(s => Debug.WriteLine(s));
        //}

        #region Tables
        public DbSet<ThesaurusEntry> Thesauruses { get; set; }
        public DbSet<ThesaurusEntryTranslation> ThesaurusEntryTranslations { get; set; }
        public DbSet<Version> Versions { get; set; }
        public DbSet<AdministrativeData> AdministrativeDatas { get; set; }
        public DbSet<O4CodeableConcept> O4CodeableConcepts { get; set; }
        public DbSet<Personnel> Personnel { get; set; }
        public DbSet<PersonnelAddress> PersonnelAddresses { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationRelation> OrganizationRelations { get; set; }
        public DbSet<OrganizationClinicalDomain> OrganizationClinicalDomains { get; set; }
        public DbSet<ClinicalTrial> ClinicalTrials { get; set; }
        public DbSet<Code> Codes { get; set; }
        public DbSet<OrganizationIdentifier> OrganizationIdentifiers { get; set; }
        public DbSet<GlobalThesaurusUser> GlobalThesaurusUsers { get; set; }
        public DbSet<CodeSystem> CodeSystems { get; set; }

        //This table is deprecated, but we shouldn't delete it until we are sure that we have migrated clinical domain to codes in all instances
        public DbSet<ClinicalDomain> ClinicalDomains { get; set; }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<OutsideUser> OutsideUsers { get; set; }
        public DbSet<Encounter> Encounters { get; set; }
        public DbSet<EpisodeOfCare> EpisodeOfCares { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientIdentifier> PatientIdentifiers { get; set; }
        public DbSet<PatientAddress> PatientAddresses { get; set; }
        public DbSet<PatientChemotherapyData> PatientChemotherapyDatas { get; set; }
        public DbSet<OrganizationTelecom> OrganizationTelecoms { get; set; }
        public DbSet<PatientTelecom> PatientTelecoms { get; set; }
        public DbSet<PatientContactTelecom> PatientContactTelecoms { get; set; }
        public DbSet<PatientContactAddress> PatientContactAddresses { get; set; }
        public DbSet<PatientContact> PatientContacts { get; set; }
        public DbSet<Communication> Communications { get; set; }
        public DbSet<ThesaurusMerge> ThesaurusMerges { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionModule> PermissionModules { get; set; }
        public DbSet<GlobalThesaurusUserRole> GlobalThesaurusUserRoles { get; set; }
        public DbSet<GlobalThesaurusRole> GlobalThesaurusRoles { get; set; }
        public DbSet<ChemotherapySchema> ChemotherapySchemas { get; set; }
        public DbSet<ChemotherapySchemaInstance> ChemotherapySchemaInstances { get; set; }
        public DbSet<ChemotherapySchemaInstanceVersion> ChemotherapySchemaInstanceVersions { get; set; }
        public DbSet<LiteratureReference> LiteratureReferences { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<MedicationDose> MedicationDoses { get; set; }
        public DbSet<MedicationInstance> MedicationInstances { get; set; }
        public DbSet<MedicationDoseInstance> MedicationDoseInstances { get; set; }
        public DbSet<BodySurfaceCalculationFormula> BodySurfaceCalculationFormulas { get; set; }
        public DbSet<RouteOfAdministration> RouteOfAdministrations { get; set; }
        public DbSet<MedicationDoseType> MedicationDoseTypes { get; set; }
        public DbSet<MedicationReplacement> MedicationReplacements { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<CodeSet> CodeSets { get; set; }
        public DbSet<InboundAlias> InboundAliases { get; set; }
        public DbSet<OutboundAlias> OutboundAliases { get; set; }
        public DbSet<CodeAssociation> CodeAssociations { get; set; }
        public DbSet<PersonnelTeam> PersonnelTeams { get; set; }
        public DbSet<PersonnelTeamRelation> PersonnelTeamRelations { get; set; }
        public DbSet<PersonnelTeamOrganizationRelation> PersonnelTeamOrganizationRelations { get; set; }
        public DbSet<PersonnelIdentifier> PersonnelIdentifiers { get; set; }
        public DbSet<PositionPermission> PositionPermissions { get; set; }
        public DbSet<HL7MessageLog> HL7MessageLogs { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ErrorMessageLog> ErrorMessageLogs { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskDocument> TaskDocuments { get; set; }
        public DbSet<OrganizationCommunicationEntity> OrganizationCommunicationEntities { get; set; }
        public DbSet<PersonnelOccupation> PersonnelOccupations { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectPersonnelRelation> ProjectPersonnelRelations { get; set; }
        public DbSet<ProjectDocumentRelation> ProjectDocumentRelations { get; set; }
        public DbSet<ProjectPatientRelation> ProjectPatientRelations { get; set; }
        public DbSet<ApiRequestLog> ApiRequestLogs { get; set; }
        public DbSet<PersonnelEncounterRelation> PersonnelEncounterRelations { get; set; }
        public DbSet<PatientList> PatientLists { get; set; }
        public DbSet<PatientListPersonnelRelation> PatientListPersonnelRelations { get; set; }
        public DbSet<FormCodeRelation> FormCodeRelations { get; set; }
        public DbSet<PatientListPatientRelation> PatientListPatientRelations { get; set; }

        #endregion /Tables

        #region Views
        public DbSet<PersonnelPositionPermissionView> PersonnelPositionPermissionViews { get; set; }
        public DbSet<CodeAliasView> CodeAliasViews { get; set; }
        public DbSet<PersonnelView> PersonnelViews { get; set; }
        public DbSet<EncounterView> EncounterViews { get; set; }
        public DbSet<ThesaurusEntryView> ThesaurusEntryViews { get; set; }

        #endregion /Views

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Requires additional configuration when class references to the same entity many times

            modelBuilder.Entity<OrganizationRelation>()
            .HasOne(or => or.Parent)
            .WithMany()
            .HasForeignKey(or => or.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrganizationRelation>()
            .HasOne(or => or.Child)
            .WithMany()
            .HasForeignKey(or => or.ChildId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChemotherapySchemaInstance>()
            .HasOne(s => s.Creator)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChemotherapySchema>()
            .HasOne(c => c.Creator)
            .WithMany()
            .HasForeignKey(c => c.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonnelIdentifier>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonnelAddress>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonnelAcademicPosition>()
            .HasOne(s => s.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonnelPosition>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonnelTeamRelation>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ThesaurusEntry>()
            .HasOne(c => c.EntityState)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientList>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientList>()
            .HasOne(c => c.EpisodeOfCareType)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PatientListPersonnelRelation>()
            .HasOne(c => c.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Code>()
               .HasOne(c => c.CreatedBy)
               .WithMany()
               .HasForeignKey(c => c.CreatedById);

            modelBuilder.Entity<Code>()
                .HasOne<CodeSet>()
                .WithMany()
                .HasForeignKey(c => c.CodeSetId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CodeSet>()
                .HasOne(cs => cs.ThesaurusEntry)
                .WithMany() 
                .HasForeignKey(cs => cs.ThesaurusEntryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Personnel>()
                .HasOne(p => p.PersonnelOccupation)
                .WithMany()
                .HasForeignKey(p => p.PersonnelOccupationId);

            modelBuilder.Entity<EpisodeOfCare>(entity =>
            {
                entity.OwnsOne(e => e.Period, p =>
                {
                    p.Property(pp => pp.Start).HasColumnName("Period_Start");
                    p.Property(pp => pp.End).HasColumnName("Period_End");
                });
            });

            modelBuilder.Entity<EpisodeOfCare>()
                .HasOne(e => e.Status)
                .WithMany()
                .HasForeignKey(e => e.StatusCD)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EpisodeOfCare>()
                .HasOne(e => e.Type)
                .WithMany()
                .HasForeignKey(e => e.TypeCD)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MedicationReplacement>()
                .HasOne(mr => mr.ReplaceMedication)
                .WithMany()
                .HasForeignKey(mr => mr.ReplaceMedicationId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MedicationReplacement>()
                .HasOne(mr => mr.ReplaceWithMedication)
                .WithMany()
                .HasForeignKey(mr => mr.ReplaceWithMedicationId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MedicationReplacement>()
                .HasOne(mr => mr.Creator)
                .WithMany()
                .HasForeignKey(mr => mr.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonnelOccupation>()
                .HasOne(po => po.Personnel)
                .WithMany()
                .HasForeignKey(po => po.PersonnelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonnelOccupation>()
                .HasOne(po => po.OccupationCategory)
                .WithMany()
                .HasForeignKey(po => po.OccupationCategoryCD)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonnelOccupation>()
                .HasOne(po => po.OccupationSubCategory)
                .WithMany()
                .HasForeignKey(po => po.OccupationSubCategoryCD)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonnelOccupation>()
                .HasOne(po => po.Occupation)
                .WithMany()
                .HasForeignKey(po => po.OccupationCD)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonnelOccupation>()
                .HasOne(po => po.PersonnelSeniority)
                .WithMany()
                .HasForeignKey(po => po.PersonnelSeniorityCD)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Task>()
                .HasOne(t => t.Patient)
                .WithMany()
                .HasForeignKey(t => t.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Task>()
                .HasOne(t => t.TaskType)
                .WithMany()
                .HasForeignKey(t => t.TaskTypeCD)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Task>()
                .HasOne(t => t.TaskStatus)
                .WithMany()
                .HasForeignKey(t => t.TaskStatusCD)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Task>()
                .HasOne(t => t.TaskPriority)
                .WithMany()
                .HasForeignKey(t => t.TaskPriorityCD)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Task>()
                .HasOne(t => t.TaskClass)
                .WithMany()
                .HasForeignKey(t => t.TaskClassCD)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Task>()
                .HasOne(t => t.TaskDocument)
                .WithMany()
                .HasForeignKey(t => t.TaskDocumentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EpisodeOfCare>()
                .HasOne(e => e.Patient)
                .WithMany(p => p.EpisodeOfCares)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ThesaurusEntry>()
               .HasOne(c => c.StateCode)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PersonnelPositionPermissionView>().ToView("PersonnelPositionPermissionViews");
            modelBuilder.Entity<EncounterView>().ToView("EncounterViews");
            modelBuilder.Entity<CodeAliasView>().ToView("CodeAliasViews");
            modelBuilder.Entity<PersonnelView>().ToView("PersonnelViews");

            #endregion
        }
    }
}
