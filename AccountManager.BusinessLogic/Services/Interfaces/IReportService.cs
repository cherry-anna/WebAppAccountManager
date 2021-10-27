using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Domain.Models;

namespace AccountManager.BusinessLogic.Services.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<Report>> GetReportsAsync();
        Task<Report> CreateReportAsync( int projectId, DateTime jobDate, TimeSpan duration, string description);
    }
}
