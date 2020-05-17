using AutoMapper;
using Microsoft.Data.SqlClient;
using ProjectManagementSystem.DataAccess.Interfaces;
using ProjectManagementSystem.DataAccess.Models;
using ProjectManagementSystem.Domain.Exceptions;
using ProjectManagementSystem.Domain.Interfaces;
using ProjectManagementSystem.Domain.Models.Project;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

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
        private readonly IProjectStateService _projectStateService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="projectStateService">The project state service.</param>
        public ProjectService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IProjectStateService projectStateService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _projectStateService = projectStateService;
            _projectRepository = _unitOfWork.GetRepository<Project>();
        }

        /// <inhertidoc/>
        public async Task<ProjectResponseDto[]> GetAllProjects()
        {
            Project[] projects = await _projectRepository.Get();
            ProjectResponseDto[] responseDtoArray = _mapper.Map<ProjectResponseDto[]>(projects);

            return responseDtoArray;
        }

        /// <inhertidoc/>
        public async Task<ProjectResponseDto> GetById(int id)
        {
            Project project = await _projectRepository.GetByIdAsync(id);
            var responseDto = _mapper.Map<ProjectResponseDto>(project);

            return responseDto;
        }

        /// <inhertidoc/>
        public async Task<ProjectResponseDto> Create(ProjectRequestCreateDto projectRequestCreateDto)
        {
            var entity = _mapper.Map<Project>(projectRequestCreateDto);

            await _projectRepository.InsertAsync(entity);
            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (SqlException e)
            {
                throw new CreationException(e.Message);
            }

            var responseDto = _mapper.Map<ProjectResponseDto>(entity);

            return responseDto;
        }

        /// <inhertidoc/>
        public async Task<ProjectResponseDto> Update(ProjectRequestUpdateDto projectRequestUpdateDto)
        {
            // ReSharper disable once PossibleInvalidOperationException
            var existingProject = await GetEntityById(projectRequestUpdateDto.ProjectId.Value);
            var existingProjectParentId = existingProject.ParentId;
            var entityToSave = _mapper.Map(projectRequestUpdateDto, existingProject);

            _projectRepository.Update(entityToSave);
            
            if (existingProjectParentId != projectRequestUpdateDto.ParentId)
            {
                await _projectStateService.CheckProjectAndParentsToChangeState(projectRequestUpdateDto.ParentId);
                await _projectStateService.CheckProjectAndParentsToChangeState(existingProjectParentId);
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (SqlException e)
            {
                throw new UpdateException(e.Message);
            }

            var responseDto = _mapper.Map<ProjectResponseDto>(entityToSave);

            return responseDto;
        }

        /// <inhertidoc/>
        public async Task Delete(int id)
        {
            var existingProject = await GetEntityById(id);
            _projectRepository.Delete(existingProject);
            await _projectStateService.CheckProjectAndParentsToChangeState(existingProject.ParentId);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<Project> GetEntityById(int id)
        {
            var entity = await _projectRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new RecordNotFoundException($"There is no project with id {id}");
            }

            return entity;
        }
    }
}
