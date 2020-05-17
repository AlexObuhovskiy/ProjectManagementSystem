using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Domain.Interfaces;

namespace ProjectManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _projectService.GetAllProjects();

            return Ok(projects);
        }
    }
}
