namespace sReportsV2.Domain.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameRemainingTablesAndIndexes : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.O4CodeableConcept", newName: "O4CodeableConcepts");
            RenameIndex(table: "dbo.ThesaurusEntries", name: "IX_AdministrativeData_Id", newName: "IX_AdministrativeDataId");
            RenameIndex(table: "dbo.PatientContactAddresses", name: "IX_CountryId", newName: "IX_CountryCD");
            RenameIndex(table: "dbo.SmartOncologyPatients", name: "IX_MultipleB_Id", newName: "IX_MultipleBirthId");
            RenameIndex(table: "dbo.Patients", name: "IX_MultipleB_Id", newName: "IX_MultipleBirthId");
            RenameIndex(table: "dbo.OrganizationTelecoms", name: "IX_Organization_Id", newName: "IX_OrganizationId");
            RenameIndex(table: "dbo.Communications", name: "IX_Patient_Id", newName: "IX_PatientId");
            RenameIndex(table: "dbo.EpisodeOfCares", name: "IX_SmartOncologyPatient_Id", newName: "IX_SmartOncologyPatientId");
            RenameIndex(table: "dbo.Communications", name: "IX_SmartOncologyPatient_Id", newName: "IX_SmartOncologyPatientId");
            RenameIndex(table: "dbo.O4CodeableConcepts", name: "IX_ThesaurusEntry_Id", newName: "IX_ThesaurusEntryId");

        }

        public override void Down()
        {
            RenameIndex(table: "dbo.O4CodeableConcepts", name: "IX_ThesaurusEntryId", newName: "IX_ThesaurusEntry_Id");
            RenameIndex(table: "dbo.Communications", name: "IX_SmartOncologyPatientId", newName: "IX_SmartOncologyPatient_Id");
            RenameIndex(table: "dbo.EpisodeOfCares", name: "IX_SmartOncologyPatientId", newName: "IX_SmartOncologyPatient_Id");
            RenameIndex(table: "dbo.Communications", name: "IX_PatientId", newName: "IX_Patient_Id");
            RenameIndex(table: "dbo.OrganizationTelecoms", name: "IX_OrganizationId", newName: "IX_Organization_Id");
            RenameIndex(table: "dbo.Patients", name: "IX_MultipleBirthId", newName: "IX_MultipleB_Id");
            RenameIndex(table: "dbo.SmartOncologyPatients", name: "IX_MultipleBirthId", newName: "IX_MultipleB_Id");
            RenameIndex(table: "dbo.PatientContactAddresses", name: "IX_CountryCD", newName: "IX_CountryId");
            RenameIndex(table: "dbo.ThesaurusEntries", name: "IX_AdministrativeDataId", newName: "IX_AdministrativeData_Id");
            RenameTable(name: "dbo.O4CodeableConcepts", newName: "O4CodeableConcept");
        }
    }
}
