using AccountManager.DataAccess.Context;
using AccountManager.DataAccess.Repositories.Interfaces;
using AccountManager.Domain.Models;

namespace AccountManager.DataAccess.Repositories.Implementation
{
    public class EmployeeRepository : BaseRepository<Employee, int, AccountManagerContext>, IEmployeeRepository
    {
        public EmployeeRepository(AccountManagerContext context) : base(context)
        {
        }
    }
}
