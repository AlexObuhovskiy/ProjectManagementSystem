using System.Net;
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
        /// Excels the specified date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [Produces("application/octet-stream")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(byte[]))]
        public IActionResult Excel([FromBody]ExcelExportRequestDto dto)
        {
            var content = _excelExportService.ExportForDate(dto.ExportDate);

            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "users.xlsx");
        }
    }
}
