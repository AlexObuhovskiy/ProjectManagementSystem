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
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectProfile"/> class.
        /// </summary>
        public ProjectProfile()
        {
            CreateMap<Project, ProjectResponseDto>();
        }
    }
}