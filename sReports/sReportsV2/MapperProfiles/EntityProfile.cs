using AutoMapper;
using sReportsV2.Common.Entities;
using sReportsV2.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.MapperProfiles
{
    public class EntityProfile : Profile
    {
        public EntityProfile()
        {
            CreateMap<DataIn, EntityFilter>();
        }
    }
}