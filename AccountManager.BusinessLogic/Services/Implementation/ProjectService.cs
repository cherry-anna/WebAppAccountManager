using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.DataAccess.Repositories.Interfaces;
using AccountManager.Domain.Models;

namespace AccountManager.BusinessLogic.Services.Implementation
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

  

        public async Task<IEnumerable<Project>> GetProjectsAsync()
        {
            
            return await _projectRepository.GetAllAsync();
        }


        public async Task<Project> CreateProjectAsync(string name, string description)
        {
            var item = new Project
            {
                Name = name,
                Description = description
            };

            var insertedItem = await _projectRepository.InsertAsync(item);
            await _projectRepository.UnitOfWork.SaveChangesAsync();

            return insertedItem;
        }
    }
}
