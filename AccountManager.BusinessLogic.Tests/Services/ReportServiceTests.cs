using AccountManager.BusinessLogic.Models;
using AccountManager.BusinessLogic.Services.Implementation;
using AccountManager.DataAccess.Context;
using AccountManager.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AccountManager.BusinessLogic.Tests.Services
{
    public class ReportServiceTests : IDisposable
    {
        private readonly AccountManagerContext _accountManagerContext;
        private readonly ReportService _service;
        private readonly Mock<IMapper> _mapperMock;
        public ReportServiceTests()
        {
            DbContextOptions<AccountManagerContext> dbContextOptions = new DbContextOptionsBuilder<AccountManagerContext>()
           .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
           .Options;
            _accountManagerContext = new AccountManagerContext(dbContextOptions);
            SeedDb();
            _mapperMock = new Mock<IMapper>();
            _service = new ReportService(_accountManagerContext, _mapperMock.Object);
        }
        public void Dispose()
        {
            _accountManagerContext.Dispose();
        }
        [Fact]
        public async void GetReportsAsync_Exist_Passed()
        {
            //Arrange
            //Act
            var reports = await _service.GetReportsAsync();
            //Assert
            Assert.NotNull(reports);
            Assert.Equal(5, reports.Count());
        }
        [Fact]
        public async void CreateProjectAsync_WithoutTime_Passed()
        {
            //Arrange
            int projectId = 1;
            int employeeId = 1;
            int userId = 1;
            DateTime jobDate = DateTime.Today;
            TimeSpan duration = new TimeSpan(1, 30, 0);
            string description = "Some description";
            //Act
            Report report = await _service.CreateReportAsync(projectId, employeeId, userId, jobDate, duration, description);
            //Assert
            Assert.Equal(employeeId, report.Employee.Id);
        }
        [Fact]
        public async void CreateProjectAsync_WithTime_Passed()
        {
            //Arrange
            int projectId = 1;
            int employeeId = 1;
            int userId = 1;
            DateTime jobDate = DateTime.Today;
            TimeSpan duration = new TimeSpan(1, 30, 0);
            string description = "Some description";
            TimeSpan? startJobTime = new TimeSpan(15, 0, 0); //11 am 
            //Act
            Report report = await _service.CreateReportAsync(projectId, employeeId, userId, jobDate, duration, description, startJobTime);
            //Assert
            Assert.Equal(employeeId, report.Employee.Id);
        }
        [Fact]
        public async void CreateProjectAsync_TheProjectDoesNotExist_ExceptionIsThrown()
        {
            //Arrange
            int notExistsProjectId = 3;
            int employeeId = 1;
            int userId = 1;
            DateTime jobDate = DateTime.Today;
            TimeSpan duration = new TimeSpan(1, 30, 0);
            string description = "Some description";
            //Act
            var exception = await Record.ExceptionAsync(() => _service.CreateReportAsync(notExistsProjectId, employeeId, userId, jobDate, duration, description));
            //Assert
            Assert.NotNull(exception);
            Assert.Equal("Project with ID - 3 does not exist.", exception.Message);
        }
        [Fact]
        public async void CreateProjectAsync_EmployeeNotInTheProject_ExceptionIsThrown()
        {
            //Arrange
            int projectId = 1;
            int employeeId = 3;
            int userId = 1;
            DateTime jobDate = DateTime.Today;
            TimeSpan duration = new TimeSpan(1, 30, 0);
            string description = "Some description";
            //Act
            var exception = await Record.ExceptionAsync(() => _service.CreateReportAsync(projectId, employeeId, userId, jobDate, duration, description));
            //Assert
            Assert.NotNull(exception);
            Assert.Equal("Employee with ID - 3 does not participate in the project - Project 1.", exception.Message);
        }
        [Fact]
        public async void CreateProjectAsync_MoreThan24Hours_ExceptionIsThrown()
        {
            //Arrange
            int projectId = 1;
            int employeeId = 1;
            int userId = 1;
            DateTime jobDate = new DateTime(2021, 12, 15);
            TimeSpan duration = new TimeSpan(1, 30, 0);
            string description = "Some description";
            //Act
            var exception = await Record.ExceptionAsync(() => _service.CreateReportAsync(projectId, employeeId, userId, jobDate, duration, description));
            //Assert
            Assert.NotNull(exception);
            Assert.Equal("It is impossible to register for a day more than 24 hours. For декабря 15, 2021 total registered - 24 hours 0 minutes.", exception.Message);
        }
        [Fact]
        public async void CreateProjectAsync_ReportAtTheSpecifiedTimeExists_ExceptionIsThrown()
        {
            //Arrange
            int projectId = 1;
            int employeeId = 1;
            int userId = 1;
            DateTime jobDate = new DateTime(2021, 12, 14);
            TimeSpan duration = new TimeSpan(1, 30, 0);
            string description = "Some description";
            TimeSpan? startJobTime = new TimeSpan(9, 0, 0); //9 am 
            //Act
            var exception = await Record.ExceptionAsync(() => _service.CreateReportAsync(projectId, employeeId, userId, jobDate, duration, description, startJobTime));
            //Assert
            Assert.NotNull(exception);
            Assert.Equal("There is already a report at the specified time.", exception.Message);
        }
        [Fact]
        public async void GetReportsByUserIdAsync_Exist_Passed()
        {
            //Arrange
            int userId = 1;
            //Act
            IEnumerable<Report> reports = await _service.GetReportsByUserIdAsync(userId);
            //Assert
            Assert.Equal(5, reports.Count());
        }
        [Fact]
        public async void UpdateReportAsync_WithoutTime_Passed()
        {
            //Arrange
            int reportId = 1;
            int projectId = 1;
            int employeeId = 1;
            int currentUserId = 1;
            DateTime jobDate = DateTime.Today;
            TimeSpan newDuration = new TimeSpan(2, 30, 0);
            string description = "Some description";
            //Act
            Report report = await _service.UpdateReportAsync(reportId, projectId, employeeId, currentUserId, jobDate, newDuration, description);
            //Assert
            Assert.Equal(newDuration.TotalMinutes, report.Duration);
        }
        [Fact]
        public async void UpdateReportAsync_WithTime_Passed()
        {
            //Arrange
            int reportId = 4;
            int projectId = 1;
            int employeeId = 1;
            int currentUserId = 1;
            DateTime jobDate = DateTime.Today;
            TimeSpan newDuration = new TimeSpan(2, 30, 0);
            string description = "Some description";
            TimeSpan? startJobTime = new TimeSpan(11, 0, 0); //11 am 
            //Act
            Report report = await _service.UpdateReportAsync(reportId, projectId, employeeId, currentUserId, jobDate, newDuration, description, startJobTime);
            //Assert
            Assert.Equal(newDuration.TotalMinutes, report.Duration);
        }
        [Fact]
        public async void UpdateReportAsync_ChangeTheReportType_ExceptionIsThrown()
        {
            //Arrange
            int reportId = 1;
            int projectId = 1;
            int employeeId = 1;
            int currentUserId = 1;
            DateTime jobDate = DateTime.Today;
            TimeSpan newDuration = new TimeSpan(2, 30, 0);
            string description = "Some description";
            TimeSpan? startJobTime = new TimeSpan(11, 0, 0); //11 am 
            //Act
            var exception = await Record.ExceptionAsync(() => _service.UpdateReportAsync(reportId, projectId, employeeId, currentUserId, jobDate, newDuration, description, startJobTime));
            //Assert
            Assert.NotNull(exception);
            Assert.Equal("The edited report is of the timeless type.It is forbidden to change the report type.", exception.Message);
        }
        [Fact]
        public async void GetManagerReportByUserAsync_Exist_Passed()
        {
            //Arrange
            int userId = 1;
            //Act
            IEnumerable<ManagerReportByUser> reports = await _service.GetManagerReportByUserAsync(userId);
            //Assert
            Assert.Single(reports);
            Assert.Equal(5, reports.ElementAt(0).ReportCount);
        }
        [Fact]
        public async void GetManagerReportByProjectAsync_Exist_Passed()
        {
            //Arrange
            int projectId = 1;
            //Act
            IEnumerable<ManagerReportByProject> reports = await _service.GetManagerReportByProjectAsync(projectId);
            //Assert
            Assert.Single(reports);
            Assert.Equal(5, reports.ElementAt(0).ReportCount);
        }

        [Fact]
        public async void GetManagerReportByMonthAsync_Exist_Passed()
        {
            //Arrange
            int month = 12;
            int year = 2021;
            //Act
            IEnumerable<ManagerReportByMonth> reports = await _service.GetManagerReportByMonthAsync(month, year);
            //Assert
            Assert.Single(reports);
            Assert.Equal(5, reports.ElementAt(0).ReportCount);
        }
        [Fact]
        public async void DeleteReportAsync_Exist_Passed()
        {
            //Arrange
            int reportId = 1;
            //Act
            await _service.DeleteReportAsync(reportId);
            //Assert
            Assert.True(_accountManagerContext.Reports.Any(e => e.Id == 1));
        }

        private void SeedDb()
        {
            List<User> users = new List<User>
                {
            new User { UserName = "User 1" },
            new User { UserName = "User 2" },
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
            Employee employee = employees.FirstOrDefault(e => e.Id == 1);
            Project project = projects.FirstOrDefault(p => p.Id == 1);
            DateTime jobDate = new DateTime(2021, 12, 14);
            int duration = 120;
            string description = "Some description";
            int startJobTime = (int)(new TimeSpan(9, 0, 0)).TotalMinutes; //9 am 
            List<Report> reports = new List<Report>
            {
                new Report(employee,project,jobDate,duration,description),
                new Report(employee,project,jobDate,duration+10,description),
                new Report(employee,project,jobDate,duration+20,description),
                new Report(employee,project,jobDate,startJobTime,duration+20,description),

                new Report(employee,project,new DateTime(2021, 12, 15),24*60,description),
            };
            _accountManagerContext.AddRange(reports);
            _accountManagerContext.SaveChanges();
        }
    }
}