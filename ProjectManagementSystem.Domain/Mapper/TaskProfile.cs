using AutoMapper;
using ProjectManagementSystem.DataAccess.Models;
using ProjectManagementSystem.Domain.Enums;
using ProjectManagementSystem.Domain.Extensions;
using ProjectManagementSystem.Domain.Models.Task;

namespace ProjectManagementSystem.Domain.Mapper
{
    /// <summary>
    /// Class TaskProfile.
    /// Implements the <see cref="AutoMapper.Profile" />
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class TaskProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskProfile"/> class.
        /// </summary>
        public TaskProfile()
        {
            CreateMap<Task, TaskResponseDto>()
                .ForMember(dest => dest.State,
                    opt => opt.MapFrom(src => (State)src.State));

            CreateMap<TaskRequestCreateDto, Task>()
                .ForMember(dest => dest.TaskId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.State,
                    opt => opt.Ignore())
                .ForMember(dest => dest.StartDate,
                    opt => opt.Ignore())
                .ForMember(dest => dest.FinishDate,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ProjectId,
                    opt => opt.MapFrom(src => src.ProjectId.Value))
                .IgnoreAllVirtual();

            CreateMap<TaskRequestUpdateDto, Task>()
                .ForMember(dest => dest.State,
                    opt => opt.MapFrom(src => (int)src.State.Value))
                .ForMember(dest => dest.StartDate,
                    opt => opt.Ignore())
                .ForMember(dest => dest.FinishDate,
                    opt => opt.Ignore())
                .ForMember(dest => dest.TaskId,
                    opt => opt.MapFrom(src => src.TaskId.Value))
                .ForMember(dest => dest.ProjectId,
                    opt => opt.MapFrom(src => src.ProjectId.Value))
                .IgnoreAllVirtual();
        }
    }
}