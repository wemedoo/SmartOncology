namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using sReportsV2.Domain.Sql.Entities.ProjectEntry;
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public partial class MigrateDataToProjectRelationTables : DbMigration
    {
        public override void Up()
        {
            string migrateProjectPatientRelations = @"
                INSERT INTO ProjectPatientRelations (ProjectId, PatientId, EntryDatetime, LastUpdate, CreatedById, ActiveFrom, ActiveTo, EntityStateCD) 
                    SELECT ct.ProjectId, ctr.PatientId, ctr.EntryDatetime, ctr.LastUpdate, ctr.CreatedById, ctr.ActiveFrom, ctr.ActiveTo, ctr.EntityStateCD
                    FROM ClinicalTrialPatientRelations ctr 
                    INNER JOIN ClinicalTrials ct ON ct.ClinicalTrialId = ctr.ClinicalTrialId;
            ";

            Sql(migrateProjectPatientRelations);

            string migrateProjectPersonnelRelations = @"
                INSERT INTO ProjectPersonnelRelations (ProjectId, PersonnelId, EntryDatetime, LastUpdate, CreatedById, ActiveFrom, ActiveTo, EntityStateCD) 
                    SELECT ct.ProjectId, ctr.PersonnelId, ctr.EntryDatetime, ctr.LastUpdate, ctr.CreatedById, ctr.ActiveFrom, ctr.ActiveTo, ctr.EntityStateCD
                    FROM ClinicalTrialPersonnelRelations ctr 
                    INNER JOIN ClinicalTrials ct ON ct.ClinicalTrialId = ctr.ClinicalTrialId;
            ";

            Sql(migrateProjectPersonnelRelations);

            string migrateProjectDocumentRelations = @"
                    INSERT INTO ProjectDocumentRelations (ProjectId, FormId, EntryDatetime, LastUpdate, CreatedById, ActiveFrom, ActiveTo, EntityStateCD) 
                        SELECT ct.ProjectId, ctr.FormId, ctr.EntryDatetime, ctr.LastUpdate, ctr.CreatedById, ctr.ActiveFrom, ctr.ActiveTo, ctr.EntityStateCD
                        FROM ClinicalTrialDocumentRelations ctr 
                        INNER JOIN ClinicalTrials ct ON ct.ClinicalTrialId = ctr.ClinicalTrialId;
            ";

            Sql(migrateProjectDocumentRelations);
        }

        public override void Down()
        {
        }
    }
}
