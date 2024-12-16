using AutoMapper;
using sReportsV2.DTOs.DTOs.TaskEntry.DataIn;
using sReportsV2.Domain.Sql.Entities.TaskEntry;
using sReportsV2.DTOs.DTOs.TaskEntry.DataOut;
using sReportsV2.Common.Extensions;

namespace sReportsV2.MapperProfiles
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<TaskDataIn, Task>()
              .IgnoreAllNonExisting()
              .ForMember(o => o.TaskId, opt => opt.MapFrom(src => src.TaskId))
              .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.PatientId))
              .ForMember(o => o.EncounterId, opt => opt.MapFrom(src => src.EncounterId))
              .ForMember(o => o.TaskTypeCD, opt => opt.MapFrom(src => src.TaskTypeCD))
              .ForMember(o => o.TaskStatusCD, opt => opt.MapFrom(src => src.TaskStatusCD))
              .ForMember(o => o.TaskPriorityCD, opt => opt.MapFrom(src => src.TaskPriorityCD))
              .ForMember(o => o.TaskClassCD, opt => opt.MapFrom(src => src.TaskClassCD))
              .ForMember(o => o.TaskDescription, opt => opt.MapFrom(src => src.TaskDescription))
              .ForMember(o => o.TaskEntityId, opt => opt.MapFrom(src => src.TaskEntityId))
              .ForMember(o => o.TaskStartDateTime, opt => opt.MapFrom(src => src.TaskStartDateTime))
              .ForMember(o => o.TaskEndDateTime, opt => opt.MapFrom(src => src.TaskEndDateTime))
              .ForMember(o => o.ScheduledDateTime, opt => opt.MapFrom(src => src.ScheduledDateTime))
              .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
              .AfterMap<CommonGlobalAfterMapping<Task>>();

            CreateMap<Task, TaskDataOut>()
              .ForMember(o => o.TaskId, opt => opt.MapFrom(src => src.TaskId))
              .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.PatientId))
              .ForMember(o => o.EncounterId, opt => opt.MapFrom(src => src.EncounterId))
              .ForMember(o => o.TaskTypeCD, opt => opt.MapFrom(src => src.TaskTypeCD))
              .ForMember(o => o.TaskStatusCD, opt => opt.MapFrom(src => src.TaskStatusCD))
              .ForMember(o => o.TaskPriorityCD, opt => opt.MapFrom(src => src.TaskPriorityCD))
              .ForMember(o => o.TaskClassCD, opt => opt.MapFrom(src => src.TaskClassCD))
              .ForMember(o => o.TaskDescription, opt => opt.MapFrom(src => src.TaskDescription))
              .ForMember(o => o.TaskEntityId, opt => opt.MapFrom(src => src.TaskEntityId))
              .ForMember(o => o.TaskStartDateTime, opt => opt.MapFrom(src => src.TaskStartDateTime))
              .ForMember(o => o.TaskEndDateTime, opt => opt.MapFrom(src => src.TaskEndDateTime))
              .ForMember(o => o.ScheduledDateTime, opt => opt.MapFrom(src => src.ScheduledDateTime))
              .ForMember(o => o.TaskDocumentCD, opt => opt.MapFrom(src => src.TaskDocument.TaskDocumentCD));

            CreateMap<TaskFilterDataIn, TaskFilter>();

            CreateMap<TaskDocumentDataIn, TaskDocument>()
                .IgnoreAllNonExisting()
                .AfterMap<CommonGlobalAfterMapping<TaskDocument>>();

            CreateMap<TaskDocument, TaskDocumentDataOut>()
              .ForMember(o => o.TaskDocumentId, opt => opt.MapFrom(src => src.TaskDocumentId))
              .ForMember(o => o.TaskDocumentCD, opt => opt.MapFrom(src => src.TaskDocumentCD))
              .ForMember(o => o.FormId, opt => opt.MapFrom(src => src.FormId))
              .ForMember(o => o.FormTitle, opt => opt.MapFrom(src => src.FormTitle));
        }
    }
}