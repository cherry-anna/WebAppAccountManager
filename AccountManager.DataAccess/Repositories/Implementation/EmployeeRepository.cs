using AccountManager.DataAccess.Context;
using AccountManager.DataAccess.Repositories.Interfaces;
using AccountManager.Domain.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AccountManager.DataAccess.Repositories.Implementation
{
    public class EmployeeRepository : BaseRepository<Employee, int, AccountManagerContext>, IEmployeeRepository
    {
        public EmployeeRepository(AccountManagerContext context) : base(context)
        {
        }

        public async Task <IEnumerable<Employee>> GetAllWithProjectsAsync()
        {

            return await _context.Employees.Include(e => e.Projects).ToListAsync<Employee>(); ;

        }

    }
}
