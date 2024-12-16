using AutoMapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using sReportsV2.Cache.Singleton;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Sql.Entities.Base;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using System;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.DTOs.Pagination;
using System.Linq;

namespace sReportsV2.MapperProfiles
{
    public class CommonProfile : Profile
    {
        public CommonProfile()
        {
            CreateMap<TelecomBase, TelecomDTO>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.ToBase64String(src.RowVersion)))
                ;

            CreateMap<TelecomDTO, TelecomBase>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.FromBase64String(src.RowVersion)))
                ;

            CreateMap<AddressBase, AddressDTO>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.AddressTypeCD, opt => opt.MapFrom(src => src.AddressTypeCD))
                .ForMember(o => o.CountryCD, opt => opt.MapFrom(src => src.CountryCD))
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.ToBase64String(src.RowVersion)))
                .AfterMap((entity, dto) =>
                 {
                     dto.Country = SingletonDataContainer.Instance.GetCodePreferredTerm(entity.CountryCD.GetValueOrDefault());
                 });

            CreateMap<AddressDTO, AddressBase>()
             .IgnoreAllNonExisting()
             .ForMember(o => o.AddressTypeCD, opt => opt.MapFrom(src => src.AddressTypeCD))
             .ForMember(o => o.CountryCD, opt => opt.MapFrom(src => src.CountryCD))
             .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.FromBase64String(src.RowVersion)))
             .ForMember(d => d.Country, opt => opt.Ignore());

            CreateMap<IdentifierDataIn, IdentifierBase>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.IdentifierTypeCD, opt => opt.MapFrom(src => src.IdentifierTypeCD))
                .ForMember(d => d.IdentifierUseCD, opt => opt.MapFrom(src => src.IdentifierUseCD))
                .ForMember(d => d.IdentifierValue, opt => opt.MapFrom(src => src.IdentifierValue))
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.FromBase64String(src.RowVersion)))
                ;

            CreateMap<IdentifierBase, IdentifierDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(o => o.Value, opt => opt.MapFrom(src => src.IdentifierValue))
                .ForMember(o => o.IdentifierUseId, opt => opt.MapFrom(src => src.IdentifierUseCD))
                .ForMember(o => o.Use, opt => opt.MapFrom(src => SingletonDataContainer.Instance.GetCodes().FirstOrDefault(x => x.Id == src.IdentifierUseCD)))
                .ForMember(o => o.IdentifierTypeId, opt => opt.MapFrom(src => src.IdentifierTypeCD))
                .ForMember(o => o.System, opt => opt.MapFrom(src => SingletonDataContainer.Instance.GetCodes().FirstOrDefault(x => x.Id == src.IdentifierTypeCD)))
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.ToBase64String(src.RowVersion)))
                ;
        }
    }
}