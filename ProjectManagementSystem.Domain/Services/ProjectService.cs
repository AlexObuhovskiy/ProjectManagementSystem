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

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper.</param>
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