namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Enums;
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEncounterViewDates : DbMigration
    {
        public override void Up()
        {
            string updateEncounterView = $@"
                CREATE or ALTER  VIEW [dbo].[EncounterViews]
                    AS
					select
					encounters.EncounterId
					,patients.NameGiven
					,patients.NameFamily
					,patients.GenderCD
					,patients.BirthDate
					,patients.PatientId
                    ,encounters.StatusCD
				    ,encounters.AdmissionDate
				    ,encounters.DischargeDate
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
            Sql(updateEncounterView);
        }
        
        public override void Down()
        {
        }
    }
}
