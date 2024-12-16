using NHapi.Model.V231.Message;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Exceptions;
using sReportsV2.DAL.Sql.Implementations;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.HL7.DTO;
using sReportsV2.SqlDomain.Implementations;
using sReportsV2.SqlDomain.Interfaces;
using System;

namespace sReportsV2.HL7.Handlers.IncomingHandlers
{
    public class ADT_A08Handler : HL7IncomingMessageHandler
    {
        public ADT_A08Handler(IncomingMessageMetadataDTO messageMetadata) : base(messageMetadata)
        {
        }

        public override void Process(SReportsContext dbContext)
        {
            Domain.Sql.Entities.Patient.Patient patient = CreatePatientFromMessage(MessageMetadata.ParsedMessage as ADT_A01);

            IPatientDAL patientDAL = new PatientDAL(dbContext);
            IOrganizationDAL organizationDAL = new OrganizationDAL(dbContext);

            Domain.Sql.Entities.Patient.Patient patientDB = GetPatient(patientDAL, patient);
            if (patientDB != null)
            {
                OverrideDoctors(new PersonnelDAL(dbContext), patient);

                patientDB.Copy(patient, doHL7CopyContacts: true);
                Encounter procedeedEncounter = patientDB.CopyEocAndEncountersFromHL7(GetEpisodeOfCare(patient));

                var defaultOrganization = organizationDAL.GetByName(ResourceTypes.CompanyName);
                SetPatientOrganization(defaultOrganization, patientDB);

                OverrideDates(patientDB);

                CommitTransaction(dbContext, patientDAL, patientDB, procedeedEncounter);
            }
            else
            {
                throw new HL7RejectMessageException("There is no patient with given identifiers");
            }
        }
    }
}