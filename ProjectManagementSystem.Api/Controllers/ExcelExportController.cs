using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Domain.Interfaces;
using ProjectManagementSystem.Domain.Models.ExcelExport;

namespace ProjectManagementSystem.Api.Controllers
{
    /// <summary>
    /// Class ExcelExportController.
    /// Implements the <see cref="Microsoft.AspNetCore.Mvc.Controller" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [ApiController]
    [Route("api/[controller]")]
    public class ExcelExportController : Controller
    {
        private readonly IExcelExportService _excelExportService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExportController"/> class.
        /// </summary>
        /// <param name="excelExportService">The excel export service.</param>
        public ExcelExportController(IExcelExportService excelExportService)
        {
            _excelExportService = excelExportService;
        }

        /// <summary>
        /// Exports the inprogress projects and tasks for date.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns>Task&lt;IActionResult&gt;.</returns>
        [HttpPost]
        [Produces("application/octet-stream")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(byte[]))]
        public async Task<IActionResult> ExportInprogressProjectsAndTasksForDate([FromBody]ExcelExportRequestDto dto)
        {
            var content = await _excelExportService.ExportInprogressForDate(dto.ExportDate);

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Project and tasks in progress.xlsx");
        }
    }
}
