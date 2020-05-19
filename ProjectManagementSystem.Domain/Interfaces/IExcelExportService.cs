using System;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Domain.Interfaces
{
    /// <summary>
    /// Interface IExcelExportService
    /// </summary>
    public interface IExcelExportService
    {
        /// <summary>
        /// Exports the excel bytes with project and tasks inprogress for date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>System.Byte[].</returns>
        Task<byte[]> ExportInprogressForDate(DateTime date);
    }
}