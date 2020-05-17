using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.DataAccess.Extensions;
using ProjectManagementSystem.DataAccess.Interfaces;
using ProjectManagementSystem.Domain.Enums;
using ProjectManagementSystem.Domain.Exceptions;
using ProjectManagementSystem.Domain.Interfaces;
using ProjectManagementSystem.Domain.Models.Task;

namespace ProjectManagementSystem.Domain.Services
{
    /// <summary>
    /// Class TaskService for work with tasks.
    /// Implements the <see cref="ITaskService" />
    /// </summary>
    /// <seealso cref="ITaskService" />
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<DataAccess.Models.Task> _taskRepository;
        private readonly IMapper _mapper;
        private readonly IProjectStateService _projectStateService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="projectStateService">The project state service.</param>
        public TaskService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IProjectStateService projectStateService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _projectStateService = projectStateService;
            _taskRepository = _unitOfWork.GetRepository<DataAccess.Models.Task>();
        }

        /// <inhertidoc/>
        public async Task<TaskResponseDto[]> GetAllTasks()
        {
            var tasks = await _taskRepository.Get();
            var responseDtoArray = _mapper.Map<TaskResponseDto[]>(tasks);

            return responseDtoArray;
        }

        /// <inhertidoc/>
        public async Task<TaskResponseDto> GetById(int id)
        {
            var task = await GetEntityById(id);
            var responseDto = _mapper.Map<TaskResponseDto>(task);

            return responseDto;
        }

        /// <inhertidoc/>
        public async Task<TaskResponseDto> Create(TaskRequestCreateDto taskRequestCreateDto)
        {
            var entity = _mapper.Map<DataAccess.Models.Task>(taskRequestCreateDto);

            await _taskRepository.InsertAsync(entity);
            
            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (SqlException e)
            {
                throw new CreationException(e.Message);
            }

            var responseDto = _mapper.Map<TaskResponseDto>(entity);

            return responseDto;
        }

        /// <inhertidoc/>
        public async Task<TaskResponseDto> Update(TaskRequestUpdateDto taskRequestUpdateDto)
        {
            // ReSharper disable once PossibleInvalidOperationException
            var existingTask = await GetEntityById(taskRequestUpdateDto.TaskId.Value);
            var existingTaskProjectId = existingTask.ProjectId;
            var entityToSave = _mapper.Map(taskRequestUpdateDto, existingTask);
            SetupTaskByState(entityToSave);

            _taskRepository.Update(entityToSave);

            await _projectStateService.CheckProjectAndParentsToChangeState(taskRequestUpdateDto.ProjectId);

            if (existingTaskProjectId != taskRequestUpdateDto.ProjectId)
            {
                await _projectStateService.CheckProjectAndParentsToChangeState(existingTaskProjectId);
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (SqlException e)
            {
                throw new UpdateException(e.Message);
            }

            var responseDto = _mapper.Map<TaskResponseDto>(entityToSave);

            return responseDto;
        }

        /// <inhertidoc/>
        public async Task Delete(int id)
        {
            var existingTask = _taskRepository.GetTaskWithAllChildren(id);
            if (existingTask == null)
            {
                throw new RecordNotFoundException($"There is no {nameof(DataAccess.Models.Task)} with id {id}");
            }

            DeleteTaskWithAllChildren(existingTask);
            await _projectStateService.CheckProjectAndParentsToChangeState(existingTask.ProjectId);
            await _unitOfWork.SaveChangesAsync();
        }

        private void SetupTaskByState(DataAccess.Models.Task task)
        {
            switch ((State)task.State)
            {
                case State.Planned:
                    task.StartDate = null;
                    task.FinishDate = null;
                    break;
                case State.InProgress:
                    task.StartDate = DateTime.UtcNow;
                    task.FinishDate = null;
                    break;
                case State.Completed:
                    task.FinishDate = DateTime.UtcNow;
                    break;
            }
        }

        private void DeleteTaskWithAllChildren(DataAccess.Models.Task taskToDeleteChildren)
        {
            foreach (var task in taskToDeleteChildren.InverseParent)
            {
                DeleteTaskWithAllChildren(task);
                _taskRepository.Delete(task);
            }

            _taskRepository.Delete(taskToDeleteChildren);
        }

        private async Task<DataAccess.Models.Task> GetEntityById(int id)
        {
            var entity = await _taskRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new RecordNotFoundException($"There is no {nameof(DataAccess.Models.Task)} with id {id}");
            }

            return entity;
        }
    }
}
