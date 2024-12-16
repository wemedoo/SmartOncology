using AutoMapper;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.ClinicalTrial;
using sReportsV2.DTOs.DTOs.TrialManagement;

namespace sReportsV2.MapperProfiles
{
    public class ClinicalTrialProfile : Profile
    {
        public ClinicalTrialProfile()
        {
            CreateMap<ClinicalTrialDataIn, ClinicalTrial>()
                .IgnoreAllNonExisting()
                .AfterMap<CommonGlobalAfterMapping<ClinicalTrial>>();

            CreateMap<ClinicalTrial, ClinicalTrialDataOut>()
                .IgnoreAllNonExisting();

            CreateMap<TrialManagementFilterDataIn, TrialManagementFilter>();
        }
    }
}