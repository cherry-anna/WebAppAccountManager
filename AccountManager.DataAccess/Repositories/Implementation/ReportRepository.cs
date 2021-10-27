using AccountManager.DataAccess.Context;
using AccountManager.DataAccess.Repositories.Interfaces;
using AccountManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountManager.DataAccess.Repositories.Implementation
{
    public class ReportRepository : BaseRepository<Report, int, AccountManagerContext>, IReportRepository
    {
        public ReportRepository(AccountManagerContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Report>> GetReportsWithItemsAsync()
        {
            return await _context.Reports.Include(r => r.Employee).Include(r => r.Project).AsNoTracking().ToListAsync<Report>();
        }
    }
}
