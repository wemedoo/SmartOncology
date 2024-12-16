namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPatientGenderTempTable : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string createPatientTempTable = $@"
                create table dbo.PatientGenderTempTable (PatientId int, Gender nvarchar(max));
            ";
            string saveDataInPatientTempTable = $@"
                insert into dbo.PatientGenderTempTable (PatientId, Gender) 
                select p.PatientId pId
                      ,CASE
						WHEN p.GenderCD = 0 THEN 'Male'
						WHEN p.GenderCD = 1 THEN 'Female'
						WHEN p.GenderCD = 2 THEN 'Other'
						ELSE 'Unknown'
					END   
	             from dbo.Patients p;
            ";
            string createSmartOncologyPatientTempTable = $@"
                create table dbo.SmartOncologyPatientGenderTempTable (SmartOncologyPatientId int, Gender nvarchar(max));
            ";
            string saveDataInSmartOncologyPatientTempTable = $@"
                insert into dbo.SmartOncologyPatientGenderTempTable (SmartOncologyPatientId, Gender) 
                select smartOncPatient.SmartOncologyPatientId pId
                      ,CASE
						WHEN smartOncPatient.GenderCD = 0 THEN 'Male'
						WHEN smartOncPatient.GenderCD = 1 THEN 'Female'
						WHEN smartOncPatient.GenderCD = 2 THEN 'Other'
						ELSE 'Unknown'
					END   
	             from dbo.SmartOncologyPatients smartOncPatient
				  ;
            ";

            context.Database.ExecuteSqlCommand(createPatientTempTable);
            context.Database.ExecuteSqlCommand(saveDataInPatientTempTable);
            context.Database.ExecuteSqlCommand(createSmartOncologyPatientTempTable);
            context.Database.ExecuteSqlCommand(saveDataInSmartOncologyPatientTempTable);
        }
        
        public override void Down()
        {
            SReportsContext context = new SReportsContext();
            string dropPatientTempTable = $@"
                drop table if exists dbo.PatientGenderTempTable;
            ";
            string dropSmartOncologyPatientTempTable = $@"
                drop table if exists dbo.SmartOncologyPatientGenderTempTable;
            ";
            context.Database.ExecuteSqlCommand(dropPatientTempTable);
            context.Database.ExecuteSqlCommand(dropSmartOncologyPatientTempTable);
        }
    }
}
