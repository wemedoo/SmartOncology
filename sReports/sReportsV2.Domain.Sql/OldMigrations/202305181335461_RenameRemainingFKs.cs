namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    
    public partial class RenameRemainingFKs : DbMigration
    {
        public override void Up()
        {
            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.ExecuteRenameFKs(GetForeignKeyToBeRenamed(), isUpMigration: true);
        }

        public override void Down()
        {
            SReportsContext sReportsContext = new SReportsContext();
            sReportsContext.ExecuteRenameFKs(GetForeignKeyToBeRenamed(), isUpMigration: false);
        }

        private List<Tuple<string, string>> GetForeignKeyToBeRenamed()
        {
            return new List<Tuple<string, string>>()
            {
                new Tuple<string, string>       ("FK_dbo.ChemotherapySchemaInstances_dbo.SmartOncologyPatients_PatientId","FK_dbo.ChemotherapySchemaInstances_dbo.SmartOncologyPatients_SmartOncologyPatientId"),
                new Tuple<string, string>("FK_dbo.ChemotherapySchemaInstances_dbo.Personnel_Creator_Id", "FK_dbo.ChemotherapySchemaInstances_dbo.Personnel_CreatorId"),
                new Tuple<string, string>("FK_dbo.Communications_dbo.SmartOncologyPatients_SmartOncologyPatient_Id",        "FK_dbo.Communications_dbo.SmartOncologyPatients_SmartOncologyPatientId"),
                new Tuple<string, string>("FK_dbo.Communications_dbo.Patients_Patient_Id", "FK_dbo.Communications_dbo.Patients_PatientId"),
                new Tuple<string, string>("FK_dbo.Transactions_dbo.HL7MessageLog_HL7MessageLogId",        "FK_dbo.Transactions_dbo.HL7MessageLogs_HL7MessageLogId"),
                new Tuple<string, string>("FK_dbo.O4CodeableConcept_dbo.CodeSystems_CodeSystemId",        "FK_dbo.O4CodeableConcepts_dbo.CodeSystems_CodeSystemId"),
                new Tuple<string, string>("FK_dbo.O4CodeableConcept_dbo.ThesaurusEntries_ThesaurusEntry_Id",        "FK_dbo.O4CodeableConcepts_dbo.ThesaurusEntries_ThesaurusEntryId"),
                new Tuple<string, string>("FK_dbo.Patients_dbo.MultipleBirths_MultipleB_Id",        "FK_dbo.Patients_dbo.MultipleBirths_MultipleBirthId"),
                new Tuple<string, string>("FK_dbo.SmartOncologyPatients_dbo.MultipleBirths_MultipleB_Id",        "FK_dbo.SmartOncologyPatients_dbo.MultipleBirths_MultipleBirthId"),
                new Tuple<string, string>("FK_dbo.OrganizationTelecoms_dbo.Organizations_Organization_Id",        "FK_dbo.OrganizationTelecoms_dbo.Organizations_OrganizationId"),
                new Tuple<string, string>("FK_dbo.ThesaurusEntries_dbo.AdministrativeDatas_AdministrativeData_Id",        "FK_dbo.ThesaurusEntries_dbo.AdministrativeDatas_AdministrativeDataId"),
                new Tuple<string, string>("FK_dbo.EpisodeOfCares_dbo.SmartOncologyPatients_SmartOncologyPatient_Id", "FK_dbo.EpisodeOfCares_dbo.SmartOncologyPatients_SmartOncologyPatientId")
            };
        }
    }
}
