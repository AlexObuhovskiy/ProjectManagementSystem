using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProjectManagementSystem.Api.Infrastructure;
using ProjectManagementSystem.Domain.Exceptions;
using ProjectManagementSystem.Domain.Interfaces;
using ProjectManagementSystem.Domain.Models.Project;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjectManagementSystem.Api.Controllers
{
    /// <summary>
    /// Class ProjectController.
    /// Implements the <see cref="Controller" />
    /// </summary>
    /// <seealso cref="Controller" />
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectController"/> class.
        /// </summary>
        /// <param name="projectService">The project service.</param>
        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// Gets all projects.
        /// </summary>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ProjectResponseDto[]))]
        [SwaggerOperation(Tags = new[] { Constatns.Project })]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _projectService.GetAllProjects();

            return Ok(projects);
        }

        /// <summary>
        /// Gets the project by identifier.
        /// </summary>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ProjectResponseDto))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(void))]
        [SwaggerOperation(Tags = new[] { Constatns.Project })]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var project = await _projectService.GetById(id);

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        /// <summary>
        /// Creates the project.
        /// </summary>
        /// <param name="projectRequestCreateDto">The project request DTO.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ProjectResponseDto))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ModelStateDictionary))]
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(string))]
        [SwaggerOperation(Tags = new[] { Constatns.Project })]
        public async Task<IActionResult> CreateProject(ProjectRequestCreateDto projectRequestCreateDto)
        {
            try
            {
                var projectResponseDto = await _projectService.Create(projectRequestCreateDto);

                return CreatedAtAction(
                    nameof(GetProjectById),
                    new { id = projectResponseDto.ProjectId },
                    projectResponseDto);
            }
            catch (CreationException e)
            {
                return Conflict(e.Message);
            }
        }

        /// <summary>
        /// Updates the project.
        /// </summary>
        /// <param name="projectRequestUpdateDto">The project request update dto.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ProjectResponseDto))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ModelStateDictionary))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(string))]
        [SwaggerOperation(Tags = new[] { Constatns.Project })]
        public async Task<IActionResult> UpdateProject(ProjectRequestUpdateDto projectRequestUpdateDto)
        {
            try
            {
                var projectResponseDto = await _projectService.Update(projectRequestUpdateDto);

                return Ok(projectResponseDto);
            }
            catch (UpdateException e)
            {
                return Conflict(e.Message);
            }
            catch (RecordNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        /// <summary>
        /// Deletes the project.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(void))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [SwaggerOperation(Tags = new[] { Constatns.Project })]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                await _projectService.Delete(id);

                return Ok();
            }
            catch (RecordNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
