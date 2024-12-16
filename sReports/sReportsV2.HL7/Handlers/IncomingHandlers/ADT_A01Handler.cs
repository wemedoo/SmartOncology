using NHapi.Model.V231.Message;
using sReportsV2.Common.Constants;
using sReportsV2.DAL.Sql.Implementations;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.HL7.DTO;
using sReportsV2.SqlDomain.Implementations;
using sReportsV2.SqlDomain.Interfaces;

namespace sReportsV2.HL7.Handlers.IncomingHandlers
{
    public class ADT_A01Handler : HL7IncomingMessageHandler
    {
        public ADT_A01Handler(IncomingMessageMetadataDTO messageMetadata) : base(messageMetadata)
        {
        }

        public override void Process(SReportsContext dbContext)
        {
            Domain.Sql.Entities.Patient.Patient patient = CreatePatientFromMessage(MessageMetadata.ParsedMessage as ADT_A01);

            IPatientDAL patientDAL = new PatientDAL(dbContext);
            IOrganizationDAL organizationDAL = new OrganizationDAL(dbContext);
            OverrideDoctors(new PersonnelDAL(dbContext), patient);

            Domain.Sql.Entities.Patient.Patient patientDB = GetPatient(patientDAL, patient);
            Encounter procedeedEncounter = null;
            if (patientDB != null)
            {
                patientDB.Copy(patient, doHL7CopyContacts: true);
                procedeedEncounter = patientDB.CopyEocAndEncountersFromHL7(GetEpisodeOfCare(patient));
            }
            else
            {
                patientDB = patient;
                procedeedEncounter = GetEncounter(patientDB);
            }
            var defaultOrganization = organizationDAL.GetByName(ResourceTypes.CompanyName);
            SetPatientOrganization(defaultOrganization, patientDB);

            OverrideDates(patientDB, overrideEntryDatetime: true);

            CommitTransaction(dbContext, patientDAL, patientDB, procedeedEncounter);
        }
    }
}