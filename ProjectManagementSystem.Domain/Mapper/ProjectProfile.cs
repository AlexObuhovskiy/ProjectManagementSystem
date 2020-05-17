using AutoMapper;
using ProjectManagementSystem.DataAccess.Models;
using ProjectManagementSystem.Domain.Enums;
using ProjectManagementSystem.Domain.Extensions;
using ProjectManagementSystem.Domain.Models.Project;

namespace ProjectManagementSystem.Domain.Mapper
{
    /// <summary>
    /// Class ProjectProfile.
    /// Implements the <see cref="AutoMapper.Profile" />
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class ProjectProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectProfile"/> class.
        /// </summary>
        public ProjectProfile()
        {
            CreateMap<Project, ProjectResponseDto>()
                .ForMember(dest => dest.State,
                    opt => opt.MapFrom(src => (State)src.State));
           
            CreateMap<ProjectRequestCreateDto, Project>()
                .ForMember(dest => dest.State,
                    opt => opt.Ignore())
                .ForMember(dest => dest.StartDate,
                    opt => opt.Ignore())
                .ForMember(dest => dest.FinishDate,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ProjectId,
                    opt => opt.Ignore())
                .IgnoreAllVirtual();

            CreateMap<ProjectRequestUpdateDto, Project>()
                .ForMember(dest => dest.State,
                    opt => opt.Ignore())
                .ForMember(dest => dest.StartDate,
                    opt => opt.Ignore())
                .ForMember(dest => dest.FinishDate,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ProjectId,
                    opt => opt.Ignore())
                .IgnoreAllVirtual();
        }
    }
}