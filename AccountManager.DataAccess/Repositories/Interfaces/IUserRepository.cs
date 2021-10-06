using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Domain.Models;

namespace AccountManager.DataAccess.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User, int>
    {
        Task<User> GetByNameAsync(string name);
    }
}
