using NHapi.Model.V231.Message;
using sReportsV2.Common.Exceptions;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.SqlDomain.Implementations;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Linq;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Enums;
using sReportsV2.HL7.DTO;
using sReportsV2.Common.Helpers;
using Microsoft.EntityFrameworkCore;

namespace sReportsV2.HL7.Handlers.IncomingHandlers
{
    public class ADT_A03Handler : HL7IncomingMessageHandler
    {
        public ADT_A03Handler(IncomingMessageMetadataDTO messageMetadata) : base(messageMetadata)
        {
        }

        public override void Process(SReportsContext dbContext)
        {
            Domain.Sql.Entities.Patient.Patient patient = CreatePatientFromMessage();

            IPatientDAL patientDAL = new PatientDAL(dbContext);
            Domain.Sql.Entities.Patient.Patient patientDB =
                GetPatient(patientDAL, patient)
                ?? throw new HL7RejectMessageException("There is no patient with given identifiers");

            EpisodeOfCare episodeOfCareDB = GetEpisodeOfCare(patientDB);
            int episodeOfCareId = episodeOfCareDB != null ? episodeOfCareDB.EpisodeOfCareId : 0;
            Encounter upcomingEncounter = GetEncounter(patient);
            Encounter dbEncounter =
                GetEncounterDB(dbContext, upcomingEncounter?.EncounterIdentifiers.FirstOrDefault(), episodeOfCareId)
                ?? throw new HL7RejectMessageException("There is no encounter with given identifiers");

            if (!dbEncounter.DischargeDate.HasValue)
            {
                dbEncounter.DischargeDate = upcomingEncounter.DischargeDate;
                dbEncounter.OverrideDates(MessageMetadata.TransactionDatetime.Value);
            }
            else
            {
                throw new HL7RejectMessageException("There is already discharde date set");
            }
            CommitTransaction(dbContext, patientDAL, patientDB, procedeedEncounter: dbEncounter, hasToUpdatePatient: false);
        }

        private Domain.Sql.Entities.Patient.Patient CreatePatientFromMessage()
        {
            ADT_A03 msg = MessageMetadata.ParsedMessage as ADT_A03;
            Domain.Sql.Entities.Patient.Patient patient = HL7IncomingHelper.ProcessPID(msg.PID);
            patient.EpisodeOfCares.Add(new Domain.Sql.Entities.EpisodeOfCare.EpisodeOfCare()
            {
                StatusCD = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.EOCStatus, "Finished").GetValueOrDefault(),
                Encounters = new System.Collections.Generic.List<Domain.Sql.Entities.Encounter.Encounter>()
                {
                    HL7IncomingHelper.ProcessPV1(msg.PV1)
                },
                Description = "Episode of care received from external source (HL7)",
                Period = new PeriodDatetime()
                {
                    Start = DateTime.Now
                }
            });

            return patient;
        }

        private Encounter GetEncounterDB(SReportsContext dbContext, EncounterIdentifier upcomingEncounter, int episodeOfCareId)
        {
            return dbContext.Encounters
                .Include(e => e.EncounterIdentifiers)
                .WhereEntriesAreActive()
                .FirstOrDefault(e =>
                    e.EpisodeOfCareId == episodeOfCareId
                    && e.EncounterIdentifiers.Any(eI =>
                        eI.IdentifierTypeCD != null && eI.IdentifierTypeCD == upcomingEncounter.IdentifierTypeCD
                        && eI.IdentifierPoolCD != null && eI.IdentifierPoolCD == upcomingEncounter.IdentifierPoolCD
                        && eI.IdentifierValue == upcomingEncounter.IdentifierValue)
                );
        }
    }
}