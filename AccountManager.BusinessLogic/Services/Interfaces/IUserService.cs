using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Domain.Models;

namespace AccountManager.BusinessLogic.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> AuthenticateUserAsync(string login, string password);
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> CreateUserAsync(string name, string password);
        Task ChangeUserNameAsync(int userId, string newName);
        Task ChangeUserPasswordAsync(string oldPassword, string newPassword);
        Task SetUserPasswordAsync(int userId, string newPassword);
        Task DeleteUserAsync(int userId);
    }
}
