using AccountManager.BusinessLogic.Services.Implementation;
using AccountManager.DataAccess.Context;
using AccountManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AccountManager.BusinessLogic.Tests.Services
{
    public class ProjectServiceTests
    {
        private readonly Mock<AccountManagerContext> _contexMock;
        private readonly ProjectService _service;

        public ProjectServiceTests()
        {

            //var optionsBuilder = new DbContextOptionsBuilder<AccountManagerContext>();
            //_contexMock = new Mock<AccountManagerContext>(optionsBuilder.Options);

            //// var options = new DbContextOptionsBuilder<AccountManagerContext>()
            ////.UseInMemoryDatabase(databaseName: "MovieListDatabase")
            ////.Options;
            //// DbContextOptions<AccountManagerContext> contexOptions = new DbContextOptionsBuilder<AccountManagerContext>().Options;
            //// _contexMock = new Mock<AccountManagerContext>(contexOptions);
            //_service = new ProjectService(_contexMock.Object);
        }

        [Fact]
        public async void GetProjectsAsync_Exist_Passed()
        {
            //Arrange
            var data = new List<Project>
             {
             new Project { Name = "BBB" },
             new Project { Name = "ZZZ" },
             new Project { Name = "AAA" },
             }.AsQueryable();
            var mockSet = new Mock<DbSet<Project>>();
            mockSet.As<IQueryable<Project>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator);

            // mockSet.Setup(m => m.Include(It.IsAny<String>())).Returns(mockSet.Object);
           // mockSet.As<IAsyncEnumerable<Project>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<System.Threading.CancellationToken>()).Returns((IAsyncEnumerator<Project>)data.AsAsyncEnumerable().GetAsyncEnumerator()));


            var mockContext = new Mock<AccountManagerContext>();
            mockContext.SetupGet(c => c.Projects).Returns(mockSet.Object);

            var service = new ProjectService(mockContext.Object);


            //Act
            var projects = await service.GetProjectsAsync();
            //Assert
            Assert.Equal(3, projects.Count());
            Assert.Equal("AAA", projects.ElementAt(0).Name);
            Assert.Equal("BBB", projects.ElementAt(1).Name);
            Assert.Equal("ZZZ", projects.ElementAt(2).Name);
        }
    }


}
