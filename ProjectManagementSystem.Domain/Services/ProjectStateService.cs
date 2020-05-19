using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.DataAccess.Extensions;
using ProjectManagementSystem.DataAccess.Interfaces;
using ProjectManagementSystem.DataAccess.Models;
using ProjectManagementSystem.Domain.Enums;
using ProjectManagementSystem.Domain.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystem.Domain.Services
{
    /// <summary>
    /// Class ProjectStateService.
    /// Implements the <see cref="IProjectStateService" />
    /// </summary>
    /// <seealso cref="IProjectStateService" />
    public class ProjectStateService : IProjectStateService
    {
        private readonly IGenericRepository<Project> _projectRepository;
        private readonly IGenericRepository<DataAccess.Models.Task> _taskRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectStateService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public ProjectStateService(IUnitOfWork unitOfWork)
        {
            _projectRepository = unitOfWork.GetRepository<Project>();
            _taskRepository = unitOfWork.GetRepository<DataAccess.Models.Task>();
        }

        /// <inhertidoc/>
        public async Task CheckProjectAndParentsToChangeState(int? id)
        {
            if (!id.HasValue)
            {
                return;
            }

            Project project = await _projectRepository.GetByIdAsync(id.Value);

            var currentState = await GetCurrentProjectStateById(project);
            if (currentState == (State)project.State)
            {
                return;
            }

            ChangeProjectState(project, currentState);

            await CheckProjectAndParentsToChangeState(project.ParentId);
        }

        private async Task<State> GetCurrentProjectStateById(Project project)
        {
            var allProjectIds = _projectRepository.GetAllProjectIdArray(project);
            var tasks = await _taskRepository.GetAllTaskIncludeSubProjects(allProjectIds);

            if (tasks.Any() && tasks.All(task => (State)task.State == State.Completed))
            {
                return State.Completed;
            }

            if (tasks.Any(task => (State)task.State == State.InProgress))
            {
                return State.InProgress;
            }

            return State.Planned;
        }

        private void ChangeProjectState(Project project, State state)
        {
            switch (state)
            {
                case State.Planned:
                    break;
                case State.InProgress:
                    project.StartDate ??= DateTime.UtcNow;
                    project.FinishDate = null;
                    break;
                case State.Completed:
                    project.FinishDate = DateTime.UtcNow;
                    break;
            }

            project.State = (int)state;
        }
    }
}