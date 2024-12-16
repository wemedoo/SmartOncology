namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Enums;
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEncounterViewOnlyNonDeletedPatient : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            string updateEncounterViewShowOnlyNonDeletedPatient = $@"
                CREATE or ALTER  VIEW [dbo].[EncounterViews]
                    AS
					select
					encounters.EncounterId
					,patients.NameGiven
					,patients.NameFamily
					,patients.GenderCD
					,patients.BirthDate
					,patients.PatientId
				    ,encounters.AdmitDatetime
				    ,encounters.DischargeDatetime
                    ,encounters.EpisodeOfCareId
                    ,encounters.EntityStateCD
                    ,encounters.[RowVersion]
					,encounters.[EntryDatetime]
					,encounters.[LastUpdate]
					,encounters.[ActiveFrom]
					,encounters.[ActiveTo]
					,encounters.[CreatedById]
					from dbo.Encounters encounters
					left join dbo.Patients patients
					on encounters.PatientId = patients.PatientId
					where patients.EntityStateCD != ${(int)EntityStateCode.Deleted};
            ";

            context.Database.ExecuteSqlCommand(updateEncounterViewShowOnlyNonDeletedPatient);
        }
        
        public override void Down()
        {
        }
    }
}
