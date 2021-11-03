using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.DataAccess.Context;
using AccountManager.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AccountManager.BusinessLogic.Services.Implementation
{
    public class ReportService : IReportService
    {
        private readonly AccountManagerContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const int maxMinutesPerDay = 24*60;
        public ReportService(AccountManagerContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Report>> GetReportsAsync()
        {
            return await _context.Reports.Include(r=>r.Project).Include(r => r.Employee)
                .AsNoTracking().ToListAsync<Report>();
        }
        public async Task<IEnumerable<Report>> GetCurrentUserReportsAsync()
        {
            // get current user id
            HttpContext context = _httpContextAccessor.HttpContext;
            int userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return await _context.Reports.Where(u=>u.Employee.UserId== userId).Include(r => r.Project).Include(r => r.Employee)
                .AsNoTracking().ToListAsync<Report>();
        }
        public async Task<Report> CreateReportAsync( int projectId, DateTime jobDate, TimeSpan duration, string description)
        {
            // get project if exist
            Project project =await _context.Projects.Include(p=>p.Employees).FirstOrDefaultAsync(p=>p.Id == projectId);
            if (project == null)
            {
                throw new Exception();
            }
            // get current user id
            HttpContext context = _httpContextAccessor.HttpContext;
            int userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            // check is employee in project
            Employee employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            bool isEmployeeInProject = project.Employees.Any(e => e.Id == employee.Id);

            if (!isEmployeeInProject)
            {
                throw new Exception();
            }
            // count all registered durations in minutes per day
            int registeredMinutes = await _context.Reports.Where(r => r.EmployeeId == employee.Id && r.JobDate == jobDate).SumAsync(r => r.Duration);

            if ((registeredMinutes + duration.TotalMinutes) > maxMinutesPerDay)
            {
                throw new Exception();
            }


            var item = new Report
            {
                EmployeeId = employee.Id,
                ProjectId = projectId,
                JobDate = jobDate,
                Duration = (int)duration.TotalMinutes,
                Description = description
            };

            var insertedItem = await _context.Reports.AddAsync(item);
            await _context.SaveChangesAsync();
            
            return insertedItem.Entity;
        }

        public async Task<Report> CreateReportWithTimeAsync(int projectId, DateTime jobDate, TimeSpan startJobTime, TimeSpan duration, string description)
        {
            // get project if exist
            Project project = await _context.Projects.Include(p => p.Employees).FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null)
            {
                throw new Exception();
            }
            // get current user id
            HttpContext context = _httpContextAccessor.HttpContext;
            int userId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            // check is employee in project
            Employee employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            bool isEmployeeInProject = project.Employees.Any(e => e.Id == employee.Id);

            if (!isEmployeeInProject)
            {
                throw new Exception();
            }
            // count all registered durations in minutes per day
            int registeredMinutes = await _context.Reports.Where(r => r.EmployeeId == employee.Id && r.JobDate == jobDate).SumAsync(r => r.Duration);

            if ((registeredMinutes + duration.TotalMinutes) > maxMinutesPerDay)
            {
                throw new Exception();
            }

            // the searching of report with time crossing
            int startTimeInMinutes =(int)startJobTime.TotalMinutes;
            int endTimeInMinutes = startTimeInMinutes+(int)duration.TotalMinutes;
            bool isReportWithTimeExist = await _context.Reports.Where(r => r.EmployeeId == employee.Id && r.JobDate == jobDate
            && r.StartJobTime != null && r.EndJobTime != null)
                .AnyAsync(r => r.EndJobTime > startTimeInMinutes && r.StartJobTime < endTimeInMinutes);
            
            if (isReportWithTimeExist)
            {
                throw new Exception();
            }

            var item = new Report
            {
                EmployeeId = employee.Id,
                ProjectId = projectId,
                JobDate = jobDate,
                StartJobTime= (int)startJobTime.TotalMinutes,
                Duration = (int)duration.TotalMinutes,
                Description = description
            };

            var insertedItem = await _context.Reports.AddAsync(item);
            await _context.SaveChangesAsync();

            return insertedItem.Entity;
        }
    }
}
