using AutoMapper;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.DTOs.Prompt.DataIn;
using sReportsV2.Domain.MongoDb.Entities.Promp;
using sReportsV2.DTOs.DTOs.Prompt.DataOut;
using sReportsV2.DTOs.DTOs.UploadPatientData.DataIn;
using sReportsV2.Domain.Sql.Entities.UploadPatientData;
using sReportsV2.DTOs.DTOs.UploadPatientData.DataOut;

namespace sReportsV2.MapperProfiles
{
    public class UploadPatientDataAndPromptProfile : Profile
    {
        public UploadPatientDataAndPromptProfile()
        {
            CreateMap<PromptDataIn, PromptFormFilter>()
                .IgnoreAllNonExisting();

            CreateMap<PromptFormVersion, PromptFormVersionDataOut>()
               .IgnoreAllNonExisting();

            CreateMap<PromptField, PromptFieldDataOut>()
               .IgnoreAllNonExisting();

            CreateMap<UploadPatientDataIn, UploadPatientData>()
                .AfterMap<CommonGlobalAfterMapping<UploadPatientData>>();

            CreateMap<UploadPatientFilterDataIn, UploadPatientDataFilter>();
            CreateMap<UploadPatientData, UploadPatientDataOut>()
             .IgnoreAllNonExisting()
            ;
        }
    }
}