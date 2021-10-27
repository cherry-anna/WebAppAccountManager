using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.DataAccess.Repositories.Interfaces;
using AccountManager.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace AccountManager.BusinessLogic.Services.Implementation
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
       
       public ReportService(IReportRepository reportRepository, IProjectRepository projectRepository, IHttpContextAccessor httpContextAccessor)
        {
            _reportRepository = reportRepository;
            _projectRepository = projectRepository;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<IEnumerable<Report>> GetReportsAsync()
        {
            return await _reportRepository.GetReportsWithItemsAsync();
        }

        public async Task<Report> CreateReportAsync( int projectId, DateTime jobDate, TimeSpan duration, string description)
        {
            HttpContext context = _httpContextAccessor.HttpContext;
            int userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            var item = new Report
            {
                EmployeeId = userId,
                ProjectId = projectId,
                JobDate = jobDate,
                Duration = (int)duration.TotalMinutes,
                Description = description
            };
            var insertedItem = await _reportRepository.InsertAsync(item);
            await _reportRepository.UnitOfWork.SaveChangesAsync();

            return insertedItem;
        }
    }
}
