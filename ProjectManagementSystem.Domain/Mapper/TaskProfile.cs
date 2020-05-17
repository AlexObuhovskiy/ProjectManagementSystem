using AutoMapper;
using ProjectManagementSystem.DataAccess.Models;
using ProjectManagementSystem.Domain.Enums;
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
        }
    }
}