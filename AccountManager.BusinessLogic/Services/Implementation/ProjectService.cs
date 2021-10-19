using System;
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
        private readonly IEmployeeRepository _employeeRepository;
        public ProjectService(IProjectRepository projectRepository, IEmployeeRepository employeeRepository)
        {
            _projectRepository = projectRepository;
            _employeeRepository = employeeRepository;
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

        public async Task AddEmployeeToProjectAsync(int idProject, int idEmployee)
        {
            Employee employee= await _employeeRepository.GetByIdAsync(idEmployee);
            Project project = await _projectRepository.GetByIdAsync(idProject);


            project.Employees.Add(employee);


            await _projectRepository.UpdateAsync(project);
            await _projectRepository.UnitOfWork.SaveChangesAsync();

            
        }
    }
}
