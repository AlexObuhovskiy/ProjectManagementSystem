using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using AutoMapper;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using ProjectManagementSystem.DataAccess.Interfaces;
using ProjectManagementSystem.Domain.Enums;
using ProjectManagementSystem.Domain.Exceptions;
using ProjectManagementSystem.Domain.Interfaces;
using ProjectManagementSystem.Domain.Models.Task;
using ProjectManagementSystem.Domain.Services;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystem.Domain.Tests.Unit.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IGenericRepository<DataAccess.Models.Task>> _taskRepository;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IProjectStateService> _projectStateService;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _taskRepository = new Mock<IGenericRepository<DataAccess.Models.Task>>();
            _mapper = new Mock<IMapper>();
            _projectStateService = new Mock<IProjectStateService>();
            _unitOfWork
                .Setup(u => u.GetRepository<DataAccess.Models.Task>())
                .Returns(_taskRepository.Object);

            _taskService = new TaskService(
                _unitOfWork.Object,
                _mapper.Object,
                _projectStateService.Object);
        }

        [Fact]
        public async Task Create_WhenSaveChangesThrowsSqlException_ThenCreationExceptionIsThrown()
        {
            // Arrange
            var taskRequestCreateDto = new TaskRequestCreateDto();
            _mapper
                .Setup(m => m.Map<DataAccess.Models.Task>(It.IsAny<TaskRequestCreateDto>()))
                .Returns(It.IsAny<DataAccess.Models.Task>());

            _taskRepository
                .Setup(p => p.InsertAsync(It.IsAny<DataAccess.Models.Task>()))
                .Returns(Task.FromResult(It.IsAny<DataAccess.Models.Task>()));

            var exception = FormatterServices.GetUninitializedObject(typeof(SqlException))
                as SqlException;

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .Throws(exception);

            // Act
            // Assert
            await Assert.ThrowsAsync<CreationException>(() => _taskService.Create(taskRequestCreateDto));
        }

        [Fact]
        public async Task Update_WhenEntityNotFound_ThenRecordNotFoundExceptionIsThrown()
        {
            // Arrange
            var dto = new TaskRequestUpdateDto
            {
                TaskId = 1
            };

            _taskRepository
                .Setup(p => p.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(It.IsAny<DataAccess.Models.Task>()));

            // Act
            // Assert
            await Assert.ThrowsAsync<RecordNotFoundException>(() => _taskService.Update(dto));
        }

        [Fact]
        public async Task Update_WhenSaveChangesThrowsSqlException_ThenUpdateExceptionIsThrown()
        {
            // Arrange
            var dto = new TaskRequestUpdateDto
            {
                TaskId = 1
            };

            _taskRepository
                .Setup(p => p.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(new DataAccess.Models.Task()));

            _mapper
                .Setup(m => m.Map(It.IsAny<TaskRequestUpdateDto>(), It.IsAny<DataAccess.Models.Task>()))
                .Returns((TaskRequestUpdateDto _, DataAccess.Models.Task t) => t);

            _taskRepository
                .Setup(p => p.Update(It.IsAny<DataAccess.Models.Task>()));

            var exception = FormatterServices.GetUninitializedObject(typeof(SqlException))
                as SqlException;

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .Throws(exception);

            // Act
            // Assert
            await Assert.ThrowsAsync<UpdateException>(() => _taskService.Update(dto));
        }

        [Fact]
        public async Task Update_TaskWithProjectIdThatHasDifferentChildProjectId_ThenUpdateExceptionIsThrown()
        {
            // Arrange
            var differentProjectId = 2;
            var dto = new TaskRequestUpdateDto
            {
                TaskId = 1,
                ProjectId = differentProjectId
            };

            var task = new DataAccess.Models.Task
            {
                TaskId = 1,
                ProjectId = 1,
                InverseParent = new List<DataAccess.Models.Task>
                {
                    new DataAccess.Models.Task
                    {
                        TaskId = 2,
                        ProjectId = 1
                    }
                }
            };

            _taskRepository
                .Setup(p => p.GetByIdAsync(It.Is<int>(x => x == 1)))
                .Returns(Task.FromResult(task));

            _mapper
                .Setup(m => m.Map(It.IsAny<TaskRequestUpdateDto>(), It.IsAny<DataAccess.Models.Task>()))
                .Returns((TaskRequestUpdateDto taskRequestUpdateDto, DataAccess.Models.Task t) =>
                {
                    t.ProjectId = differentProjectId;
                    return t;
                });

            _taskRepository
                .Setup(p => p.Update(It.IsAny<DataAccess.Models.Task>()));

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mapper
                .Setup(m => m.Map<TaskResponseDto>(It.IsAny<DataAccess.Models.Task>()))
                .Returns(It.IsAny<TaskResponseDto>());

            // Act
            // Assert
            await Assert.ThrowsAsync<UpdateException>(() => _taskService.Update(dto));
        }

        [Fact]
        public async Task Update_TaskWithProjectIdThatHasDifferentParentProjectId_ThenUpdateExceptionIsThrown()
        {
            // Arrange
            var differentProjectId = 2;
            var dto = new TaskRequestUpdateDto
            {
                TaskId = 1,
                ProjectId = differentProjectId
            };

            var task = new DataAccess.Models.Task
            {
                TaskId = 1,
                ProjectId = 1,
                ParentId = 3
            };

            var parent = new DataAccess.Models.Task
            {
                TaskId = 3,
                ProjectId = 3,
            };

            _taskRepository
                .Setup(p => p.GetByIdAsync(It.Is<int>(x => x == 1)))
                .Returns(Task.FromResult(task));

            _taskRepository
                .Setup(p => p.GetByIdAsync(It.Is<int>(x => x == 3)))
                .Returns(Task.FromResult(parent));

            _mapper
                .Setup(m => m.Map(It.IsAny<TaskRequestUpdateDto>(), It.IsAny<DataAccess.Models.Task>()))
                .Returns((TaskRequestUpdateDto taskRequestUpdateDto, DataAccess.Models.Task t) =>
                {
                    t.ProjectId = differentProjectId;
                    return t;
                });

            _taskRepository
                .Setup(p => p.Update(It.IsAny<DataAccess.Models.Task>()));

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mapper
                .Setup(m => m.Map<TaskResponseDto>(It.IsAny<DataAccess.Models.Task>()))
                .Returns(It.IsAny<TaskResponseDto>());

            // Act
            // Assert
            await Assert.ThrowsAsync<UpdateException>(() => _taskService.Update(dto));
        }

        [Theory]
        [InlineData(State.Planned, true, true)]
        [InlineData(State.Completed, true, false)]
        [InlineData(State.InProgress, false, true)]
        public async Task Update_CorrectTaskSetupWithStartAndFinishDates(
            State state,
            bool startDateIsNull,
            bool finishDateIsNull)
        {
            // Arrange
            var dto = new TaskRequestUpdateDto
            {
                TaskId = 1
            };

            var task = new DataAccess.Models.Task
            {
                State = (int) state
            };

            _taskRepository
                .Setup(p => p.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(task));

            _mapper
                .Setup(m => m.Map(It.IsAny<TaskRequestUpdateDto>(), It.IsAny<DataAccess.Models.Task>()))
                .Returns((TaskRequestUpdateDto _, DataAccess.Models.Task t) => t);

            _taskRepository
                .Setup(p => p.Update(It.IsAny<DataAccess.Models.Task>()));

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mapper
                .Setup(m => m.Map<TaskResponseDto>(It.IsAny<DataAccess.Models.Task>()))
                .Returns(It.IsAny<TaskResponseDto>());

            var utcTimeStart = DateTime.UtcNow;

            // Act
            await _taskService.Update(dto);
            var utcTimeFinish = DateTime.UtcNow;

            // Assert
            if (startDateIsNull)
            {
                task.StartDate.Should().BeNull();
            }
            else
            {
                task.StartDate.Should().BeAfter(utcTimeStart).And.BeBefore(utcTimeFinish);
            }

            if (finishDateIsNull)
            {
                task.FinishDate.Should().BeNull();
            }
            else
            {
                task.FinishDate.Should().BeAfter(utcTimeStart).And.BeBefore(utcTimeFinish);
            }
        }

        [Fact]
        public async Task Update_WhenProjectIdNotChanged_ThenCheckProjectAndParentsToChangeStateCalledOnce()
        {
            // Arrange
            var dto = new TaskRequestUpdateDto
            {
                TaskId = 1,
                ProjectId = 0
            };

            var task = new DataAccess.Models.Task();

            _taskRepository
                .Setup(p => p.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(task));

            _mapper
                .Setup(m => m.Map(It.IsAny<TaskRequestUpdateDto>(), It.IsAny<DataAccess.Models.Task>()))
                .Returns((TaskRequestUpdateDto _, DataAccess.Models.Task t) => t);

            _taskRepository
                .Setup(p => p.Update(It.IsAny<DataAccess.Models.Task>()));

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mapper
                .Setup(m => m.Map<TaskResponseDto>(It.IsAny<DataAccess.Models.Task>()))
                .Returns(It.IsAny<TaskResponseDto>());

            // Act
            await _taskService.Update(dto);

            // Assert
            _projectStateService
                .Verify(p => p.CheckProjectAndParentsToChangeState(It.IsAny<int?>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenProjectIdChanged_ThenCheckProjectAndParentsToChangeStateCalled()
        {
            // Arrange
            int projectId = 1;
            var dto = new TaskRequestUpdateDto
            {
                TaskId = 1,
                ProjectId = projectId
            };

            var task = new DataAccess.Models.Task
            {
                ProjectId = 2
            };

            _taskRepository
                .Setup(p => p.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(task));

            _mapper
                .Setup(m => m.Map(It.IsAny<TaskRequestUpdateDto>(), It.IsAny<DataAccess.Models.Task>()))
                .Returns((TaskRequestUpdateDto taskRequestUpdateDto, DataAccess.Models.Task t) =>
                {
                    t.ProjectId = projectId;
                    return t;
                });

            _taskRepository
                .Setup(p => p.Update(It.IsAny<DataAccess.Models.Task>()));

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mapper
                .Setup(m => m.Map<TaskResponseDto>(It.IsAny<DataAccess.Models.Task>()))
                .Returns(It.IsAny<TaskResponseDto>());

            // Act
            await _taskService.Update(dto);

            // Assert
            _projectStateService
                .Verify(p => p.CheckProjectAndParentsToChangeState(It.IsAny<int?>()), Times.Exactly(2));
        }

        [Fact]
        public async Task Delete_WhenEntityNotFound_ThenRecordNotFoundExceptionIsThrown()
        {
            // Arrange
            int projectId = 1;

            _taskRepository
                .Setup(p => p.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(It.IsAny<DataAccess.Models.Task>()));

            // Act
            // Assert
            await Assert.ThrowsAsync<RecordNotFoundException>(() => _taskService.Delete(projectId));
        }

        [Fact]
        public async Task Delete_WhenTwoChildrenExists_ThenDeleteCalledThreeTimesAndCheckProjectCalled()
        {
            // Arrange
            var task = new DataAccess.Models.Task
            {
                InverseParent = new List<DataAccess.Models.Task>
                {
                    new DataAccess.Models.Task(),
                    new DataAccess.Models.Task()
                }
            };

            _taskRepository
                .Setup(p => p.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(task));

            _taskRepository
                .Setup(p =>
                    p.LoadAllChildren(It.IsAny<DataAccess.Models.Task>(),
            It.IsAny<Expression<Func<DataAccess.Models.Task, IEnumerable<DataAccess.Models.Task>>>>()));

            _taskRepository
                .Setup(p => p.Delete(It.IsAny<DataAccess.Models.Task>()));

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _taskService.Delete(task.TaskId);

            // Assert
            _projectStateService
                .Verify(p =>
                    p.CheckProjectAndParentsToChangeState(It.IsAny<int?>()), Times.Once);

            _taskRepository
                .Verify(
                    p => p.Delete(It.IsAny<DataAccess.Models.Task>()),
                    Times.Exactly(3));
        }
    }
}