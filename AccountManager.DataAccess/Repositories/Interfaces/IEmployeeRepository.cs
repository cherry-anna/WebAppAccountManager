using AccountManager.Domain.Interfaces;
using AccountManager.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountManager.DataAccess.Repositories.Interfaces
{
    public interface IEmployeeRepository: IRepository<Employee, int>
    {
        Task<IEnumerable<Employee>>  GetAllWithProjectsAsync();
    }

    
}
