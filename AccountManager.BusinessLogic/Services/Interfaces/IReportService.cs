using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.BusinessLogic.Models;
using AccountManager.Domain.Models;

namespace AccountManager.BusinessLogic.Services.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<Report>> GetReportsAsync();
        Task<IEnumerable<Report>> GetReportsByUserIdAsync(int userId);
        Task<Report> CreateReportAsync( int projectId, int employeeId, int userId, DateTime jobDate, TimeSpan duration, string description, TimeSpan? startJobTime=null);
        Task<IEnumerable<ManagerReportByUser>> GetManagerReportByUserAsync(int userId);
        Task<IEnumerable<ManagerReportByProject>> GetManagerReportByProjectAsync(int projectId);
        Task<IEnumerable<ManagerReportByMonth>> GetManagerReportByMonthAsync(int month, int year);
        Task<Report> UpdateReportAsync(int reportId, int projectId, int employeeId, int currentUserId, DateTime jobDate, TimeSpan duration, string description, TimeSpan? startJobTime = null);
        Task DeleteReportAsync(int reportId);
        Task<string> GetPathOfFileManagerReportByProjectAsync(int projectId);
        Task<string> GetPathOfFileManagerReportByUserAsync(int userId);
        Task<string> GetPathOfFileManagerReportByMonthAsync(int month, int year);
    }
}
