
using System.Threading.Tasks;
using AccountManager.Domain.Interfaces;
using AccountManager.Domain.Models;

namespace AccountManager.DataAccess.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<Employee, int>
    {
        Task<Employee> GetByNameAsync(string name);
    }
}