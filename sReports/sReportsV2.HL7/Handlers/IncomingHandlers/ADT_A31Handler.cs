using NHapi.Model.V231.Message;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Exceptions;
using sReportsV2.DAL.Sql.Implementations;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.HL7.DTO;
using sReportsV2.SqlDomain.Implementations;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;

namespace sReportsV2.HL7.Handlers.IncomingHandlers
{
    public class ADT_A31Handler : HL7IncomingMessageHandler
    {
        public ADT_A31Handler(IncomingMessageMetadataDTO messageMetadata) : base(messageMetadata)
        {
        }
        public override void Process(SReportsContext dbContext)
        {
            Domain.Sql.Entities.Patient.Patient patient = CreatePatientFromMessage(MessageMetadata.ParsedMessage as ADT_A01);
            patient.EpisodeOfCares = new List<Domain.Sql.Entities.EpisodeOfCare.EpisodeOfCare>();

            IPatientDAL patientDAL = new PatientDAL(dbContext);
            IOrganizationDAL organizationDAL = new OrganizationDAL(dbContext);

            Domain.Sql.Entities.Patient.Patient patientDB = GetPatient(patientDAL, patient);
            if (patientDB != null)
            {
                patientDB.Copy(patient, doHL7CopyContacts: true);
                var defaultOrganization = organizationDAL.GetByName(ResourceTypes.CompanyName);
                SetPatientOrganization(defaultOrganization, patientDB);

                OverrideDates(patientDB, overrideEntryDatetime: true);

                CommitTransaction(dbContext, patientDAL, patientDB, procedeedEncounter: null);
            }
            else
            {
                throw new HL7RejectMessageException("There is no patient with given identifiers");
            }
        }
    }
}