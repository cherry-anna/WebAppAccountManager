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
    }
}
