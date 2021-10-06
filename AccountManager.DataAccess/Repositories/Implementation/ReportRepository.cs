using AccountManager.DataAccess.Context;
using AccountManager.DataAccess.Repositories.Interfaces;
using AccountManager.Domain.Models;

namespace AccountManager.DataAccess.Repositories.Implementation
{
    public class ReportRepository : BaseRepository<Report, int, AccountManagerContext>, IReportRepository
    {
        public ReportRepository(AccountManagerContext context) : base(context)
        {
        }
    }
}
