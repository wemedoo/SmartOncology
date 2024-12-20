﻿using AutoMapper;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Consensus;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.OutsideUser;
using sReportsV2.DTOs;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Consensus.DataIn;
using sReportsV2.DTOs.Consensus.DataOut;
using sReportsV2.DTOs.DTOs.Form.DataIn;
using sReportsV2.DTOs.DTOs.Form.DataOut;
using sReportsV2.DTOs.DTOs.FormConsensus.DTO;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Form.DataOut.Tree;
using sReportsV2.DTOs.O4CodeableConcept.DataIn;
using sReportsV2.DTOs.O4CodeableConcept.DataOut;
using System.Linq;

namespace sReportsV2.MapperProfiles
{
    public class FormProfile : Profile
    {
        public FormProfile()
        {
            CreateMap<ConsensusInstance, ConsensusInstanceDataIn>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.UserRef, opt => opt.MapFrom(src => src.UserId))
                .ReverseMap();

            CreateMap<ConsensusInstance, ConsensusInstanceDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(dest => dest.Questions, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ConsensusInstance, ConsensusDataIn>()
                .IgnoreAllNonExisting()
                .ReverseMap();

            CreateMap<ConsensusInstance, ConsensusDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.ConsensusRef))
                .ReverseMap();

            CreateMap<OutsideUserDataIn, Domain.Sql.Entities.OutsideUser.OutsideUser>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.OutsideUserAddress, opt => opt.MapFrom(src => src.Address))
                .ForMember(o => o.OutsideUserId, opt => opt.MapFrom(src => src.Id))
                .AfterMap<CommonGlobalAfterMapping<OutsideUser>>();

            CreateMap<sReportsV2.Domain.Sql.Entities.OutsideUser.OutsideUser, ConsensusUserDataOut>()
                .ForMember(o => o.Id, opt => opt.MapFrom(src => src.OutsideUserId))
                .ForMember(o => o.Address, opt => opt.MapFrom(src => src.OutsideUserAddress));

            CreateMap<OutsideUserAddress, AddressDTO>()
              .ForMember(o => o.Id, opt => opt.MapFrom(src => src.OutsideUserAddressId))
              .IncludeBase<AddressBase, AddressDTO>();

            CreateMap<AddressDTO, OutsideUserAddress>()
                .IncludeBase<AddressDTO, AddressBase>()
                .ForMember(o => o.OutsideUserAddressId, opt => opt.MapFrom(src => src.Id))
                .AfterMap<CommonGlobalAfterMapping<OutsideUserAddress>>();

            CreateMap<Consensus, ConsensusDataIn>()
                .IgnoreAllNonExisting()
                .ReverseMap();
            CreateMap<Consensus, ConsensusDataOut>()
                .IgnoreAllNonExisting()
                .ReverseMap();

            CreateMap<QuestionOccurenceConfigDTO, QuestionOccurenceConfig>().ReverseMap();

            CreateMap<ConsensusQuestion, ConsensusQuestionDataIn>()
                .IgnoreAllNonExisting()
                .ReverseMap();
            CreateMap<ConsensusQuestion, ConsensusQuestionDataOut>().ReverseMap();

            CreateMap<ConsensusIteration, ConsensusIterationDataIn>()
                .IgnoreAllNonExisting()
                .ReverseMap();

            CreateMap<ConsensusIteration, ConsensusIterationDataOut>()
                .IgnoreAllNonExisting()
                .ReverseMap();

            CreateMap<sReportsV2.Domain.Entities.Form.Version, VersionDTO>().ReverseMap();

            CreateMap<EnumData, EnumDTO>().ReverseMap();

            CreateMap<FormFilterDataIn, FormFilterData>()
                .IgnoreAllNonExisting()
                .ReverseMap();

            CreateMap<Form, FormTreeDataOut>()
               .IgnoreAllNonExisting()
               .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(d => d.Chapters, opt => opt.MapFrom(src => src.Chapters))
               .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
               .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
               .ReverseMap();

            CreateMap<FormChapter, FormTreeChapterDataOut>()
               .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(d => d.Pages, opt => opt.MapFrom(src => src.Pages))
               .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
               .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
               .ReverseMap();

            CreateMap<FormPage, FormTreePageDataOut>()
               .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(d => d.FieldSets, opt => opt.MapFrom(src => src.ListOfFieldSets.SelectMany(x => x)))
               .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
               .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
               .ReverseMap();

            CreateMap<FieldSet, FormTreeFieldSetDataOut>()
               .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(d => d.Fields, opt => opt.MapFrom(src => src.Fields))
               .ForMember(d => d.Label, opt => opt.MapFrom(src => src.Label))
               .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
               .ReverseMap();

            CreateMap<Field, FormTreeFieldDataOut>()
               .IgnoreAllNonExisting()
               .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
               .ForMember(d => d.Label, opt => opt.MapFrom(src => src.Label))
               .ReverseMap();

            CreateMap<FieldSelectable, FormTreeFieldDataOut>()
               .IncludeBase<Field, FormTreeFieldDataOut>()
               .ForMember(d => d.Values, opt => opt.MapFrom(src => src.Values));

            CreateMap<FieldString, FormTreeFieldDataOut>()
               .IncludeBase<Field, FormTreeFieldDataOut>();

            CreateMap<FormFieldValue, FormTreeFieldValueDataOut>()
               .IgnoreAllNonExisting()
               .ForMember(d => d.Value, opt => opt.MapFrom(src => src.Value))
               .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
               .ForMember(d => d.Label, opt => opt.MapFrom(src => src.Label))
               .ReverseMap();

            CreateMap<Form, FormEpisodeOfCareDataOut>()
                .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
                .ForMember(d => d.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id));

            /*DATA OUT*/

            CreateMap<Form, FormDataOut>()
            .IgnoreAllNonExisting()
            .ForMember(d => d.ParentFieldInstanceDependencies, opt => opt.Ignore())
            .ForMember(d => d.About, opt => opt.MapFrom(src => src.About))
            .ForMember(d => d.Chapters, opt => opt.MapFrom(src => src.Chapters))
            .ForMember(d => d.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
            .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(d => d.State, opt => opt.MapFrom(src => src.State))
            .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(d => d.Language, opt => opt.MapFrom(src => src.Language))
            .ForMember(d => d.Notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(d => d.FormState, opt => opt.MapFrom(src => src.FormState))
            .ForMember(d => d.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(d => d.Version, opt => opt.MapFrom(src => src.Version))
            .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
            .ForMember(d => d.DocumentProperties, opt => opt.MapFrom(src => src.DocumentProperties))
            .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
            .ForMember(d => d.WorkflowHistory, opt => opt.MapFrom(src => src.WorkflowHistory))
            .ForMember(d => d.CustomHeaderFields, opt => opt.MapFrom(src => src.CustomHeaderFields))
            .ForMember(d => d.AvailableForTask, opt => opt.MapFrom(src => src.AvailableForTask))
            .ForMember(d => d.NullFlavors, opt => opt.MapFrom(src => src.NullFlavors))
            .ForMember(d => d.OrganizationIds, opt => opt.MapFrom(src => src.OrganizationIds))
            .ReverseMap();

            CreateMap<FormDataIn, FormDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.ParentFieldInstanceDependencies, opt => opt.Ignore())
                .ForMember(d => d.CustomHeaderFields, opt => opt.MapFrom(src => src.CustomHeaderFields))
                .ForMember(d => d.NullFlavors, opt => opt.MapFrom(src => src.NullFlavors))
                .ReverseMap();

            CreateMap<FormAbout, FormAboutDataOut>();

            CreateMap<FormChapter, FormChapterDataOut>()
                .IgnoreAllNonExisting();

            CreateMap<FormPage, FormPageDataOut>()
                .IgnoreAllNonExisting();

            CreateMap<FieldSet, FormFieldSetDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.AllParentFieldInstanceDependencies, opt => opt.Ignore())
                .ForMember(d => d.ParentFieldInstanceDependencies, opt => opt.Ignore());

            CreateMap<FormFieldDependable, FormFieldDependableDataOut>()
                .IgnoreAllNonExisting()
                .ReverseMap();

            CreateMap<FormFieldValue, FormFieldValueDataOut>().ReverseMap();
            CreateMap<LayoutStyle, FormLayoutStyleDataOut>().ReverseMap();
            CreateMap<Help, FormHelpDataOut>();
            CreateMap<FormEpisodeOfCare, FormEpisodeOfCareDataDataOut>();

            /*DATA IN*/
            CreateMap<FormDataIn, Form>()
            .IgnoreAllNonExisting()
            .ForMember(d => d.About, opt => opt.MapFrom(src => src.About))
            .ForMember(d => d.Chapters, opt => opt.MapFrom(src => src.Chapters))
            .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(d => d.State, opt => opt.MapFrom(src => src.State))
            .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(d => d.Language, opt => opt.MapFrom(src => src.Language))
            .ForMember(d => d.Version, opt => opt.MapFrom(src => src.Version))
            .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
            .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
            .ForMember(d => d.DocumentProperties, opt => opt.MapFrom(src => src.DocumentProperties))
            .ForMember(d => d.CustomHeaderFields, opt => opt.MapFrom(src => src.CustomHeaderFields))
            .ForMember(d => d.AvailableForTask, opt => opt.MapFrom(src => src.AvailableForTask))
            .ForMember(d => d.NullFlavors, opt => opt.MapFrom(src => src.NullFlavors))
            .ForMember(d => d.OrganizationIds, opt => opt.MapFrom(src => src.OrganizationIds))
            .ForMember(d => d.Invalid, opt => opt.MapFrom(src => src.Invalid))
            .ReverseMap();

            CreateMap<DTOs.Form.DataIn.FormEpisodeOfCareDataDataIn, FormEpisodeOfCare>();
            CreateMap<DTOs.Form.DataIn.FormEpisodeOfCareDataDataIn, FormEpisodeOfCareDataDataOut>();

            CreateMap<FormAboutDataIn, FormAbout>();
            CreateMap<FormAboutDataIn, FormAboutDataOut>();

            CreateMap<FormPageImageMap, FormPageImageMapDataOut>();

            CreateMap<FormPageImageMapDataIn, FormPageImageMap>();
            CreateMap<FormPageImageMapDataIn, FormPageImageMapDataOut>();

            CreateMap<FormChapterDataIn, FormChapter>()
                .IgnoreAllNonExisting();
            CreateMap<FormChapterDataIn, FormChapterDataOut>()
                .IgnoreAllNonExisting();

            CreateMap<FormPageDataIn, FormPage>()
                .IgnoreAllNonExisting();
            CreateMap<FormPageDataIn, FormPageDataOut>()
                .IgnoreAllNonExisting();

            CreateMap<FormFieldSetDataIn, FieldSet>()
                .IgnoreAllNonExisting();
            CreateMap<FormFieldSetDataIn, FormFieldSetDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.AllParentFieldInstanceDependencies, opt => opt.Ignore())
                .ForMember(d => d.ParentFieldInstanceDependencies, opt => opt.Ignore());

            CreateMap<FormFieldDependableDataIn, FormFieldDependable>();

            CreateMap<FormFieldValueDataIn, FormFieldValue>()
                .IgnoreAllNonExisting();

            CreateMap<FormLayoutStyleDataIn, LayoutStyle>();
            CreateMap<FormLayoutStyleDataIn, FormLayoutStyleDataOut>();

            CreateMap<FormHelpDataIn, FormHelpDataOut>();

            CreateMap<KeyValue, KeyValueDTO>().ReverseMap();
            CreateMap<ReferalInfo, ReferralInfoDTO>()
                .IgnoreAllNonExisting()
                .ReverseMap();

            CreateMap<FormStatus, FormStatusDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.Created, opt => opt.MapFrom(src => src.Created))
                .ForMember(d => d.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<FormFieldDependableDataIn, FormFieldDependableDataOut>()
                .IgnoreAllNonExisting()
                .ReverseMap();
            CreateMap<FormFieldValueDataIn, FormFieldValueDataOut>().ReverseMap();

            CreateMap<O4CodeableConcept, O4CodeableConceptDataIn>()
                .IgnoreAllNonExisting()
                .ForMember(dest => dest.System, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<O4CodeableConcept, O4CodeableConceptDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(x => x.System, opt => opt.Ignore())
                .ReverseMap();

            // Custom Headers
            CreateMap<CustomHeaderField, CustomHeaderFieldDataOut>().ReverseMap();
            CreateMap<CustomHeaderFieldDataIn, CustomHeaderField>().ReverseMap();
            CreateMap<CustomHeaderFieldDataIn, CustomHeaderFieldDataOut>().ReverseMap();

        }
    }
}