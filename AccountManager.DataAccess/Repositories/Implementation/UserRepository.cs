using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using AccountManager.DataAccess.Context;
using AccountManager.DataAccess.Repositories.Interfaces;
using AccountManager.Domain.Models;

namespace AccountManager.DataAccess.Repositories.Implementation
{
    public class UserRepository : BaseRepository<User, int, AccountManagerContext>, IUserRepository
    {
        public UserRepository(AccountManagerContext context) : base(context)
        {
        }

        public async Task<User> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.UserName == name);
        }
    }
}
