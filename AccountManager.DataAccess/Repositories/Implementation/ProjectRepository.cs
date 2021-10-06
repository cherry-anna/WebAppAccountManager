using AccountManager.DataAccess.Context;
using AccountManager.DataAccess.Repositories.Interfaces;
using AccountManager.Domain.Models;

namespace AccountManager.DataAccess.Repositories.Implementation
{
    public class ProjectRepository : BaseRepository<Project, int, AccountManagerContext>, IProjectRepository
    {
        public ProjectRepository(AccountManagerContext context) : base(context)
        {
        }
    }
}
