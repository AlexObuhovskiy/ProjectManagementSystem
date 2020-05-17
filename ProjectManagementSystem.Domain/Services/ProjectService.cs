using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ProjectManagementSystem.DataAccess.Interfaces;
using ProjectManagementSystem.DataAccess.Models;
using ProjectManagementSystem.Domain.Interfaces;
using ProjectManagementSystem.Domain.Models.Project;

namespace ProjectManagementSystem.Domain.Services
{
    /// <summary>
    /// Class ProjectService.
    /// Implements the <see cref="IProjectService" />
    /// </summary>
    /// <seealso cref="IProjectService" />
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Project> _projectRepository;
        private readonly IMapper _mapper;

        public ProjectService(
            IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _projectRepository = _unitOfWork.GetRepository<Project>();
        }

        /// <inhertidoc/>
        public async Task<ProjectResponseDto[]> GetAllProjects()
        {
            Project[] projects = await _projectRepository.Get();
            ProjectResponseDto[] responseDtoArray = _mapper.Map<ProjectResponseDto[]>(projects);

            return responseDtoArray;
        }
    }
}