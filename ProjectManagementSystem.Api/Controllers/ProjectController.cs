using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.DataAccess.Models;

namespace ProjectManagementSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        public ProjectController(ProjectManagementSystemContext context)
        {
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
