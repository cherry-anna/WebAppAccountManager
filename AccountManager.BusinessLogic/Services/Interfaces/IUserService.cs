using System.Threading.Tasks;
using AccountManager.Domain.Models;

namespace AccountManager.BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        Task<Employee> AuthenticateUserAsync(string login, string password);
        Task<Employee> RegisterUserAsync(string login, string password);
    }
}
