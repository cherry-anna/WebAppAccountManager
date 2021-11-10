using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Domain.Models;

namespace AccountManager.BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> AuthenticateUserAsync(string login, string password);
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> CreateUserAsync(string name, string password, int roleId);
        Task UpdateUserAsync(int userId, string name, string password);
        Task ChangeUserPasswordAsync(int userId, string oldPassword, string newPassword);
        Task DeleteUserAsync(int userId);
    }
}
