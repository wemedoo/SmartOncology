using AutoMapper;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.FieldInstanceHistory;
using sReportsV2.DTOs.DTOs.FieldInstanceHistory.DataOut;
using sReportsV2.DTOs.DTOs.FieldInstanceHistory.FieldInstanceHistoryDataIn;

namespace sReportsV2.MapperProfiles
{
    public class FieldInstanceHistoryProfile : Profile
    {
        public FieldInstanceHistoryProfile()
        {
            CreateMap<FieldInstanceHistory, FieldInstanceHistoryDataOut>()
                .IgnoreAllNonExisting();

            CreateMap<FieldInstanceHistoryFilterDataIn, FieldInstanceHistoryFilterData>();
        }
    }
}