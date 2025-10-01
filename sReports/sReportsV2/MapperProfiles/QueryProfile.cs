using AutoMapper;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.QueryManagement;
using sReportsV2.DTOs.DTOs.QueryManagement.DataIn;
using sReportsV2.DTOs.DTOs.QueryManagement.DataOut;
using System.Linq;

namespace sReportsV2.MapperProfiles
{
    public class QueryProfile : Profile
    {
        public QueryProfile() 
        {
            CreateMap<QueryDataIn, Query>()
             .IgnoreAllNonExisting()
             .AfterMap<CommonGlobalAfterMapping<Query>>();

            CreateMap<Query, QueryDataOut>()
                .ForMember(dest => dest.History, opt => opt.MapFrom(
                    src => src.History
                        .Where(r => !r.IsDeleted())
                        .ToList()
                    )
                 )
                .IgnoreAllNonExisting();

            CreateMap<QueryHistory, QueryHistoryDataOut>()
                .IgnoreAllNonExisting();

            CreateMap<QueryHistoryDataIn, QueryHistory>()
             .IgnoreAllNonExisting();

            CreateMap<QueryFilterDataIn, QueryFilter>();
        }
    }
}
