using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Domain.Models;

namespace AccountManager.BusinessLogic.Services.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<Report>> GetReportsAsync();
        Task<IEnumerable<Report>> GetCurrentUserReportsAsync();
        Task<Report> CreateReportAsync( int projectId, DateTime jobDate, TimeSpan duration, string description);
        Task<Report> CreateReportWithTimeAsync(int projectId, DateTime jobDate, TimeSpan startJobTime, TimeSpan duration, string description);
    }
}
