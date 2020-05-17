using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Api.Infrastructure;
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
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ProjectResponseDto[]))]
        [SwaggerOperation(Tags = new[] { Constatns.Project })]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _projectService.GetAllProjects();

            return Ok(projects);
        }
    }
}
