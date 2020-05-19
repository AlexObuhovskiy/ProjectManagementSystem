using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.DataAccess.Interfaces;
using ProjectManagementSystem.DataAccess.Models;
using ProjectManagementSystem.Domain.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Domain.Services
{
    /// <summary>
    /// Class ExcelExportService.
    /// Implements the <see cref="IExcelExportService" />
    /// </summary>
    /// <seealso cref="IExcelExportService" />
    public class ExcelExportService : IExcelExportService
    {
        private readonly IGenericRepository<Project> _projectRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExportService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public ExcelExportService(IUnitOfWork unitOfWork)
        {
            _projectRepository = unitOfWork.GetRepository<Project>();
        }

        /// <inhertidoc/>
        public async Task<byte[]> ExportInprogressForDate(DateTime date)
        {
            var inProgressProjectList =
                await _projectRepository.Get(
                    p => date > p.StartDate && (date < p.FinishDate || p.FinishDate == null),
                    inc => inc.Include(p => p.Task));

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Projects with tasks in progress");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "Project code";
                worksheet.Cell(currentRow, 2).Value = "Project name";
                worksheet.Cell(currentRow, 3).Value = "Project start date";
                worksheet.Cell(currentRow, 4).Value = "Project finish date";
                
                foreach (var project in inProgressProjectList)
                {
                    currentRow++;

                    worksheet.Cell(currentRow, 1).Value = project.Code;
                    worksheet.Cell(currentRow, 2).Value = project.Name;
                    worksheet.Cell(currentRow, 3).Value = project.StartDate;
                    worksheet.Cell(currentRow, 4).Value = project.FinishDate;

                    var inProgressTasks = project.Task.Where(t =>
                        date > t.StartDate && (date < t.FinishDate || t.FinishDate == null)).ToList();

                    if (inProgressTasks.Any())
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 5).Value = "Task name";
                        worksheet.Cell(currentRow, 6).Value = "Task description";
                        worksheet.Cell(currentRow, 7).Value = "Task start date";
                        worksheet.Cell(currentRow, 8).Value = "Task finish date";
                    }

                    foreach (var task in inProgressTasks)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 5).Value = task.Name;
                        worksheet.Cell(currentRow, 6).Value = task.Description;
                        worksheet.Cell(currentRow, 7).Value = task.StartDate;
                        worksheet.Cell(currentRow, 8).Value = task.FinishDate;
                    }
                }

                for (int i = 1; i < 11; i++)
                {
                    worksheet.Column(i).AdjustToContents();
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    byte[] content = stream.ToArray();

                    return content;
                }
            }
        }
    }
}