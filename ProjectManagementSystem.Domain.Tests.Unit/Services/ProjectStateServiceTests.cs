using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using ProjectManagementSystem.DataAccess.Interfaces;
using ProjectManagementSystem.DataAccess.Models;
using ProjectManagementSystem.Domain.Enums;
using ProjectManagementSystem.Domain.Services;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystem.Domain.Tests.Unit.Services
{
    public class ProjectStateServiceTests
    {
        private readonly Mock<IGenericRepository<Project>> _projectRepository;
        private readonly Mock<IGenericRepository<DataAccess.Models.Task>> _taskRepository;
        private readonly ProjectStateService _projectStateService;

        public ProjectStateServiceTests()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            _projectRepository = new Mock<IGenericRepository<Project>>();
            _taskRepository = new Mock<IGenericRepository<DataAccess.Models.Task>>();
            unitOfWork
                .Setup(u => u.GetRepository<Project>())
                .Returns(_projectRepository.Object);
            unitOfWork
                .Setup(u => u.GetRepository<DataAccess.Models.Task>())
                .Returns(_taskRepository.Object);

            _projectStateService = new ProjectStateService(unitOfWork.Object);
        }

        [Fact]
        public async Task CheckProjectAndParentsToChangeState_WhenIdNull_ThenNothingCalls()
        {
            // Arrange
            _projectRepository
                .Setup(p => p.Get(
                    It.IsAny<Expression<Func<Project, bool>>>(),
                    It.IsAny<Func<IQueryable<Project>, IIncludableQueryable<Project, object>>>()));

            // Act
            await _projectStateService.CheckProjectAndParentsToChangeState(null);

            // Assert
            _projectRepository
                .Verify(p => p.Get(
                    It.IsAny<Expression<Func<Project, bool>>>(),
                    It.IsAny<Func<IQueryable<Project>, IIncludableQueryable<Project, object>>>()),
                    Times.Never);
        }

        [Fact]
        public async Task CheckProjectAndParentsToChangeState_WhenStateNotChanged_ThenNothingCalls()
        {
            // Arrange
            var project = new Project
            {
                ProjectId = 1,
                Task = new List<DataAccess.Models.Task>
                {
                    new DataAccess.Models.Task
                    {
                        ProjectId = 1,
                        State = (int) State.Planned
                    }
                },
                State = (int) State.Planned
            };

            _projectRepository
                .Setup(p => p.Get(
                    It.IsAny<Expression<Func<Project, bool>>>(),
                    It.IsAny<Func<IQueryable<Project>, IIncludableQueryable<Project, object>>>()))
                .Returns(Task.FromResult(new[] {project}));

            // Act
            await _projectStateService.CheckProjectAndParentsToChangeState(project.ProjectId);

            // Assert
            _projectRepository
                .Verify(p => p.Get(
                        It.IsAny<Expression<Func<Project, bool>>>(),
                        It.IsAny<Func<IQueryable<Project>, IIncludableQueryable<Project, object>>>()),
                    Times.Once);
            project.State.Should().Be((int) State.Planned);
        }

        [Theory]
        [InlineData(State.InProgress, false, true)]
        [InlineData(State.Completed, true, false)]
        public async Task CheckProjectAndParentsToChangeState_WhenStateChanged_ThenNothingCalls(
            State state,
            bool isStartDateNull,
            bool isFinishDateNull)
        {
            // Arrange

            var taskTest = new DataAccess.Models.Task
            {
                ProjectId = 1,
                State = (int) state
            };
            var project = new Project
            {
                ProjectId = 1,
                Task = new List<DataAccess.Models.Task>
                {
                    taskTest
                },
                State = (int)State.Planned
            };

            _projectRepository
                .Setup(p => p.Get(
                    It.IsAny<Expression<Func<Project, bool>>>(),
                    It.IsAny<Func<IQueryable<Project>, IIncludableQueryable<Project, object>>>()))
                .Returns(Task.FromResult(new[] {project}));
            var utcTimeStart = DateTime.UtcNow;

            _taskRepository.Setup(t => t.Get(
                    It.IsAny<Expression<Func<DataAccess.Models.Task, bool>>>(),
                    It.IsAny<Func<IQueryable<DataAccess.Models.Task>,
                        IIncludableQueryable<DataAccess.Models.Task, object>>>()))
                .Returns(Task.FromResult(new[] { taskTest }));

            // Act
            await _projectStateService.CheckProjectAndParentsToChangeState(project.ProjectId);
            var utcTimeFinish = DateTime.UtcNow;

            // Assert
            _projectRepository
                .Verify(p => p.Get(
                        It.IsAny<Expression<Func<Project, bool>>>(),
                        It.IsAny<Func<IQueryable<Project>, IIncludableQueryable<Project, object>>>()),
                    Times.Once);
            project.State.Should().Be((int) state);
            if (isStartDateNull)
            {
                project.StartDate.Should().BeNull();
            }
            else
            {
                project.StartDate.Should().BeAfter(utcTimeStart).And.BeBefore(utcTimeFinish);
            }

            if (isFinishDateNull)
            {
                project.FinishDate.Should().BeNull();
            }
            else
            {
                project.FinishDate.Should().BeAfter(utcTimeStart).And.BeBefore(utcTimeFinish);
            }
        }

        [Fact]
        public async Task CheckProjectAndParentsToChangeState_WhenStateChanged_ThenCheckParent()
        {
            // Arrange
            var taskTest = new DataAccess.Models.Task
            {
                ProjectId = 1,
                State = (int) State.InProgress
            };
            var project = new Project
            {
                ProjectId = 1,
                ParentId = 2,
                Task = new List<DataAccess.Models.Task>
                {
                    taskTest
                },
                State = (int)State.Planned
            };

            var parentProject = new Project
            {
                ProjectId = 2,
                State = (int)State.Planned
            };

            var listOfProject = new List<Project>{project, parentProject};
            int indexOfProject = 0;

            _projectRepository
                .Setup(p => p.Get(
                    It.IsAny<Expression<Func<Project, bool>>>(),
                    It.IsAny<Func<IQueryable<Project>, IIncludableQueryable<Project, object>>>()))
                .Returns((
                    Expression<Func<Project, bool>> ex,
                    Func<IQueryable<Project>, IIncludableQueryable<Project, object>> _) =>
                {
                    return Task.FromResult(new[] { listOfProject[indexOfProject]});
                })
                .Callback(() =>
                {
                    if (indexOfProject == 0)
                    {
                        indexOfProject++;
                    }
                });

            _taskRepository.Setup(t => t.Get(
                    It.IsAny<Expression<Func<DataAccess.Models.Task, bool>>>(),
                    It.IsAny<Func<IQueryable<DataAccess.Models.Task>,
                        IIncludableQueryable<DataAccess.Models.Task, object>>>()))
                .Returns(Task.FromResult(new[] {taskTest}));

            // Act
            await _projectStateService.CheckProjectAndParentsToChangeState(project.ProjectId);

            // Assert
            _projectRepository
                .Verify(p => p.Get(
                        It.IsAny<Expression<Func<Project, bool>>>(),
                        It.IsAny<Func<IQueryable<Project>, IIncludableQueryable<Project, object>>>()),
                    Times.Exactly(2));
            project.State.Should().Be((int)State.InProgress);
            parentProject.State.Should().Be((int)State.InProgress);
        }
    }
}