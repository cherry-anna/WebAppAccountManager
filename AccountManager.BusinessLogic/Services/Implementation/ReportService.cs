using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountManager.BusinessLogic.Services.Interfaces;
using AccountManager.DataAccess.Context;
using AccountManager.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AccountManager.BusinessLogic.Models;

namespace AccountManager.BusinessLogic.Services.Implementation
{
    public class ReportService : IReportService
    {
        private readonly AccountManagerContext _context;
        private const int maxMinutesPerDay = 24*60;
        private const int minutesInHour = 60;
        public ReportService(AccountManagerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Report>> GetReportsAsync()
        {
            return await _context.Reports.Include(r=>r.Project).Include(r => r.Employee).ThenInclude(e => e.User)
                .AsNoTracking().ToListAsync<Report>();
        }
        public async Task<IEnumerable<Report>> GetReportsByUserIdAsync(int userId)
        {
            return await _context.Reports.Where(u=>u.Employee.UserId== userId).Include(r => r.Project).Include(r => r.Employee)
                .AsNoTracking().ToListAsync<Report>();
        }
        public async Task<Report> CreateReportAsync( int projectId, int employeeId, int userId, DateTime jobDate, TimeSpan duration, string description, TimeSpan? startJobTime=null)
        {
            // get project if exist
            Project project =await _context.Projects.Include(p=>p.Employees).FirstOrDefaultAsync(p=>p.Id == projectId);
            if (project == null)
            {
                throw new Exception();
            }
            // check is employee in project
            Employee employee = await _context.Employees.Include(e => e.User).FirstOrDefaultAsync(e => e.Id == employeeId);
            if (employee==null)
            {
                throw new Exception();
            }
            if (employee.UserId != userId)
            {
                throw new Exception();
            }


            bool isEmployeeInProject = project.Employees.Any(e => e.Id == employee.Id);

            if (!isEmployeeInProject)
            {
                throw new Exception();
            }
            // count all registered durations in minutes per day
            int registeredMinutes = await _context.Reports.Where(r => r.Employee.Id == employee.Id && r.JobDate == jobDate).SumAsync(r => r.Duration);

            if ((registeredMinutes + duration.TotalMinutes) > maxMinutesPerDay)
            {
                throw new Exception();
            }

            if (startJobTime.HasValue)
            {
                // the searching of report with time crossing
                int startTimeInMinutes = (int)startJobTime.Value.TotalMinutes;
                int endTimeInMinutes = startTimeInMinutes + (int)duration.TotalMinutes;
                bool isReportWithTimeExist = await _context.Reports.Where(r => r.Employee.Id == employee.Id && r.JobDate == jobDate
                && r.StartJobTime != null && r.EndJobTime != null)
                    .AnyAsync(r => r.EndJobTime > startTimeInMinutes && r.StartJobTime < endTimeInMinutes);

                if (isReportWithTimeExist)
                {
                    throw new Exception();
                }
            }
            

            var report = new Report
            {
                Employee = employee,
                Project = project,
                JobDate = jobDate,
                StartJobTime = (int?)startJobTime?.TotalMinutes,
                Duration = (int)duration.TotalMinutes,
                Description = description
            };

            _context.Add(report);
            //var insertedItem = await _context.Reports.AddAsync(item);
            await _context.SaveChangesAsync();
            
            return report;
        }

        public async Task<IEnumerable<ManagerReportByUser>> GetManagerReportByUserAsync(int userId)
        {
            List<Report> reports = await  _context.Reports.Where(u => u.Employee.UserId == userId).Include(r => r.Project).Include(r => r.Employee)
               .ToListAsync();
            return reports.GroupBy(r=>r.Employee)
                .Select(e=> new ManagerReportByUser
                {
                    Project = e.First().Project.Name,
                    ReportCount = e.Count(),
                    Position = e.First().Employee.Position,
                    WorkedHours = (double)e.Sum(r=>r.Duration)/ (double)minutesInHour,
                    Rate = e.First().Employee.Rate,
                    Salary = ((decimal)e.Sum(r => r.Duration) / (decimal)minutesInHour) * (e.First().Employee.Rate)
                }
                ).ToList<ManagerReportByUser>();
            

        }

        public async Task<IEnumerable<ManagerReportByProject>> GetManagerReportByProjectAsync(int projectId)
        {
            List<Report> reports = await _context.Reports.Where(u => u.Project.Id == projectId).Include(r => r.Project)
                .Include(r => r.Employee).ThenInclude(e=>e.User)
                .ToListAsync();
            return reports.GroupBy(r => r.Employee)
                .Select(e => new ManagerReportByProject
                {
                    UserId = e.First().Employee.UserId,
                    UserName = e.First().Employee.User.UserName,
                    ReportCount = e.Count(),
                    Position = e.First().Employee.Position,
                    WorkedHours = (double)e.Sum(r => r.Duration) / (double)minutesInHour,
                    Rate = e.First().Employee.Rate,
                    Salary = ((decimal)e.Sum(r => r.Duration) / (decimal)minutesInHour) * (e.First().Employee.Rate)
                }
                ).ToList<ManagerReportByProject>();
        }
    }
}
