using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Patient.DataOut;
using System;
using System.Collections.Generic;
using sReportsV2.DTOs.Patient;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.DTOs.TaskEntry.DataOut;
using sReportsV2.DTOs.DTOs.Encounter.DataOut;

namespace sReportsV2.DTOs.Encounter
{
    public class EncounterDataOut
    {
        public int Id { get; set; }
        public int EpisodeOfCareId { get; set; }
        public int PatientId { get; set; }
        public int? StatusId { get; set; }
        public int? ClassId { get; set; }
        public int? TypeId { get; set; }
        public int? ServiceTypeId { get; set; }
        public DateTimeOffset? EntryDatetime { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public List<PatientFormInstanceDataOut> FormInstances { get; set; }
        public PeriodOffsetDTO Period { get; set; }
        public PatientDataOut Patient { get; set; }
        public DateTimeOffset? AdmitDatetime { get; set; }
        public DateTimeOffset? DischargeDatetime { get; set; }
        public UserDataOut AttendingDoctor { get; set; }
        public List<TaskDataOut> Tasks { get; set; }
        public List<EncounterPersonnelRelationDataOut> Doctors { get; set; }
    }
}