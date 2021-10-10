using AccountManager.Domain.Interfaces;
using AccountManager.Domain.Models;

namespace AccountManager.DataAccess.Repositories.Interfaces
{
    public interface IEmployeeRepository: IRepository<Employee, int>
    {
    }
}
