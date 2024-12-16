using AutoMapper;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;

namespace sReportsV2.MapperProfiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Domain.Sql.Entities.FormComment.Comment, FormCommentDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.CommentId))
                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src.PersonnelId));

            CreateMap<FormCommentDataIn, Domain.Sql.Entities.FormComment.Comment>()
                .IgnoreAllNonExisting()
                .ForMember(d =>  d.CommentId, opt => opt.MapFrom(src => src.Id))
                .ForMember(d =>  d.PersonnelId, opt => opt.MapFrom(src => src.UserId))
                .AfterMap<CommonGlobalAfterMapping<Domain.Sql.Entities.FormComment.Comment>>();
        }
    }
}