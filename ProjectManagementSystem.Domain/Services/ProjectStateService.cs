using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectStateService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public ProjectStateService(IUnitOfWork unitOfWork)
        {
            _projectRepository = unitOfWork.GetRepository<Project>();
        }

        /// <inhertidoc/>
        public async Task CheckProjectAndParentsToChangeState(int? id)
        {
            if (id == null)
            {
                return;
            }

            Project project = (
                    await _projectRepository.Get(
                        p => p.ProjectId == id,
                        p => p.Include(x => x.Task)))
                .First();

            var currentState = GetCurrentProjectState(project);
            if (currentState == (State)project.State)
            {
                return;
            }

            ChangeProjectState(project, currentState);

            await CheckProjectAndParentsToChangeState(project.ParentId);
        }

        private State GetCurrentProjectState(Project project)
        {
            if (project.Task.Any() && project.Task.All(task => (State)task.State == State.Completed))
            {
                return State.Completed;
            }

            if (project.Task.Any(task => (State)task.State == State.InProgress))
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
                    project.StartDate = DateTime.UtcNow;
                    break;
                case State.Completed:
                    project.FinishDate = DateTime.UtcNow;
                    break;
            }

            project.State = (int)state;
        }
    }
}