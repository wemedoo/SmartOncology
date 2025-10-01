using NHapi.Model.V231.Message;
using sReportsV2.Common.Enums;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Encounter;
using sReportsV2.Domain.Sql.Entities.EpisodeOfCare;
using sReportsV2.Domain.Sql.Entities.HL7;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Linq;
using sReportsV2.HL7.DTO;
using sReportsV2.Common.Constants;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Configurations;
using DocumentFormat.OpenXml.InkML;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using sReportsV2.Common.Extensions;

namespace sReportsV2.HL7.Handlers.IncomingHandlers
{
    public abstract class HL7IncomingMessageHandler
    {
        protected IncomingMessageMetadataDTO MessageMetadata { get; set; }
        
        protected HL7IncomingMessageHandler(IncomingMessageMetadataDTO messageMetadata) 
        {
            this.MessageMetadata = messageMetadata;   
        }
        protected void SetPatientOrganization(Domain.Sql.Entities.OrganizationEntities.Organization defaultOrganization, Patient patient)
        {
            if (defaultOrganization != null && patient != null && patient.OrganizationId == 0)
            {
                patient.OrganizationId = defaultOrganization.OrganizationId;
            }
        }
        protected void OverrideDates(Patient patient, bool overrideEntryDatetime = false)
        {
            DateTime dateTimeOfMessage = MessageMetadata.TransactionDatetime.Value;

            patient.OverrideDates(dateTimeOfMessage, overrideEntryDatetime);
            foreach (var patientAddress in patient.PatientAddresses)
            {
                patientAddress.OverrideDates(dateTimeOfMessage, overrideEntryDatetime);
            }
            foreach (var patientTelecom in patient.PatientTelecoms)
            {
                patientTelecom.OverrideDates(dateTimeOfMessage, overrideEntryDatetime);
            }
            foreach (var patientIdentifier in patient.PatientIdentifiers)
            {
                patientIdentifier.OverrideDates(dateTimeOfMessage, overrideEntryDatetime);
            }
            foreach (var patientContact in patient.PatientContacts)
            {
                patientContact.OverrideDates(dateTimeOfMessage, overrideEntryDatetime);
                foreach (var patientContactAddress in patientContact.PatientContactAddresses)
                {
                    patientContactAddress.OverrideDates(dateTimeOfMessage, overrideEntryDatetime);
                }
                foreach (var patientContactTelecom in patientContact.PatientContactTelecoms)
                {
                    patientContactTelecom.OverrideDates(dateTimeOfMessage, overrideEntryDatetime);
                }
            }
            foreach (var episodeOfCare in patient.EpisodeOfCares)
            {
                episodeOfCare.OverrideDates(dateTimeOfMessage, overrideEntryDatetime);
                foreach (var encounter in episodeOfCare.Encounters)
                {
                    encounter.OverrideDates(dateTimeOfMessage, overrideEntryDatetime);
                }
            }
        }
        protected void OverrideDoctors(IPersonnelDAL personnelDAL, Patient patient)
        {
            foreach (var doctorFromMessage in patient.EpisodeOfCares.SelectMany(eoc => eoc.Encounters).SelectMany(enc => enc.PersonnelEncounterRelations))
            {
                int? doctorId = personnelDAL.GetDoctorId(doctorFromMessage.Personnel.PersonnelIdentifiers);
                if (doctorId.HasValue)
                {
                    doctorFromMessage.PersonnelId = doctorId.Value;
                }
            }
        }
        protected Transaction CreateProcedeedMessageLog(int patientId, Encounter procedeedEncounter)
        {
            DateTimeOffset transactionDateTime = new DateTimeOffset(MessageMetadata.TransactionDatetime.Value.ToUniversalTime());

            return new Transaction()
            {
                PatientId = patientId,
                Encounter = procedeedEncounter,
                HL7MessageLogId = MessageMetadata.HL7MessageLogId,
                TransactionType = MessageMetadata.TransactionType,
                HL7EventType = MessageMetadata.HL7EventType,
                SourceSystemCD = MessageMetadata.SourceSystemCD,
                TransactionDirectionCD = MessageMetadata.TransactionDirectionCD,
                TransactionDatetime = transactionDateTime.GetDateTimeByTimeZone(GlobalConfig.GetTimeZoneId())
            };
        }
        protected Patient CreatePatientFromMessage(ADT_A01 msg)
        {
            Patient patient = HL7IncomingHelper.ProcessPID(msg.PID);
            patient.PatientContacts.AddRange(HL7IncomingHelper.ProcessNK1s(msg.NK1s));
            patient.EpisodeOfCares.Add(new EpisodeOfCare()
            {
                StatusCD = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.EOCStatus, "On hold").GetValueOrDefault(),
                TypeCD = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.EpisodeOfCareType, "Home and Community Care").GetValueOrDefault(),
                Encounters = new System.Collections.Generic.List<Encounter>()
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
        protected EpisodeOfCare GetEpisodeOfCare(Patient patient)
        {
            return patient.EpisodeOfCares.FirstOrDefault();
        }

        protected Encounter GetEncounter(Patient patient)
        {
            return patient.EpisodeOfCares?.FirstOrDefault()?.Encounters?.FirstOrDefault();
        }

        protected async Task CommitTransaction(SReportsContext dbContext, IPatientDAL patientDAL, Patient patientDB, Encounter procedeedEncounter, bool hasToUpdatePatient = true)
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await dbContext.Database.BeginTransactionAsync();
                try
                {
                    if (hasToUpdatePatient)
                    {
                        patientDAL.InsertOrUpdate(patientDB, null);
                    }
                    dbContext.Transactions.Add(CreateProcedeedMessageLog(patientDB.PatientId, procedeedEncounter));

                    await dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        protected Patient GetPatient(IPatientDAL patientDAL, Patient incomingPatient)
        {
            int? mrnCodeId = SingletonDataContainer.Instance.GetCodeId((int)CodeSetList.PatientIdentifierType, ResourceTypes.MedicalRecordNumber);
            PatientIdentifier mrnPatientIdentifier = incomingPatient
                .PatientIdentifiers
                .Find(pI => pI.IdentifierTypeCD == mrnCodeId);
            return patientDAL.GetBy(incomingPatient, mrnPatientIdentifier ?? new PatientIdentifier());
        }

        public abstract Task Process(SReportsContext dbContext);
    }
}
