using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ProjectManagementSystem.Api.Infrastructure;
using ProjectManagementSystem.Domain.Exceptions;
using ProjectManagementSystem.Domain.Interfaces;
using ProjectManagementSystem.Domain.Models.Task;
using Swashbuckle.AspNetCore.Annotations;

namespace ProjectManagementSystem.Api.Controllers
{
    /// <summary>
    /// Class TaskController.
    /// Implements the <see cref="Controller" />
    /// </summary>
    /// <seealso cref="Controller" />
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskController"/> class.
        /// </summary>
        /// <param name="taskService">The Task service.</param>
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// Gets all Tasks.
        /// </summary>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(TaskResponseDto[]))]
        [SwaggerOperation(Tags = new[] { Constants.Task })]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _taskService.GetAllTasks();

            return Ok(tasks);
        }

        /// <summary>
        /// Gets the task by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(TaskResponseDto))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(void))]
        [SwaggerOperation(Tags = new[] { Constants.Task })]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                var task = await _taskService.GetById(id);

                return Ok(task);
            }
            catch (RecordNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        /// <summary>
        /// Creates the task.
        /// </summary>
        /// <param name="taskRequestCreateDto">The task request create dto.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(TaskResponseDto))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ModelStateDictionary))]
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(string))]
        [SwaggerOperation(Tags = new[] { Constants.Task })]
        public async Task<IActionResult> CreateTask(TaskRequestCreateDto taskRequestCreateDto)
        {
            try
            {
                var taskResponseDto = await _taskService.Create(taskRequestCreateDto);

                return CreatedAtAction(
                    nameof(GetTaskById),
                    new { id = taskResponseDto.ProjectId },
                    taskResponseDto);
            }
            catch (CreationException e)
            {
                return Conflict(e.Message);
            }
        }
    }
}
