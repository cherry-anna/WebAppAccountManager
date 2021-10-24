using AccountManager.Domain.Interfaces;
using AccountManager.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountManager.DataAccess.Repositories.Interfaces
{
    public interface IProjectRepository: IRepository<Project, int>
    {
        Task<Project> GetTrackingByIdAsync(int projectId);
        Task<IEnumerable<Project>> GetProjectsWithItemsAsync();
    }
}
