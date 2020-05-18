using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Moq;
using ProjectManagementSystem.DataAccess.Interfaces;
using ProjectManagementSystem.DataAccess.Models;
using ProjectManagementSystem.Domain.Exceptions;
using ProjectManagementSystem.Domain.Interfaces;
using ProjectManagementSystem.Domain.Models.Project;
using ProjectManagementSystem.Domain.Services;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystem.Domain.Tests.Unit.Services
{
    public class ProjectServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IGenericRepository<Project>> _projectRepository;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IProjectStateService> _projectStateService;
        private readonly ProjectService _projectService;

        public ProjectServiceTests()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _projectRepository = new Mock<IGenericRepository<Project>>();
            _mapper = new Mock<IMapper>();
            _projectStateService = new Mock<IProjectStateService>();
            _unitOfWork
                .Setup(u => u.GetRepository<Project>())
                .Returns(_projectRepository.Object);
            
            _projectService = new ProjectService(
                _unitOfWork.Object,
                _mapper.Object,
                _projectStateService.Object);
        }

        [Fact]
        public async Task Create_WhenSaveChangesThrowsSqlException_ThenCreationExceptionIsThrown()
        {
            // Arrange
            var projectRequestCreateDto = new ProjectRequestCreateDto();
            _mapper
                .Setup(m => m.Map<Project>(It.IsAny<ProjectRequestCreateDto>()))
                .Returns(It.IsAny<Project>());

            _projectRepository
                .Setup(p => p.InsertAsync(It.IsAny<Project>()))
                .Returns(Task.FromResult(It.IsAny<Project>()));

            var exception = FormatterServices.GetUninitializedObject(typeof(SqlException))
                as SqlException;

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .Throws(exception);

            // Act
            // Assert
            await Assert.ThrowsAsync<CreationException>(() => _projectService.Create(projectRequestCreateDto));
        }

        [Fact]
        public async Task Update_WhenEntityNotFound_ThenRecordNotFoundExceptionIsThrown()
        {
            // Arrange
            var dto = new ProjectRequestUpdateDto
            {
                ProjectId = 1
            };

            _projectRepository
                .Setup(p => p.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(It.IsAny<Project>()));

            // Act
            // Assert
            await Assert.ThrowsAsync<RecordNotFoundException>(() => _projectService.Update(dto));
        }

        [Fact]
        public async Task Update_WhenSaveChangesThrowsSqlException_ThenUpdateExceptionIsThrown()
        {
            // Arrange
            var dto = new ProjectRequestUpdateDto
            {
                ProjectId = 1
            };

            _projectRepository
                .Setup(p => p.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(new Project()));

            _mapper
                .Setup(m => m.Map(It.IsAny<ProjectRequestUpdateDto>(), It.IsAny<Project>()))
                .Returns((ProjectRequestUpdateDto _, Project project) => project);

            _projectRepository
                .Setup(p => p.Update(It.IsAny<Project>()));

            var exception = FormatterServices.GetUninitializedObject(typeof(SqlException))
                as SqlException;

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .Throws(exception);

            // Act
            // Assert
            await Assert.ThrowsAsync<UpdateException>(() => _projectService.Update(dto));
        }

        [Fact]
        public async Task Update_WhenParentIdNotChanged_ThenCheckProjectAndParentsToChangeStateNotCalled()
        {
            // Arrange
            var dto = new ProjectRequestUpdateDto
            {
                ProjectId = 1
            };

            _projectRepository
                .Setup(p => p.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(new Project()));

            _mapper
                .Setup(m => m.Map(It.IsAny<ProjectRequestUpdateDto>(), It.IsAny<Project>()))
                .Returns((ProjectRequestUpdateDto _, Project project) => project);

            _projectRepository
                .Setup(p => p.Update(It.IsAny<Project>()));

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mapper
                .Setup(m => m.Map<ProjectResponseDto>(It.IsAny<Project>()))
                .Returns(It.IsAny<ProjectResponseDto>());

            // Act
            await _projectService.Update(dto);

            // Assert
            _projectStateService
                .Verify(p => p.CheckProjectAndParentsToChangeState(It.IsAny<int?>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenParentIdChanged_ThenCheckProjectAndParentsToChangeStateCalled()
        {
            // Arrange
            var dto = new ProjectRequestUpdateDto
            {
                ProjectId = 1,
                ParentId = 2
            };
            var project = new Project
            {
                ParentId = 1
            };

            _projectRepository
                .Setup(p => p.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(project));

            _projectStateService
                .Setup(
                    p => p.CheckProjectAndParentsToChangeState(It.IsAny<int?>()))
                .Returns(Task.CompletedTask);

            _mapper
                .Setup(m => m.Map(It.IsAny<ProjectRequestUpdateDto>(), It.IsAny<Project>()))
                .Returns((ProjectRequestUpdateDto _, Project p) => p);

            _projectRepository
                .Setup(p => p.Update(It.IsAny<Project>()));

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _mapper
                .Setup(m => m.Map<ProjectResponseDto>(It.IsAny<Project>()))
                .Returns(It.IsAny<ProjectResponseDto>());

            // Act
            await _projectService.Update(dto);

            // Assert
            _projectStateService
                .Verify(
                    p => p.CheckProjectAndParentsToChangeState(It.IsAny<int?>()),
                    Times.Exactly(2));
        }

        [Fact]
        public async Task Delete_WhenEntityNotFound_ThenRecordNotFoundExceptionIsThrown()
        {
            // Arrange
            int projectId = 1;

            _projectRepository
                .Setup(p => p.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(It.IsAny<Project>()));

            // Act
            // Assert
            await Assert.ThrowsAsync<RecordNotFoundException>(() => _projectService.Delete(projectId));
        }

        [Fact]
        public async Task Delete_WhenTwoChildrenExists_ThenDeleteCalledThreeTimesAndCheckProjectCalled()
        {
            // Arrange
            var project = new Project
            {
                InverseParent = new List<Project>
                {
                    new Project(),
                    new Project()
                }
            };

            _projectRepository
                .Setup(p => p.GetByIdAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(project));

            _projectRepository
                .Setup(p =>
                    p.LoadAllChildren(It.IsAny<Project>(),
            It.IsAny<Expression<Func<Project, IEnumerable<Project>>>>()));

            _projectRepository
                .Setup(p => p.Delete(It.IsAny<Project>()));

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _projectService.Delete(project.ProjectId);

            // Assert
            _projectStateService
                .Verify(p =>
                    p.CheckProjectAndParentsToChangeState(It.IsAny<int?>()), Times.Once);

            _projectRepository
                .Verify(
                    p => p.Delete(It.IsAny<Project>()),
                    Times.Exactly(3));
        }
    }
}
