using AutoMapper;
using JetBrains.Annotations;
using ProjectManagementSystem.DataAccess.Models;
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
        public ProjectProfile()
        {
            CreateMap<Project, ProjectResponseDto>();
        }
    }
}