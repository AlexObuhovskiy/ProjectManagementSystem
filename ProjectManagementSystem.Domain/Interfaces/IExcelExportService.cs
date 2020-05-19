using System;

namespace ProjectManagementSystem.Domain.Interfaces
{
    /// <summary>
    /// Interface IExcelExportService
    /// </summary>
    public interface IExcelExportService
    {
        /// <summary>
        /// Excels the export.
        /// </summary>
        /// <returns>System.Byte[].</returns>
        byte[] ExportForDate(DateTime date);
    }
}