using AutoMapper;
using sReportsV2.Common.Entities;
using sReportsV2.Domain.Sql.Entities.Base;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Domain.Sql.Entities.PatientList;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.CTCAE.DataIn;
using sReportsV2.DTOs.DTOs.PatientList;
using sReportsV2.DTOs.DTOs.Patient.DataIn;
using sReportsV2.DTOs.DTOs.PatientList.DataIn;
using sReportsV2.DTOs.Patient;
using sReportsV2.DTOs.Patient.DataIn;
using sReportsV2.DTOs.Patient.DataOut;
using System.Linq;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.DTOs.Patient.DataOut;
using sReportsV2.Cache.Singleton;
using sReportsV2.DTOs.Common.DataOut;
using System;

namespace sReportsV2.MapperProfiles
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, PatientTableDataOut>()
             .ForMember(o => o.Id, opt => opt.MapFrom(src => src.PatientId))
             .ForMember(o => o.GenderId, opt => opt.MapFrom(src => src.GenderCD))
             .ForMember(o => o.FirstName, opt => opt.MapFrom(src => src.NameGiven))
             .ForMember(o => o.LastName, opt => opt.MapFrom(src => src.NameFamily))
             .ForMember(o => o.BirthDate, opt => opt.MapFrom(src => src.BirthDate));

            CreateMap<PatientContact, ContactDTO>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.PatientContactId))
                .ForMember(o => o.ContactRoleId, opt => opt.MapFrom(src => src.ContactRoleCD))
                .ForMember(o => o.ContactRelationshipId, opt => opt.MapFrom(src => src.ContactRelationshipCD))
                .ForMember(o => o.GenderId, opt => opt.MapFrom(src => src.GenderCD))
                .ForMember(o => o.Telecoms, opt => opt.MapFrom(src => src.PatientContactTelecoms.Where(ct => !ct.IsDeleted())))
                .ForMember(o => o.Addresses, opt => opt.MapFrom(src => src.PatientContactAddresses.Where(cA => !cA.IsDeleted())))
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.ToBase64String(src.RowVersion)))
                ;

            CreateMap<PatientContactDataIn, PatientContact>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.PatientContactId, opt => opt.MapFrom(src => src.Id))
                .ForMember(o => o.ContactRoleCD, opt => opt.MapFrom(src => src.ContactRoleId))
                .ForMember(o => o.ContactRelationshipCD, opt => opt.MapFrom(src => src.ContactRelationshipId))
                .ForMember(o => o.GenderCD, opt => opt.MapFrom(src => src.GenderId))
                .ForMember(o => o.Gender, opt => opt.Ignore())
                .ForMember(o => o.PatientContactAddresses, opt => opt.MapFrom(src => src.Addresses))
                .ForMember(o => o.PatientContactTelecoms, opt => opt.MapFrom(src => src.Telecoms))
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.FromBase64String(src.RowVersion)))
                .AfterMap<CommonGlobalAfterMapping<PatientContact>>();

            CreateMap<PatientDataIn, Patient>()
                .ForMember(dest => dest.Gender, opt => opt.Ignore())
                  .IgnoreAllNonExisting()
                  .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.Id))
                  .ForMember(o => o.NameGiven, opt => opt.MapFrom(src => src.Name))
                  .ForMember(o => o.NameFamily, opt => opt.MapFrom(src => src.FamilyName))
                  .ForMember(o => o.PatientContacts, opt => opt.MapFrom(src => src.Contacts))
                  .ForMember(o => o.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                  .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                  .ForMember(o => o.MultipleBirth, opt => opt.MapFrom(src => new MultipleBirth()
                  {
                      isMultipleBorn = src.MultipleBirth,
                      Number = src.MultipleBirthNumber
                  }))
                  .ForMember(o => o.GenderCD, opt => opt.MapFrom(src => src.GenderCD))
                  .ForMember(o => o.PatientTelecoms, opt => opt.MapFrom(src => src.Telecoms))
                  .ForMember(o => o.PatientIdentifiers, opt => opt.MapFrom(src => src.Identifiers))
                  .ForMember(o => o.Communications, opt => opt.MapFrom(src => src.Communications))
                  .ForMember(o => o.PatientAddresses, opt => opt.MapFrom(src => src.Addresses))
                  .ForMember(o => o.ReligionCD, opt => opt.MapFrom(src => src.ReligionCD))
                  .ForMember(o => o.CitizenshipCD, opt => opt.MapFrom(src => src.CitizenshipCD))
                  .ForMember(o => o.MaritalStatusCD, opt => opt.MapFrom(src => src.MaritalStatusCD))
                  .ForMember(o => o.Deceased, opt => opt.MapFrom(src => src.Deceased))
                  .ForMember(o => o.DeceasedDateTime, opt => opt.MapFrom(src => src.DeceasedDateTime))
                  .ForMember(o => o.PatientChemotherapyData, opt => opt.MapFrom(src => src.PatientChemotherapyData))
                  .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.FromBase64String(src.RowVersion)))
                  .AfterMap<CommonGlobalAfterMapping<Patient>>();

            CreateMap<Patient, CTCAEPatient>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.PatientId));

            CreateMap<CTCAEPatient, Patient>()
               .IgnoreAllNonExisting()
               .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.PatientId));

            CreateMap<Patient, PatientDataOut>()
             .IgnoreAllNonExisting()
             .ForMember(o => o.Id, opt => opt.MapFrom(src => src.PatientId.ToString()))
             .ForMember(o => o.Name, opt => opt.MapFrom(src => src.NameGiven))
             .ForMember(o => o.FamilyName, opt => opt.MapFrom(src => src.NameFamily))
             .ForMember(o => o.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
             .ForMember(o => o.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
             .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
             .ForMember(o => o.MultipleBirth, opt => opt.MapFrom(src => src.MultipleBirth.isMultipleBorn))
             .ForMember(o => o.MultipleBirthNumber, opt => opt.MapFrom(src => src.MultipleBirth.Number))
             .ForMember(o => o.GenderId, opt => opt.MapFrom(src => src.GenderCD))
             .ForMember(o => o.ReligionId, opt => opt.MapFrom(src => src.ReligionCD))
             .ForMember(o => o.CitizenshipId, opt => opt.MapFrom(src => src.CitizenshipCD))
             .ForMember(o => o.MaritalStatusId, opt => opt.MapFrom(src => src.MaritalStatusCD))
             .ForMember(o => o.Addresses, opt => opt.MapFrom(src => src.PatientAddresses.Where(a => !a.IsDeleted())))
             .ForMember(o => o.Telecoms, opt => opt.MapFrom(src => src.PatientTelecoms.Where(t => !t.IsDeleted())))
             .ForMember(o => o.Contacts, opt => opt.MapFrom(src => src.PatientContacts.Where(c => !c.IsDeleted())))
             .ForMember(o => o.Communications, opt => opt.MapFrom(src => src.Communications))
             .ForMember(o => o.Identifiers, opt => opt.MapFrom(src => src.PatientIdentifiers.Where(i => !i.IsDeleted())))
             .ForMember(o => o.EpisodeOfCares, opt => opt.MapFrom(src => src.EpisodeOfCares.Where(eoc => !eoc.IsDeleted())))
            .ForMember(dest => dest.Projects, opt => opt.MapFrom(
                    src => src.ProjectPatientRelations
                        .Where(r => !r.IsDeleted())
                        .Select(x => x.Project)
                        .Where(c => c != null && !c.IsDeleted())
                        .ToList()
                )
             )
            .ForMember(o => o.PatientChemotherapyData, opt => opt.MapFrom(src => src.PatientChemotherapyData))
            .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.ToBase64String(src.RowVersion)))
            .AfterMap((entity, dto) => {
                dto.GenderName = SingletonDataContainer.Instance.GetCodePreferredTerm(entity.GenderCD.GetValueOrDefault());
            })
            ;

                CreateMap<PatientAddress, AddressDTO>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.PatientAddressId))
                .IncludeBase<AddressBase, AddressDTO>();

            CreateMap<PatientAddressDataIn, PatientAddress>()
                .IgnoreAllNonExisting()
                .IncludeBase<AddressDTO, AddressBase>()
                .ForMember(o => o.PatientAddressId, opt => opt.MapFrom(src => src.Id))
                .AfterMap<CommonGlobalAfterMapping<PatientAddress>>();

            CreateMap<PatientContactAddress, AddressDTO>()
               .ForMember(o => o.Id, opt => opt.MapFrom(src => src.PatientContactAddressId))
               .IncludeBase<AddressBase, AddressDTO>();

            CreateMap<PatientContactAddressDataIn, PatientContactAddress>()
                .IgnoreAllNonExisting()
                .IncludeBase<AddressDTO, AddressBase>()
                .ForMember(o => o.PatientContactAddressId, opt => opt.MapFrom(src => src.Id))
                .AfterMap<CommonGlobalAfterMapping<PatientContactAddress>>();

            CreateMap<PatientTelecomDataIn, PatientTelecom>()
                .IgnoreAllNonExisting()
                .IncludeBase<TelecomDTO, TelecomBase>()
                .ForMember(o => o.PatientTelecomId, opt => opt.MapFrom(src => src.Id))
                .AfterMap<CommonGlobalAfterMapping<PatientTelecom>>();

            CreateMap<PatientTelecom, TelecomDTO>()
                .IncludeBase<TelecomBase, TelecomDTO>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.PatientTelecomId));

            CreateMap<PatientContactTelecomDataIn, PatientContactTelecom>()
                .IgnoreAllNonExisting()
                .IncludeBase<TelecomDTO, TelecomBase>()
                .ForMember(o => o.PatientContactTelecomId, opt => opt.MapFrom(src => src.Id))
                .AfterMap<CommonGlobalAfterMapping<PatientContactTelecom>>();

            CreateMap<PatientContactTelecom, TelecomDTO>()
                .IncludeBase<TelecomBase, TelecomDTO>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.PatientContactTelecomId));

            CreateMap<PatientFilterDataIn, PatientFilter>()
                .IgnoreAllNonExisting();

            CreateMap<PatientByNameFilterDataIn, PatientByNameSearchFilter>();

            CreateMap<Communication, CommunicationDTO>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.CommunicationId))
                .ReverseMap();

            CreateMap<PatientIdentifier, IdentifierDataOut>()
               .ForMember(o => o.Id, opt => opt.MapFrom(src => src.PatientIdentifierId))
               .IncludeBase<IdentifierBase, IdentifierDataOut>();

            CreateMap<PatientIdentifierDataIn, PatientIdentifier>()
                .IgnoreAllNonExisting()
               .IncludeBase<IdentifierDataIn, IdentifierBase>()
               .ForMember(o => o.PatientIdentifierId, opt => opt.MapFrom(src => src.Id))
               .AfterMap<CommonGlobalAfterMapping<PatientIdentifier>>();

            CreateMap<PatientListDTO, PatientList>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.AdmissionDate, opt => opt.MapFrom(src => src.AdmissionDateFrom))
                .ForMember(o => o.DischargeDate, opt => opt.MapFrom(src => src.DischargeDateTo))
                .AfterMap<CommonGlobalAfterMapping<PatientList>>();

            CreateMap<PatientList, PatientListDTO>()
                .ForMember(o => o.AttendingDoctorName, opt => opt.MapFrom(src => src.AttendingDoctor.GetFirstAndLastName()))
                .ForMember(o => o.PersonnelTeamName, opt => opt.MapFrom(src => src.PersonnelTeam.Name))
                .ForMember(o => o.PatientListPersonnelRelations, opt => opt.MapFrom(src => src.PatientListPersonnelRelations.Where(r => !r.IsDeleted())))
                .ForMember(o => o.AdmissionDateFrom, opt => opt.MapFrom(src => src.AdmissionDate))
                .ForMember(o => o.DischargeDateTo, opt => opt.MapFrom(src => src.DischargeDate));

            CreateMap<PatientListPersonnelRelationDTO, PatientListPersonnelRelation>()
                .IgnoreAllNonExisting()
                .AfterMap<CommonGlobalAfterMapping<PatientListPersonnelRelation>>()
                .ReverseMap();

            CreateMap<PatientListPatientRelationDTO, PatientListPatientRelation>()
                .IgnoreAllNonExisting()
                .AfterMap<CommonGlobalAfterMapping<PatientListPatientRelation>>()
                .ReverseMap();

            CreateMap<EntityFilter, PatientListFilter>()
                .IgnoreAllNonExisting();

            CreateMap<PatientListFilterDataIn, PatientListFilter>()
                .IncludeBase<DataIn, EntityFilter>();

            CreateMap<PatientChemotherapyData, PatientChemotherapyDataDataOut>()
             .IgnoreAllNonExisting()
             .ForMember(dest => dest.ClinicalTrials, opt => opt.Ignore())
             .ForMember(o => o.Id, opt => opt.MapFrom(src => src.PatientChemotherapyDataId))
             .ForMember(o => o.IdentificationNumber, opt => opt.MapFrom(src => src.IdentificationNumber))
             .ForMember(o => o.Allergies, opt => opt.MapFrom(src => src.Allergies))
             .ForMember(o => o.PatientInformedFor, opt => opt.MapFrom(src => src.PatientInformedFor))
             .ForMember(o => o.PatientInformedBy, opt => opt.MapFrom(src => src.PatientInformedBy))
             .ForMember(o => o.PatientInfoSignedOn, opt => opt.MapFrom(src => src.PatientInfoSignedOn))
             .ForMember(o => o.CopyDeliveredOn, opt => opt.MapFrom(src => src.CopyDeliveredOn))
             .ForMember(o => o.CapabilityToWork, opt => opt.MapFrom(src => src.CapabilityToWork))
             .ForMember(o => o.DesireToHaveChildren, opt => opt.MapFrom(src => src.DesireToHaveChildren))
             .ForMember(o => o.FertilityConservation, opt => opt.MapFrom(src => src.FertilityConservation))
             .ForMember(o => o.SemenCryopreservation, opt => opt.MapFrom(src => src.SemenCryopreservation))
             .ForMember(o => o.EggCellCryopreservation, opt => opt.MapFrom(src => src.EggCellCryopreservation))
             .ForMember(o => o.SexualHealthAddressed, opt => opt.MapFrom(src => src.SexualHealthAddressed))
             .ForMember(o => o.ContraceptionCD, opt => opt.MapFrom(src => src.ContraceptionCD))
             .ForMember(o => o.PreviousTreatment, opt => opt.MapFrom(src => src.PreviousTreatment))
             .ForMember(o => o.TreatmentInCantonalHospitalGraubunden, opt => opt.MapFrom(src => src.TreatmentInCantonalHospitalGraubunden))
             .ForMember(o => o.HistoryOfOncologicalDisease, opt => opt.MapFrom(src => src.HistoryOfOncologicalDisease))
             .ForMember(o => o.HospitalOrPraxisOfPreviousTreatments, opt => opt.MapFrom(src => src.HospitalOrPraxisOfPreviousTreatments))
             .ForMember(o => o.DiseaseContextAtInitialPresentationCD, opt => opt.MapFrom(src => src.DiseaseContextAtInitialPresentationCD))
             .ForMember(o => o.StageAtInitialPresentation, opt => opt.MapFrom(src => src.StageAtInitialPresentation))
             .ForMember(o => o.DiseaseContextAtCurrentPresentationCD, opt => opt.MapFrom(src => src.DiseaseContextAtCurrentPresentationCD))
             .ForMember(o => o.StageAtCurrentPresentation, opt => opt.MapFrom(src => src.StageAtCurrentPresentation))
             .ForMember(o => o.Anatomy, opt => opt.MapFrom(src => src.Anatomy))
             .ForMember(o => o.Morphology, opt => opt.MapFrom(src => src.Morphology))
             .ForMember(o => o.TherapeuticContext, opt => opt.MapFrom(src => src.TherapeuticContext))
             .ForMember(o => o.ChemotherapyType, opt => opt.MapFrom(src => src.ChemotherapyType))
             .ForMember(o => o.ChemotherapyCourse, opt => opt.MapFrom(src => src.ChemotherapyCourse))
             .ForMember(o => o.ChemotherapyCycle, opt => opt.MapFrom(src => src.ChemotherapyCycle))
             .ForMember(o => o.FirstDayOfChemotherapy, opt => opt.MapFrom(src => src.FirstDayOfChemotherapy))
             .ForMember(o => o.ConsecutiveChemotherapyDays, opt => opt.MapFrom(src => src.ConsecutiveChemotherapyDays))
             ;

            CreateMap<PatientChemotherapyDataDataIn, PatientChemotherapyData>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.IdentificationNumber, opt => opt.MapFrom(src => src.IdentificationNumber))
                .ForMember(o => o.Allergies, opt => opt.MapFrom(src => src.Allergies))
                .ForMember(o => o.PatientInformedFor, opt => opt.MapFrom(src => src.PatientInformedFor))
                .ForMember(o => o.PatientInformedBy, opt => opt.MapFrom(src => src.PatientInformedBy))
                .ForMember(o => o.PatientInfoSignedOn, opt => opt.MapFrom(src => src.PatientInfoSignedOn))
                .ForMember(o => o.CopyDeliveredOn, opt => opt.MapFrom(src => src.CopyDeliveredOn))
                .ForMember(o => o.CapabilityToWork, opt => opt.MapFrom(src => src.CapabilityToWork))
                .ForMember(o => o.DesireToHaveChildren, opt => opt.MapFrom(src => src.DesireToHaveChildren))
                .ForMember(o => o.FertilityConservation, opt => opt.MapFrom(src => src.FertilityConservation))
                .ForMember(o => o.SemenCryopreservation, opt => opt.MapFrom(src => src.SemenCryopreservation))
                .ForMember(o => o.EggCellCryopreservation, opt => opt.MapFrom(src => src.EggCellCryopreservation))
                .ForMember(o => o.SexualHealthAddressed, opt => opt.MapFrom(src => src.SexualHealthAddressed))
                .ForMember(o => o.ContraceptionCD, opt => opt.MapFrom(src => src.ContraceptionCD))
                .ForMember(o => o.ClinicalTrials, opt => opt.MapFrom(src => src.ClinicalTrials))
                .ForMember(o => o.PreviousTreatment, opt => opt.MapFrom(src => src.PreviousTreatment))
                .ForMember(o => o.TreatmentInCantonalHospitalGraubunden, opt => opt.MapFrom(src => src.TreatmentInCantonalHospitalGraubunden))
                .ForMember(o => o.HistoryOfOncologicalDisease, opt => opt.MapFrom(src => src.HistoryOfOncologicalDisease))
                .ForMember(o => o.HospitalOrPraxisOfPreviousTreatments, opt => opt.MapFrom(src => src.HospitalOrPraxisOfPreviousTreatments))
                .ForMember(o => o.DiseaseContextAtInitialPresentationCD, opt => opt.MapFrom(src => src.DiseaseContextAtInitialPresentationCD))
                .ForMember(o => o.StageAtInitialPresentation, opt => opt.MapFrom(src => src.StageAtInitialPresentation))
                .ForMember(o => o.DiseaseContextAtCurrentPresentationCD, opt => opt.MapFrom(src => src.DiseaseContextAtCurrentPresentationCD))
                .ForMember(o => o.StageAtCurrentPresentation, opt => opt.MapFrom(src => src.StageAtCurrentPresentation))
                .ForMember(o => o.Anatomy, opt => opt.MapFrom(src => src.Anatomy))
                .ForMember(o => o.Morphology, opt => opt.MapFrom(src => src.Morphology))
                .ForMember(o => o.TherapeuticContext, opt => opt.MapFrom(src => src.TherapeuticContext))
                .ForMember(o => o.ChemotherapyType, opt => opt.MapFrom(src => src.ChemotherapyType))
                .ForMember(o => o.ChemotherapyCourse, opt => opt.MapFrom(src => src.ChemotherapyCourse))
                .ForMember(o => o.ChemotherapyCycle, opt => opt.MapFrom(src => src.ChemotherapyCycle))
                .ForMember(o => o.FirstDayOfChemotherapy, opt => opt.MapFrom(src => src.FirstDayOfChemotherapy))
                .ForMember(o => o.ConsecutiveChemotherapyDays, opt => opt.MapFrom(src => src.ConsecutiveChemotherapyDays))
                .AfterMap<CommonGlobalAfterMapping<PatientChemotherapyData>>();

        }
    }
}