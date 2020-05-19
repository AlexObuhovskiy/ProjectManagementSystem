using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using ProjectManagementSystem.Domain.Interfaces;

namespace ProjectManagementSystem.Domain.Services
{
    /// <summary>
    /// Class ExcelExportService.
    /// </summary>
    public class ExcelExportService : IExcelExportService
    {
        internal class User
        {
            public int Id { get; set; }
            public string Username { get; set; }
        }

        public byte[] ExportForDate(DateTime date)
        {
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "A"
                },
            };

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Users");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 2).Value = "Username";
                foreach (var user in users)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = user.Id;
                    worksheet.Cell(currentRow, 2).Value = user.Username;
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