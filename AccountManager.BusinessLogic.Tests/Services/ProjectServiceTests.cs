using AccountManager.BusinessLogic.Services.Implementation;
using AccountManager.DataAccess.Context;
using AccountManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace AccountManager.BusinessLogic.Tests.Services
{
    public class ProjectServiceTests : IDisposable
    {

        private readonly AccountManagerContext _accountManagerContext;
        private readonly ProjectService _service;

        public ProjectServiceTests()
        {
            DbContextOptions<AccountManagerContext> dbContextOptions = new DbContextOptionsBuilder<AccountManagerContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;
            _accountManagerContext = new AccountManagerContext(dbContextOptions);
           // if(_accountManagerContext.Projects.Count()==0)
            SeedDb();
            _service = new ProjectService(_accountManagerContext);
        }
        public void Dispose()
        {
            _accountManagerContext.Dispose();
        }
        [Fact]
        public async void GetProjectsAsync_Exist_Passed()
        {
            //Arrange
            //Act
            var projects = await _service.GetProjectsAsync();
            //Assert
            Assert.Single(projects);
            Assert.Equal("Project 1", projects.ElementAt(0).Name);
        }
        [Fact]
        public async void CreateProjectAsync_Exist_Passed()
        {
            //Arrange
            string name = "NewProject";
            string description = "NewProject description";
            //Act
            Project project = await _service.CreateProjectAsync(name, description);
            //Assert
            Assert.Equal("NewProject", project.Name);
        }
        [Fact]
        public async void CreateProjectAsync_ProjectWithThisNameExist_ExceptionIsThrown()
        {
            //Arrange
            string name = "Project 1";
            string description = "NewProject description";
            //Act
            var exception = await Record.ExceptionAsync(() => _service.CreateProjectAsync(name, description));
            //Assert
            Assert.NotNull(exception);
            Assert.Equal("A project with the same name already exists.", exception.Message);
        }
        [Fact]
        public async void AddUserToProjectAsync_Exist_Passed()
        {
            //Arrange
            int projectId = 1;
            int userId = 3;
            decimal rate = 50;
            string position = "director";
            //Act
            await _service.AddUserToProjectAsync(projectId, userId, rate, position);
            //Assert
            Assert.True(_accountManagerContext.Employees.Any(e => e.Id == 1 && e.ProjectId == 1));
        }
        [Fact]
        public async void AddUserToProjectAsync_UserWithIDNotFound_ExceptionIsThrown()
        {
            //Arrange
            int projectId = 1;
            int notExistsUserId = 5;
            decimal rate = 50;
            string position = "director";
            //Act
            var exception = await Record.ExceptionAsync(() => _service.AddUserToProjectAsync(projectId, notExistsUserId, rate, position));
            //Assert
            Assert.NotNull(exception);
            Assert.Equal("User with ID-5 not found.", exception.Message);
        }
        [Fact]
        public async void AddUserToProjectAsync_ProjectWithIDNotFound_ExceptionIsThrown()
        {
            //Arrange
            int notExistsProjectId = 2;
            int userId = 3;
            decimal rate = 50;
            string position = "director";
            //Act
            var exception = await Record.ExceptionAsync(() => _service.AddUserToProjectAsync(notExistsProjectId, userId, rate, position));
            //Assert
            Assert.NotNull(exception);
            Assert.Equal("Project with ID-2 not found.", exception.Message);
        }
        private void SeedDb()
        {
            List<User> users = new List<User>
                {
            new User { UserName = "User 1" },
            new User {  UserName = "User 2" },
            new User { UserName = "User 3" }
        };
            _accountManagerContext.AddRange(users);
            _accountManagerContext.SaveChanges();

            List<Employee> employees = new List<Employee>
                {
            new Employee { UserId = 1,ProjectId =1,Position="engineer",Rate = 20.0M },
            new Employee { UserId = 2,ProjectId =1,Position="mechanic",Rate = 15.0M }
        };
            List<Project> projects = new List<Project>
                {
            new Project { Name = "Project 1",Description = "Description Project 1",Employees = employees}
        };
            _accountManagerContext.AddRange(projects);
            _accountManagerContext.SaveChanges();

        }
    }
}