using AutoMapper;
using System.Linq;
using sReportsV2.Domain.Sql.Entities.ProjectEntry;
using sReportsV2.DTOs.DTOs.ProjectManagement.DataIn;
using sReportsV2.DTOs.DTOs.ProjectManagement.DataOut;
using sReportsV2.DTOs.Organization;
using System;
using sReportsV2.DTOs.DTOs.Patient.DataIn;
using sReportsV2.Common.Extensions;
using sReportsV2.DTOs.Common;
using sReportsV2.Common.Entities;
using sReportsV2.Domain.Sql.Entities.User;
using System.Collections.Generic;

namespace sReportsV2.MapperProfiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<ProjectDataIn, Project>()
                .IgnoreAllNonExisting()
                .AfterMap<CommonGlobalAfterMapping<Project>>();

            CreateMap<Project, ProjectDataOut>()
                .IgnoreAllNonExisting()
                .ForMember(d => d.Personnels, opt => opt.MapFrom(src => src.ProjectPersonnelRelations != null
                    ? src.ProjectPersonnelRelations
                        .Where(x => x.Personnel != null && !x.Personnel.IsDeleted())
                        .Select(x => x.Personnel)
                        .ToList()
                    : new List<Personnel>()));

            CreateMap<ProjectFilterDataIn, ProjectFilter>()
                .IncludeBase<DataIn, EntityFilter>();

            CreateMap<PatientProjectDataIn, ProjectPatientRelation>()
                .IgnoreAllNonExisting()
                .AfterMap<CommonGlobalAfterMapping<ProjectPatientRelation>>();

            CreateMap<ProjectDocumentDataIn, ProjectDocumentRelation>()
                .IgnoreAllNonExisting()
                .AfterMap<CommonGlobalAfterMapping<ProjectDocumentRelation>>();

            CreateMap<ProjectPersonnelDataIn, ProjectPersonnelRelation>()
                .IgnoreAllNonExisting()
                .AfterMap<CommonGlobalAfterMapping<ProjectPersonnelRelation>>();

            CreateMap<Project, ProjectTableDataOut>()
             .ForMember(o => o.ProjectId, opt => opt.MapFrom(src => src.ProjectId))
             .ForMember(o => o.ProjectName, opt => opt.MapFrom(src => src.ProjectName));
        }
    }
}