namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Enums;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEOCTypeToEncounterView : DbMigration
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
                    ,encounters.TypeCD
                    ,encounters.StatusCD
                    ,encounters.AdmissionDate
                    ,encounters.DischargeDate
                    ,encounters.EpisodeOfCareId
                    ,episodeOfCare.TypeCD as EpisodeOfCareTypeCD
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
                    left join dbo.EpisodeOfCares episodeOfCare
                    on encounters.EpisodeOfCareId = episodeOfCare.EpisodeOfCareId
                    where GETDATE() BETWEEN encounters.[ActiveFrom] AND encounters.[ActiveTo] AND (encounters.EntityStateCD != {(int)EntityStateCode.Deleted} OR encounters.EntityStateCD IS NULL)
                        AND GETDATE() BETWEEN patients.[ActiveFrom] AND patients.[ActiveTo] AND (patients.EntityStateCD != {(int)EntityStateCode.Deleted} OR patients.EntityStateCD IS NULL)
                        AND GETDATE() BETWEEN episodeOfCare.[ActiveFrom] AND episodeOfCare.[ActiveTo] AND (episodeOfCare.EntityStateCD != {(int)EntityStateCode.Deleted} OR episodeOfCare.EntityStateCD IS NULL);
            ";
            Sql(updateEncounterView);
        }
        
        public override void Down()
        {
        }
    }
}
