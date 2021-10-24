using AccountManager.DataAccess.Context;
using AccountManager.DataAccess.Repositories.Interfaces;
using AccountManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountManager.DataAccess.Repositories.Implementation
{
    public class ProjectRepository : BaseRepository<Project, int, AccountManagerContext>, IProjectRepository
    {
        public ProjectRepository(AccountManagerContext context) : base(context)
        {
        }

        public async Task<Project> GetTrackingByIdAsync(int projectId)
        {
            return await _context.Projects.Where(p => p.Id == projectId).Include(p => p.Employees).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Project>> GetProjectsWithItemsAsync()
        {
            return await _context.Projects.Include(p => p.Employees).AsNoTracking().ToListAsync<Project>();
        }
    }
}
