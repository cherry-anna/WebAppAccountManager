using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.DataAccess.Context;
using AccountManager.Domain.Exceptions;
using AccountManager.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountManager.BusinessLogic.Services.Implementation
{
    public class ProjectService : IProjectService
    {
        private readonly AccountManagerContext _context;
        public ProjectService(AccountManagerContext context)
        {
             _context=context;
        }
        public async Task<IEnumerable<Project>> GetProjectsAsync()
        {
            return await _context.Projects.Include(p => p.Employees).ThenInclude(e=>e.User).AsNoTracking().ToListAsync<Project>();
        }
        public async Task<Project> CreateProjectAsync(string name, string description)
        {
            var item = new Project
            {
                Name = name,
                Description = description
            };

            var insertedItem = await _context.Projects.AddAsync(item);
            await _context.SaveChangesAsync();

            return insertedItem.Entity; 
        }

        public async Task AddUserToProjectAsync(int projectId, int userId, decimal rate, string position)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if(user==null)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.NotFound, $"User with ID-{userId} not found.");
            }
            Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null)
            {
                throw new ExceptionAccountManager((int)HttpStatusCode.NotFound, $"Project with ID-{projectId} not found.");
            }

            Employee employee = new Employee
            {
                UserId = userId,
                ProjectId = projectId,
                Rate = rate,
                Position = position
            };

            _context.Add(employee);

            await _context.SaveChangesAsync();
        }
    }
}
