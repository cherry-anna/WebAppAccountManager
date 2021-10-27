using AccountManager.Domain.Interfaces;
using AccountManager.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountManager.DataAccess.Repositories.Interfaces
{
    public interface IReportRepository : IRepository<Report, int>
    {
        Task<IEnumerable<Report>> GetReportsWithItemsAsync();
    }
}
