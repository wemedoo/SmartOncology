namespace sReportsV2.Domain.Sql.Migrations
{
    using sReportsV2.Common.Constants;
    using sReportsV2.DAL.Sql.Sql;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetTelecomPropertiesFromTempTable : DbMigration
    {
        public override void Up()
        {
            SReportsContext context = new SReportsContext();
            
            string dropTempTable = $@"
                drop table if exists dbo.TelecomPropertiesTempTable; 
            ";

            context.Database.ExecuteSqlCommand(CreateInsertTelecomSystemCommand("1", "Telecoms", "telecomS", "TelecomId"));
            context.Database.ExecuteSqlCommand(CreateInsertTelecomUseCommand("1", "Telecoms", "telecomU", "TelecomId", CodeSetAttributeNames.TelecomUseType));
            context.Database.ExecuteSqlCommand(CreateInsertTelecomSystemCommand("2", "PatientTelecoms", "patientTelecomS", "PatientTelecomId"));
            context.Database.ExecuteSqlCommand(CreateInsertTelecomUseCommand("2", "PatientTelecoms", "patientTelecomU", "PatientTelecomId", CodeSetAttributeNames.TelecommunicationUseType));
            context.Database.ExecuteSqlCommand(CreateInsertTelecomSystemCommand("3", "PatientContactTelecoms", "patientContactTelecomS", "PatientContactTelecomId"));
            context.Database.ExecuteSqlCommand(CreateInsertTelecomUseCommand("3", "PatientContactTelecoms", "patientContactTelecomU", "PatientContactTelecomId", CodeSetAttributeNames.TelecommunicationUseType));
            context.Database.ExecuteSqlCommand(dropTempTable);
        }
        
        public override void Down()
        {
        }

        private string CreateInsertTelecomSystemCommand(string orderNumber, string tableName, string tableAlias, string tableIdName)
        {
            return $@"
                update {tableAlias} set {tableAlias}.SystemCD = codeS{orderNumber}.CodeId
                FROM dbo.Codes codeS{orderNumber}
                inner join dbo.ThesaurusEntryTranslations tranThCodeS{orderNumber} on tranThCodeS{orderNumber}.ThesaurusEntryId = codeS{orderNumber}.ThesaurusEntryId
                inner join dbo.TelecomPropertiesTempTable telecomPropertiesTempS{orderNumber} on telecomPropertiesTempS{orderNumber}.[System] = tranThCodeS{orderNumber}.PreferredTerm
                inner join dbo.{tableName} {tableAlias} on {tableAlias}.{tableIdName} = telecomPropertiesTempS{orderNumber}.{tableIdName}
                inner join dbo.codeSets cSS{orderNumber} on codeS{orderNumber}.CodeSetId = cSS{orderNumber}.CodeSetId
                inner join dbo.ThesaurusEntryTranslations tranThCodeSetS{orderNumber} on tranThCodeSetS{orderNumber}.ThesaurusEntryId = cSS{orderNumber}.ThesaurusEntryId
                where tranThCodeSetS{orderNumber}.PreferredTerm = 'Telecom System type'
                ;
            ";
        }

        private string CreateInsertTelecomUseCommand(string orderNumber, string tableName, string tableAlias, string tableIdName, string codeSetName)
        {
            return $@"
                update {tableAlias} set {tableAlias}.UseCD = codeU{orderNumber}.CodeId
                FROM dbo.Codes codeU{orderNumber}
                inner join dbo.ThesaurusEntryTranslations tranThCodeU{orderNumber} on tranThCodeU{orderNumber}.ThesaurusEntryId = codeU{orderNumber}.ThesaurusEntryId
                inner join dbo.TelecomPropertiesTempTable telecomPropertiesTempU{orderNumber} on telecomPropertiesTempU{orderNumber}.[Use] = tranThCodeU{orderNumber}.PreferredTerm
                inner join dbo.{tableName} {tableAlias} on {tableAlias}.{tableIdName} = telecomPropertiesTempU{orderNumber}.{tableIdName}
                inner join dbo.codeSets cSU{orderNumber} on codeU{orderNumber}.CodeSetId = cSU{orderNumber}.CodeSetId
                inner join dbo.ThesaurusEntryTranslations tranThCodeSetU{orderNumber} on tranThCodeSetU{orderNumber}.ThesaurusEntryId = cSU{orderNumber}.ThesaurusEntryId
                where tranThCodeSetU{orderNumber}.PreferredTerm = '{codeSetName}'
                ;
            ";
        }
    }
}
