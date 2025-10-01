using AutoMapper;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance;
using sReportsV2.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Patient.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.Cache.Singleton;
using System.Linq;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Extensions;

namespace sReportsV2.MapperProfiles
{
    public class FormInstanceProfile : Profile
    {
        public FormInstanceProfile()
        {
            CreateMap<FormInstancePerDomain, FormInstancePerDomainDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.Count, opt => opt.MapFrom(src => src.Count))
                .ForMember(d => d.Domain, opt => opt.MapFrom(src => src.Domain.ToString()))
                .AfterMap((src, dest, context) =>
                {
                    string language = (string)context.Items["Language"];
                    dest.Label = SingletonDataContainer.Instance.GetCodesByCodeSetId((int)CodeSetList.ClinicalDomain)
                        .Find(x => x.Id == src.Domain)
                        ?.Thesaurus.GetPreferredTermByTranslationOrDefault(language);
                });

            CreateMap<FormInstance, PatientFormInstanceDataOut>()
                .IgnoreAllNonExisting();

            CreateMap<FormInstancePreview, FormInstanceTableDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(d => d.Version, opt => opt.MapFrom(src => src.Version))
                .ForMember(d => d.Language, opt => opt.MapFrom(src => src.Language))
                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(d => d.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(d => d.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
                .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(d => d.ProjectId, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(d => d.FieldsToDisplay, opt => opt.MapFrom(src => src.FieldsToDisplay));

            CreateMap<FormInstance, FormDataOut>()
            .ForMember(dest => dest.WorkflowHistory, opt => opt.Ignore())
            .IgnoreAllNonExisting()
            .ForMember(d => d.CustomHeaderFields, opt => opt.Ignore())
            .ForMember(d => d.ParentFieldInstanceDependencies, opt => opt.Ignore())
            .ForMember(d => d.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
            .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(d => d.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
            .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(d => d.Version, opt => opt.MapFrom(src => src.Version))
            .ForMember(d => d.Language, opt => opt.MapFrom(src => src.Language))
            .ForMember(d => d.Notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(d => d.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(d => d.FormState, opt => opt.MapFrom(src => src.FormState))
            .ReverseMap();

            CreateMap<FormInstance, DiagnosticReportDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(d => d.Version, opt => opt.MapFrom(src => src.Version))
                .ReverseMap();

            CreateMap<FormInstanceFilterData, FormInstanceFilterDataIn>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.CustomFieldFiltersDataIn, opt => opt.MapFrom(src => src.CustomFieldFiltersData))
                .ReverseMap();

            CreateMap<FormInstance, FormInstanceReferralDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
                .ReverseMap();

            CreateMap<FormInstanceMetadata, FormInstanceMetadataDataOut>();

            CreateMap<FormInstance, FormInstanceMetadataDataOut>()
                .ForMember(d => d.FormInstanceId, opt => opt.MapFrom(src => src.Id))
                ;

            CreateMap<FormInstancePartialLockOrUnlockDataIn, FormInstancePartialLock>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.NextState, opt => opt.MapFrom(src => src.ChapterPageNextState));
            CreateMap<FormInstanceLockUnlockRequestDataIn, FormInstanceLockUnlockRequest>()
                .IgnoreAllNonExisting();

            CreateMap<FieldInstance, FieldDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.Id, dst => dst.MapFrom(src => src.FieldId));

            CreateMap<FieldInstanceValue, FieldInstanceValueDataOut>();
        }
    }
}