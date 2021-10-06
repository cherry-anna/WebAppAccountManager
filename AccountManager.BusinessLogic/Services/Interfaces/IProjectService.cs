using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.Domain.Models;

namespace AccountManager.BusinessLogic.Services.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetProjectsAsync();
        
        Task<Project> CreateProjectAsync(string name, string description);
    }
}
